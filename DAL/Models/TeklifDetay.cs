using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class TeklifDetay
    {
        public int Id { get; set; }
        public int? TeklifId { get; set; }
        public string StockKodu { get; set; } = null!;
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public int Kdv { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal KdvTutari { get; set; }
        public string ParaBirimiId { get; set; } = null!;
        public decimal Kur { get; set; }
        public decimal? Tutar { get; set; }
        public decimal? ToplamTutar { get; set; }

        public virtual Teklifler? Teklif { get; set; }
    }
}
