using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IAltFirmaRepository
    {
        Task<int> AltFirmaInsert(AltFirmaInsert T,int FirmaId);
        Task<int> Count( int FirmaId);

        Task<int> TumCount(int FirmaId);

        Task AltFirmaUpdate(AltFirmaUpdate T, int FirmaId);
        Task AltFirmaDelete(int id , int FirmaId, int KullanıcıId);
        Task<IEnumerable<AltFirmaList>> List(int FirmaId);
        Task<IEnumerable<AltFirmaList>> TumFirmaList(int FirmaId);

        Task<IEnumerable<AltFirmaDetails>> Details(int AltFirmaId,int FirmaId);

        Task<int> OzellikInsert(AltFirmaOzellikInsert T, int FirmaId);
        Task OzellikUpdate(AltFirmaOzellikUpdate T, int FirmaId);




    }
}
