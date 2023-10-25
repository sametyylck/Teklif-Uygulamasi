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
    public class KullanıcıRepository : IKullanıcı
    {
        private readonly IDbConnection _db;

        public KullanıcıRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(string? T, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            var list = await _db.QueryAsync<KullanıcıInsertResponse>(@$"Select * from Kullanıcılar where FirmaId=@FirmaId and Yonetici=0 and Aktif=1", prm);
            return list.Count();
        }

        public async Task Delete(int Id, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Update  Kullanıcılar set  DeleteDate=@Date ,Aktif=@Aktif, DeleteKullanıcıId=@KullanıcıId where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<IEnumerable<KullanıcıInsertResponse>> Details(int T, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@Id", T);
            var list = await _db.QueryAsync<KullanıcıInsertResponse>(@$"Select * from Kullanıcılar where FirmaId=@FirmaId  and Yonetici=0 and Id=@Id and Aktif=1", prm);
            return list;
        }

        public async Task<int> Insert(KullanıcıInsert T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@AdSoyad", T.AdSoyad);
            prm.Add("@KullaniciAdi", T.KullaniciAdi);
            prm.Add("@Sifre", T.Sifre);
            prm.Add("@Aktif", true);
            prm.Add("@Yonetici", false);
            prm.Add("@firma", CompanyId);

            var sql = @"Insert into Kullanıcılar (AdSoyad,KullaniciAdi,Sifre,Aktif,Yonetici,FirmaId) OUTPUT INSERTED.[Id]  values  (@AdSoyad,@KullaniciAdi,@Sifre,@Aktif,@Yonetici,@firma)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }

        public async Task<IEnumerable<KullanıcıInsertResponse>> List(string? T, int CompanyId)
        {

            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            var list = await _db.QueryAsync<KullanıcıInsertResponse>(@$"Select * from Kullanıcılar where FirmaId=@FirmaId and ISNULL(Kullanıcılar.AdSoyad,0) LIKE '%{T}%' and ISNULL(Kullanıcılar.KullaniciAdi,0) LIKE '%{T}%' and Yonetici=0 and Aktif=1", prm);
            return list;
        }

        public async Task Update(KullanıcıUpdate T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", T.Id);

            prm.Add("@AdSoyad", T.AdSoyad);
            prm.Add("@KullaniciAdi", T.KullaniciAdi);
            prm.Add("@Sifre", T.Sifre);
            prm.Add("@Aktif", true);
            prm.Add("@Yonetici", true);
            prm.Add("@FirmaId", CompanyId);

            var sql = @"Update Kullanıcılar  set AdSoyad=@AdSoyad,KullaniciAdi=@KullaniciAdi,Sifre=@Sifre where Id=@Id and FirmaId=@FirmaId and Yonetici=0";
            await _db.ExecuteAsync(sql, prm);

        }
    }
}
