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

namespace BL.Control.Sepet
{
    public class SepetControl : ISepetControl
    {
        private readonly IDbConnection _db;

        public SepetControl(IDbConnection db)
        {
            _db = db;
        }

        public async Task<string> Insert(int? ParaBirimiId, int? MusteriId, int FirmaId)
        {
            var list = await _db.QueryAsync<SepetInsert>(@$"select
            (select Id from Musteriler where Id={MusteriId} and FirmaId={FirmaId})as MusteriId,(select Id from ParaBirimleri where Id={ParaBirimiId})as ParaBirimiId");
            foreach (var item in list)
            {
                if (item.MusteriId==null)
                {
                    return ("MusteriId bulunamadı");

                }
                if (item.ParaBirimiId==null)
                {
                    return ("ParaBirimi bulunamadı");

                }
                else
                {
                    return("true");
                }
            }

            return ("true");
        }

        public async Task<string> InsertDetay(int StokId, int SepetId, int BirimId, int? Id, int FirmaId)
        {
            var list = await _db.QueryAsync<SepetDetayInsert>(@$"select
            (select Id from Stoklar where Id={StokId} and FirmaId={FirmaId})as StokId,(select Id from Sepet where Id={SepetId})as SepetId,(Select Id from Birim where Id={BirimId} and FirmaId={FirmaId})as BirimId,(Select Id from Birim where Id={BirimId} and FirmaId={FirmaId})as BirimId,");
            foreach (var item in list)
            {
                if (item.StokId == null)
                {
                    return ("StokId bulunamadı");

                }
                if (item.SepetId == null)
                {
                    return ("SepetId bulunamadı");

                }
                if (item.BirimId == null)
                {
                    return ("BirimId bulunamadı");

                }
                else
                {
                    return ("true");
                }
            }
            return("false");
        }
    }
}
