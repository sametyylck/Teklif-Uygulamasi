using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IDil
    {
        Task<int> Insert(string T, int FirmaId);
        Task<int> Count(string? T, int FirmaId);

        Task<IEnumerable<DilDTO>> List(string? T, int CompanyId);
    }
}
