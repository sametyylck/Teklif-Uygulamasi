using DAL.DTO;
using DAL.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class RegisterRepository : IKullanıcılar
    {
         IDbConnection _db;

        public RegisterRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> KullanıcıInsert(RegisterDTO T,int firmaid)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@AdSoyad", T.AdSoyad);
            prm.Add("@KullaniciAdi", T.KullaniciAdi);
            prm.Add("@Sifre", T.Sifre);
            prm.Add("@Aktif", true);
            prm.Add("@Yonetici", true);
            prm.Add("@firma", firmaid);

            var sql = @"Insert into Kullanıcılar (AdSoyad,KullaniciAdi,Sifre,Aktif,Yonetici,FirmaId) OUTPUT INSERTED.[Id]  values  (@AdSoyad,@KullaniciAdi,@Sifre,@Aktif,@Yonetici,@firma)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }
        public async Task<int> FirmaInsert(RegisterDTO T)
        {
            DynamicParameters prm = new DynamicParameters();
 
            prm.Add("@Aktif", true);
            prm.Add("@FirmaAd", T.FirmaAd);
            prm.Add("@Tel", T.Tel);
            prm.Add("@Mail", T.Mail);
            prm.Add("@Address", T.Address);
            var sql = @"Insert into Firma (FirmaAd,Tel,Mail,Adres,Aktif) OUTPUT INSERTED.[Id]  values  (@FirmaAd,@Tel,@Mail,@Address,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }

   
    }
}
