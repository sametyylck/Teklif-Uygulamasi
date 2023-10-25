using DAL.DTO;
using DAL.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class MusterilerRepository : IMusteriler
    {
        private readonly IDbConnection _db;

        public MusterilerRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(string? T, int CompanyId)
        {
            string sql = $@"select Count(*) as kayitsayisi from (
            Select Musteriler.Id,Musteriler.Unvan,Musteriler.Adres,Musteriler.Mail,Musteriler.Telefon,Musteriler.VergiNo,Musteriler.VergiDairesi from Musteriler 
            where Musteriler.FirmaId = {CompanyId} and Aktif=1 and
            (ISNULL(Musteriler.Adres,0) LIKE '%{T}%' 
            or ISNULL(Musteriler.Mail,0) LIKE '%{T}%' 
            or ISNULL(Musteriler.Telefon,0) LIKE '%{T}%'
            or ISNULL(Musteriler.Unvan,0) LIKE '%{T}%') )kayıtsayisi  
            ";

            var list = await _db.QueryFirstAsync<int>(sql);
            return list;
        }
        public async Task Delete(int id, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Update  Musteriler set FirmaId=@FirmaId , DeleteDate=@Date ,Aktif=@Aktif, DeleteKullanıcıId=@KullanıcıId where Id=@Id";
            await _db.ExecuteAsync(sql, prm);
        }
        public async Task<IEnumerable<MusteriUpdate>> Detail(IdControl T,int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@id", T.Id);
            prm.Add("@CompanyId", FirmaId);

            var list = await _db.QueryAsync<MusteriUpdate>($"select * from Musteriler where Id=@id and FirmaId=@CompanyId", prm);
            return list.ToList();
        }
        public async Task<int> Insert(MusteriInsert T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Unvan", T.Unvan);
            prm.Add("@Adres", T.Adres);
            prm.Add("@Telefon", T.Telefon);
            prm.Add("@Mail", T.Mail);
            prm.Add("@VergiDairesi", T.VergiDairesi);
            prm.Add("@VergiNo", T.VergiNo);
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId",KullanıcıId);

            var sql = @"Insert into Musteriler (Unvan,VergiNo,VergiDairesi,Adres,Telefon,Mail,FirmaId,KullanıcıId,Aktif) OUTPUT INSERTED.[Id]  values  (@Unvan,@VergiNo,@VergiDairesi,@Adres,@Telefon,@Mail,@FirmaId,@KullanıcıId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }
        public async Task<IEnumerable<MusteriUpdate>> List(string? T, int CompanyId, int KullanıcıId, int? KAYITSAYISI, int? SAYFA)
        {
            if (KAYITSAYISI == null || SAYFA == null)
            {
                string sql = $@"
            Select Musteriler.Id,Musteriler.Unvan,Musteriler.Adres,Musteriler.Mail,Musteriler.Telefon,Musteriler.VergiNo,Musteriler.VergiDairesi from Musteriler 
            where Musteriler.FirmaId = {CompanyId} and Aktif=1 and
            (ISNULL(Musteriler.Adres,0) LIKE '%{T}%' 
            or ISNULL(Musteriler.Mail,0) LIKE '%{T}%' 
            or ISNULL(Musteriler.Telefon,0) LIKE '%{T}%'
            or ISNULL(Musteriler.Unvan,0) LIKE '%{T}%')
          ";

                var list = await _db.QueryAsync<MusteriUpdate>(sql);
                return list;
            }
            else
            {
                string sql = $@"DECLARE @KAYITSAYISI int DECLARE @SAYFA int SET @KAYITSAYISI ={KAYITSAYISI}
            SET @SAYFA = {SAYFA} 
            Select Musteriler.Id,Musteriler.Unvan,Musteriler.Adres,Musteriler.Mail,Musteriler.Telefon,Musteriler.VergiNo,Musteriler.VergiDairesi from Musteriler 
            where Musteriler.FirmaId = {CompanyId} and Aktif=1 and
            (ISNULL(Musteriler.Adres,0) LIKE '%{T}%' 
            or ISNULL(Musteriler.Mail,0) LIKE '%{T}%' 
            or ISNULL(Musteriler.Telefon,0) LIKE '%{T}%'
            or ISNULL(Musteriler.Unvan,0) LIKE '%{T}%')
            ORDER BY Musteriler.Id OFFSET @KAYITSAYISI * (@SAYFA - 1) ROWS FETCH NEXT @KAYITSAYISI ROWS ONLY; ";

                var list = await _db.QueryAsync<MusteriUpdate>(sql);
                return list;
            }


        }
        public async Task Update(MusteriUpdate T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Unvan", T.Unvan);
            prm.Add("@Id", T.Id);
            prm.Add("@Adres", T.Adres);
            prm.Add("@Telefon", T.Telefon);
            prm.Add("@Mail", T.Mail);
            prm.Add("@VergiDairesi", T.VergiDairesi);
            prm.Add("@VergiNo", T.VergiNo);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);
            var sql = @"Update Musteriler set Unvan=@Unvan,VergiDairesi=@VergiDairesi,VergiNo=@VergiNo,Adres=@Adres,Telefon=@Telefon,Mail=@Mail where Id=@Id";
            await _db.ExecuteAsync(sql, prm);
        }



        public async Task<int> InsertAdres(AdresInsert T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@AdresAdi", T.AdresAdi);
            prm.Add("@MusteriId", T.MusteriId);
            prm.Add("@Adres", T.Adres);
            prm.Add("@AdSoyad", T.AdSoyad);
            prm.Add("@Mail", T.Mail);
            prm.Add("@SirketIsmi", T.SirketIsmi);
            prm.Add("@Tel", T.Tel);
            prm.Add("@Ulke", T.Ulke);
            prm.Add("@Sehir", T.Sehir);
            prm.Add("@Ilce", T.Ilce);
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Insert into Adres (AdresAdi,MusteriId,SirketIsmi,Adres,AdSoyad,Mail,Tel,Ulke,Sehir,Ilce,FirmaId,KullanıcıId,Aktif) OUTPUT INSERTED.[Id]  values  (@AdresAdi,@MusteriId,@SirketIsmi,@Adres,@AdSoyad,@Mail,@Tel,@Ulke,@Sehir,@Ilce,@FirmaId,@KullanıcıId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }
        public async Task UpdateAdres(AdresUpdate T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@AdresAdi", T.AdresAdi);
            prm.Add("@MusteriId", T.MusteriId);
            prm.Add("@Id", T.Id);
            prm.Add("@Adres", T.Adres);
            prm.Add("@AdSoyad", T.AdSoyad);
            prm.Add("@Mail", T.Mail);
            prm.Add("@SirketIsmi", T.SirketIsmi);
            prm.Add("@Tel", T.Tel);
            prm.Add("@Ulke", T.Ulke);
            prm.Add("@Sehir", T.Sehir);
            prm.Add("@Ilce", T.Ilce);
            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);
            var sql = @"Update Adres set AdresAdi=@AdresAdi,Adres=@Adres,AdSoyad=@AdSoyad,Mail=@Mail,SirketIsmi=@SirketIsmi,Tel=@Tel,Ulke=@Ulke,Sehir=@Sehir,Ilce=@Ilce where Id=@Id and MusteriId=@MusteriId and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
        public async Task<IEnumerable<AdresDTO>> ListAdres(string? T, int CompanyId, int KullanıcıId, int? KAYITSAYISI, int? SAYFA)
        {
            if (KAYITSAYISI == null || SAYFA == null)
            {
                string sql = $@" Select Adres.Id,Adres.MusteriId,Adres.AdresAdi,Adres.AdSoyad,Adres.SirketIsmi,Adres.Tel,Adres.Mail,Adres.Ulke,Adres.Sehir,Adres.Ilce,Adres.Adres from Adres 
            where Adres.FirmaId = {CompanyId} and Aktif=1     ";
                var list = await _db.QueryAsync<AdresDTO>(sql);
                return list;
            }
            else
            {
                string sql = $@"DECLARE @KAYITSAYISI int DECLARE @SAYFA int SET @KAYITSAYISI ={KAYITSAYISI}
            SET @SAYFA = {SAYFA} 
            Select Adres.Id,Adres.MusteriId,Adres.AdresAdi,Adres.AdSoyad,Adres.SirketIsmi,Adres.Tel,Adres.Mail,Adres.Ulke,Adres.Sehir,Adres.Ilce,Adres.Adres from Adres 
            where Adres.FirmaId = {CompanyId} and Aktif=1 
            ORDER BY Adres.Id OFFSET @KAYITSAYISI * (@SAYFA - 1) ROWS FETCH NEXT @KAYITSAYISI ROWS ONLY; ";

                var list = await _db.QueryAsync<AdresDTO>(sql);
                return list;
            }


        }
        public async Task<IEnumerable<AdresDTO>> DetailAdres(int  T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@id", T);
            prm.Add("@CompanyId", FirmaId);

            var list = await _db.QueryAsync<AdresDTO>($"select * from Adres where Id=@id and FirmaId=@CompanyId", prm);
            return list.ToList();
        }
        public async Task DeleteAdres(int id, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Update Adres set FirmaId=@FirmaId , DeleteDate=@Date ,Aktif=@Aktif, DeleteKullanıcıId=@KullanıcıId where Id=@Id";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<int> CountAdres(string? T, int CompanyId)
        {
            string sql = $@" Select Adres.Id,Adres.MusteriId,Adres.AdresAdi,Adres.AdSoyad,Adres.SirketIsmi,Adres.Tel,Adres.Mail,Adres.Ulke,Adres.Sehir,Adres.Ilce,Adres.Adres from Adres 
            where Adres.FirmaId = {CompanyId} and Aktif=1     ";
            var list = await _db.QueryAsync<AdresDTO>(sql);
            return list.Count();
        }
    }
}
