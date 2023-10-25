using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class GunlukKur
    {
        public int Id { get; set; }
        public string KurIsmi { get; set; }
        public DateTime Tarih { get; set; }
        public decimal Deger { get; set; }
    }
}
