using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Stoklar
{
    public interface IStoklarService
    {
        Task<string> Insert(StoklarInsert T, int FirmaId);

        Task<string> Update(StoklarUpdate T, int FirmaId);

    }
}
