using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface ISepet
    {
        Task<int> Insert(SepetInsert T, int FirmaId, int KullanıcıId);
        Task<int> Count(int FirmaId);

        Task<int> InsertDetay(SepetDetayInsert T, int FirmaId, int KullanıcıId);
        Task DeleteSoft(int Id , int FirmaId);
        Task Delete(int Id, int FirmaId);
        Task DeleteDetay(int Id, int FirmaId);

        Task Update(SepetUpdate T,int FirmaId,int KullanıcıId);
        Task UpdateDetay(SepetDetayUpdate T, int FirmaId, int KullanıcıId);
        Task<IEnumerable<SepetListResponse>> List(int FirmaId);
        Task<IEnumerable<StoklarListResponse>> DilKontrol(int? StokId,int Dil,int FirmaId);

        Task<IEnumerable<SepetList>> Details(int id,int FirmaId);

    }
}
