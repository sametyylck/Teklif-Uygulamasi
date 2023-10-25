using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IKullanıcılar
    {
        Task<int> KullanıcıInsert(RegisterDTO T,int firmaid);
        Task<int> FirmaInsert(RegisterDTO T);
    }
}
