using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Teklif
{
    public interface ITeklifControl
    {
        Task<List<string>> Control(int TeklifId,int dil,int FirmaId);
    }
}
