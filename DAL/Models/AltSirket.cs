using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class AltSirket
    {
        public AltSirket()
        {
            Tekliflers = new HashSet<Teklifler>();
        }

        public int Id { get; set; }
        public string? SirketIsmi { get; set; }
        public int? Tel { get; set; }
        public string? Adres { get; set; }
        public string? Mail { get; set; }
        public string? Resim { get; set; }

        public virtual ICollection<Teklifler> Tekliflers { get; set; }
    }
}
