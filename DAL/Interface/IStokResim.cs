using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IStokResim
    {
        Task<int> Insert(Image request,int StokId,  int FirmaId, int KullanıcıId);
        Task Update(ImageUpdate T,int StokId,int Id,  int FirmaId, int KullanıcıId);
        Task Delete(int  id, int CompanyId, int KullanıcıId);
        Task<IEnumerable<ImageList>> List(int CompanyId);
        Task<IEnumerable<ImageList>> Details(int StokId,int FirmaId);
    }
}
