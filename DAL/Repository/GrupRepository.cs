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
    public class GrupRepository : IGrup
    {
        private readonly IDbConnection _db;


        public GrupRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(string? T, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            string sql = $@"select Count(*)as kayitsayisi from TeklifGrup where FirmaId=@FirmaId AND Aktif=1";

            var list = await _db.QueryFirstAsync<int>(sql, prm);
            return list;
        }

        public async Task Delete(string harf,int TeklifId, int CompanyId, int KullanıcıId)
        {

            DynamicParameters prm = new DynamicParameters();
            prm.Add("@TeklifId", TeklifId);
            prm.Add("@harf", harf);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @"Delete from TeklifGrup where Harf=@Harf and FirmaId=@FirmaId and TeklifId=@TeklifId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<IEnumerable<GrupDTO>> Details(int T, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            string sql = $@"select * from TeklifGrup where FirmaId=@FirmaId AND Aktif=1 and TeklifId={T}";

            var list = await _db.QueryAsync<GrupDTO>(sql, prm);
            return list;

        }

        public async Task<int> Insert(List<GrupInsert> A, int CompanyId, int KullanıcıId)
        {
            foreach (var T in A)
            {
                DynamicParameters prm = new DynamicParameters();
                prm.Add("@Harf", T.Harf);
                prm.Add("@Anlami", T.Anlami);
                prm.Add("@TeklifId", T.TeklifId);
                prm.Add("@Aktif", true);
                prm.Add("@FirmaId", CompanyId);
                prm.Add("@KullanıcıId", KullanıcıId);

                var sql = @"Insert into TeklifGrup (TeklifId,Anlami,Harf,FirmaId,KullanıcıId) OUTPUT INSERTED.[Id]  values  (@TeklifId,@Anlami,@Harf,@FirmaId,@KullanıcıId)";
                await _db.QuerySingleAsync<int>(sql, prm);
            }
            return 1;
        }

        public async Task<IEnumerable<GrupDTO>> List(int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            string sql = $@"select * from TeklifGrup where FirmaId=@FirmaId AND Aktif=1)";

            var list = await _db.QueryAsync<GrupDTO>(sql, prm);
            return list;
        }

        public async Task Update(GrupUpdate T, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Harf", T.Harf);
            prm.Add("@Anlami", T.Anlami);
            prm.Add("@TeklifId", T.TeklifId);

            prm.Add("@Id", T.Id);
            prm.Add("@FirmaId", CompanyId);
            var sql = @"Update TeklifGrup set Harf=@Harf,Anlami=@Anlami where Id=@Id and TeklifId=@TeklifId and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
    }
}
