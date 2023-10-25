using BL.Control.IdControl;
using BL.Control.Sepet;
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
using System.Data;
using Validation.Kullanicilar;

namespace TMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SepetController : ControllerBase
    {
        private readonly ISepet _sepet;
        private readonly IUserService _userservice;
        private readonly IDbConnection _db;
        private readonly IDControl _idcontroler;
        private readonly IValidator<SepetInsert> _sepetinsert;
        private readonly IValidator<SepetDetayInsert> _sepetdetayinsert;
        private readonly IValidator<SepetDetayUpdate> _sepetdetayupdate;
        private readonly IValidator<SepetUpdate> _sepetupdate;
        private readonly ISepetControl _sepetcontrol;



        public SepetController(ISepet sepet, IUserService userservice, IDbConnection db, IDControl idcontroler, IValidator<SepetInsert> sepetinsert, IValidator<SepetDetayInsert> sepetdetayinsert, IValidator<SepetDetayUpdate> sepetdetayupdate, IValidator<SepetUpdate> sepetupdate, ISepetControl sepetcontrol)
        {
            _sepet = sepet;
            _userservice = userservice;
            _db = db;
            _idcontroler = idcontroler;
            _sepetinsert = sepetinsert;
            _sepetdetayinsert = sepetdetayinsert;
            _sepetdetayupdate = sepetdetayupdate;
            _sepetupdate = sepetupdate;
            _sepetcontrol = sepetcontrol;
        }

        [HttpPost("Insert"), Authorize]
        public async Task<ActionResult<SepetInsert>> Insert(SepetInsert request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            ValidationResult result = await _sepetinsert.ValidateAsync(request);
            if (result.IsValid)
            {
                var hata = await _sepetcontrol.Insert(request.ParaBirimiId, request.MusteriId, CompanyId);
                if (hata=="true")
                {
                    int id = await _sepet.Insert(request, CompanyId, KullanıcıId);
                    string sql = $@"select s.Id,s.AltFirmaId,s.SepetAdi,s.MusteriId,Musteriler.Unvan as MusteriUnvanı,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi ,s.IslemTarihi,s.IskontoOrani,s.IskontoBirim,s.FirmaId,s.Kur from Sepet s
            left join Musteriler on Musteriler.Id=s.MusteriId
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            where s.Id={id}";
                    var list = await _db.QueryAsync<SepetInsertResponse>(sql);
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
                return BadRequest(result.ToString());
            }




        }
        [HttpPost("InsertDetay"), Authorize]
        public async Task<ActionResult<SepetDetayInsertResponse>> InsertDetay(SepetDetayInsert request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            ValidationResult result = await _sepetdetayinsert.ValidateAsync(request);
            if (result.IsValid)
            {
                int id = await _sepet.InsertDetay(request, CompanyId, KullanıcıId);
                string sql = $@"select se.Id,se.SepetId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.BirimId,Birim.Isim as BirimIsmi, se.StokId,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,se.Hediye,se.Grup
            from SepetDetay se
			left join Stoklar on Stoklar.Id=se.StokId
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
			left join Birim on Birim.id=se.BirimId
            where se.Id={id} and se.FirmaId={CompanyId}";
                var list = await _db.QueryAsync<SepetDetayInsertResponse>(sql);
                return Ok(list);
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result.ToString());
            }






        }

        [HttpPut("Update"), Authorize]
        public async Task<ActionResult<SepetUpdate>> Update(SepetUpdate request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];

            ValidationResult result = await _sepetupdate.ValidateAsync(request);
            if (result.IsValid)
            {
                await _sepet.Update(request, CompanyId, KullanıcıId);
                string sql = $@"select s.Id,s.SepetAdi,s.MusteriId,Musteriler.Unvan as MusteriUnvanı,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi ,s.IslemTarihi,s.IskontoOrani,s.IskontoBirim,s.FirmaId,s.Kur from Sepet s
            left join Musteriler on Musteriler.Id=s.MusteriId
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            where s.Id={request.Id}";
                var list = await _db.QueryAsync<SepetInsertResponse>(sql);
                return Ok(list);
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result.ToString());
            }




        }
        [HttpPut("UpdateDetay"), Authorize]
        public async Task<ActionResult<SepetDetayUpdate>> UpdateDetay(SepetDetayUpdate request)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];

            ValidationResult result = await _sepetdetayupdate.ValidateAsync(request);
            if (result.IsValid)
            {
                await _sepet.UpdateDetay(request, CompanyId, KullanıcıId);
                string sql = $@"select se.Id,se.SepetId,se.StokAdi,se.Aciklama,se.BirimId,Birim.Isim as BirimIsmi, se.StokId,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim,se.Hediye
            from SepetDetay se
			left join Birim on Birim.id=se.BirimId
            where se.Id={request.Id} and se.FirmaId={CompanyId}";
                var list = await _db.QueryAsync<SepetDetayUpdate>(sql);
                return Ok(list);
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result.ToString());
            }



        }
        [HttpDelete("Delete"), Authorize]
        public async Task<ActionResult<dynamic>> Delete(int id)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _idcontroler.IdControl(id, "Sepet", CompanyId);
            if (hata == "true")
            {
                await _sepet.Delete(id, CompanyId);
                return Ok("Basarli sekilde silindi");

            }
            else
            {
                return BadRequest(hata);
            }

        }
        [HttpDelete("DeleteDetay"), Authorize]
        public async Task<ActionResult<dynamic>> DeleteDetay(int id)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _idcontroler.IdControl(id, "SepetDetay", CompanyId);
            if (hata == "true")
            {
                await _sepet.DeleteDetay(id, CompanyId);
                return Ok("Basarli sekilde silindi");

            }
            else
            {
                return BadRequest(hata);
            }




        }

        [HttpGet("Details"), Authorize]
        public async Task<ActionResult<SepetDTO>> Details(int id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];

            var list = await _sepet.Details(id, CompanyId);
            return Ok(list);

        }

        [HttpGet("List"), Authorize]
        public async Task<ActionResult<SepetDTO>> List()
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _sepet.List(CompanyId);
            var count = await _sepet.Count(CompanyId);

            return Ok(new { list, count });

        }
    }
}
