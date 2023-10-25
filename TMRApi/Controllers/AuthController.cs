using BL.Control.Resim;
using DAL.Service.KurService;
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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Validation.Kullanicilar;

namespace TMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IKullanıcılar _kullanici;
        private readonly IDbConnection _db;
        private readonly IValidator<RegisterDTO> _registervalidation;
        private readonly IUserService _user;
        private readonly IResim _resim;
        private readonly IKurService _kur;
        private readonly IDil _dil;
        public AuthController(IConfiguration configuration, IKullanıcılar kullanici, IDbConnection db, IValidator<RegisterDTO> registervalidation, IUserService user, IResim resim, IKurService kur, IDil dil)
        {
            _configuration = configuration;
            _kullanici = kullanici;
            _db = db;
            _registervalidation = registervalidation;
            _user = user;
            _resim = resim;
            _kur = kur;
            _dil = dil;
        }
        private string CreateToken(Claims response)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role,value:response.Izin.ToString()),
                new Claim(ClaimTypes.GivenName,value: response.FirmaId),
                new Claim(ClaimTypes.Gender,value:response.KullanıcıId),

            };


            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        [HttpPost("register")]
        public async Task<ActionResult<RegisterDTO>> Register([FromForm] RegisterDTO request)
        {
            ValidationResult result = await _registervalidation.ValidateAsync(request);
            if (result.IsValid)
            {


                int id = await _kullanici.FirmaInsert(request);
                int kullaniciid = await _kullanici.KullanıcıInsert(request, id);
                await _dil.Insert("TR", id);
                string sql = $@"Select * from Kullanıcılar where Id={kullaniciid}";
                var list = await _db.QueryAsync<RegisterResponse>(sql);
                return Ok(list);
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result.ToString());
            }





        }
        //[HttpPost("/products/images")]
        //public async Task<IActionResult> AddImagesToProduct([FromForm] ProductImages productImages)
        //{

        //    productImages.ImageUrl = productImages.ImageUrl + Path.GetExtension(productImages.File.FileName);
        //    if (dbProduct.productImages.Where(i => i.ImageUrl == productImages.ImageUrl).Any())
        //    {
        //        return BadRequest("Aynı ImageUrl adında resim bulunmaktadır.");
        //    }

        //    try
        //    {

        //        string path = Path.Combine(@"C:\Users\aykut\OneDrive\Belgeler\Visual Studio Code\React\ecommerce-app\public\images\product-details", productImages.ImageUrl);
        //        using (Stream stream = new FileStream(path, FileMode.Create))
        //        {
        //            productImages.File.CopyTo(stream);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    var createdProductImages = await _productRepo.AddImagesToProduct(productImages);

        //    return Ok(createdProductImages);
        //}



        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Login A)
        {

            DynamicParameters prm = new DynamicParameters();
            prm.Add("@KullaniciAdi", A.KullaniciAdi);
            prm.Add("@Sifre", A.Sifre);

            string sql = $@"Select * from Kullanıcılar where KullaniciAdi=@KullaniciAdi and Sifre=@Sifre and Aktif='true'";
            var list = await _db.QueryAsync<RegisterResponse>(sql, prm);
            if (list.Count() <= 0)
            {
                return BadRequest("Giriş bilgileriniz kontrol ediniz.");

            }
            Claims res = new();
            res.KullanıcıId = list.First().Id.ToString();
            res.FirmaId = list.First().FirmaId.ToString();
            res.Izin = list.First().Yonetici;

            string token = CreateToken(res);
            return (token);




        }
    }
}
