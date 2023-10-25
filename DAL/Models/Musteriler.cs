using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Musteriler
    {
        public Musteriler()
        {
            Sepets = new HashSet<Sepet>();
            Tekliflers = new HashSet<Teklifler>();
        }

        public int Id { get; set; }
        public string Unvan { get; set; } = null!;
        public string Adres { get; set; } = null!;
        public string Telefon { get; set; } = null!;
        public string? Mail { get; set; }

        public virtual ICollection<Sepet> Sepets { get; set; }
        public virtual ICollection<Teklifler> Tekliflers { get; set; }
    }
}
