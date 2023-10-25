using BL.Control.IdControl;
using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using Dapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Data;

namespace TMRApi.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class MusterilerController : ControllerBase
    {
        private readonly IMusteriler _musteri;
        private readonly IDbConnection _db;
        private readonly IUserService _userservice;
        private readonly IValidator<MusteriInsert> _musterivalidation;
        private readonly IValidator<IdControl> _idcontrol;
        private readonly IValidator<MusteriUpdate> _musteriupdate;
        private readonly IDControl _idcontroler;

        public MusterilerController(IMusteriler musteri, IDbConnection db, IUserService userservice, IValidator<MusteriInsert> musterivalidation, IValidator<IdControl> idcontrol, IValidator<MusteriUpdate> musteriupdate, IDControl idcontroler)
        {
            _musteri = musteri;
            _db = db;
            _userservice = userservice;
            _musterivalidation = musterivalidation;
            _idcontrol = idcontrol;
            _musteriupdate = musteriupdate;
            _idcontroler = idcontroler;
        }

        [HttpPost("Insert")]
        public async Task<ActionResult<MusteriUpdate>> Insert(MusteriInsert request)
        {
            ValidationResult result = await _musterivalidation.ValidateAsync(request);
            if (result.IsValid)
            {
                var getlist = _userservice.GetId();
                int CompanyId = getlist[0];
                int KullanıcıId = getlist[1];
                int id = await _musteri.Insert(request, CompanyId, KullanıcıId);
                string sql = $@"Select * from Musteriler where Id={id}";
                var list = await _db.QueryAsync<MusteriUpdate>(sql);
                return Ok(list);
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result.ToString());
            }






        }
        [HttpPut("Update")]
        public async Task<ActionResult<MusteriUpdate>> Update(MusteriUpdate request)
        {
            ValidationResult result = await _musteriupdate.ValidateAsync(request);
            if (result.IsValid)
            {
                var getlist = _userservice.GetId();
                int CompanyId = getlist[0];
                var hata = await _idcontroler.IdControl(request.Id, "Musteriler", CompanyId);
                if (hata == "true")
                {
                    int KullanıcıId = getlist[1];
                    await _musteri.Update(request, CompanyId, KullanıcıId);
                    string sql = $@"Select * from Musteriler where Id={request.Id}";
                    var list = await _db.QueryAsync<MusteriUpdate>(sql);
                    return Ok(list);
                }
                else
                {
                    return BadRequest(hata);
                }



            }

            else
            {
                result.AddToModelState(this.ModelState);
            }
            return BadRequest(result.ToString());







        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<MusteriUpdate>> Delete(int id)
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _idcontroler.IdControl(id, "Musteriler", CompanyId);
            if (hata == "true")
            {

                await _musteri.Delete(id, CompanyId, KullanıcıId);
                return Ok("Silme islemi basarili");
            }
            else
            {
                return BadRequest(hata);
            }














        }
        [HttpPost("Details")]
        public async Task<ActionResult<MusteriUpdate>> Details(IdControl T)
        {
            ValidationResult result = await _idcontrol.ValidateAsync(T);
            if (result.IsValid)
            {

                var getlist = _userservice.GetId();
                int CompanyId = getlist[0];
                int KullanıcıId = getlist[1];
                var list = await _musteri.Detail(T, CompanyId);
                return Ok(list);
            }

            else
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result.ToString());
            }






        }
        [HttpPost("List")]
        public async Task<ActionResult<MusteriUpdate>> List(string? T, int? KAYITSAYISI, int? SAYFA)
        {
            // ValidationResult result = await _musterivalidation.ValidateAsync(request);
            //if (result.IsValid)

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _musteri.List(T, CompanyId, KullanıcıId, KAYITSAYISI, SAYFA);
            var count = await _musteri.Count(T, CompanyId);
            return Ok(new { list, count });

            //else
            //{
            //    result.AddToModelState(this.ModelState);
            //    return BadRequest(result.ToString());
            //}






        }


        [HttpPost("InsertAdres")]
        public async Task<ActionResult<AdresDTO>> InsertAdres(AdresInsert request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _musteri.InsertAdres(request, CompanyId, KullanıcıId);
            string sql = $@"Select * from Adres where Id={id}";
            var list = await _db.QueryAsync<AdresDTO>(sql);
            return Ok(list);


        }
        [HttpPut("UpdateAdres")]
        public async Task<ActionResult<AdresDTO>> UpdateAdres(AdresUpdate request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _musteri.UpdateAdres(request, CompanyId, KullanıcıId);
            string sql = $@"Select * from Adres where Id={request.Id}";
            var list = await _db.QueryAsync<MusteriUpdate>(sql);
            return Ok(list);


        }
        [HttpDelete("DeleteAdres")]
        public async Task<ActionResult<AdresDTO>> DeleteAdres(int id)
        {


            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _idcontroler.IdControl(id, "Adres", CompanyId);
            if (hata == "true")
            {

                await _musteri.DeleteAdres(id, CompanyId, KullanıcıId);
                return Ok("Silme islemi basarili");
            }
            else
            {
                return BadRequest(hata);
            }














        }
        [HttpPost("DetailsAdres")]
        public async Task<ActionResult<AdresDTO>> DetailsAdres(int id)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _musteri.DetailAdres(id, CompanyId);
            return Ok(list);

        }
        [HttpPost("ListAdres")]
        public async Task<ActionResult<AdresDTO>> ListAdres(string? T, int? KAYITSAYISI, int? SAYFA)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var list = await _musteri.ListAdres(T, CompanyId, KullanıcıId, KAYITSAYISI, SAYFA);
            var count = await _musteri.CountAdres(T, CompanyId);
            return Ok(new { list, count });


        }




    }
}
