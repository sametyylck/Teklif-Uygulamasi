using BL.Control.IdControl;
using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Validation.Kullanicilar;

namespace TMRApi.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class BirimController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IUserService _userservice;
        private readonly IBirim _birim;
        private readonly IDControl _idcontrol;


        public BirimController(IDbConnection db, IUserService userservice, IBirim birim, IDControl idcontrol)
        {
            _db = db;
            _userservice = userservice;
            _birim = birim;
            _idcontrol = idcontrol;
        }

        [HttpPost("Insert")]
        public async Task<ActionResult<BirimDTO>> Insert(BirimInsert request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int id = await _birim.Insert(request, CompanyId);
            string sql = $@"Select * from Birim where Id={id}";
            var list = await _db.QueryAsync<BirimDTO>(sql);
            return Ok(list);


        }
        [HttpPut("Update")]
        public async Task<ActionResult<BirimUpdate>> Update(BirimUpdate request)
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var hata = await _idcontrol.IdControl(request.Id, "Birim", CompanyId);
            if (hata == "true")
            {
                int KullanıcıId = getlist[1];
                await _birim.Update(request, CompanyId);
                string sql = $@"Select * from Birim where Id={request.Id}";
                var list = await _db.QueryAsync<BirimUpdate>(sql);
                return Ok(list);
            }
            else
            {
                return BadRequest(hata);
            }

        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<RegisterDTO>> Delete(int id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _birim.Delete(id, CompanyId);
            return Ok("Silme islemi basarili");

        }

        [HttpPost("List")]
        public async Task<ActionResult<BirimDTO>> List(string? T)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _birim.List(T, CompanyId);
            var count = await _birim.Count(T, CompanyId);
            return Ok(new { list, count });

        }

    

    }
}
