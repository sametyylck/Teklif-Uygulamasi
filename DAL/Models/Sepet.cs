using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Sepet
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public decimal Miktar { get; set; }
        public int KullanıcıId { get; set; }
        public int MusteriId { get; set; }

        public virtual Kullanıcılar Kullanıcı { get; set; } = null!;
        public virtual Musteriler Musteri { get; set; } = null!;
    }
}
