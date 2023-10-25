using DAL.DTO;
using DAL.Interface;
using DAL.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class StoklarRepository : IStoklar
    {
        private readonly IDbConnection _db;
        public StoklarRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(int? dil, string? T, int CompanyId, int? KAYITSAYISI, int? SAYFA)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            if (dil == null)
            {
                var bit = await _db.QueryFirstAsync<int>(@$"select Id from Dil where Varsayilan=1 and Aktif=1", prm);
                prm.Add("@Dil", bit);

            }
            else
            {
                prm.Add("@Dil", dil);

            }
            if (KAYITSAYISI==null || SAYFA==null)
            {
                var liste = await _db.QueryFirstAsync<int>(@$"select Count(*) as kayitsayisi from(
            Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi ,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
            where Stoklar.FirmaId=@FirmaId and Stoklar.Aktif=1 and Stoklar.Dil_id=@Dil and
            (ISNULL(StokKategori.Isim,0) LIKE '%{T}%' 
              or ISNULL(StokAdi,0) LIKE '%{T}%' 
             or ISNULL(StokKodu,0) LIKE '%{T}%'
             or ISNULL(ParaBirimleri.Kod,0) LIKE '%{T}%' or
             ISNULL(Aciklama,0) LIKE '%{T}%' 
              or ISNULL(Birim.Isim,0) LIKE '%{T}%' 
             or ISNULL(BirimFiyat,0) LIKE '%{T}%'
             or ISNULL(KDVOranı,0) LIKE '%{T}%' or
             ISNULL(Dil.Isim,0) LIKE '%{T}%'))as kayitsayisi ", prm);
                return liste;
            }
            else
            {

                var list = await _db.QueryFirstAsync<int>(@$"    select Count(*) as kayitsayisi from(
            Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik 
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
            where Stoklar.FirmaId=@FirmaId and Stoklar.Aktif=1 and Stoklar.Dil_id=@Dil and
            (ISNULL(StokKategori.Isim,0) LIKE '%{T}%' 
              or ISNULL(StokAdi,0) LIKE '%{T}%' 
             or ISNULL(StokKodu,0) LIKE '%{T}%'
             or ISNULL(ParaBirimleri.Kod,0) LIKE '%{T}%' or
             ISNULL(Aciklama,0) LIKE '%{T}%' 
              or ISNULL(Birim.Isim,0) LIKE '%{T}%' 
             or ISNULL(BirimFiyat,0) LIKE '%{T}%'
             or ISNULL(KDVOranı,0) LIKE '%{T}%' or
             ISNULL(Dil.Isim,0) LIKE '%{T}%')
             )as kayitsayisi", prm);
                return list;
            }

        }

        public async Task Delete(int Id, int FirmaId,int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KullanıcıId", KullanıcıId);
            var sql = @"Update  Stoklar set  DeleteDate=@Date ,Aktif=@Aktif, DeleteKullanıcıId=@KullanıcıId where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
        public async Task<int> Insert(StoklarInsert T,int FirmaId,int KullanıcıId)
        {
            var desi = (T.Boy * T.Yukseklik * T.En) / 3000;

            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KategoriId", T.KategoriId);
            prm.Add("@StokAdı", T.StokAdı);
            prm.Add("@StokKodu", T.StokKodu);
            prm.Add("@Aciklama", T.Aciklama);
            prm.Add("@ParaBirimiId", T.ParaBirimiId);
            prm.Add("@BirimId", T.BirimId);
            prm.Add("@BirimFiyat", T.BirimFiyat);
            prm.Add("@KDVOranı", T.KDVOranı);
            prm.Add("@Dil_id", T.Dil_id);
            prm.Add("@Marka", T.Marka);
            prm.Add("@AmbalajAgirlik", T.AmbalajAgirlik);
            prm.Add("@AmbalajsizAgirlik", T.AmbalajsizAgirlik);
            prm.Add("@Desi",desi);
            prm.Add("@En", T.En);
            prm.Add("@Boy", T.Boy);
            prm.Add("@Yukseklik", T.Yukseklik);


            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Insert into Stoklar (KategoriId,StokAdi,StokKodu,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik,Aciklama,ParaBirimiId,BirimId,BirimFiyat,KDVOranı,Dil_id,FirmaId,KullanıcıId,Aktif) OUTPUT INSERTED.[Id]  values  (@KategoriId,@StokAdı,@StokKodu,@Marka,@AmbalajAgirlik,@AmbalajsizAgirlik,@Desi,@En,@Boy,@Yukseklik,@Aciklama,@ParaBirimiId,@BirimId,@BirimFiyat,@KDVOranı,@Dil_id,@FirmaId,@KullanıcıId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }
        public async Task<IEnumerable<StoklarListResponse>> List(int? dil,string? kelime, int CompanyId, int? KAYITSAYISI, int? SAYFA)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            if (dil==null)
            {
                var bit = await _db.QueryFirstAsync<int>(@$"select Id from Dil where Varsayilan=1 and Aktif=1", prm);
                prm.Add("@Dil", bit);

            }
            else
            {
                prm.Add("@Dil", dil);

            }
            if (KAYITSAYISI==null || SAYFA==null)
            {
                var list = await _db.QueryAsync<StoklarListResponse>(@$"
            Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi,StokResim.Resim,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik  
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
			left join StokResim on StokResim.StokId=Stoklar.Id and StokResim.Aktif=1

            where Stoklar.FirmaId=@FirmaId and Stoklar.Aktif=1 and Stoklar.Dil_id=@Dil and
            (ISNULL(StokKategori.Isim,0) LIKE '%{kelime}%' 
              or ISNULL(StokAdi,0) LIKE '%{kelime}%' 
             or ISNULL(StokKodu,0) LIKE '%{kelime}%'
             or ISNULL(ParaBirimleri.Kod,0) LIKE '%{kelime}%' or
             ISNULL(Aciklama,0) LIKE '%{kelime}%' 
              or ISNULL(Birim.Isim,0) LIKE '%{kelime}%' 
             or ISNULL(BirimFiyat,0) LIKE '%{kelime}%'
             or ISNULL(KDVOranı,0) LIKE '%{kelime}%' or
             ISNULL(Dil.Isim,0) LIKE '%{kelime}%') ", prm);
         
                return list;
            }
            else
            {
                var list = await _db.QueryAsync<StoklarListResponse>(@$"DECLARE @KAYITSAYISI int DECLARE @SAYFA int SET @KAYITSAYISI ={KAYITSAYISI}  SET @SAYFA = {SAYFA} 
            Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi,StokResim.Resim,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik  
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
			left join StokResim on StokResim.StokId=Stoklar.Id and StokResim.Aktif=1

            where Stoklar.FirmaId=@FirmaId and Stoklar.Aktif=1 and Stoklar.Dil_id=@Dil and
            (ISNULL(StokKategori.Isim,0) LIKE '%{kelime}%' 
              or ISNULL(StokAdi,0) LIKE '%{kelime}%' 
             or ISNULL(StokKodu,0) LIKE '%{kelime}%'
             or ISNULL(ParaBirimleri.Kod,0) LIKE '%{kelime}%' or
             ISNULL(Aciklama,0) LIKE '%{kelime}%' 
              or ISNULL(Birim.Isim,0) LIKE '%{kelime}%' 
             or ISNULL(BirimFiyat,0) LIKE '%{kelime}%'
             or ISNULL(KDVOranı,0) LIKE '%{kelime}%' or
             ISNULL(Dil.Isim,0) LIKE '%{kelime}%')
              ORDER BY Stoklar.Id OFFSET @KAYITSAYISI * (@SAYFA - 1) ROWS FETCH NEXT @KAYITSAYISI ROWS ONLY;", prm);
                return list;
            }
     

        }

        public async Task<IEnumerable<StoklarDetails>> Details(int id,int Dil, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            //Girilen id'nin parent mi yoksa degilmi kontrolu
            var stokid = await _db.QueryFirstAsync<int>(@$"select ISNULL(StokId,0) from Stoklar where Id={id} and Aktif=1", prm);
            var dil_id = await _db.QueryFirstAsync<int>(@$"select
            ISNULL((select ISNULL(StokId,0) from Stoklar where StokId={id} and Aktif=1 and Dil_id={Dil}),0)", prm);


            if (stokid==0 && dil_id==0)
            {
                var list = await _db.QueryAsync<StoklarDetails>(@$"
            Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
            where Stoklar.FirmaId={CompanyId} and Stoklar.Aktif=1 and Stoklar.Id={id} and Stoklar.Dil_id={Dil}", prm);
                foreach (var item in list)
                {
                    var RESİM = await _db.QueryAsync<ImageDetail>(@$"
            select  * from StokResim where StokId={item.Id} and Aktif=1", prm);
                    item.Resim = RESİM;
                }
                return list;
            }
            else
            {
                var list = await _db.QueryAsync<StoklarDetails>(@$"
            Select Stoklar.Id,Stoklar.KategoriId,StokKategori.Isim as KategoriIsmi,Stoklar.StokAdi,Stoklar.StokKodu,Stoklar.Aciklama,Stoklar.ParaBirimiId,
            ParaBirimleri.Kod as ParaBirimiIsmi,Stoklar.BirimId,Birim.Isim as BirimIsimi,Stoklar.BirimFiyat,Stoklar.KDVOranı,Stoklar.Dil_id,Dil.Isim as DilIsmi,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik 
            from Stoklar
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on ParaBirimleri.Id=ParaBirimiId
            left join Birim on Birim.Id=BirimId
            left join Dil on Dil.Id=Stoklar.Dil_id
            where Stoklar.FirmaId={CompanyId} and Stoklar.Aktif=1 and Stoklar.StokId={id} and Stoklar.Dil_id={Dil}", prm);

                foreach (var item in list)
                {
                    var RESİM = await _db.QueryAsync<ImageDetail>(@$"
                     select * from StokResim where StokId={item.Id} and Aktif=1", prm);
                    item.Resim = RESİM;

                }
                return list;
            }


            

        }

        public async Task Update(StoklarUpdate T, int FirmaId,int KullanıcıId)
        {
            //Update ve Insert(dil) ayni anda

            var stokid = await _db.QueryFirstAsync<int>(@$"select ISNULL((select ISNULL(id,0) from Stoklar where Id={T.Id} and Aktif=1 and Dil_id={T.Dil_id}),0)");
            var dil_id = await _db.QueryFirstAsync<int>(@$"select
            ISNULL((select ISNULL(StokId,0) from Stoklar where StokId={T.Id} and Aktif=1 and Dil_id={T.Dil_id}),0)");

            if (stokid!=0)
            {
                var desi = (T.Boy * T.Yukseklik * T.En) / 3000;

                DynamicParameters prm = new DynamicParameters();
                prm.Add("@Id", T.Id);
                prm.Add("@Aktif", true);
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@KategoriId", T.KategoriId);
                prm.Add("@StokAdı", T.StokAdı);
                prm.Add("@StokKodu", T.StokKodu);
                prm.Add("@Aciklama", T.Aciklama);
                prm.Add("@Marka", T.Marka);
                prm.Add("@AmbalajAgirlik", T.AmbalajAgirlik);
                prm.Add("@AmbalajsizAgirlik", T.AmbalajsizAgirlik);
                prm.Add("@Desi", desi);
                prm.Add("@En", T.En);
                prm.Add("@Boy", T.Boy);
                prm.Add("@Yukseklik", T.Yukseklik);
                prm.Add("@ParaBirimiId", T.ParaBirimiId);
                prm.Add("@BirimId", T.BirimId);
                prm.Add("@BirimFiyat", T.BirimFiyat);
                prm.Add("@KDVOranı", T.KDVOranı);
                prm.Add("@Dil_id", T.Dil_id);
                var sql = @"Update Stoklar set KategoriId=@KategoriId,StokAdi=@StokAdı,StokKodu=@StokKodu,Aciklama=@Aciklama,BirimId=@BirimId,ParaBirimiId=@ParaBirimiId,BirimFiyat=@BirimFiyat,KDVOranı=@KDVOranı,Marka=@Marka,AmbalajAgirlik=@AmbalajAgirlik,AmbalajsizAgirlik=@AmbalajsizAgirlik,Desi=@Desi,En=@En,Boy=@Boy,Yukseklik=@Yukseklik where Id=@Id and FirmaId=@FirmaId and Dil_id=@Dil_id";
                await _db.ExecuteAsync(sql, prm);
            }
            else
            {
                if (dil_id==0)
                {
                    var desi = (T.Boy * T.Yukseklik * T.En) / 3000;
                    DynamicParameters prm = new DynamicParameters();
                    prm.Add("@StokId", T.Id);
                    prm.Add("@Aktif", true);
                    prm.Add("@FirmaId", FirmaId);
                    prm.Add("@KategoriId", T.KategoriId);
                    prm.Add("@StokAdı", T.StokAdı);
                    prm.Add("@StokKodu", T.StokKodu);
                    prm.Add("@Aciklama", T.Aciklama);
                    prm.Add("@Marka", T.Marka);
                    prm.Add("@AmbalajAgirlik", T.AmbalajAgirlik);
                    prm.Add("@AmbalajsizAgirlik", T.AmbalajsizAgirlik);
                    prm.Add("@Desi", desi);
                    prm.Add("@En", T.En);
                    prm.Add("@Boy", T.Boy);
                    prm.Add("@Yukseklik", T.Yukseklik);
                    prm.Add("@ParaBirimiId", T.ParaBirimiId);
                    prm.Add("@BirimId", T.BirimId);
                    prm.Add("@BirimFiyat", T.BirimFiyat);
                    prm.Add("@KDVOranı", T.KDVOranı);
                    prm.Add("@Dil_id", T.Dil_id);
                    prm.Add("@KullanıcıId", KullanıcıId);
                    var sql = @"Insert into Stoklar (StokId,KategoriId,StokAdi,StokKodu,Aciklama,AmbalajAgirlik,AmbalajsizAgirlik,ParaBirimiId,BirimId,BirimFiyat,KDVOranı,Dil_id,FirmaId,KullanıcıId,Aktif) OUTPUT INSERTED.[Id]  values  (@StokId,@KategoriId,@StokAdı,@StokKodu,@Aciklama,@AmbalajAgirlik,@AmbalajsizAgirlik,@ParaBirimiId,@BirimId,@BirimFiyat,@KDVOranı,@Dil_id,@FirmaId,@KullanıcıId,@Aktif)";
                   int id= await _db.QuerySingleAsync<int>(sql, prm);

                }
                else
                {

                    var desi = (T.Boy * T.Yukseklik * T.En) / 3000;
                    DynamicParameters prm = new DynamicParameters();
                    prm.Add("@Id", T.Id);
                    prm.Add("@Aktif", true);
                    prm.Add("@FirmaId", FirmaId);
                    prm.Add("@KategoriId", T.KategoriId);
                    prm.Add("@StokAdı", T.StokAdı);
                    prm.Add("@StokKodu", T.StokKodu);
                    prm.Add("@Aciklama", T.Aciklama);
                    prm.Add("@Marka", T.Marka);
                    prm.Add("@AmblajAgirlik", T.AmbalajAgirlik);
                    prm.Add("@AmblajsizAgirlik", T.AmbalajsizAgirlik);
                    prm.Add("@Desi", desi);
                    prm.Add("@En", T.En);
                    prm.Add("@Boy", T.Boy);
                    prm.Add("@Yukseklik", T.Yukseklik);
                    prm.Add("@ParaBirimiId", T.ParaBirimiId);
                    prm.Add("@BirimId", T.BirimId);
                    prm.Add("@BirimFiyat", T.BirimFiyat);
                    prm.Add("@KDVOranı", T.KDVOranı);
                    prm.Add("@Dil_id", T.Dil_id);

                    var sql = @"Update Stoklar set KategoriId=@KategoriId,StokAdi=@StokAdı,StokKodu=@StokKodu,Aciklama=@Aciklama,BirimId=@BirimId,ParaBirimiId=@ParaBirimiId,BirimFiyat=@BirimFiyat,KDVOranı=@KDVOranı,Marka=@Marka,AmblajAgirlik=@AmblajAgirlik,AmbalajsizAgirlik=@AmbalajsizAgirlik,Desi=@Desi,En=@En,Boy=@Boy,Yukseklik=@Yukseklik where StokId=@Id and FirmaId=@FirmaId and Dil_id=@Dil_id";
                    await _db.ExecuteAsync(sql, prm);
                }


            }




            

           

        }
    }
}
