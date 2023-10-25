using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Sepet
{
    public interface ISepetControl
    {
        Task<string> Insert(int? ParaBirimiId,int? MusteriId,int FirmaId);
        Task<string> InsertDetay(int StokId, int SepetId,int BirimId,int? Id, int FirmaId);
    }
}
