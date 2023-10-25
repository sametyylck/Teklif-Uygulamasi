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
    public class DilRepository : IDil
    {
        private readonly IDbConnection _db;

        public DilRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(string? T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);

            var list = await _db.QueryFirstAsync<int>(@$"Select Count(*)as kayitsayisi from Dil where FirmaId=@FirmaId and (ISNULL(Dil.Isim,0) LIKE '%{T}%') and Aktif=1", prm);
            return list;
        }

        public async Task<int> Insert(string isim, int FirmaId)
        {
            if (isim=="TR")
            {
                DynamicParameters prm = new DynamicParameters();
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@Isim", isim);
                prm.Add("@Aktif", true);
                prm.Add("@Varsayilan", true);
                var sql = @"Insert into Dil (Isim,FirmaId,Aktif,Varsayilan) OUTPUT INSERTED.[Id]  values  (@Isim,@FirmaId,@Aktif,@Varsayilan)";
                return await _db.QuerySingleAsync<int>(sql, prm);
            }
            else
            {
                DynamicParameters prm = new DynamicParameters();
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@Isim", isim);
                prm.Add("@Aktif", true);
                prm.Add("@Varsayilan", false);
                var sql = @"Insert into Dil (Isim,FirmaId,Aktif,Varsayilan) OUTPUT INSERTED.[Id]  values  (@Isim,@FirmaId,@Aktif,@Varsayilan)";
                return await _db.QuerySingleAsync<int>(sql, prm);
            }
      
        }

        public async Task<IEnumerable<DilDTO>> List(string? T, int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            var list = await _db.QueryAsync<DilDTO>(@$"Select * from Dil where FirmaId=@FirmaId and (ISNULL(Dil.Isim,0) LIKE '%{T}%') and Aktif=1", prm);
            return list;
        }

    }
}
