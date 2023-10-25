using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Resim
{
    public interface IResim
    {
        Task<string> Eklimi(IFormFile Resim,int StokId, int FirmaId);
    }
}
