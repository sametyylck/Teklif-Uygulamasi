using DAL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Kullanicilar
{
    public class MusteriValidation:AbstractValidator<MusteriInsert>
    {
        public MusteriValidation()
        {
            RuleFor(x => x.Unvan).NotEmpty().WithMessage("Unvan bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull(); ;
            RuleFor(x => x.Mail).NotEmpty().WithMessage("Mail bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull().WithMessage("Mail zorunlu alan").EmailAddress().WithMessage("Mail yazılımı yanlıs"); ;
            RuleFor(x => x.Telefon).NotEmpty().WithMessage("Telefon bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull(); ;
            RuleFor(x => x.Adres).NotEmpty().WithMessage("Adres bos geçilemez").NotNull().WithMessage("Adres Alan");
        }
    }
    public class MusteriUpdateValidation : AbstractValidator<MusteriUpdate>
    {
        public MusteriUpdateValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id bos geçilemez").NotNull().WithMessage("Mail zorunlu alan"); ;

            RuleFor(x => x.Unvan).NotEmpty().WithMessage("Unvan bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull().WithMessage("Mail zorunlu alan"); ;
            RuleFor(x => x.Mail).NotEmpty().WithMessage("Mail bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull().WithMessage("Mail zorunlu alan").EmailAddress().WithMessage("Mail yazılımı yanlıs"); ;
            RuleFor(x => x.Telefon).NotEmpty().WithMessage("Telefon bos geçilemez").MaximumLength(50).MinimumLength(2).NotNull(); ;
            RuleFor(x => x.Adres).NotEmpty().WithMessage("Adres bos geçilemez").NotNull().WithMessage("Adres Alan");
        }
    }
}
