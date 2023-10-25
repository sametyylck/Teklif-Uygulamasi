using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class RegisterDTO
    {
        public string FirmaAd { get; set; } = null!;
        public string AdSoyad { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Tel { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; } 
    }
    public class Login

    {
        public string KullaniciAdi { get; set; } = null!;
        public string Sifre { get; set; } = null!;
    }
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = null!;
        public string KullaniciAdi { get; set; } = null!;
        public string Sifre { get; set; } = null!;
        public bool Aktif { get; set; }
        public bool Yonetici { get; set; }
        public int FirmaId { get; set; }
    }
    public class Claims

    {
        public string FirmaId { get; set; } = null!;
        public string KullanıcıId { get; set; } = null!;
        public bool Izin { get; set; }
    }
    public class Alfabe

    {
        public string Harf { get; set; } = null!;

    }


}
