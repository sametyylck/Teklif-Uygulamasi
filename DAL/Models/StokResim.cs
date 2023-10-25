using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class StokResim
    {
        public int Id { get; set; }
        public int StokId { get; set; }
        public string Resim { get; set; } = null!;

        public virtual Stoklar Stok { get; set; } = null!;
    }
}
