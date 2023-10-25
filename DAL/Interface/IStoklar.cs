using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IStoklar
    {
        Task<int> Insert(StoklarInsert T,int FirmaId,int KullanıcıId);
        Task<int> Count(int? dil, string? T, int CompanyId, int? KAYITSAYISI, int? SAYFA);

        Task Update(StoklarUpdate T, int FirmaId, int KullanıcıId);
        Task Delete(int Id, int FirmaId, int KullanıcıId);
        Task<IEnumerable<StoklarListResponse>> List(int? dil,string? T,int CompanyId,int? KAYITSAYISI,int? SAYFA);
        Task<IEnumerable<StoklarDetails>> Details(int id ,int dil,int CompanyId);

    }
}
