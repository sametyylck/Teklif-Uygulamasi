using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Data;

namespace TMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeklifGrupController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IGrup _grup;
        private readonly IUserService _userservice;


        public TeklifGrupController(IDbConnection db, IGrup grup, IUserService userservice)
        {
            _db = db;
            _grup = grup;
            _userservice = userservice;
        }

        [HttpGet]
        public async Task<ActionResult<RegisterDTO>> HarfListe()
        {
            var list = await _db.QueryAsync<string>(@$"select * from Grup");

            return Ok(list);

        }
        [HttpPost("Insert"), Authorize]
        public async Task<ActionResult<RegisterDTO>> Insert(List<GrupInsert> T)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _grup.Insert(T, CompanyId, KullanıcıId);
            return Ok();

        }
        [HttpPut("Update"), Authorize]
        public async Task<ActionResult<RegisterDTO>> Update(GrupUpdate T)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _grup.Update(T, CompanyId, KullanıcıId);

            return Ok();

        }
        [HttpGet("Details"), Authorize]
        public async Task<ActionResult<GrupDTO>> Details(int id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list= await _grup.Details(id,CompanyId);

            return Ok(list);

        }
        [HttpGet("List"), Authorize]
        public async Task<ActionResult<GrupDTO>> List()
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _grup.List(CompanyId);

            return Ok(list);

        }
        [HttpDelete("Delete"), Authorize]
        public async Task<ActionResult<RegisterDTO>> Delete(string Harf,int TeklifId)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _grup.Delete(Harf, TeklifId,CompanyId,KullanıcıId);
            return Ok();

        }
    }
}
