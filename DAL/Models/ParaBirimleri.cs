using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class ParaBirimleri
    {
        public ParaBirimleri()
        {
            Stoklars = new HashSet<Stoklar>();
            Tekliflers = new HashSet<Teklifler>();
        }

        public int Id { get; set; }
        public string BirimId { get; set; } = null!;
        public string Kod { get; set; } = null!;

        public virtual ICollection<Stoklar> Stoklars { get; set; }
        public virtual ICollection<Teklifler> Tekliflers { get; set; }
    }
}
