using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Vergi
    {
        public int Id { get; set; }
        public string? Isim { get; set; }
        public decimal? Deger { get; set; }
    }
}
