using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IKullanıcı
    {
        Task<int> Insert(KullanıcıInsert T, int CompanyId, int KullanıcıId);
        Task Update(KullanıcıUpdate T, int CompanyId, int KullanıcıId);
        Task Delete(int Id, int CompanyId, int KullanıcıId);
        Task<IEnumerable<KullanıcıInsertResponse>> Details(int T, int CompanyId);

        Task<IEnumerable<KullanıcıInsertResponse>> List(string? T, int CompanyId);
        Task<int> Count(string? T, int CompanyId);
    }
}
