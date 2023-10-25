using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class SepetInsert
    {
        public int? MusteriId { get; set; }
        public string? SepetAdi { get; set; }
        public int? ParaBirimiId { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }


    }
    public class SepetInsertResponse
    {
        public int Id { get; set; }
        public string? SepetAdi { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvanı { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

    }

    public class SepetDetayInsert
    {
        public int? SepetId { get; set; }
        public int? StokId { get; set; }
        public int? BirimId { get; set; }
        public decimal? Miktar { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? Hediye { get; set; }
        public char? Grup { get; set; }



    }
    public class SepetDetayInsertResponse
    {
        public int Id { get; set; }
        public int SepetAdi { get; set; }
        public int? SepetId { get; set; }
        public int? StokId { get; set; }
        public char Grup { get; set; }
        public int? BirimId { get; set; }
        public string? BirimIsmi { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? Aciklama { get; set; }
        public string? KategoriIsmi { get; set; }
        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? Hediye { get; set; }
        public int ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }

    }


    public class SepetUpdate
    {
        public int Id { get; set; }
        public int? MusteriId { get; set; }
        public int? ParaBirimiId { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

    }
    public class SepetDetayUpdate
    {
        public int Id { get; set; }
        public int? SepetId { get; set; }
        public int? StokId { get; set; }
        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? Hediye { get; set; }
        public char Grup { get; set; }



    }

    public class SepetDTO
    {
        public int SepetId { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvani { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

        public int SepetDetayId { get; set; }
        public int? StokId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? Aciklama { get; set; }
        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOrani { get; set; }
        public decimal? DetayIskontoOrani { get; set; }
        public bool? DetayIskontoBirim { get; set; }
    }

    public class SepetListResponse
    {
        public int SepetId { get; set; }
        public string? SepetAdi { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvani { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

    }


    public class SepetList
    {
        public int SepetId { get; set; }
        public string? SepetAdi { get; set; }

        public int? MusteriId { get; set; }
        public string? MusteriUnvani { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

        public IEnumerable<SepetDetayInsertResponse> SepetDetay { get; set; }
    }


}
