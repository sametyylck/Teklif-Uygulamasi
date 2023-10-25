using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class StocKategoriDTO
    {
        public int Id { get; set; }
        public string? Isim { get; set; }
        public int Dil_id { get; set; }
    }
    public class StocKategoriInsert
    {
        public string? Isim { get; set; }
        public int Dil_id { get; set; }
    }
}
