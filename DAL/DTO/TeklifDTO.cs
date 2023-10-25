using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class TeklifDTO
    {
        public int TeklifId { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvani { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? TeklifTarihi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public DateTime? GecerlilikTarihi { get; set; }
        public decimal? Kur { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

        public int TeklifDetayId { get; set; }
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
    public class TeklifListResponse
    {
        public int Id { get; set; }
        public string? TeklifAdi { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvani { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? TeklifTarihi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public DateTime? GecerlilikTarihi { get; set; }
        public decimal? Kur { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? IskontoGoster { get; set; }
        public bool? MarkaGoster { get; set; }
        public bool? StokKoduGoster { get; set; }

        public decimal? ToplamTutar { get; set; }

    }
    public class InsertTeklifDetay
    {
        public int? TeklifId { get; set; }
        public int? StokId { get; set; }
        public int ParaBirimiId { get; set; }
        public string? Grup { get; set; }




    }

    public class TeklifListe
    {
        public int TeklifId { get; set; }
        public string? TeklifAdi { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvani { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? StokKoduGoster { get; set; }
        public bool? MarkaGoster { get; set; }
        public bool? IskontoGoster { get; set; }
        public IEnumerable<TeklifDetayInsertResponse> TeklifDetay { get; set; }
    }

    public class TeklifInsert
    {
        public int? MusteriId { get; set; }
        public string? SepetAdi { get; set; }
        public int? ParaBirimiId { get; set; }
        public DateTime? TeklifTarihi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public DateTime? GecerlilikTarihi { get; set; }
        public decimal? Kur { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }

    }
    public class TeklifInsertRevizyon
    {
        public int? MusteriId { get; set; }
        public int? RevizyonId { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? TeklifAdi { get; set; }
        public decimal? IskontoOrani { get; set; }
    }

    public class TeklifInsertResponse
    {
        public int Id { get; set; }
        public int? MusteriId { get; set; }
        public string? MusteriUnvanı { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? StokKoduGoster { get; set; }
        public bool? MarkaGoster { get; set; }
        public bool? IskontoGoster { get; set; }

    }

    public class TeklifDetayInsert
    {
        public int? TeklifId { get; set; }
        public int? StokId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? KategoriIsmi { get; set; }
        public int BirimId { get; set; }
        public bool Hediye { get; set; }
        public string? Aciklama { get; set; }
        public string? Grup { get; set; }

        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }


    }
    public class DetayInsert
    {
        public int? TeklifId { get; set; }
        public int? StokId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? KategoriIsmi { get; set; }
        public int BirimId { get; set; }
        public bool Hediye { get; set; }
        public string? Aciklama { get; set; }
        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOranı { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }


    }


    public class Pdf
    {
        public int? TeklifId { get; set; }
        public int? StokId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public int BirimId { get; set; }
        public string? Aciklama { get; set; }
        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public string? Resim { get; set; }

    }

    public class TeklifDetayInsertResponse

    {
        public int Id { get; set; }
        public int? TeklifId { get; set; }
        public int? StokId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? KategoriIsmi { get; set; }
        public string? Aciklama { get; set; }
        public decimal? Miktar { get; set; }
        public int BirimId { get; set; }
        public string? BirimIsmi { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? Hediye { get; set; }
        public string? Grup { get; set; }
        public string? GrupKarisilik { get; set; }
        public string? MarkaAdi { get; set; }

        public int ParaBirimiId { get; set; }
        public string ParaBirimiIsmi { get; set; }
        public string Resim { get; set; }
    }

    public class TeklifUpdate
    {
        public int Id { get; set; }
        public int? MusteriId { get; set; }
        public int? ParaBirimiId { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? StokKoduGoster { get; set; }
        public bool? MarkaGoster { get; set; }
        public bool? IskontoGoster { get; set; }

    }
    public class TeklifDetayUpdate
    {
        public int Id { get; set; }
        public int? TeklifId { get; set; }
        public int? StokId { get; set; }
        public string? Aciklama { get; set; }
        public string? Grup { get; set; }

        public decimal? Miktar { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public bool? IskontoBirim { get; set; }
        public bool? Hediye { get; set; }


    }

    public class TeklifList
    {
        public string? MusteriUnvani { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public DateTime? TeklifTarihi { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public DateTime? GecerlilikTarihi { get; set; }
        public decimal? IskontoOrani { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? Aciklama { get; set; }
        public decimal? Miktar { get; set; }
    }
    public class PdfTeklif
    {
        public DateTime? TeklifTarihi { get; set; }
        public decimal? Kur { get; set; }
        public int? MusteriId { get; set; }
        public string Unvan { get; set; } 
        public string Adres { get; set; } 
        public string Telefon { get; set; } 
        public string? Mail { get; set; }


    }
    public class PdfTeklifResponse
    {
        public DateTime? TeklifTarihi { get; set; }
        public DateTime? GecerlilikTarihi { get; set; }
        public int? MusteriId { get; set; }
        public string Unvan { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string? Mail { get; set; }
        public string? Logo { get; set; }
        public string? AltMetin { get; set; }
        public bool IskontoGoster { get; set; }
        public bool StokKoduGoster { get; set; }
        public bool MarkaGoster { get; set; }
        public decimal IskontoOrani { get; set; }
        public bool IskontoBirim { get; set; }

    }


}
