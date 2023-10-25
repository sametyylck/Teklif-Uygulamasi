using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class BirimDTO
    {
        public int Id { get; set; }
        public string? Isim { get; set; }
    }
    public class BirimInsert
    {
        public string? Isim { get; set; }
    }
    public class BirimUpdate
    {
        public int Id { get; set; }
        public string? Isim { get; set; }
    }

}
