using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.IdControl
{
    public class IdControlService : IDControl
    {
        private readonly IDbConnection _db;

        public IdControlService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<string> IdControl(int? id, string tablo, int FirmaId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@Tablo", tablo);
            param.Add("id", id);
            param.Add("@CompanyId", FirmaId);
            if (tablo == "Firma")
            {
                string sql = $"Select Id from {tablo} where Id=@id";
                var kontrol = await _db.QueryAsync<int>(sql, param);
                if (kontrol.Count() == 0)
                {
                    return ("Boyle bir id yok.");
                }
                else
                {
                    return ("true");
                }
            }
            else
            {
                string sql = $"Select Id from {tablo} where Id=@id and FirmaId = @CompanyId";
                var kontrol = await _db.QueryAsync<int>(sql, param);
                if (kontrol.Count() == 0)
                {
                    return ("Boyle bir id yok.");
                }
                else
                {
                    return ("true");
                }
            }
        }
    }
}
