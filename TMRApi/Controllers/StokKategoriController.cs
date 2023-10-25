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
    [Route("api/[controller]")]
    [ApiController]
    public class StokKategoriController : ControllerBase
    {
        private readonly IStokKategori _stokkategori;
        private readonly IUserService _userservice;
        private readonly IDbConnection _db;
        private readonly IDControl _idcontrol;

        public StokKategoriController(IStokKategori stokkategori, IUserService userservice, IDbConnection db, IDControl idcontrol)
        {
            _stokkategori = stokkategori;
            _userservice = userservice;
            _db = db;
            _idcontrol = idcontrol;
        }

        [HttpPost("Insert"), Authorize]
        public async Task<ActionResult<StocKategoriInsert>> Insert(StocKategoriInsert request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _stokkategori.Insert(request, CompanyId, KullanıcıId);
            string sql = $@"Select * from StokKategori where Id={id}";
            var list = await _db.QueryAsync<StocKategoriDTO>(sql);
            return Ok(list);
        }


        [HttpPut("Update"), Authorize]
        public async Task<ActionResult<StocKategoriDTO>> Update(StocKategoriDTO request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _idcontrol.IdControl(request.Id, "StokKategori", CompanyId);
            if (hata == "true")
            {
                await _stokkategori.Update(request, CompanyId, KullanıcıId);
                var dil_id = await _db.QueryFirstAsync<int>(@$"select
            ISNULL((select ISNULL(KategoriParentid,0) from StokKategori where KategoriParentid={request.Id} and Aktif=1 and Dil_id={request.Dil_id}),0)");
                if (dil_id==0)
                {
                    string sql = $@"Select * from StokKategori where Id={request.Id}";
                    var list = await _db.QueryAsync<StocKategoriDTO>(sql);
                    return Ok(list);
                }
                else
                {
                    string sql = $@"Select * from StokKategori where KategoriParentid={request.Id}";
                    var list = await _db.QueryAsync<StocKategoriDTO>(sql);
                    return Ok(list);
                }

                
            }
            else
            {
                return BadRequest(hata);
            }











        }
        [HttpDelete("Delete"), Authorize]
        public async Task<ActionResult<StocKategoriDTO>> Delete(int Id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _idcontrol.IdControl(Id, "StokKategori", CompanyId);
            if (hata == "true")
            {
                await _stokkategori.Delete(Id, CompanyId, KullanıcıId);
                return Ok("Silme islemi basarili");
            }
            else
            {
                return BadRequest(hata);
            }

        }


        [HttpPost("List"), Authorize]
        public async Task<ActionResult<StocKategoriDTO>> List(string? T,int dil)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _stokkategori.List(T,dil,CompanyId);
            var count = await _stokkategori.Count(T,dil,CompanyId);

            return Ok(new { list, count });








        }

        [HttpPost("Details"), Authorize]

        public async Task<ActionResult<StocKategoriDTO>> Details(int id, int dil)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _stokkategori.Details(id, dil, CompanyId);

            return Ok(list);








        }


    }
}
