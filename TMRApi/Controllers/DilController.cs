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
    public class DilController : ControllerBase
    {

        private readonly IDbConnection _db;
        private readonly IUserService _userService;
        private readonly IDil _dil;

        public DilController(IDil dil, IDbConnection db, IUserService userService)
        {
            _dil = dil;
            _db = db;
            _userService = userService;
        }

        [HttpPost("Insert")]
        public async Task<ActionResult<DilDTO>> Insert(string request)
        {

            var getlist = _userService.GetId();
            int CompanyId = getlist[0];
            int id = await _dil.Insert(request, CompanyId);
            string sql = $@"Select * from Dil where Id={id}";
            var list = await _db.QueryAsync<DilDTO>(sql);
            return Ok(list);

        }

        [HttpPost("List")]
        public async Task<ActionResult<DilDTO>> List(string? T)
        {

            var getlist = _userService.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _dil.List(T, CompanyId);
            var count = await _dil.Count(T, CompanyId);
            return Ok(new { list, count });

        }
    }
}
