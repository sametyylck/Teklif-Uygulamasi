using DAL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Kullanicilar
{
    public class SepetInsertValidations:AbstractValidator<SepetInsert>
    {
        public SepetInsertValidations()
        {
            RuleFor(x => x.MusteriId).NotEmpty().WithMessage("MusteriId bos geçilemez").NotNull(); ;
            RuleFor(x => x.SepetAdi).NotEmpty().WithMessage("SepetAdi bos geçilemez").MaximumLength(50).MinimumLength(1).NotNull().WithMessage("SepetAdi zorunlu alan");
            RuleFor(x => x.ParaBirimiId).NotEmpty().WithMessage("ParaBirimi bos geçilemez").NotNull(); ;


        }
    }
    public class SepetInsertDetayValidation : AbstractValidator<SepetDetayInsert>
    {
        public SepetInsertDetayValidation()
        {
            RuleFor(x => x.SepetId).NotEmpty().WithMessage("SepetId bos geçilemez").NotNull(); ;
            RuleFor(x => x.StokId).NotEmpty().WithMessage("StokId bos geçilemez").NotNull().WithMessage("StokId zorunlu alan");
            RuleFor(x => x.BirimId).NotEmpty().WithMessage("BirimId bos geçilemez").NotNull(); ;
            RuleFor(x => x.Miktar).NotEmpty().WithMessage("Miktar bos geçilemez").NotNull().WithMessage("Miktar zorunlu Alan");

        }
    }

    public class SepetUpdateDetayValidation : AbstractValidator<SepetDetayUpdate>
    {
        public SepetUpdateDetayValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id bos geçilemez").NotNull();
            RuleFor(x => x.SepetId).NotEmpty().WithMessage("SepetId bos geçilemez").NotNull();
            RuleFor(x => x.StokId).NotEmpty().WithMessage("StokId bos geçilemez").NotNull().WithMessage("StokId zorunlu alan");
            RuleFor(x => x.BirimFiyat).NotEmpty().WithMessage("BirimFiyat bos geçilemez").NotNull(); ;
            RuleFor(x => x.Miktar).NotEmpty().WithMessage("Miktar bos geçilemez").NotNull().WithMessage("Miktar zorunlu Alan");

        }
    }
    public class SepetUpdateValidations : AbstractValidator<SepetUpdate>
    {
        public SepetUpdateValidations()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("MusteriId bos geçilemez").NotNull(); ;
            RuleFor(x => x.MusteriId).NotEmpty().WithMessage("MusteriId bos geçilemez").NotNull(); ;
            RuleFor(x => x.ParaBirimiId).NotEmpty().WithMessage("ParaBirimi bos geçilemez").NotNull(); ;

        }
    }



}
