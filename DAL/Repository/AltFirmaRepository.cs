using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class AltFirmaRepository : IAltFirmaRepository
    {
        private readonly IDbConnection _db;
        private readonly IHostingEnvironment _host;

        public AltFirmaRepository(IDbConnection db, IHostingEnvironment host)
        {
            _db = db;
            _host = host;
        }

        public async Task AltFirmaDelete(int id, int FirmaId,int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@KullanıcıId", KullanıcıId);

            var sql = @$"Update  Firma set  DeleteDate=@Date ,Aktif=@Aktif, DeleteKullanıcıId=@KullanıcıId where Id=@Id and Id!={FirmaId}";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<int> AltFirmaInsert(AltFirmaInsert T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaAd", T.FirmaAd);
            prm.Add("@Adres", T.Adres);
            prm.Add("@Tel", T.Tel);
            prm.Add("@Mail", T.Mail);
            prm.Add("@Aktif", true);
            prm.Add("@ParentId", FirmaId);

            var sql = @"Insert into Firma (FirmaAd,Adres,Tel,Mail,ParentId,Aktif) OUTPUT INSERTED.[Id]  values  (@FirmaAd,@Adres,@Tel,@Mail,@ParentId,@Aktif)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }

        public async Task AltFirmaUpdate(AltFirmaUpdate T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaAd", T.FirmaAd);
            prm.Add("@Id", T.Id);
            prm.Add("@Adres", T.Adres);
            prm.Add("@Tel", T.Tel);
            prm.Add("@Mail", T.Mail);
            prm.Add("@FirmaId", FirmaId);
            var sql = @"Update Firma set FirmaAd=@FirmaAd,Adres=@Adres,Tel=@Tel,Mail=@Mail where Id=@Id";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<int> Count(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            string sql = $@"select Count(*) from Firma where ParentId={FirmaId} and Aktif=1";

            var list = await _db.QueryFirstAsync<int>(sql, prm);
            return list;
        }
        public async Task<int> TumCount(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            string sql = $@"select Count(*) from Firma where Aktif=1 or ParentId={FirmaId}";

            var list = await _db.QueryFirstAsync<int>(sql, prm);
            return list;
        }


        public async Task<IEnumerable<AltFirmaDetails>> Details(int AltFirmaId, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            string sql = $@"select f.Id,f.FirmaAd,f.Tel,f.Mail,f.Adres from Firma f 
   
            where f.Id={AltFirmaId} and  Aktif=1";

            var list = await _db.QueryAsync<AltFirmaDetails>(sql, prm);
            foreach (var item in list)
            {
                string sqlq = $@"select fo.Id,fo.Logo,fo.AltMetin,fo.AltFirmaId from FirmaOzellikler fo 
            where fo.AltFirmaId={AltFirmaId} and FirmaId={FirmaId}";

                var detay = await _db.QueryAsync<AltFirmaOzellikDetail>(sqlq, prm);
                item.Ozellik = detay;
            }
            return list;
        }

        public async Task<IEnumerable<AltFirmaList>> List(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            string sql = $@"select * from Firma where ParentId=@FirmaId and Aktif=1";

            var list = await _db.QueryAsync<AltFirmaList>(sql, prm);
            return list;
        }
        public async Task<IEnumerable<AltFirmaList>> TumFirmaList(int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            string sql = $@"select * from Firma where Aktif=1 or ParentId={FirmaId}";

            var list = await _db.QueryAsync<AltFirmaList>(sql, prm);
            return list;
        }


        public async Task<int> OzellikInsert(AltFirmaOzellikInsert T, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            var LogoPath = " ";
            if (T.Logo!=null)
            {
                LogoPath = Path.Combine(_host.WebRootPath, T.Logo.FileName);
                using (Stream stream = new FileStream(LogoPath, FileMode.Create))
                {
                    T.Logo.CopyTo(stream);
                }
            }
            prm.Add("@AltFirmaId", T.AltFirmaId);
            prm.Add("@AltMetin", T.AltMetin);
            prm.Add("@LogoPath", T.Logo.FileName);
            prm.Add("@FirmaId", FirmaId);

            var sql = @"Insert into FirmaOzellikler (AltFirmaId,AltMetin,Logo,FirmaId) OUTPUT INSERTED.[Id]  values  (@AltFirmaId,@AltMetin,@LogoPath,@FirmaId)";
            return await _db.QuerySingleAsync<int>(sql, prm);
        }

        public async Task OzellikUpdate(AltFirmaOzellikUpdate T, int FirmaId)
        {
           DynamicParameters prm = new DynamicParameters();
            var LogoPath = "";
            if (T.Logo != null)
            {
                LogoPath = Path.Combine(_host.WebRootPath, T.Logo.FileName);
                using (Stream stream = new FileStream(LogoPath, FileMode.Create))
                {
                    T.Logo.CopyTo(stream);
                }

            }
            prm.Add("@Id", T.Id);
            prm.Add("@FirmaId", FirmaId);
            prm.Add("@AltMetin", T.AltMetin);
            prm.Add("@LogoPath", T.Logo.FileName);

            var sql = @$"Update  FirmaOzellikler set  AltMetin=@AltMetin ,Logo=@LogoPath where Id=@Id and FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }
    }
}
