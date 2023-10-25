using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IStokKategori
    {
        Task<int> Insert(StocKategoriInsert T,int CompanyId, int KullanıcıId);
        Task Update(StocKategoriDTO T ,int CompanyId, int KullanıcıId);
        Task Delete(int Id, int CompanyId, int KullanıcıId);
        Task<IEnumerable<StocKategoriDTO>> Details(int T, int Dil,int CompanyId);

        Task<IEnumerable<StocKategoriDTO>> List(string? T,int Dil, int CompanyId);
        Task<int> Count(string? T,int Dil,int CompanyId);

    }
}
