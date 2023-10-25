using DAL.DTO;
using Dapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Kullanicilar
{
    public class KullanicilarValidations:AbstractValidator<RegisterDTO>
    {
        private readonly IDbConnection _db;
        private bool KullaniciAdi(string kullaniciadi)
        {
            DynamicParameters prm = new();
            prm.Add("@kullaniciadi", kullaniciadi);
            string sqlquery = $@"Select Count(*)as varmı from Kullanıcılar where KullaniciAdi=@kullaniciadi";
            var list = _db.Query<int>(sqlquery, prm);
            if (list.Count() != 0)
            {
                return true;

            }
            return false;
        }
        public KullanicilarValidations(IDbConnection db)
        {
            RuleFor(x => x.AdSoyad).NotEmpty().WithMessage("AdSoyad bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull(); ;
            RuleFor(x => x.Mail).NotEmpty().WithMessage("Mail bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull().WithMessage("Mail zorunlu alan").EmailAddress().WithMessage("Mail yazılımı yanlıs"); ;
            RuleFor(x => x.FirmaAd).NotEmpty().WithMessage("FirmaAd bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull(); ;
            RuleFor(x => x.Tel).NotEmpty().WithMessage("FirmaAd bos geçilemez").NotNull().WithMessage("Zorunlu Alan");


            RuleFor(x => x.KullaniciAdi).NotEmpty().WithMessage("KullaniciAdi bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull().Must(x=>KullaniciAdi(x)).WithMessage("Boyle bir kullanici var");
            RuleFor(x => x.Sifre).NotEmpty().WithMessage("Sifre bos gecilemez").MaximumLength(50).MinimumLength(2).NotNull(); ;
            _db = db;
        }
    }
}
