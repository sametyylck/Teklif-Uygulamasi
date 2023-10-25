using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Resim
{
    public class Resim : IResim
    {
        private readonly IDbConnection _db;
        private readonly IHostingEnvironment _host;

        public Resim(IDbConnection db, IHostingEnvironment host)
        {
            _db = db;
            _host = host;
        }

        public async Task<string> Eklimi(IFormFile Resim,int StokId, int FirmaId)
        {
            DynamicParameters prm = new();
            string sql = $@"Select * from StokResim where StokId={StokId} and Aktif=1 and FirmaId={FirmaId}";
            var ekli = await _db.QueryAsync(sql);
            if (ekli.Count()!=0)
            {
                return ("İd'de resim ekli ");
            }

            var yol = _host.WebRootPath;
            var isim = yol + Resim.FileName;
            prm.Add("@Path", isim);
            string sqlquery = $@"Select * from StokResim where Resim=@Path";
            var list = await _db.QueryAsync(sqlquery, prm);
            if (list.Count() != 0)
            {
                return (isim + " " + "Boyle bir isimde resim ekli!");
            }
            return ("true");
        }
    }
}
