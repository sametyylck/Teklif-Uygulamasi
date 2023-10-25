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
    public class BirimRepository : IBirim
    {
        private readonly IDbConnection _db;
        public BirimRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Count(string? T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            var list = await _db.QueryFirstAsync<int>(@$"Select Count(*) as kayitsaiyisi from Birim where FirmaId=@FirmaId and ISNULL(Birim.Isim,0) LIKE '%{T}%'", prm);
            return list;
        }

        public async Task Delete(int Id, int FİrmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FİrmaId);
            var sql = @"Delete from  Birim where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<int> Insert(BirimInsert T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@Isim", T.Isim);
            prm.Add("@Aktif", true);


            var sql = @"Insert into Birim (Isim,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@Isim,@FirmaId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }

        public async Task<IEnumerable<BirimDTO>> List(string? T, int CompanyId)
        {

            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);

            var list = await _db.QueryAsync<BirimDTO>(@$"Select * from Birim where FirmaId=@FirmaId and ISNULL(Birim.Isim,0) LIKE '%{T}%'", prm);
            return list;
        }

        public async Task Update(BirimUpdate t, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@Isim", t.Isim);
            prm.Add("@Id", t.Id);
            var sql = @"Update Birim set Isim= @Isim where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
    }
}
