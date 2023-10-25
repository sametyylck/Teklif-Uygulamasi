using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Stoklar
    {
        public Stoklar()
        {
            StokResims = new HashSet<StokResim>();
        }

        public int Id { get; set; }
        public int? KategoriId { get; set; }
        public string StokKodu { get; set; } = null!;
        public string StokAdı { get; set; } = null!;
        public string? Aciklama { get; set; }
        public int ParaBirimiId { get; set; }
        public decimal BirimFiyat { get; set; }
        public int Kdv { get; set; }
        public decimal Kdvoranı { get; set; }
        public string? Dil_id { get; set; }

        public virtual StokKategori? Kategori { get; set; }
        public virtual ParaBirimleri ParaBirimiNavigation { get; set; } = null!;
        public virtual ICollection<StokResim> StokResims { get; set; }
    }
}
