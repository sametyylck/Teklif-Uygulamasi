using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class StokKategori
    {
        public StokKategori()
        {
            Stoklars = new HashSet<Stoklar>();
        }

        public int Id { get; set; }
        public string Ad { get; set; } = null!;

        public virtual ICollection<Stoklar> Stoklars { get; set; }
    }
}
