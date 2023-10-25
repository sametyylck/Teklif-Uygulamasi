using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IBirim
    {
        Task<int> Insert(BirimInsert T,int FirmaId);
        Task<int> Count(string? T, int FirmaId);

        Task Update(BirimUpdate t, int FirmaId);
        Task Delete(int  Id, int FİrmaId);

        Task<IEnumerable<BirimDTO>> List(string? T, int CompanyId);
    }
}
