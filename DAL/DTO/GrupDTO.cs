using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class GrupDTO
    {
        public int Id { get; set; }
        public int TeklifId { get; set; }
        public string Harf { get; set; }
        public string Anlami { get; set; }
    }
    public class GrupInsert
    {
        public int TeklifId { get; set; }
        public string Harf { get; set; }
        public string Anlami { get; set; }
    }
    public class GrupUpdate
    {
        public int Id { get; set; }
        public int TeklifId { get; set; }
        public string Harf { get; set; }
        public string Anlami { get; set; }
    }
}
