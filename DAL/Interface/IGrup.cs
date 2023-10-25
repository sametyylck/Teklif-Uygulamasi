using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IGrup
    {
        Task<int> Insert(List<GrupInsert>  T, int CompanyId, int KullanıcıId);
        Task Update(GrupUpdate T, int CompanyId, int KullanıcıId);
        Task Delete(string harf,int TeklifId, int CompanyId, int KullanıcıId);
        Task<IEnumerable<GrupDTO>> Details(int id,int CompanyId);

        Task<IEnumerable<GrupDTO>> List(int CompanyId);
        Task<int> Count(string? T, int CompanyId);
    }
}
