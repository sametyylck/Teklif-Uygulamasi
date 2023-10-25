using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class AltFirmaDetails
    {
        public int ID { get; set; }
        public string FirmaAd { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Adres { get; set; }
        public IEnumerable<AltFirmaOzellikDetail> Ozellik { get; set; }
    }

    public class AltFirmaDTO
    {
        public int ID { get; set; }
        public string FirmaAd { get; set; }
        public string Tel { get; set; }
        public string  Mail { get; set; }
        public string Adres { get; set; }
        public int OzellikId { get; set; }
        public string Logo { get; set; }
        public string AltMetin { get; set; }

    }
    public class AltFirmaList
    {
        public int ID { get; set; }
        public string FirmaAd { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Adres { get; set; }


    }

    public class AltFirmaUpdate
    {
        public int Id { get; set; }
        public string FirmaAd { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Adres { get; set; }

    }
    public class AltFirmaInsert
    {
        public string FirmaAd { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public string Adres { get; set; }


    }
    public class AltFirmaOzellik
    {
        public int ID { get; set; }
        public string FirmaAd { get; set; }
        public string Logo { get; set; }
        public string AltMetin { get; set; }
        public int AltFirmaId { get; set; }

    }
    public class AltFirmaOzellikInsert
    {
        public IFormFile Logo { get; set; }
        public string AltMetin { get; set; }
        public int AltFirmaId { get; set; }

    }
    public class AltFirmaOzellikDetail
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string AltMetin { get; set; }
        public int AltFirmaId { get; set; }

    }

    public class AltFirmaOzellikUpdate
    {
        public int Id { get; set; }
        public IFormFile Logo { get; set; }
        public string AltMetin { get; set; }
        public int AltFirmaId { get; set; }

    }


}
