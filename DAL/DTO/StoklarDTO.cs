using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class StoklarDTO
    {
        public int? Id { get; set; }
        public int? StockId { get; set; }
        public int? KategoriId { get; set; }
        public string StokKodu { get; set; } = null!;
        public string StokAdi { get; set; } = null!;
        public string? Aciklama { get; set; }
        public int? ParaBirimiId { get; set; }
        public int? BirimId { get; set; }
        public string? KategoriIsmi { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal Desi { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal? KDVOranı { get; set; }
        public string? Dil_id { get; set; }
        public int? KullanıcıId { get; set; }
        public int? FirmaId { get; set; }
        public DateTime DeleteDate { get; set; }
        public bool Aktif  { get; set; }
    }
    public class StoklarList
    {

        public string? KategoriIsmi { get; set; }
        public string? StokKodu { get; set; } 
        public string? StokAdı { get; set; } 
        public string? Aciklama { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public string? BirimIsmi { get; set; }
        public decimal? BirimFiyat { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal Desi { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }
        public decimal? KDVOranı { get; set; }
        public string?  DilIsmi { get; set; }

    }
    public class StoklarInsert
    {
        public int? KategoriId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdı { get; set; }
        public string? Aciklama { get; set; }
        public int? ParaBirimiId { get; set; }
        public int? BirimId { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOranı { get; set; }
        public int? Dil_id { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }


    }
    public class StoklarInsertResponse
    {
        public int Id { get; set; }
        public int? KategoriId { get; set; }
        public string? KatagoriIsmi { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? Aciklama { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public int? BirimId { get; set; }
        public string? BirimIsmi { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOranı { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal Desi { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }
        public int? Dil_id { get; set; }
        public string? DilIsmi { get; set; }

    }
    public class StoklarUpdate
    {
        public int? Id { get; set; }
        public int? KategoriId { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdı { get; set; }
        public string? Aciklama { get; set; }
        public int? ParaBirimiId { get; set; }
        public int? BirimId { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOranı { get; set; }
        public int Dil_id { get; set; }

    }
    public class StoklarListResponse
    {
        public int? Id { get; set; }
        public int? KategoriId { get; set; }
        public string? KategoriIsmi { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? Aciklama { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public int? BirimId { get; set; }
        public string? BirimIsimi { get; set; }
        public decimal? BirimFiyat { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal Desi { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }
        public decimal? KDVOranı { get; set; }
        public int? Dil_id { get; set; }
        public string? DilIsmi { get; set; }
        public string? Resim { get; set; }
        public float ToplamTutar { get; set; }

    }
    public class StoklarDetails
    {
        public int? Id { get; set; }
        public int? KategoriId { get; set; }
        public string? KategoriIsmi { get; set; }
        public string? StokKodu { get; set; }
        public string? StokAdi { get; set; }
        public string? Aciklama { get; set; }
        public int? ParaBirimiId { get; set; }
        public string? ParaBirimiIsmi { get; set; }
        public int? BirimId { get; set; }
        public string? BirimIsimi { get; set; }
        public decimal? BirimFiyat { get; set; }
        public decimal? KDVOranı { get; set; }
        public string Marka { get; set; }
        public decimal AmbalajAgirlik { get; set; }
        public decimal AmbalajsizAgirlik { get; set; }
        public decimal Desi { get; set; }
        public decimal En { get; set; }
        public decimal Boy { get; set; }
        public decimal Yukseklik { get; set; }
        public int? Dil_id { get; set; }
        public string? DilIsmi { get; set; }
        public IEnumerable<ImageDetail>? Resim { get; set; }

    }


}
