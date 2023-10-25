using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace TMRApi.Controllers
{
    [Route("api/[controller]"),Authorize]
    [ApiController]
    public class KullanıcıController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IKullanıcı _kullanici;
        private readonly IUserService _userservice;
        public KullanıcıController(IDbConnection db, IKullanıcı kullanici, IUserService userservice)
        {
            _db = db;
            _kullanici = kullanici;
            _userservice = userservice;
        }

        [HttpPost("Insert")]
        public async Task<ActionResult<KullanıcıInsertResponse>> Insert( KullanıcıInsert request)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _kullanici.Insert(request,CompanyId,KullanıcıId);
            string sql = $@"Select * from Kullanıcılar where Id={id}";
            var list = await _db.QueryAsync<KullanıcıInsertResponse>(sql);
            return Ok(list);







        }

        [HttpPost("Update")]
        public async Task<ActionResult<KullanıcıUpdate>> Update( KullanıcıUpdate request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _kullanici.Update(request,CompanyId,KullanıcıId);
            string sql = $@"Select * from Kullanıcılar where Id={request.Id}";
            var list = await _db.QueryAsync<RegisterResponse>(sql);
            return Ok(list);



        }

        [HttpPost("Delete")]
        public async Task<ActionResult<RegisterDTO>> Delete(int request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];

            await _kullanici.Delete(request,CompanyId,KullanıcıId);
            return Ok("Bsarılı");



        }
        [HttpPost("List")]
        public async Task<ActionResult<KullanıcıInsertResponse>> List(string? request)
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _kullanici.List(request,CompanyId);
            var count = await _kullanici.Count(request, CompanyId);
            return Ok(new {list,count});



        }

        [HttpPost("Detail")]
        public async Task<ActionResult<KullanıcıInsertResponse>> Detail(int request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _kullanici.Details(request,CompanyId);
            return Ok(list);

        }



    }
}
