using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Kullanıcılar
    {
        public Kullanıcılar()
        {
            Sepets = new HashSet<Sepet>();
            Tekliflers = new HashSet<Teklifler>();
        }

        public int Id { get; set; }
        public string AdSoyad { get; set; } = null!;
        public string KullaniciAdi { get; set; } = null!;
        public string Sifre { get; set; } = null!;
        public bool Aktif { get; set; }
        public bool Yonetici { get; set; }

        public virtual ICollection<Sepet> Sepets { get; set; }
        public virtual ICollection<Teklifler> Tekliflers { get; set; }
    }
}
