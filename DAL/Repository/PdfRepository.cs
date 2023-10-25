using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class Deneme12 : IPdf
    {
        private readonly IDbConnection _db;
        private readonly IHostingEnvironment _host;

        public Deneme12(IDbConnection db, IHostingEnvironment host)
        {
            _db = db;
            _host = host;
        }

        public string header()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Document</title>
    <style>
      @media print {
        body {
          width: 21cm;
          height: 29.7cm;
          margin: 30mm 45mm 30mm 45mm;
          /* change the margins as you want them to be. */
        }
      }
      body {
        width: 21cm;
        height: 29.7cm;
        margin: 30mm 45mm 30mm 45mm;
        font-family: Arial, sans-serif;
        font-size: 14px;
        /* change the margins as you want them to be. */
      }
      .w100 {
        width: 100%;
      }
      .flex {
        display: flex;
        flex-direction: column;
      }
      table {
        border-collapse: collapse;
      }
      th,
      .category {
        color: #ff0000;
        font-weight: 600;
      }
      th,
      td {
        text-align: left;
        padding: 0.5rem 0.5rem;
      }
      table tr th {
        border-bottom: 1px solid black;
        padding: 0px 0.5rem;
      }
      table tr td:not(.category) {
        border-bottom: 6px solid #dbdbdb;
      }
      table tr td img {
        width: 99px;
      }
    </style>
  </head>
  <body>
    <div class=""flex w100"">
      <img src=""logo.png"" style=""height=100px"" alt=""Logo"" />

      <div>
        <div style=""display: flex; justify-content: center; font-weight: 600; font-size: larger"">Teklif Formu</div>
        <div style=""display: flex; justify-content: space-between"">
          <div><span style=""flex: 1 0 0%"">Firma:</span> <b>Geçgel Otomotiv</b></div>
          <div><span>Tarih:</span> <span>10.05.2021</span></div>
        </div>
        <div style=""display: flex; justify-content: space-between"">
          <div><span>Adres:</span></div>
          <div><span>Teklif No:</span> <span>A10052100006</span></div>
        </div>
        <div style=""display: flex; justify-content: space-between"">
          <div><span>İlgili:</span></div>
          <div><span>Vergi Dairesi:</span></div>
        </div>
        <div style=""display: flex; justify-content: space-between"">
          <div><span>Tel:</span></div>
          <div><span>Vergi No:</span></div>
        </div>
      </div>

      <table class=""w100"">
        <tr>
          <th>Kod</th>
          <th>Ürün</th>
          <th>Açıklama</th>
          <th>Miktar</th>
          <th>Birim Fiyat</th>
          <th>Tutar</th>
          <th></th>
        </tr>
</table>
</body>
</html>");
            return sb.ToString();
        }


     

        public string Pdf(int TeklifId, int FirmaId, int KullanıcıId)
        {


            var sb = new StringBuilder();

            string sqlquery = $@"select DISTINCT se.KategoriIsmi
            from TeklifDetay se
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId}
			Order By se.KategoriIsmi";
            var kategori = _db.Query<TeklifDetayInsertResponse>(sqlquery);
            sb.Append(@"<<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Document</title>
    <link rel=""stylesheet"" href=""assets/styles.css"">
    <style>
        @media print {
            body {
                width: 21cm;
                height: 29.7cm;
                margin: 30mm 45mm 30mm 45mm;
                /* change the margins as you want them to be. */
            }
        }

        body {
            width: 21cm;
            height: 29.7cm;
            margin: 30mm 45mm 30mm 45mm;
            font-family: Arial, sans-serif;
            font-size: 14px;
            /* change the margins as you want them to be. */
        }

        .w100 {
            width: 100%;
        }

        .flex {
            display: flex;
        }

        table {
            border-collapse: collapse;
        }

        th,
        .category {
            color: #ff0000;
            font-weight: 600;
        }

        th,
        td {
            text-align: left;
            padding: 0.5rem 0.5rem;
        }

        table tr th {
            border-bottom: 1px solid black;
            padding: 0px 0.5rem;
        }

        table tr td:not(.category) {
            border-bottom: 6px solid #dbdbdb;
        }

        table tr td img {
            width: 60px;
        }

        .title {
            text-align: center;
            font-weight: 600;
            font-size: larger;
        }

        .info div div:nth-child(2n + 1) {
            float: left;
        }

        .info div div:nth-child(2n) {
            float: right;
        }

        .info div::after {
            content: """";
            clear: both;
            display: table;
        }



    </style>

</head>
<body>
  
        <table class=""w100"">   
   ");

            foreach (var item in kategori)
            {


                sb.Append(@$"<tr>
            <td style=""color:red"">{item.KategoriIsmi}</td>
            <tr>");
                string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.StokId,
            se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,StokResim.Resim
            from TeklifDetay se
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId} and se.KategoriIsmi='{item.KategoriIsmi}'
			Order By se.StokAdi";
                var detay = _db.Query<TeklifDetayInsertResponse>(sql);
                decimal? toplam = 0;
                decimal? toplamiskonto = 0;
                decimal? toplamkdv = 0;
                decimal? Toplamanatutar;
                foreach (var icerik in detay)
                {
                    string path = _host.WebRootPath + $"\\{icerik.Resim}";
                    toplam = (icerik.Miktar * icerik.BirimFiyat);
                    var kdv = toplam * (icerik.KDVOrani / 100);
                    var iskontodegeri = toplam * (icerik.IskontoOrani / 100);
                    var iskontolutoplam = toplam - iskontodegeri;
                    var geneltoplam = iskontolutoplam + kdv;
                    toplam += geneltoplam;
                    sb.Append(@$" 
    
         <tr>
        <td>{icerik.StokKodu}</td>
        <td><img src=""{path}"" /></td>
        <td>
          <div>Patates Soyma Makinesi</div>
          <div>{icerik.Aciklama}</div>
          <div>10 kg/sefer, 380 V</div>
        </td>
        <td>{icerik.Miktar}</td>
        <td>{icerik.BirimFiyat}</td>
        <td>{iskontolutoplam}</td>
        <td></td>
      </tr>
            ");
                


                }
                sb.Append(@$" 
             <tr>
        <td><td>
        <td>
          <div></div>
          <div></div>
          <div></div>
        </td>
        <td></td>
        <td></td>
        <td></td>
        <td>Total:{toplam}</td>
      </tr>");

            }

            sb.Append(@"        </table>
</body>
</html>");

            return sb.ToString();
        }








        //        public string Pdfeski()
        //        {
        //            string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.StokId,se.BirimId,Birim.Isim,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,StokResim.Resim 
        //            from TeklifDetay se
        //			left join StokResim on StokResim.StokId=se.StokId
        //            left join Birim on Birim.Id=se.BirimId
        //            where se.TeklifId=7 and se.FirmaId=2";
        //            var list = _db.Query<Pdf>(sql);
        //            var sb = new StringBuilder();

        //            sb.Append(@"<html>
        //	<head>
        //		<meta charset=""utf-8"">
        //		<title>Teklif Forumu</title>
        //		<link rel=""stylesheet"" href=""style.css"">
        //	</head>
        //	<body>
        //		<header>
        //			<h1>Teklif Forumu</h1>
        //			<address contenteditable>
        //			       <table  class=""inventory"">
        //                <tr>
        //                    <th><span contenteditable>FirmaAdı</span></td>
        //                    <th><span contenteditable>aadsdsd</span></td>
        //                </tr>
        //                <tr>
        //                    <th><span contenteditable>Adres</span></td>
        //                    <th><span contenteditable>fdgfdgfdg</span></td>
        //                </tr>
        //                <tr>
        //                    <th><span contenteditable>Tel</span></td>
        //                    <th><span contenteditable>045515135</span></td>
        //                </tr>
        //                <tr>
        //                    <th><span contenteditable>Mail</span></td>
        //                    <th><span contenteditable>aa@gmail.com</span></td>
        //                </tr>
        //            </table>
        //			</address>
        //			<span><img alt="""" src=""""><input type=""file"" accept=""image/*""></span>
        //		</header>
        //		<article>
        //			<address contenteditable>
        //				<p>Some Company<br>c/o Some Guy</p>
        //			</address>
        //			<table class=""meta"">
        //				<tr>
        //					<th><span contenteditable>Invoice #</span></th>
        //					<td><span contenteditable>101138</span></td>
        //				</tr>
        //				<tr>
        //					<th><span contenteditable>Date</span></th>
        //					<td><span contenteditable>January 1, 2012</span></td>
        //				</tr>
        //				<tr>
        //					<th><span contenteditable>Amount Due</span></th>
        //					<td><span id=""prefix"" contenteditable>$</span><span>600.00</span></td>
        //				</tr>
        //			</table>
        //			<table class=""inventory"">
        //				<thead>
        //					<tr>
        //						<th><span contenteditable>Resim</span></th>
        //						<th><span contenteditable>StokAdı</span></th>
        //						<th><span contenteditable>Aciklama</span></th>
        //						<th><span contenteditable>BirimFiyat</span></th>
        //						<th><span contenteditable>Miktar</span></th>
        //					</tr>
        //				</thead>
        //                       ");
        //            foreach (var emp in list)
        //            {
        //                string path = _host.WebRootPath + $"\\{emp.Resim}";
        //                sb.AppendFormat(@$" <tbody>
        //					<tr>
        //                        <td><span data-prefix></span><span><img src='{path}' style=""width: 90px;""></span></td>
        //						<td><a class=""cut"">-</a><span contenteditable>{emp.StokAdi}</span></td>
        //						<td><span contenteditable>{emp.Aciklama}</span></td>
        //						<td><span data-prefix>$</span><span contenteditable>{emp.BirimFiyat}</span></td>
        //						<td><span contenteditable>{emp.Miktar}</span></td>
        //					</tr>
        //			 ");
        //            }
        //            sb.Append(@$"
        //        ");

        //            sb.Append(@" 

        //                	</tbody>
        //			</table>
        //                <table class=""balance"">
        //				<tr>
        //					<th><span contenteditable>Total</span></th>
        //					<td><span data-prefix>$</span><span>600.00</span></td>
        //				</tr>
        //				<tr>
        //					<th><span contenteditable>Amount Paid</span></th>
        //					<td><span data-prefix>$</span><span contenteditable>0.00</span></td>
        //				</tr>
        //				<tr>
        //					<th><span contenteditable>Balance Due</span></th>
        //					<td><span data-prefix>$</span><span>600.00</span></td>
        //				</tr>
        //			</table>
        //		</article>
        //	</body>
        //</html>  ");
        //            return sb.ToString();
        //        }

    }


}
