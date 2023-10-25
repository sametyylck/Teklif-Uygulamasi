using DAL.DTO;
using DAL.Interface;
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
    public class StokKategoriRepository : IStokKategori
    {
        private readonly IDbConnection _db;

        public StokKategoriRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(string? T,int Dil,int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            string sql = $@"select Count(*)as kayitsayisi from StokKategori where FirmaId=@FirmaId AND Aktif=1 and (ISNULL(StokKategori.Isim,0) LIKE '%{T}%') and Dil_id={Dil}";

            var list = await _db.QueryFirstAsync<int>(sql, prm);
            return list;
        }

        public async Task Delete(int Id, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Update  StokKategori set  DeleteDate=@Date ,Aktif=@Aktif, DeleteKullanıcıId=@KullanıcıId where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }



        public async Task<int> Insert(StocKategoriInsert T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Isim", T.Isim);
            prm.Add("@Dil_id", T.Dil_id);

            prm.Add("@Aktif", true);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Insert into StokKategori (Isim,FirmaId,KullanıcıId,Aktif,Dil_id) OUTPUT INSERTED.[Id]  values  (@Isim,@FirmaId,@KullanıcıId,@Aktif,@Dil_id)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }

        public async Task<IEnumerable<StocKategoriDTO>> List(string? T,int Dil,int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            string sql = $@"select * from StokKategori where FirmaId=@FirmaId AND Aktif=1 and  (ISNULL(StokKategori.Isim,0) LIKE '%{T}%') and Dil_id={Dil}";

            var list = await _db.QueryAsync<StocKategoriDTO>(sql,prm);
            return list;
        }
        public async Task<IEnumerable<StocKategoriDTO>> Details(int id,int Dil, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            var stokid = await _db.QueryFirstAsync<int>(@$"select ISNULL(KategoriParentid,0) from StokKategori where Id={id} and Aktif=1", prm);
            var dil_id = await _db.QueryFirstAsync<int>(@$"select
            ISNULL((select ISNULL(KategoriParentid,0) from StokKategori where KategoriParentid={id} and Aktif=1 and Dil_id={Dil}),0)", prm);

            if (stokid==0 && dil_id==0)
            {
                string sql = $@"select * from StokKategori where FirmaId=@FirmaId AND Aktif=1 and Id={id} and Dil_id={Dil}";

                var list = await _db.QueryAsync<StocKategoriDTO>(sql, prm);
                return list;
            }
            else
            {
                string sql = $@"select * from StokKategori where FirmaId=@FirmaId AND Aktif=1 and KategoriParentid={id} and Dil_id={Dil}";

                var list = await _db.QueryAsync<StocKategoriDTO>(sql, prm);
                return list;
            }

         
        }

        public async Task Update(StocKategoriDTO T,int CompanyId,int KullanıcıId )
        {

            var kategoriId = await _db.QueryFirstAsync<int>(@$"select ISNULL((select ISNULL(id,0) from StokKategori where Id={T.Id} and Aktif=1 and Dil_id={T.Dil_id}),0)");
            var dil_id = await _db.QueryFirstAsync<int>(@$"select
            ISNULL((select ISNULL(KategoriParentid,0) from StokKategori where KategoriParentid={T.Id} and Aktif=1 and Dil_id={T.Dil_id}),0)");
            if (kategoriId!=0)
            {
                DynamicParameters prm = new DynamicParameters();
                prm.Add("@Isim", T.Isim);
                prm.Add("@Id", T.Id);
                prm.Add("@FirmaId", CompanyId);
                var sql = @"Update StokKategori set Isim=@Isim where Id=@Id and FirmaId=@FirmaId";
                await _db.ExecuteAsync(sql, prm);
            }
            else
            {
                if (dil_id==0)
                {
                    DynamicParameters prm = new DynamicParameters();
                    prm.Add("@Isim", T.Isim);
                    prm.Add("@Dil_id", T.Dil_id);
                    prm.Add("@KategoriParentid", T.Id);

                    prm.Add("@Aktif", true);
                    prm.Add("@FirmaId", CompanyId);
                    prm.Add("@KullanıcıId", KullanıcıId);

                    var sql = @"Insert into StokKategori (Isim,KategoriParentid,FirmaId,KullanıcıId,Aktif,Dil_id) OUTPUT INSERTED.[Id]  values  (@Isim,@KategoriParentid,@FirmaId,@KullanıcıId,@Aktif,@Dil_id)";
                    await _db.QuerySingleAsync<int>(sql, prm);
                }
                else
                {
                    DynamicParameters prm = new DynamicParameters();
                    prm.Add("@Isim", T.Isim);
                    prm.Add("@Id", T.Id);
                    prm.Add("@FirmaId", CompanyId);
                    var sql = @$"Update StokKategori set Isim=@Isim where KategoriParentid=@Id and FirmaId=@FirmaId and Dil_id={T.Dil_id}";
                    await _db.ExecuteAsync(sql, prm);
                }
            }
        
        }
    }
}
