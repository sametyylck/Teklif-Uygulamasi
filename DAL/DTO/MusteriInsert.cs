using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class MusteriInsert
    {

        public string Unvan { get; set; } = null!;
        public string Adres { get; set; } = null!;
        public string Telefon { get; set; } = null!;
        public string? Mail { get; set; }
        public string? VergiDairesi { get; set; }
        public int? VergiNo { get; set; }

    }
    public class MusteriUpdate
    {

        public int Id { get; set; }
        public string Unvan { get; set; } = null!;
        public string Adres { get; set; } = null!;
        public string Telefon { get; set; } = null!;
        public string? Mail { get; set; }
        public string? VergiDairesi { get; set; }
        public int? VergiNo { get; set; }
    }
    public class IdControl
    {

        public int? Id { get; set; }
    }
    public class MusteriList
    {
        public string? Unvan { get; set; } 
        public string? Adres { get; set; } 
        public string? Telefon { get; set; } 
        public string? Mail { get; set; }
    }

}
