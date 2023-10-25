using DAL.DTO;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Stoklar
{
    public class StoklarService : IStoklarService
    {
        private readonly IDbConnection _db;

        public StoklarService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<string> Insert(StoklarInsert T, int FirmaId)
        {
            var list = await _db.QueryAsync<StoklarInsert>(@$"select
            (select Id from StokKategori where Id={T.KategoriId} and FirmaId={FirmaId})as KategoriId,(select Id from ParaBirimleri where Id={T.ParaBirimiId})as ParaBirimiId,(Select Id from Birim where Id={T.BirimId} and FirmaId={FirmaId})as BirimId,(select Id from Dil where Id={T.Dil_id} and FirmaId={FirmaId})as Dil_id");

            if (list.First().Dil_id == null)
            {
                return ("Girilen Dil bulunamadi.");
            }
            if (list.First().KategoriId == null)
            {
                return ("Girilen katagori bulunamadı");
            }
            if (list.First().ParaBirimiId == null)
            {
                return ("Girilen para birimi bulunamadı");
            }
            if (list.First().BirimId == null)
            {
                return ("Girilen birim bulunamadı");
            }
            else
            {
                return ("true");
            }

        }
        public async Task<string> Update(StoklarUpdate T, int FirmaId)
        {
            var list = await _db.QueryAsync<StoklarDTO>(@$"select
			(select Id from ParaBirimleri where Id={T.ParaBirimiId})as ParaBirimiId,
			(Select Id from Birim where Id={T.BirimId} and FirmaId={FirmaId} )as BirimId,
			(select Id from Stoklar where Id ={T.Id} and FirmaId ={FirmaId} and Aktif=1 )as Id,
            (select Id from StokKategori where Id={T.KategoriId} and FirmaId={FirmaId} and Aktif=1 ) as KategoriId
");

           
            if (list.First().KategoriId == null)
            {
                return ("Girilen katagori bulunamadı");
            }
            if (list.First().ParaBirimiId == null)
            {
                return ("Girilen para birimi bulunamadı");
            }
            if (list.First().BirimId == null)
            {
                return ("Girilen birim bulunamadı");
            }
            if (list.First().Id == null)
            {
                return ("Girilen Id bulunamadı");
            }
            else
            {
                return ("true");
            }
        }
    }
}
