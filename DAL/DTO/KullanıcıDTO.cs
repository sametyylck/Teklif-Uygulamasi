using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class KullanıcıDTO
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
    }
    public class KullanıcıUpdate
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
    }
    public class KullanıcıInsert
    {
        public string AdSoyad { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
    }
    public class KullanıcıInsertResponse
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
    }
}
