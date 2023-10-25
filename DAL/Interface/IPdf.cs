using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IPdf
    {
        string Pdf(int TeklifId,int FirmaId,int KullanıcıId);
        string header();
    }
}
