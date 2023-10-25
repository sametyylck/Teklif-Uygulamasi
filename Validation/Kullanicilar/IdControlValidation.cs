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
    public class IdControlValidation:AbstractValidator<IdControl>
    {
        public IdControlValidation()
        {

            RuleFor(x => x.Id).NotEmpty().WithMessage("id bos gecilemez").NotNull().WithMessage("id zorunlu alan");
 
        }
    }
}
