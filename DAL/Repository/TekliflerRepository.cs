using DAL.DTO;
using DAL.Interface;
using DAL.Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using DAL.Service.KurService;
using Dapper;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using PdfSharp.Drawing;
using PdfSharp;
using System.Net.Mail;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace DAL.Repository
{
    public class TekliflerRepository : ITeklifler
    {
        private readonly IDbConnection _db;
        private readonly IKurService _kur;
        private readonly ISepet _sepet;

        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _host;

        public TekliflerRepository(IDbConnection db, IKurService kur, ISepet sepet, Microsoft.AspNetCore.Hosting.IHostingEnvironment host)
        {
            _db = db;
            _kur = kur;
            _sepet = sepet;
            _host = host;
        }

        public async Task DeleteDetay(int Id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            var sql = @"Delete from TeklifDetay where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
        public async Task Delete(int Id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            var sqlquery = @$"Select Id from TeklifDetay where SepetId={Id}";
            var list = await _db.QueryAsync<IdControl>(sqlquery);
            foreach (var item in list)
            {
                prm.Add("@Id", item.Id);
                var sql = @$"Delete from TeklifDetay where Id=@Id and FirmaId=@FirmaId";
                await _db.ExecuteAsync(sql, prm);
            }
            var sqlkomut = @$"Delete from Teklifler where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sqlkomut, prm);


        }

        public async Task DeleteSoft(int Id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            var sql = @"Update  Teklifler set Aktif=@Aktif where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<int> Insert(int SepetId, int FirmaId, int KullanıcıId)
        {
            var sepetlist = await _db.QueryAsync<TeklifInsert>(@$"select * from Sepet where Id={SepetId} and Aktif=1");
            foreach (var T in sepetlist)
            {

                DynamicParameters prm = new DynamicParameters();
                prm.Add("@Aktif", true);
                prm.Add("@MusteriId", T.MusteriId);
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@KullanıcıId", KullanıcıId);
                prm.Add("@ParaBirimiId", T.ParaBirimiId);
                prm.Add("@IslemTarihi", DateTime.Now);
                prm.Add("@IskontoBirim", T.IskontoBirim);
                prm.Add("@StokKoduGoster", false);
                prm.Add("@MarkaGoster", false);
                prm.Add("@IskontoGoster", false);

                prm.Add("@TeklifAdi", T.SepetAdi);
                prm.Add("@Durum", 1);

                prm.Add("@IskontoOrani", T.IskontoOrani);

                var sql = @"Insert into Teklifler (MusteriId,TeklifAdi,ParaBirimiId,Durum,IslemTarihi,IskontoBirim,IskontoOrani,KullanıcıId,FirmaId,Aktif,StokKoduGoster,MarkaGoster,IskontoGoster) OUTPUT INSERTED.[Id]  values  (@MusteriId,@TeklifAdi,@ParaBirimiId,@Durum,@IslemTarihi,@IskontoBirim,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif,@StokKoduGoster,@MarkaGoster,@IskontoGoster)";
                return await _db.QuerySingleAsync<int>(sql, prm);
            }
            return 1;


        }

        //ürün eklemek icin
        public async Task<int> InsertTeklifDetay(InsertTeklifDetay A, int FirmaId, int KullanıcıId)
        {

            DynamicParameters prm = new DynamicParameters();
            var teklif = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={A.ParaBirimiId}");
            var teklifparabirimi = await _kur.TarihsizKur(teklif, FirmaId);

            var parabirimi = await _db.QueryFirstAsync<int>(@$"select ParaBirimiId from Stoklar where Id={A.StokId} and Aktif=1");
            var parabirimiIsmi = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={parabirimi}");
            var stokparabirimi = await _kur.TarihsizKur(parabirimiIsmi, FirmaId);



            var stoklist = await _db.QueryAsync<DetayInsert>(@$"select S.BirimId,S.StokKodu,S.StokAdi,s.KategoriId,StokKategori.Isim as KategoriIsmi,s.Aciklama,s.KDVOranı,s.BirimFiyat from Stoklar S 
left join StokKategori on StokKategori.Id=s.KategoriId
where s.Id={A.StokId} and s.Aktif=1");
            foreach (var T in stoklist)
            {

                var TLkarsiligi = T.BirimFiyat * stokparabirimi;
                var teklifbirim = TLkarsiligi / teklifparabirimi;
                T.BirimFiyat = teklifbirim;

                prm.Add("@Aktif", true);
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@KullanıcıId", KullanıcıId);

                prm.Add("@TeklifId", A.TeklifId);
                prm.Add("@StokId", A.StokId);
                prm.Add("@BirimId", T.BirimId);
                prm.Add("@StokKodu", T.StokKodu);
                prm.Add("@Miktar", 1);
                prm.Add("@StokAdı", T.StokAdi);
                prm.Add("@Grup", A.Grup);

                prm.Add("@KategoriIsmi", T.KategoriIsmi);
                prm.Add("@Hediye", false);
                prm.Add("@Aciklama", T.Aciklama);
                prm.Add("@BirimFiyat", T.BirimFiyat);
                prm.Add("@KDVOrani", T.KDVOranı);
                prm.Add("@IskontoOrani", 0);
                prm.Add("@IskontoBirim", false);
            }




            var sql = @"Insert into TeklifDetay (TeklifId,BirimId,Hediye,Grup,KategoriIsmi,IskontoBirim,StokAdi,Aciklama,StokId,StokKodu,Miktar,BirimFiyat,KDVOrani,IskontoOrani,KullanıcıId,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@TeklifId,@BirimId,@Hediye,@Grup,@KategoriIsmi,@IskontoBirim,@StokAdı,@Aciklama,@StokId,@StokKodu,@Miktar,@BirimFiyat,@KDVOrani,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);





        }

        //teklif detayi direk insert
        public async Task InsertDetay(int TeklifId, int SepetId, int FirmaId, int KullanıcıId)
        {

            var sepetlist = await _db.QueryAsync<TeklifDetayInsert>(@$"select * from SepetDetay where SepetId={SepetId} and Aktif=1");
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KullanıcıId", KullanıcıId);
            foreach (var T in sepetlist)
            {
                prm.Add("@TeklifId", TeklifId);
                prm.Add("@StokId", T.StokId);
                prm.Add("@BirimId", T.BirimId);
                prm.Add("@StokKodu", T.StokKodu);
                prm.Add("@Miktar", T.Miktar);
                prm.Add("@StokAdı", T.StokAdi);
                prm.Add("@KategoriIsmi", T.KategoriIsmi);
                prm.Add("@Hediye", T.Hediye);
                prm.Add("@Grup", T.Grup);


                prm.Add("@Aciklama", T.Aciklama);
                prm.Add("@BirimFiyat", T.BirimFiyat);
                prm.Add("@KDVOrani", T.KDVOrani);
                prm.Add("@IskontoOrani", T.IskontoOrani);
                prm.Add("@IskontoBirim", T.IskontoBirim);


                var sql = @"Insert into TeklifDetay (TeklifId,BirimId,Grup,Hediye,KategoriIsmi,IskontoBirim,StokAdi,Aciklama,StokId,StokKodu,Miktar,BirimFiyat,KDVOrani,IskontoOrani,KullanıcıId,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@TeklifId,@BirimId,@Grup,@Hediye,@KategoriIsmi,@IskontoBirim,@StokAdı,@Aciklama,@StokId,@StokKodu,@Miktar,@BirimFiyat,@KDVOrani,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif)";
                await _db.ExecuteAsync(sql, prm);
            }




        }

        public async Task<IEnumerable<TeklifListResponse>> List(string? kelime, int FirmaId, int? KAYITSAYISI, int? SAYFA)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);

            if (KAYITSAYISI == null && SAYFA == null)
            {
                var list = await _db.QueryAsync<TeklifListResponse>(@$"
         select s.Id ,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.TeklifAdi,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.TeklifTarihi,s.IslemTarihi,s.GecerlilikTarihi,s.IskontoOrani,s.IskontoBirim,s.IskontoGoster,s.MarkaGoster,s.StokKoduGoster
            from Teklifler s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Dil on Dil.Id=s.Dil_id
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId={FirmaId} and s.Aktif=1 and 
			(ISNULL(Musteriler.Unvan,0) LIKE '%{kelime}%' 
              or ISNULL(ParaBirimleri.Kod,0) LIKE '%{kelime}%' 
             or ISNULL(ParaBirimleri.Kod,0) LIKE '%{kelime}%' or
             ISNULL(s.IskontoOrani,0) LIKE '%{kelime}%' or ISNULL(s.TeklifAdi,0) LIKE '%{kelime}%' )", prm);
                decimal? geneltoplam = 0;
                foreach (var item in list)
                {
                    string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.StokId,se.BirimId,Birim.Isim,se.Grup,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim 
            from TeklifDetay se
            left join Birim on Birim.Id=se.BirimId
            where se.TeklifId={item.Id} and se.FirmaId={FirmaId}";
                    var teklifdetay = await _db.QueryAsync<TeklifDetayInsertResponse>(sql);
                    foreach (var detay in teklifdetay)
                    {
                        if (detay.IskontoBirim == true)
                        {
                            detay.BirimFiyat = (detay.BirimFiyat - detay.IskontoOrani) * detay.Miktar;
                        }
                        else
                        {
                            detay.BirimFiyat = (detay.BirimFiyat - ((detay.IskontoOrani / 100) * detay.BirimFiyat)) * detay.Miktar;
                        }
                        var toplam = detay.BirimFiyat + ((detay.KDVOrani / 100) * detay.BirimFiyat);
                        geneltoplam += toplam;
                    }
                    if (item.IskontoBirim == true)
                    {
                        geneltoplam = geneltoplam - item.IskontoOrani;
                    }
                    else
                    {
                        geneltoplam = geneltoplam - (geneltoplam * (item.IskontoOrani / 100));
                    }
                    item.ToplamTutar = geneltoplam;
                    geneltoplam = 0;

                }




                //List<TeklifListResponse> liste = new();

                //foreach (var item in list)
                //{

                //    var teklifid = await _db.QueryAsync<TeklifListResponse>(@$"select * from Teklifler where RevizyonId={item.Id} and Aktif=1 and FirmaId={FirmaId}
                //Order By IslemTarihi DESC", prm);
                //    var teklifler = await _db.QueryAsync<TeklifListResponse>(@$"select * from Teklifler where Id={item.Id} and Aktif=1 and FirmaId={FirmaId}", prm);
                //    if (teklifid.Count() != 0)
                //    {
                //        liste.Add(teklifid.First());
                //    }
                //    else
                //    {
                //        liste.Add(teklifler.First());
                //    }

                //}
                return list;

            }
            else
            {
                var list = await _db.QueryAsync<TeklifListResponse>(@$"DECLARE @KAYITSAYISI int DECLARE @SAYFA int SET @KAYITSAYISI ={KAYITSAYISI} SET @SAYFA = {SAYFA}
        select s.Id ,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.TeklifAdi,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.TeklifTarihi,s.IslemTarihi,s.GecerlilikTarihi,s.Kur,s.IskontoOrani,s.IskontoBirim
            from Teklifler s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Dil on Dil.Id=s.Dil_id
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId={FirmaId} and s.Aktif=1  and 
			(ISNULL(Musteriler.Unvan,0) LIKE '%{kelime}%' 
              or ISNULL(ParaBirimleri.Kod,0) LIKE '%{kelime}%' 
             or ISNULL(ParaBirimleri.Kod,0) LIKE '%{kelime}%' or
             ISNULL(s.IskontoOrani,0) LIKE '%{kelime}%' or ISNULL(s.TeklifAdi,0) LIKE '%{kelime}%' )
			  ORDER BY s.Id OFFSET @KAYITSAYISI * (@SAYFA - 1) ROWS FETCH NEXT @KAYITSAYISI ROWS ONLY;", prm);
                decimal? geneltoplam = 0;
                foreach (var item in list)
                {
                    string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.StokId,se.BirimId,Birim.Isim,se.StokKodu,se.Miktar,se.Grup,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim 
            from TeklifDetay se
            left join Birim on Birim.Id=se.BirimId
            where se.TeklifId={item.Id} and se.FirmaId={FirmaId}";
                    var teklifdetay = await _db.QueryAsync<TeklifDetayInsertResponse>(sql);
                    foreach (var detay in teklifdetay)
                    {
                        if (detay.IskontoBirim == true)
                        {
                            detay.BirimFiyat = (detay.BirimFiyat - detay.IskontoOrani) * detay.Miktar;
                        }
                        else
                        {
                            detay.BirimFiyat = (detay.BirimFiyat - ((detay.IskontoOrani / 100) * detay.BirimFiyat)) * detay.Miktar;
                        }
                        var toplam = detay.BirimFiyat + ((detay.KDVOrani / 100) * detay.BirimFiyat);
                        geneltoplam += toplam;
                    }
                    if (item.IskontoBirim == true)
                    {
                        geneltoplam = geneltoplam - item.IskontoOrani;
                    }
                    else
                    {
                        geneltoplam = geneltoplam - (geneltoplam * (item.IskontoOrani / 100));
                    }
                    item.ToplamTutar = geneltoplam;

                }
                //List<TeklifListResponse> liste = new();

                //foreach (var item in list)
                //{

                //    var teklifid = await _db.QueryAsync<TeklifListResponse>(@$"select * from Teklifler where RevizyonId={item.Id} and Aktif=1 and FirmaId={FirmaId}
                //Order By IslemTarihi DESC", prm);
                //    var teklifler = await _db.QueryAsync<TeklifListResponse>(@$"select * from Teklifler where Id={item.Id} and Aktif=1 and FirmaId={FirmaId}", prm);
                //    if (teklifid.Count() != 0)
                //    {
                //        liste.Add(teklifid.First());
                //    }
                //    else
                //    {
                //        liste.Add(teklifler.First());
                //    }

                //}
                return list;
            }





        }
        public async Task<int> Count(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);

            var list = await _db.QueryAsync<TeklifListResponse>(@$"
        select s.Id ,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.TeklifAdi,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.TeklifTarihi,s.IslemTarihi,s.GecerlilikTarihi,s.Kur,s.IskontoOrani,s.IskontoGoster,s.StokKoduGoster,s.MarkaGoster
            from Teklifler s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId={FirmaId} and s.Aktif=1", prm);
            //List<TeklifListResponse> liste = new();

            //foreach (var item in list)
            //{

            //    var teklifid = await _db.QueryAsync<TeklifListResponse>(@$"select * from Teklifler where RevizyonId={item.Id} and Aktif=1 and FirmaId={FirmaId}
            //    Order By IslemTarihi DESC", prm);
            //    var teklifler = await _db.QueryAsync<TeklifListResponse>(@$"select * from Teklifler where Id={item.Id} and Aktif=1 and FirmaId={FirmaId}", prm);
            //    if (teklifid.Count() != 0)
            //    {
            //        liste.Add(teklifid.First());
            //    }
            //    else
            //    {
            //        liste.Add(teklifler.First());
            //    }

            //}
            return list.Count();
        }

        public async Task<IEnumerable<TeklifListe>> Details(int id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@id", id);

            var list = await _db.QueryAsync<TeklifListe>(@$"select s.Id as TeklifId,s.TeklifAdi,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.TeklifTarihi,s.IslemTarihi,s.GecerlilikTarihi,s.Kur,s.IskontoOrani,s.IskontoBirim,s.IskontoGoster,s.StokKoduGoster,s.MarkaGoster
            from Teklifler s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId=@FirmaId and s.Id=@id and  s.Aktif=1", prm);
            foreach (var item in list)
            {
                string sql = $@"select se.Id,se.KategoriIsmi,se.TeklifId,se.StokAdi,se.Grup,se.Hediye,se.Aciklama,se.StokId,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,se.Hediye,se.IskontoBirim,se.BirimId,Birim.Isim as BirimIsmi
            from TeklifDetay se
			left join Stoklar on Stoklar.Id=se.StokId
            left join Birim on Birim.Id=se.BirimId
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={item.TeklifId} and SE.FirmaId={FirmaId}";
                var detay = await _db.QueryAsync<TeklifDetayInsertResponse>(sql);
                item.TeklifDetay = detay;
            }
            return list;
        }

        public async Task Update(TeklifUpdate T, int FirmaId, int KullanıcıId)
        {

            var parabirimi = await _db.QueryFirstAsync<int>(@$"select ParaBirimiId from Teklifler where Id={T.Id} and Aktif=1");
            var parabirimiIsmi = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={parabirimi}");
            if (T.ParaBirimiId == parabirimi)
            {
                DynamicParameters prm = new DynamicParameters();
                prm.Add("@Aktif", true);
                prm.Add("@MusteriId", T.MusteriId);
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@KullanıcıId", KullanıcıId);
                prm.Add("@Id", T.Id);
                prm.Add("@ParaBirimiId", T.ParaBirimiId);
                prm.Add("@IslemTarihi", DateTime.Now);
                prm.Add("@IskontoOrani", T.IskontoOrani);
                prm.Add("@IskontoBirim", T.IskontoBirim);
                prm.Add("@StokKoduGoster", T.StokKoduGoster);
                prm.Add("@MarkaGoster", T.MarkaGoster);
                prm.Add("@IskontoGoster", T.IskontoGoster);

                var sql = @"Update Teklifler set  MusteriId=@MusteriId,IskontoBirim=@IskontoBirim,StokKoduGoster=@StokKoduGoster,MarkaGoster=@MarkaGoster,IskontoGoster=@IskontoGoster,ParaBirimiId=@ParaBirimiId,IslemTarihi=@IslemTarihi,IskontoOrani=@IskontoOrani where Id=@Id and FirmaId=@FirmaId ";
                await _db.ExecuteAsync(sql, prm);

            }
            else if (T.ParaBirimiId != parabirimi)
            {
                var liste = await _db.QueryAsync<TeklifDetayInsertResponse>(@$"select * from TeklifDetay where TeklifId={T.Id} and Aktif=1 and FirmaId={FirmaId}");
                foreach (var item in liste)
                {
                    DynamicParameters prm = new();
                    prm.Add("@Id", item.Id);
                    prm.Add("@SepetId", item.TeklifId);
                    var requestparabirimi = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={T.ParaBirimiId}");
                    var requestpara = await _kur.TarihsizKur(requestparabirimi, FirmaId);

                    var stokparabirimi = await _kur.TarihsizKur(parabirimiIsmi, FirmaId);
                    var TLkarsiligi = item.BirimFiyat * stokparabirimi;
                    var sepetbirimkarsilik = TLkarsiligi / requestpara;
                    item.BirimFiyat = sepetbirimkarsilik;
                    prm.Add("fiyat", item.BirimFiyat);
                    prm.Add("Kur", requestpara);
                    prm.Add("FirmaId", FirmaId);

                    prm.Add("ParaBirimiId", T.ParaBirimiId);

                    var sql = @$"Update TeklifDetay set BirimFiyat=@fiyat where Id=@Id and Id=@Id and TeklifId=@SepetId";
                    await _db.ExecuteAsync(sql, prm);
                    var sqla = @$"Update Teklifler set  ParaBirimiId=@ParaBirimiId,Kur=@Kur where Id=@SepetId and FirmaId=@FirmaId ";
                    await _db.ExecuteAsync(sqla, prm);
                }

            }

        }

        public async Task UpdateDetay(TeklifDetayUpdate T, int FirmaId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KullanıcıId", KullanıcıId);
            prm.Add("@StokId", T.StokId);
            prm.Add("@Id", T.Id);
            prm.Add("@TeklifId", T.TeklifId);
            prm.Add("@Miktar", T.Miktar);
            prm.Add("@Aciklama", T.Aciklama);
            prm.Add("@Hediye", T.Hediye);
            prm.Add("@IskontoBirim", T.IskontoBirim);
            prm.Add("@IskontoBirim", T.Grup);

            prm.Add("@BirimFiyat", T.BirimFiyat);
            prm.Add("@KDVOrani", T.KDVOrani);
            prm.Add("@IskontoOrani", T.IskontoOrani);
            var sql = @"Update  TeklifDetay set Miktar=@Miktar,BirimFiyat=@BirimFiyat,Hediye=@Hediye,
            IskontoBirim=@IskontoBirim,Aciklama=@Aciklama,KDVOrani=@KDVOrani,Grup=@Grup,
            IskontoOrani=@IskontoOrani where Id=@Id and StokId=@StokId and TeklifId=@TeklifId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<string> Cizim(CizimA request, int TeklifId, int FirmaId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            int a = 1;
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<html>
<head>
<style>
@media print {{
    body {{
        width: 35.7cm;
        height: 24cm;
        margin:0;
        padding:0;
        /* change the margins as you want them to be. */
    }}
}}
img
{{
height:100%;
width:100%;
}}

</style>
</head>

<body>");
            foreach (var item in request.Resim)
            {
                System.IO.FileInfo FF = new FileInfo(item.FileName);
                var uzanti = FF.Extension;
                var isim = Path.Combine("Cizim/", "resim" + a + uzanti);


                using (Stream stream = new FileStream(isim, FileMode.Create))
                {
                    item.CopyTo(stream);
                }
                string path = @$"{_host.WebRootPath}/../Cizim" + $"\\resim{a}{uzanti}";
                sb.Append(@$"<img src=""{path}"" />");

            }
            sb.Append(@"</body></html>");
            return sb.ToString();
        }

        public async Task TeklifGonderme(int SepetId, int FirmaId, int KullanıcıId)
        {

            SmtpClient sc = new SmtpClient();
            sc.Port = 587;
            sc.Host = "smtp.gmail.com";
            sc.EnableSsl = true;

            sc.Credentials = new NetworkCredential("eposta@gmail.com", "gmail_sifre");

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("eposta@gmail.com", "Ekranda Görünecek İsim");

            mail.To.Add("alici1@mail.com");

            mail.Subject = "E-Posta Konusu";
            mail.IsBodyHtml = true;
            mail.Body = "E-Posta İçeriği";

            mail.Attachments.Add(new Attachment(@"C:\Rapor.xlsx"));
            mail.Attachments.Add(new Attachment(@"C:\Sonuc.pptx"));

            sc.Send(mail);

        }

    }
}


