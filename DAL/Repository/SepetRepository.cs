using DAL.DTO;
using DAL.Interface;
using DAL.Service.KurService;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class SepetRepository : ISepet
    {
        private readonly IDbConnection _db;
        private readonly IKurService _kur;
        public SepetRepository(IDbConnection db, IKurService kur)
        {
            _db = db;
            _kur = kur;
        }

        public async Task Delete(int Id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            var sqlquery = @$"Select Id from SepetDetay where SepetId={Id}";
            var list = await _db.QueryAsync<IdControl>(sqlquery);
            foreach (var item in list)
            {
                var sql = @$"Delete from SepetDetay where SepetId={Id} and FirmaId={FirmaId}";
                await _db.ExecuteAsync(sql, prm);
            }
            var sqlkomut = @$"Delete from Sepet where Id={Id} and FirmaId={FirmaId}";
            await _db.ExecuteAsync(sqlkomut, prm);


        }
        public async Task DeleteDetay(int Id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            var sql = @"Delete from SepetDetay where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task DeleteSoft(int Id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            var sql = @"Update  Sepet set Aktif=@Aktif where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
        public async Task<int> Count(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);

            var list = await _db.QueryFirstAsync<int>(@$"select Count(*)as kayitsayisi from(
            select s.Id as SepetId,s.MusteriId,s.SepetAdi, Musteriler.Unvan as MusteriUnvani,s.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,s.IslemTarihi,
            s.Kur,s.IskontoOrani
            from Sepet s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId=@FirmaId and s.Aktif=1)as kayitsayisi", prm);
            return list;
        }

        public async Task<int> Insert(SepetInsert T, int FirmaId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            var kod = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={T.ParaBirimiId}");
            var kur = await _kur.TarihsizKur(kod, FirmaId);
            prm.Add("@Aktif", true);
            prm.Add("@MusteriId", T.MusteriId);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KullanıcıId", KullanıcıId);
            prm.Add("@SepetAdi", T.SepetAdi);
            prm.Add("@ParaBirimiId", T.ParaBirimiId);
            prm.Add("@IslemTarihi", DateTime.Now);
            prm.Add("@Kur",kur);
            prm.Add("@IskontoOrani", T.IskontoOrani);
            prm.Add("@IskontoBirim", T.IskontoBirim);


            var sql = @"Insert into Sepet (MusteriId,IskontoBirim,SepetAdi,ParaBirimiId,IslemTarihi,IskontoOrani,KullanıcıId,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@MusteriId,@IskontoBirim,@SepetAdi,@ParaBirimiId,@IslemTarihi,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }
        public async Task<int> InsertDetay(SepetDetayInsert T,  int FirmaId, int KullanıcıId)
        {
            //ayni ürün varsa miktar artırıyoruz.
            var list = await _db.QueryAsync<SepetDTO>(@$"select (select StokId from SepetDetay where StokId={T.StokId} and SepetId={T.SepetId})as StokId,
                (select Miktar from SepetDetay where StokId={T.StokId} and Aktif=1 and SepetId={T.SepetId})as Miktar,
                     (select Id from SepetDetay where StokId={T.StokId} and Aktif=1 and SepetId={T.SepetId})as SepetDetayId,
                    (select ParaBirimiId from Sepet where Id={T.SepetId} and Aktif=1 )as ParaBirimiId ");
            var kod = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={list.First().ParaBirimiId}");

            var sepetParabirimi = await _kur.TarihsizKur(kod,FirmaId);
            if (list.First().StokId != null)
            {
                DynamicParameters prm = new DynamicParameters();
                var yenimiktar = T.Miktar + list.First().Miktar;
                prm.Add("@miktar", yenimiktar);
                var sqlquery = @$"Update SepetDetay set  Miktar=@miktar where  StokId={T.StokId} and SepetId={T.SepetId} and FirmaId={FirmaId} ";
                await _db.ExecuteAsync(sqlquery, prm);
                return list.First().SepetDetayId;

            }
            else
            {


                DynamicParameters prm = new DynamicParameters();
                var liste = await _db.QueryAsync<StoklarInsertResponse>(@$"select KategoriId,StokKategori.Isim as KatagoriIsmi,StokAdi,Aciklama,StokKodu,BirimFiyat,KDVOranı,ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi from Stoklar
                      left join StokKategori on StokKategori.Id=Stoklar.KategoriId
					  left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
                       where Stoklar.Id={T.StokId} and Stoklar.Aktif=1 and Stoklar.FirmaId={FirmaId}");

                foreach (var item in liste)
                {

                    if (item.ParaBirimiId == list.First().ParaBirimiId)
                    {

                    }
                    else
                    {
                        var stokparabirimi = await _kur.TarihsizKur(item.ParaBirimiIsmi, FirmaId);
                        var TLkarsiligi = item.BirimFiyat * stokparabirimi;
                        var sepetbirimkarsilik = TLkarsiligi / sepetParabirimi;
                        item.BirimFiyat = sepetbirimkarsilik;
                    }


                    prm.Add("@Aktif", true);
                    prm.Add("@FirmaId", FirmaId);
                    prm.Add("@KullanıcıId", KullanıcıId);
                    prm.Add("@SepetId", T.SepetId);
                    prm.Add("@StokId", T.StokId);
                    prm.Add("@KategoriId", item.KatagoriIsmi);
                    prm.Add("@StokKodu", item.StokKodu);
                    prm.Add("@Miktar", T.Miktar);
                    prm.Add("@StokAdı", item.StokAdi);
                    prm.Add("@BirimId", T.BirimId);
                    prm.Add("@Aciklama", item.Aciklama);
                    prm.Add("@BirimFiyat", item.BirimFiyat);
                    prm.Add("@KDVOrani", item.KDVOranı);
                    prm.Add("@IskontoOrani", T.IskontoOrani);
                    prm.Add("@Hediye", T.Hediye);
                    prm.Add("@Grup", T.Grup);
                    prm.Add("@IskontoBirim", T.IskontoBirim);
                    prm.Add("@Aktif", true);

                }

                var sql = @"Insert into SepetDetay (SepetId,StokAdi,Hediye,IskontoBirim,Grup,BirimId,KategoriIsmi,Aciklama,StokId,StokKodu,Miktar,BirimFiyat,KDVOrani,IskontoOrani,KullanıcıId,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@SepetId,@StokAdı,@Hediye,@IskontoBirim,@Grup,@BirimId,@KategoriId,@Aciklama,@StokId,@StokKodu,@Miktar,@BirimFiyat,@KDVOrani,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif)";
                return await _db.QuerySingleAsync<int>(sql, prm);


            }


        }
        public async Task<IEnumerable<SepetListResponse>> List(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);

            var list = await _db.QueryAsync<SepetListResponse>(@$"select s.Id as SepetId,s.MusteriId,s.SepetAdi, Musteriler.Unvan as MusteriUnvani,s.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,s.IslemTarihi,
            s.Kur,s.IskontoOrani,s.IskontoBirim
            from Sepet s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId=@FirmaId and s.Aktif=1", prm);

            return list;
        }
        public async Task<IEnumerable<StoklarListResponse>> DilKontrol(int? StokId, int dil, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@dil", dil);
            prm.Add("@StokId", StokId);
            var stokid = await _db.QueryFirstAsync<int>(@$"select ISNULL((select ISNULL(id,0) from Stoklar where Id={StokId} and Aktif=1 and Dil_id={dil}),0)");
            if (stokid != 0)
            {

                var list = await _db.QueryAsync<StoklarListResponse>(@$"Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi 
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId and StokKategori.Dil_id=@dil
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
            where Stoklar.FirmaId=@FirmaId and Stoklar.Aktif=1 and Stoklar.Id=@StokId and Stoklar.Dil_id=@dil", prm);
                return list;
            }
            else
            {

                var list = await _db.QueryAsync<StoklarListResponse>(@$"Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi 
            from Stoklar
            left join StokKategori on StokKategori.KategoriParentid=KategoriId and StokKategori.Dil_id=@dil
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
            where Stoklar.FirmaId=@FirmaId and Stoklar.Aktif=1 and Stoklar.StokId=@StokId and Stoklar.Dil_id=@dil", prm);
                return list;
            }

        }

        public async Task Update(SepetUpdate T, int FirmaId, int KullanıcıId)
        {
            var parabirimi = await _db.QueryFirstAsync<int>(@$"select ParaBirimiId from Sepet where Id={T.Id} and Aktif=1");

            var parabirimiIsmi = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={parabirimi}");
            var kod = await _db.QueryFirstAsync<string>(@$"select Kod from ParaBirimleri where Id={T.ParaBirimiId}");

            var sepetParabirimi = await _kur.TarihsizKur(kod, FirmaId);
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
                prm.Add("@IskontoBirim", T.IskontoBirim);

                prm.Add("@IskontoOrani", T.IskontoOrani);
                var sql = @"Update Sepet set  MusteriId=@MusteriId,ParaBirimiId=@ParaBirimiId,IslemTarihi=@IslemTarihi,IskontoBirim=@IskontoBirim,IskontoOrani=@IskontoOrani where Id=@Id and FirmaId=@FirmaId ";
                await _db.ExecuteAsync(sql, prm);
            }
            if (T.ParaBirimiId != parabirimi)
            {
                var liste = await _db.QueryAsync<SepetDetayInsertResponse>(@$"select * from SepetDetay where SepetId={T.Id} and Aktif=1 and FirmaId={FirmaId}");
                foreach (var item in liste)
                {
                    DynamicParameters prm = new();
                    prm.Add("@Id", item.Id);
                    prm.Add("@SepetId", item.SepetId);

                    var stokparabirimi = await _kur.TarihsizKur(parabirimiIsmi, FirmaId);
                    var TLkarsiligi = item.BirimFiyat * stokparabirimi;
                    var sepetbirimkarsilik = TLkarsiligi / sepetParabirimi;
                    item.BirimFiyat = sepetbirimkarsilik;
                    prm.Add("fiyat", item.BirimFiyat);
                    prm.Add("FirmaId", FirmaId);

                    prm.Add("ParaBirimiId", T.ParaBirimiId);

                    var sql = @$"Update SepetDetay set BirimFiyat=@fiyat where Id=@Id and Id=@Id and SepetId=@SepetId";
                    await _db.ExecuteAsync(sql, prm);
                  
                }
            }



        }



        public async Task UpdateDetay(SepetDetayUpdate T, int FirmaId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KullanıcıId", KullanıcıId);
            prm.Add("@StokId", T.StokId);
            prm.Add("@Id", T.Id);
            prm.Add("@SepetId", T.SepetId);
            prm.Add("@Miktar", T.Miktar);
            prm.Add("@BirimFiyat", T.BirimFiyat);
            prm.Add("@IskontoOrani", T.IskontoOrani);
            prm.Add("@IskontoBirim", T.IskontoBirim);
            prm.Add("@Hediye", T.Hediye);
            prm.Add("@Grup", T.Grup);


            var sql = @"Update SepetDetay set Miktar=@Miktar,Grup=@Grup,BirimFiyat=@BirimFiyat,Hediye=@Hediye,IskontoBirim=@IskontoBirim,IskontoOrani=@IskontoOrani where Id=@Id and StokId=@StokId and SepetId=@SepetId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<IEnumerable<SepetList>> Details(int id, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@id", id);

            var list = await _db.QueryAsync<SepetList>(@$"select s.Id as SepetId,s.AltFirmaId,Firma.FirmaAd,s.SepetAdi,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.IslemTarihi,s.Kur,s.IskontoOrani,s.IskontoBirim
            from Sepet s
            left join Firma on Firma.Id=s.AltFirmaId
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId=@FirmaId and s.Id=@id and  s.Aktif=1", prm);

            //Detayi ayri bir list icerisinde gösterme
            foreach (var item in list)
            {
                prm.Add("@SepetId", item.SepetId);

                var detay = await _db.QueryAsync<SepetDetayInsertResponse>(@$"select sd.Id as Id,sd.SepetId,sd.StokId,
            sd.StokAdi,sd.StokKodu,sd.Aciklama,sd.KategoriIsmi,sd.BirimId,Birim.Isim as BirimIsmi,sd.KdvOrani,sd.Miktar,sd.BirimFiyat,sd.IskontoOrani,sd.IskontoBirim,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,sd.Hediye,sd.Grup
            from SepetDetay sd  
			left join Stoklar on Stoklar.Id=sd.StokId
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
			left join Birim On Birim.id=sd.BirimId
            where sd.FirmaId=@FirmaId and sd.Aktif=1 and sd.SepetId=@SepetId", prm);
                item.SepetDetay = detay;
            }
            return list;
        }
    }
}
