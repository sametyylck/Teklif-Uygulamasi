using DAL.Service.KurService;
using BL.UserService;
using DAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KurController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IKurService _kurservice;

        public KurController(IUserService userservice, IKurService kurservice)
        {
            _userservice = userservice;
            _kurservice = kurservice;
        }

        [HttpPost("TarihliKur")]
        public async Task<ActionResult<dynamic>> TarihliKur(DateTime tarih, string kod)
        {
            var kur = await _kurservice.TarihliKur(tarih, kod);
            return Ok(kur);


        }
        [HttpPost("TarihsizKur")]
        public async Task<ActionResult<decimal>> TarihsizKur(string kod)
        {
            var getlist = _userservice.GetId();
            int FirmaId = getlist[0];
            var kur = await _kurservice.TarihsizKur(kod,FirmaId);
            return Ok(kur);
    

        }
    }
}
