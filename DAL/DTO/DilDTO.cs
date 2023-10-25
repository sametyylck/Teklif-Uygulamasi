using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class DilDTO
    {
    
            public int Id { get; set; }
            public string? Isim { get; set; }
        
    }
    public class DilInsert
    {
        public string? Isim { get; set; }

    }
    public class DilUpdate
    {
        public int Id { get; set; }
        public string? Isim { get; set; }

    }
}
