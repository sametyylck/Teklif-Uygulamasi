using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.IdControl
{
    public interface IDControl
    {
        Task<string> IdControl(int? id, string tablo, int FirmaId);
    }
}
