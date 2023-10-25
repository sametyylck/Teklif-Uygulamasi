using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class Image
    {
        public IFormFile Resim { get; set; }
    }

    public class CizimA
    {
        public List<IFormFile> Resim { get; set; }
    }

    public class ImageUpdate
    {
        public IFormFile Resim { get; set; }
    }
    public class ImageList
    {
        public int? Id { get; set; }
        public int? StokId { get; set; }
        public string? Resim { get; set; }
        public string? Path { get; set; }
    }
    public class CizimList
    {
        public int? Id { get; set; }
        public int? TeklifId { get; set; }
        public string Resim { get; set; }
    }

    public class ImageDetail
    {
        public int? Id { get; set; }
        public int? StokId { get; set; }
        public string? Resim { get; set; }
    }


}
