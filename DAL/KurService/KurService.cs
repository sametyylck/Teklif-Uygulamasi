using DAL.DTO;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DAL.Service.KurService
{
    public class KurService : IKurService
    {
        private readonly IDbConnection _db;

        public KurService(IDbConnection db)
        {
            _db = db;
        }

        public Task<decimal> TarihsizKur(string kod,int FirmaId)
        {
            DynamicParameters prm = new();

            if (kod=="TL" || kod=="tl")
            {
                decimal deger = 1;
                return Task.FromResult(deger);
            }
            var tarih = DateTime.Now;
            string sql = $@"select * from GunlukKur where KurIsmi='{kod}' and  convert(date,Tarih) LIKE '%{tarih.Date.ToString("yyyy-MM-dd")}%'";
            var list = _db.Query<GunlukKur>(sql,prm);
            if (list.Count()!=0)
            {
                if (tarih.Hour>=list.First().Tarih.Hour+2)
                {
                    goto devam;
                }
                else
                {
                    return Task.FromResult(list.First().Deger);

                }

            }

            devam:
            XmlDocument xml = new XmlDocument();
          
            string xmlStr;

            string[] newTarih;
            string gun, ay, yil;

            if (tarih.DayOfWeek == DayOfWeek.Saturday)
            {
                tarih = tarih.AddDays(-1);
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
            }
            else if (tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                tarih = tarih.AddDays(-2);
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
            }
            else
            {
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
            }

            string url = string.Format("https://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml", yil, ay, gun);

            try
            {
                using (var wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(url);
                }
            }
            catch
            {
            yenidenKurCek:
                tarih = tarih.AddDays(-1);
                string sql2 = $@"select * from GunlukKur where KurIsmi='{kod}' and  convert(date,Tarih) LIKE '%{tarih.Date.ToString("yyyy-MM-dd")}%'";
                var list2 = _db.Query<GunlukKur>(sql2,prm);
                if (list2.Count() != 0)
                {
                    
                        return Task.FromResult(list2.First().Deger);

                }

                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
                url = string.Format("https://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml", yil, ay, gun);
                try
                {
                    using (var wc = new WebClient())
                    {
                        xmlStr = wc.DownloadString(url);
                    }

                }
                catch (Exception)
                {
                    goto yenidenKurCek;
                }
            }

            xml.LoadXml(xmlStr);

            var Tarih_Date_Nodes = xml.SelectSingleNode("//Tarih_Date");
            var CurrencyNodes = Tarih_Date_Nodes.SelectNodes("//Currency");
            int CurrencyLength = CurrencyNodes.Count;


            decimal dovizSatis = 0;
            for (int i = 0; i < CurrencyLength; i++)
            {
                var cn = CurrencyNodes[i];

                if (cn.Attributes["CurrencyCode"].Value == kod)
                {
                    
                    dovizSatis = Convert.ToDecimal(cn.ChildNodes[4].InnerXml.Replace(".", ","));
                    prm.Add("@Tarih", tarih);
                    prm.Add("@doviz", dovizSatis);     
                    prm.Add("@FirmaId", FirmaId);
                    prm.Add("@kod", kod);
                    if (list.Count()==0)
                    {
                        var sqlquery = @$"Insert into GunlukKur (KurIsmi,Tarih,Deger,FirmaId) values  (@kod,'{tarih}',@doviz,@FirmaId)";
                        _db.Execute(sqlquery, prm);
                        return Task.FromResult(dovizSatis);
                    }
                    else
                    {
                        prm.Add("@tarih", tarih);
                        prm.Add("@doviz", dovizSatis);
                        prm.Add("@FirmaId", FirmaId);
                        prm.Add("@Id", list.First().Id);

                        var sqlqu = @$"Update GunlukKur set Tarih=@tarih,Deger=@doviz  where FirmaId=@FirmaId and Id=@Id";
                        _db.Execute(sqlqu, prm);
                        return Task.FromResult(dovizSatis);
                    }    
                    
                   
                }
            }
            return Task.FromResult(dovizSatis);

        }

        public Task<decimal> TarihliKur(DateTime tarih, string kod)
        {
            XmlDocument xml = new XmlDocument();
            string xmlStr;

            string[] newTarih;
            string gun, ay, yil;

            if (tarih.DayOfWeek == DayOfWeek.Saturday)
            {
                tarih = tarih.AddDays(-1);
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
            }
            else if (tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                tarih = tarih.AddDays(-2);
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
            }
            else
            {
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
            }

            string url = string.Format("https://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml", yil, ay, gun);

            try
            {
                using (var wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(url);
                }
            }
            catch
            {
            yenidenKurCek:
                tarih = tarih.AddDays(-1);
                newTarih = tarih.ToString("dd.MM.yyyy").Split('.');
                gun = newTarih[0];
                ay = newTarih[1];
                yil = newTarih[2];
                url = string.Format("https://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml", yil, ay, gun);
                try
                {
                    using (var wc = new WebClient())
                    {
                        xmlStr = wc.DownloadString(url);
                    }

                }
                catch (Exception)
                {
                    goto yenidenKurCek;
                }
            }

            xml.LoadXml(xmlStr);

            var Tarih_Date_Nodes = xml.SelectSingleNode("//Tarih_Date");
            var CurrencyNodes = Tarih_Date_Nodes.SelectNodes("//Currency");
            int CurrencyLength = CurrencyNodes.Count;


            decimal dovizSatis = 0;
            for (int i = 0; i < CurrencyLength; i++)
            {
                var cn = CurrencyNodes[i];

                if (cn.Attributes["CurrencyCode"].Value == kod)
                {
                    dovizSatis = Convert.ToDecimal(cn.ChildNodes[4].InnerXml.Replace(".", ","));
                    return Task.FromResult(dovizSatis);
                }
            }
            return Task.FromResult(dovizSatis);
        }
    }
}
