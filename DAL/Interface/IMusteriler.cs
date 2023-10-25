using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IMusteriler
    {
        Task<int> Insert(MusteriInsert T,int CompanyId,int KullanıcıId);
        Task<int> Count(string? T, int CompanyId);

        Task Update(MusteriUpdate T,int CompanyId, int KullanıcıId);
        Task Delete(int id , int CompanyId, int KullanıcıId);
        Task<IEnumerable<MusteriUpdate>> List(string? T, int CompanyId, int KullanıcıId, int? KAYITSAYISI, int? SAYFA);
        Task<IEnumerable<MusteriUpdate>> Detail(IdControl T,int FirmaId);

        Task<int> InsertAdres(AdresInsert T, int CompanyId, int KullanıcıId);
        Task UpdateAdres(AdresUpdate T, int CompanyId, int KullanıcıId);

        Task<IEnumerable<AdresDTO>> ListAdres(string? T, int CompanyId, int KullanıcıId, int? KAYITSAYISI, int? SAYFA);
        Task<IEnumerable<AdresDTO>> DetailAdres(int T, int FirmaId);
        Task DeleteAdres(int id, int CompanyId, int KullanıcıId);
        Task<int> CountAdres(string? T, int CompanyId);

    }
}
