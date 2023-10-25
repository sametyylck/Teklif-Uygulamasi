using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Teklifler
    {
        public Teklifler()
        {
            TeklifDetays = new HashSet<TeklifDetay>();
        }

        public int Id { get; set; }
        public int MusteriId { get; set; }
        public int ParaBirimiId { get; set; }
        public DateTime TeklifTarihi { get; set; }
        public DateTime IslemTarihi { get; set; }
        public int Kullanıcı { get; set; }
        public decimal Kur { get; set; }
        public decimal? TeklifTutarı { get; set; }
        public decimal? IskontoOrani { get; set; }
        public decimal? IskontoTutari { get; set; }
        public decimal? ToplamKdv { get; set; }
        public int Durum { get; set; }
        public int? AltSirketId { get; set; }
        public string? Dil_id { get; set; }
        public int? RevizyonId { get; set; }

        public virtual AltSirket? AltSirket { get; set; }
        public virtual Kullanıcılar KullanıcıNavigation { get; set; } = null!;
        public virtual Musteriler Musteri { get; set; } = null!;
        public virtual ParaBirimleri ParaBirimiNavigation { get; set; } = null!;
        public virtual ICollection<TeklifDetay> TeklifDetays { get; set; }
    }
}
