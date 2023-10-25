using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Service.KurService
{
    public interface IKurService
    {
        Task<decimal> TarihsizKur(string kod,int FirmaId);
        Task<decimal> TarihliKur(DateTime Tarih, string kod);

    }
}
