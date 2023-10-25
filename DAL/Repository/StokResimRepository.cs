using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class StokResimRepository : IStokResim
    {
        private readonly IDbConnection _db;
        private readonly IHostingEnvironment _host;
       

        public StokResimRepository(IDbConnection db, IHostingEnvironment host)
        {
            _db = db;
            _host = host;
        }


        public async Task Delete(int Id, int CompanyId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Id", Id);
            prm.Add("@Date", DateTime.Now);
            prm.Add("@Aktif", false);
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@KullanıcıId", KullanıcıId);


            var sql = @"Update StokResim set Aktif=@Aktif,DeleteDate=@Date,DeleteKullanıcıId=@KullanıcıId where  Id=@Id  and  FirmaId=@FirmaId";
            await _db.ExecuteAsync(sql, prm);
        }

        public async Task<IEnumerable<ImageList>> Details(int StokId, int FirmaId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", FirmaId);
            string sql = $@"select * from StokResim where FirmaId=@FirmaId and StokId={StokId} ";

            var list = await _db.QueryAsync<ImageList>(sql, prm);
            return list;
        }

        public async Task<int> Insert(Image request,int StokId, int FirmaId, int KullanıcıId)
        {


            DynamicParameters prm = new DynamicParameters();
            
               
              var isim = Path.Combine(_host.WebRootPath, request.Resim.FileName);
                using (Stream stream = new FileStream(isim, FileMode.Create))
                {
                    request.Resim.CopyTo(stream);
                }
                prm.Add("@StokId", StokId);
                prm.Add("@Aktif", true);
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@Resim", request.Resim.FileName);
                prm.Add("@KullanıcıId", KullanıcıId);

                var sql = @"Insert into StokResim (StokId,Resim,FirmaId,KullanıcıId,Aktif) OUTPUT INSERTED.[Id]  values  (@StokId,@Resim,@FirmaId,@KullanıcıId,@Aktif)";
                return await _db.QuerySingleAsync<int>(sql, prm);
            
         


        }

        public async Task<IEnumerable<ImageList>> List(int CompanyId)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            string sql = $@"select * from StokResim where FirmaId=@FirmaId";

            var list = await _db.QueryAsync<ImageList>(sql,prm);
            return list;
        }
        public async Task Update(ImageUpdate T,int StokId,int Id,int FirmaId,int KullanıcıId)
        {
           
            Image B = new();
            B.Resim = T.Resim;
           await Delete(Id, FirmaId, KullanıcıId);
            await Insert(B,StokId, FirmaId, KullanıcıId);
        }
    }
}

