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
    [Route("api/[controller]"),Authorize]
    [ApiController]
    public class AltFirmaController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IDbConnection _db;
        private readonly IAltFirmaRepository _firma;

        public AltFirmaController(IUserService userservice, IDbConnection db, IAltFirmaRepository firma)
        {
            _userservice = userservice;
            _db = db;
            _firma = firma;
        }

        [HttpPost("AltFirmaInsert")]
        public async Task<ActionResult<AltFirmaInsert>> AltFirmaInsert(AltFirmaInsert request)
        {
           
                var getlist = _userservice.GetId();
                int CompanyId = getlist[0];
                 int id = await _firma.AltFirmaInsert(request, CompanyId);
            string sql = $@"select * from Firma f 
            where f.Id={id} and ParentId={CompanyId} and Aktif=1";

            var list = await _db.QueryAsync<AltFirmaUpdate>(sql);
            return Ok(list);
           
        }
        [HttpPut("Update")]
        public async Task<ActionResult<AltFirmaUpdate>> Update(AltFirmaUpdate request)
        {
           
                var getlist = _userservice.GetId();
                int CompanyId = getlist[0];
                int KullanıcıId = getlist[1];
                await _firma.AltFirmaUpdate(request, CompanyId);
            string sql = $@"select * from Firma f 
            where f.Id={request.Id} and ParentId={CompanyId} and Aktif=1";

            var list = await _db.QueryAsync<AltFirmaUpdate>(sql);
            return Ok(list);
                
   

        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<AltFirmaDTO>> Delete(int id)
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _firma.AltFirmaDelete(id, CompanyId, KullanıcıId);
            return Ok("Silme islemi basarili");
          
        }
        [HttpPost("Details")]
        public async Task<ActionResult<AltFirmaDTO>> Details(int AltFirmaId)
        {
            

                var getlist = _userservice.GetId();
                int CompanyId = getlist[0];
                int KullanıcıId = getlist[1];
                 var list = await _firma.Details(AltFirmaId, CompanyId);
                return Ok(list);

        }
        [HttpPost("List")]
        public async Task<ActionResult<AltFirmaDTO>> List()
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _firma.List(CompanyId);
            var count = await _firma.Count(CompanyId);

            return Ok(new { list,count });

        }

        [HttpPost("TumFirmaList")]

        public async Task<ActionResult<AltFirmaDTO>> TumFirmaList()
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _firma.TumFirmaList(CompanyId);
            var count = await _firma.TumCount(CompanyId);

            return Ok(new { list,count});

        }



        [HttpPost("AltFirmaOzellikInsert")]

        public async Task<ActionResult<AltFirmaOzellikInsert>> AltFirmaOzellikInsert([FromForm]AltFirmaOzellikInsert request)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            string sqla = $@"select * from  FirmaOzellikler where AltFirmaId={request.AltFirmaId}";
            var liste = await _db.QueryAsync<AltFirmaDTO>(sqla);
            if (liste.Count()!=0)
            {
                return BadRequest("Bu altfirmanın ozelligi ekli");
            }
            int id = await _firma.OzellikInsert(request, CompanyId);
            string sql = $@"select f.Id,f.FirmaAd,f.Tel,f.Mail,f.Adres,fo.Logo,fo.Id as OzellikId,fo.AltMetin,fo.AltFirmaId from Firma f 
            left join FirmaOzellikler fo on fo.AltFirmaId=f.Id
            where f.Id={id} and Aktif=1 and FirmaId={CompanyId}";

            var list = await _db.QueryAsync<AltFirmaDTO>(sql);
            return Ok(list);
        }
        [HttpPost("AltFirmaOzellikUpdate")]

        public async Task<ActionResult<AltFirmaOzellikUpdate>> AltFirmaOzellikUpdate([FromForm] AltFirmaOzellikUpdate request)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _firma.OzellikUpdate(request, CompanyId);
            string sql = $@"select f.Id,f.FirmaAd,f.Tel,f.Mail,f.Adres,fo.Logo,fo.AltMetin,fo.AltFirmaId from Firma f 
            left join FirmaOzellikler fo on fo.FirmaId=f.Id
            where fo.Id={request.Id} and Aktif=1";

            var list = await _db.QueryAsync<AltFirmaDTO>(sql);
            return Ok(list);
        }

    }
}
