using BL.Control.Resim;
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
    [Route("api/[controller]")]
    [ApiController]
    public class StokResimController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IUserService _userservice;
        private readonly IStokResim _stokresim;
        private readonly IResim _resim;
        public StokResimController(IDbConnection db, IUserService userservice, IStokResim stokresim, IResim resim)
        {
            _db = db;
            _userservice = userservice;
            _stokresim = stokresim;
            _resim = resim;
        }

        [HttpPost("Insert"), Authorize]
        public async Task<ActionResult<Image>> Insert([FromForm] Image request, int StokId)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _resim.Eklimi(request.Resim, StokId, CompanyId);
            if (hata == "true")
            {
                await _stokresim.Insert(request, StokId, CompanyId, KullanıcıId);

                string sql = $@"Select * from StokResim where StokId={StokId} and FirmaId={CompanyId}";
                var list = await _db.QueryAsync(sql);
                return Ok(list);
            }
            else
            {

                return BadRequest(hata);
            }

        }


        [HttpPut("Update"), Authorize]
        public async Task<ActionResult<ImageUpdate>> Update([FromForm] ImageUpdate request, int StokId, int Id)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];

            await _stokresim.Update(request, StokId, Id, CompanyId, KullanıcıId);
            string sql = $@"Select * from StokResim where Id={Id}";

            var list = await _db.QueryAsync<ImageUpdate>(sql);
            return Ok(list);


        }
        [HttpDelete("Delete"), Authorize]
        public async Task<ActionResult<IdControl>> Delete(int id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _stokresim.Delete(id, CompanyId, KullanıcıId);
            return Ok("Silme islemi basarili");
        }


        [HttpPost("List"), Authorize]
        public async Task<ActionResult<RegisterDTO>> List()
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _stokresim.List(CompanyId);
            return Ok(list);
        }

        [HttpPost("Details"), Authorize]
        public async Task<ActionResult<RegisterDTO>> Details(int StokId)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _stokresim.Details(StokId, CompanyId);
            return Ok(list);
        }
    }
}

