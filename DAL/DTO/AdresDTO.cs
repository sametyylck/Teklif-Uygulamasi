using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class AdresDTO
    {
        public int Id { get; set; }
        public string AdresAdi { get; set; }
        public int MusteriId { get; set; }
        public string AdSoyad { get; set; }
        public string SirketIsmi { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Ulke { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string Adres { get; set; }

    }
   
    public class AdresInsert
    {
        public string AdresAdi { get; set; }
        public int MusteriId { get; set; }
        public string AdSoyad { get; set; }
        public string SirketIsmi { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Ulke { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string Adres { get; set; }

    }
    public class AdresUpdate
    {
        public int Id { get; set; }
        public string AdresAdi { get; set; }
        public int MusteriId { get; set; }
        public string AdSoyad { get; set; }
        public string SirketIsmi { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Ulke { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string Adres { get; set; }

    }


}
