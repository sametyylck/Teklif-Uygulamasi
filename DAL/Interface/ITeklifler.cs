using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface ITeklifler
    {
        Task<int> Insert(int SepetId,int FirmaId,int KullanıcıId);

        Task<int> Count(int FirmaId);

        Task InsertDetay(int TeklifId,int SepetId, int FirmaId, int KullanıcıId);
        Task<int> InsertTeklifDetay(InsertTeklifDetay A, int FirmaId, int KullanıcıId);

        Task Update(TeklifUpdate T,int FirmaId,int KullanıcıId);
        Task UpdateDetay(TeklifDetayUpdate T, int FirmaId,int KullanıcıId);
        Task<string> Cizim(CizimA request,int TeklifId, int FirmaId, int KullanıcıId);
        Task DeleteDetay(int Id, int FirmaId);
        Task DeleteSoft(int Id, int FirmaId);
        Task Delete(int Id, int FirmaId);
        Task<IEnumerable<TeklifListe>> Details(int id,int FirmaId);
        Task<IEnumerable<TeklifListResponse>> List(string? kelime, int FirmaId, int? KAYITSAYISI, int? SAYFA);
    }
}
