using BL.Control.Resim;
using BL.Control.Teklif;
using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using DAL.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using PdfSharp;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing.Printing;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace TMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeklifController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IDbConnection _db;
        private readonly ITeklifler _tk;
        private readonly IRevizyon _rz;
        private readonly IPdf _pdf;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _host;
        private readonly IMemoryCache _memorycath;
        private readonly ITeklifControl _teklifcontrol;
        public TeklifController(IUserService userservice, IDbConnection db, ITeklifler tk, IRevizyon rz, IPdf pdf, Microsoft.AspNetCore.Hosting.IHostingEnvironment host, IMemoryCache memorycath, ITeklifControl teklifcontrol)
        {
            _userservice = userservice;
            _db = db;
            _tk = tk;
            _rz = rz;
            _pdf = pdf;
            _host = host;
            _memorycath = memorycath;
            _teklifcontrol = teklifcontrol;
        }

        [HttpPost("Insert"), Authorize]
        public async Task<ActionResult<TeklifUpdate>> Insert(int SepetId)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _tk.Insert(SepetId, CompanyId, KullanıcıId);
            await _tk.InsertDetay(id, SepetId, CompanyId, KullanıcıId);
            var list = await _db.QueryAsync<TeklifListe>(@$"select s.Id as TeklifId,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.IslemTarihi,s.IskontoOrani,S.IskontoBirim,s.StokKoduGoster,s.MarkaGoster,s.IskontoGoster
            from Teklifler s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId={CompanyId} and s.Id={id} and  s.Aktif=1");
            foreach (var item in list)
            {
                string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.StokId,se.BirimId,Birim.Isim as BirimIsmi,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim 
            from TeklifDetay se
            left join Birim on Birim.Id=se.BirimId
            where se.TeklifId={item.TeklifId} and se.FirmaId={CompanyId}";
                var detay = await _db.QueryAsync<TeklifDetayInsertResponse>(sql);
                item.TeklifDetay = detay;
            }
            return Ok(list);


        }

        [HttpPost("InsertTeklifDetay"), Authorize]

        public async Task<ActionResult<TeklifUpdate>> InsertTeklifDetay(InsertTeklifDetay T)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _tk.InsertTeklifDetay(T, CompanyId, KullanıcıId);
            string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.StokId,se.BirimId,Birim.Isim as BirimIsmi,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim 
            from TeklifDetay se
            left join Birim on Birim.Id=se.BirimId
            where se.Id={id} and se.FirmaId={CompanyId}";
            var detay = await _db.QueryAsync<TeklifDetayInsertResponse>(sql);


            return Ok(detay);


        }


        [HttpPost("TeklifGonderme"), Authorize]
        public async Task<ActionResult<TeklifUpdate>> TeklifGonderme(int TeklifId, int AltFirmaId, int Dil, int GecerlilikTarihi)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var TeklifTarihi = DateTime.Now;
            var hata = await _teklifcontrol.Control(TeklifId, Dil, CompanyId);
            if (hata.Count() == 0)
            {

                return Ok();
            }
            else
            {

                return Ok(hata);
            }



        }

        [HttpPut("Update"), Authorize]
        public async Task<ActionResult<SepetUpdate>> Update(TeklifUpdate request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _tk.Update(request, CompanyId, KullanıcıId);
            string sql = $@"select s.Id,s.MusteriId,s.ParaBirimiId,ParaBirimleri.Kod,s.IslemTarihi,s.IskontoOrani,s.Kur,s.FirmaId,s.MarkaGoster,s.IskontoGoster,s.StokKoduGoster from Teklifler s
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            where s.Id={request.Id}";
            var list = await _db.QueryAsync<TeklifInsertResponse>(sql);
            return Ok(list);


        }
        [HttpPut("UpdateDetay"), Authorize]
        public async Task<ActionResult<TeklifDetayUpdate>> UpdateDetay(TeklifDetayUpdate request)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _tk.UpdateDetay(request, CompanyId, KullanıcıId);
            string sql = $@"select se.Id,se.TeklifId,se.Aciklama,se.StokId,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim,se.Hediye 
            from TeklifDetay se
            where se.Id={request.Id} and FirmaId={CompanyId}";
            var list = await _db.QueryAsync<TeklifDetayUpdate>(sql);
            return Ok(list);

        }
        [HttpDelete("Delete"), Authorize]
        public async Task<ActionResult<dynamic>> Delete(int id)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _tk.DeleteSoft(id, CompanyId);
            return Ok("Basarli sekilde silindi");
        }
        [HttpDelete("DeleteDetay"), Authorize]
        public async Task<ActionResult<dynamic>> DeleteDetay(int id)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _tk.DeleteDetay(id, CompanyId);
            return Ok("Basarli sekilde silindi");


        }

        [HttpGet("List"), Authorize]
        public async Task<ActionResult<TeklifDTO>> List(string? kelime, int? KAYITSAYISI, int? SAYFA)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _tk.List(kelime, CompanyId, KAYITSAYISI, SAYFA);
            var count = await _tk.Count(CompanyId);

            return Ok(new { list, count });

        }
        [HttpGet("Details"), Authorize]
        public async Task<ActionResult<SepetDTO>> Details(int id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _tk.Details(id, CompanyId);
            return Ok(list);

        }

        [HttpPost("Revizyon"), Authorize]
        public async Task<ActionResult<TeklifDTO>> Revizyon(int TeklifId)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            int id = await _rz.RevizyonInsert(TeklifId, CompanyId, KullanıcıId);
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@FirmaId", CompanyId);
            prm.Add("@id", id);
            var list = await _db.QueryAsync<TeklifListe>(@$"select s.Id as TeklifId,s.AltFirmaId,Firma.FirmaAd,s.MusteriId, Musteriler.Unvan as MusteriUnvani,s.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,s.Dil_id,Dil.Isim as DilIsmi,s.TeklifTarihi,s.IslemTarihi,s.GecerlilikTarihi,s.Kur,s.IskontoOrani
            from Teklifler s
            left join Firma on Firma.Id=s.AltFirmaId
            left join ParaBirimleri on ParaBirimleri.Id=s.ParaBirimiId
            left join Dil on Dil.Id=s.Dil_id
            left join Musteriler on Musteriler.Id=s.MusteriId
            where s.FirmaId={CompanyId} and s.Id={id} and  s.Aktif=1");
            foreach (var item in list)
            {
                string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.StokId,se.BirimId,Birim.Isim,se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,se.IskontoBirim 
            from TeklifDetay se
            left join Birim on Birim.Id=se.BirimId
            where se.TeklifId={item.TeklifId} and se.FirmaId={CompanyId}";
                var detay = await _db.QueryAsync<TeklifDetayInsertResponse>(sql);
                item.TeklifDetay = detay;
            }

            return Ok(list);


        }

        [HttpPost("Cizim"), Authorize]
        public async Task<ActionResult<TeklifUpdate>> Cizim([FromForm] CizimA a, int TeklifId)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            await _tk.Cizim(a, TeklifId, CompanyId, KullanıcıId);
            return Ok();


        }


        [HttpPost("TeklifOnay"), Authorize]
        public async Task<ActionResult<TeklifDTO>> TeklifOnay(int TeklifId)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            return Ok();


        }


        [HttpGet("PdfHtml"), Authorize]
        public async Task<ActionResult<PdfTeklifResponse>> PdfHtml(int TeklifId, int AltFirmaId, int Dil, string? html)
        {
            var getlist = _userservice.GetId();
            int FirmaId = getlist[0];
            int KullanıcıId = getlist[1];
            string slq = $@"select Id from Teklifler tk          
            where tk.Id={TeklifId} and tk.FirmaId={FirmaId}";
            var varmi = await _db.QueryAsync<PdfTeklifResponse>(slq);
            if (varmi.Count() == 0)
            {
                return BadRequest("Boyle bir id yok");
            }



            var sqsl = @$"Update Teklifler set  AltFirmaId={AltFirmaId},Dil_id={Dil} where Id={TeklifId} and FirmaId={FirmaId} ";
            await _db.ExecuteAsync(sqsl);


            string sqqlq = $@"select tk.AltFirmaId,tk.MusteriId,Musteriler.Unvan,Musteriler.Telefon,Musteriler.Mail,Musteriler.Adres,tk.TeklifTarihi,tk.GecerlilikTarihi,
            FirmaOzellikler.Logo,FirmaOzellikler.AltMetin
            from Teklifler tk 
            left join FirmaOzellikler on FirmaOzellikler.AltFirmaId=tk.AltFirmaId
            left join Musteriler on Musteriler.Id=TK.MusteriId
            where tk.Id={TeklifId} and tk.FirmaId={FirmaId}";
            var teklif = await _db.QueryAsync<PdfTeklifResponse>(sqqlq);


            string kelime = "";
            var rnd = new Random();
            for (int i = 0; i < 13; i++)
            {
                kelime += ((char)rnd.Next('A', 'Z')).ToString();
            }

            string sqlquery = $@"select DISTINCT(se.Grup),TeklifGrup.Anlami
            from TeklifDetay se
			left join TeklifGrup on TeklifGrup.Harf=se.Grup
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId}
			Order By se.Grup";
            var kategorisimi = _db.Query<TeklifDetayInsertResponse>(sqlquery);


            string sqql = $@"select Count(*) from(
            select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.StokId,
            se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,StokResim.Resim
            from TeklifDetay se
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId} and se.Aktif=1
			)as kayitsayisi";
            var dongu = await _db.QueryFirstAsync<decimal>(sqql);
            var kacadet = dongu / 10;
            kacadet = Math.Ceiling(kacadet);

            DynamicParameters prm = new();
            var sb = new StringBuilder();
            int sayac = 0;
            var kategori = 0;
            decimal? toplamkackaldı = 0;
            decimal? geneltoplam = 0;
            decimal? ToplamKdvTutari = 0;
            decimal? KdvsizToplam = 0;
            decimal? kactane = 0;
            decimal? kdvlitoplam = 0;
            decimal? header = 0;
            var isim = 0;
            foreach (var aa in kategorisimi)
            {
                string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.StokId,
            se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,TeklifGrup.Anlami as GrupKarisilik,se.IskontoOrani,Stoklar.ParaBirimiId,se.Grup,ParaBirimleri.Kod as ParaBirimiIsmi,StokResim.Resim
            from TeklifDetay se
			left join TeklifGrup on TeklifGrup.Harf=se.Grup

			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId} and se.Grup='{aa.Grup}'
			Order By se.Grup";
                kategori++;
                isim = 0;
                var detay = _db.Query<TeklifDetayInsertResponse>(sql);
                if (aa.KategoriIsmi == null)
                {
                    aa.KategoriIsmi = "";
                }

                if (sayac != 0)
                {



                    sb.Append(@$" 
             <tr style=""page-break-inside: avoid;"">
        <td><td>
        <td>
          <div></div>
          <div></div>
          <div></div>
        </td>
        <td></td>
        <td></td>
        <td></td>
         <td  class=""tdk"" style=""color:blue"">Total:{kdvlitoplam?.ToString("N")}</td>

      </tr>");




                    kdvlitoplam = 0;
                    if (kactane == 7)
                    {
                        sb.Append(@$"<tr  style=""page-break-before: always;""><td class=""category""  style=""color:blue"">{aa.Grup + " " + aa.GrupKarisilik}+</td></tr>");
                        kactane = 0;

                    }
                    else
                    {
                        sb.Append(@$"<tr  style=""page-break-inside: avoid;""><td class=""category""  style=""color:blue"">{aa.Grup + " " + aa.GrupKarisilik}</td></tr>");

                    }

                }

                foreach (var icerik in detay.Select((value, index) => new { value, index }))
                {
                    string path = _host.WebRootPath + $"\\{icerik.value.Resim}";
                    prm.Add("@Resim", path);
                    decimal? birim = 0;
                    if (icerik.value.IskontoBirim == true)
                    {
                        birim = (icerik.value.BirimFiyat - icerik.value.IskontoOrani);

                    }
                    else
                    {
                        birim = (icerik.value.BirimFiyat - (icerik.value.BirimFiyat * icerik.value.IskontoOrani / 100));

                    }
                    var KdvTutari = birim * (icerik.value.KDVOrani / 100);
                    ToplamKdvTutari += KdvTutari;
                    decimal? toplamkdvsiz = birim * icerik.value.Miktar;
                    KdvsizToplam += toplamkdvsiz;
                    decimal? ToplamKdvli = (birim + KdvTutari) * icerik.value.Miktar;
                    kdvlitoplam += ToplamKdvli;
                    geneltoplam += kdvlitoplam;
                    icerik.value.BirimFiyat = birim;

                    if (header == 0)
                    {
                        foreach (var item in teklif)
                        {
                            string resim = _host.WebRootPath + $"\\{item.Logo}";
                            sb.Append(@$"<!DOCTYPE html>
<html lang=""tr"">

<head>
  <meta charset=""UTF-8"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Document</title>
 <style>
    @media print {{
      body {{
        width: 21cm;
        height: 29.7cm;
        /* change the margins as you want them to be. */
      }}
    }}

    body {{
      width: 21cm;
      height: 29.7cm;
      font-family: Arial, sans-serif;
      font-size: 14px;
      margin: 0;
      padding: 0;
      /* change the margins as you want them to be. */
    }}

    .w100 {{
      width: 100%;
    }}
     .tdk
    {{text-align:left;
      padding: 0.5rem 0.5rem;
      width: 80px;

    }}
    table {{
      border-collapse: collapse;
    }}

    th,
    .category {{
      color: #ff0000;
      font-weight: 600;
    }}

    th,
    td {{
      text-align: left;
      padding: 0.5rem 0.5rem;
    }}

    table tr th {{
      border-bottom: 1px solid black;
      padding: 0px 0.5rem;
    }}

   

    table tr td {{
      width: 60px;
      height: 20px;
    }}

    .title {{
      text-align: center;
      font-weight: 600;
      font-size: larger;
    }}
    main{{
      min-height: 100vh;
    }}

    .info div div:nth-child(2n + 1) {{
      float: right;
    }}
  </style>
  <style type=""text/css"" media=""print"">
    @page
    {{
        size: auto;
        margin: 2mm 6mm 0mm 16mm;
    }}
    body{{
      position: relative;
       
    }}
   


  .spacer {{height: 2em;}} /* height of footer + a little extra */
   footer {{left: 0;
            width: 100%;
            position: running(footer);
            height: 170px;
  }}
    thead
    {{
        display: table-header-group;
    }}
 
    /*
        tekrar etmesini istersen
        tfoot
        {{
            display: table-footer-group;
        }}
    */
    tfoot
    {{
        display: table-row-group;
        bottom: 0;

    }}
    
    
</style>
</head>
<body>
    <table id=""customers"">
        <thead>
            <tr>
                <td colspan=""10"">
                    <img src=""{resim}"" style=""width: 850px;height: 150px;""/>
                    <div>
                        <div class=""title"">Teklif Formu</div>
                        <div class=""info"">
                            <div>
                          <div><span>Tarih:</span> <b>{item.TeklifTarihi}</b></div>
                            <div><span>Firma:</span> <span>{item.Unvan}</span></div>
                          </div>
                          <div>
                            <div><span>Teklif No:</span></div>
                            <div><span>Adres No:</span> <span>{item.Adres}</span></div>
                          </div>
                          <div>
                            <div><span>Vergi Dairesi:</span></div>
                            <div><span>İlgili:</span></div>
                          </div>
                           <div>
                            <div><span>Tel:</span>{item.Telefon}</div>
                            <div><span>Vergi No:</span></div>
                          </div>
                  
                        </div>
                        <tr style=""border-bottom: 6px solid #dbdbdb;"">
                            <th>Kod</th>
                            <th>Ürün</th>
                            <th style=""width: 400px;"">Aciklama</th>
                            <th>Miktar</th>
                            <th style=""width: 45px;"">Birim Fiyat</th>
                            <th style=""width: 50px;"">KDV Orani</th>
                            <th>Tutar</th>
                            <th></th>
                          </tr>
                          <tr>
                </td>
            </tr>
            
        </thead>
        <tbody>");
                        }

                    }
                    if (sayac == 0)
                    {


                        if (isim == 0)
                        {
                            sb.Append(@$"<tr><td class=""category""  style=""color:blue;page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">{aa.Grup + " " + aa.GrupKarisilik}</td></tr>");
                            isim++;
                        }




                    }
                    sayac++;
                    header++;


                    if (kactane >= 7 && icerik.index != detay.Count())
                    {
                        sb.Append(@$" 
    
          <tr style=""page-break-before: always;border-bottom: 6px solid #dbdbdb;"">
                <td>A.01</td>
                <td><img style=""width: 65px;height:65px"" src=""{path}"" alt=""Patates Soyma Makinesi"" /></td>
                <td>
                  {icerik.value.StokAdi}<br>
                  50*70*92<br>
                  {icerik.value.Aciklama}
                </td>
                <td>{icerik.value.Miktar?.ToString("F")}</td>
                <td>{icerik.value.BirimFiyat?.ToString("F")}</td>
                <td>{icerik.value.KDVOrani?.ToString("F")}</td>
                <td>{ToplamKdvli?.ToString("F")}</td>
              </tr>
            ");
                        kactane = 0;

                    }

                    else
                    {
                        sb.Append(@$" 
    
          <tr style=""page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">
                <td>A.01</td>
                <td><img style=""width: 65px;height:65px"" src=""{path}"" alt=""Patates Soyma Makinesi"" /></td>
                <td>
                  {icerik.value.StokAdi}<br>
                  50*70*92<br>
                  {icerik.value.Aciklama}
                </td>
                <td>{icerik.value.Miktar?.ToString("F")}</td>
                <td>{icerik.value.BirimFiyat?.ToString("F")}</td>
                <td>{icerik.value.KDVOrani?.ToString("F")}</td>
                <td>{ToplamKdvli?.ToString("F")}</td>
              </tr>
            ");
                    }



                    kactane++;



                    if (sayac == dongu)
                    {


                        sb.Append(@$" 
             <tr style=""page-break-inside: avoid;"" >
        <td><td>
        <td>
          <div></div>
          <div></div>
          <div></div>
        </td>
        <td></td>
        <td></td>
        <td></td>
        <td class=""tdk"" style=""color:blue"">Total:{kdvlitoplam?.ToString("F")}</td>
      </tr>");
                        if (sayac == 6 || sayac == 7)
                        {
                            sb.Append(@$"<tr style=""page-break-before: always;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Toplam:{KdvsizToplam?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">KDV Tutarı:{KdvTutari?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Genel Toplam:{geneltoplam?.ToString("F")}</td>
          </tr>
</tbody>");

                        }
                        else
                        {
                            sb.Append(@$"<tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Toplam:{KdvsizToplam?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">KDV Tutarı:{KdvTutari?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Genel Toplam:{geneltoplam?.ToString("F")}</td>
          </tr>
</tbody>");

                        }
                        sb.Append(@"         <tfoot >
            <trstyle=""page-break-inside:avoid;"">
                <td colspan=""10"">
                    <p style=""margin-left: 20px;"">
                        Nakliye, nakliye sigortası ve montaj KONYA şehir içi firmamıza aittir. Şehir dışı alıcıya aittir.
                    </p>
                    <p style=""margin-left: 20px;"">
                        Her türlü inşaat işleri (duvar, yer, dekorasyon, elektrik, su, doğalgaz, havalandırma tesisatları) alıcıya aittir.
                    </p>
                    <p style=""margin-left: 20px;"">
                        Montaj esnasında taşıma için vinç ihtiyacı olduğu takdirde ALICI tarafından karşılanacaktır.
                    </p>
                    <p style=""margin-left: 20px;"">Fiyatlar 1 gün süre ile geçerlidir</p>
                    <p style=""margin-left: 20px;"">Teslim tarihi …/…/……</p>
                    <p style=""margin-left: 20px;"">Ürünlerin miktar değişikliklerinde karşılıklı mutabakat halinde proforma yeniden güncellenir.</p>
                    <h2 style=""text-align: center;"">Onay</h2>
                    <table style=""width: 100%;"">
                        <tr>
                          <td  style=""float: left;margin-left: 68px;font-size: 25px;"">Alıcı</td>  
                        <td style=""float: right;margin-right: 68px; font-size: 25px; "">Satıcı</td>
                        </tr>
                    </table>
                </td>
            </tr>
        </tfoot>
</table>");
                        if (kactane == 1)
                        {
                            sb.Append(@" <footer>

        <div style=""border-style: inset;margin-left: 78px;"">

          <td style=""margin-left: 100px;width: 400px;"">TMR ENDÜSTRİYEL MUTFAK ve HİJYEN EKİPMANLARI - TAMER ERVURAL / Mevlana Vd. 115 527 02632</td>
          <br>
          <td  style=""margin-left: 160px;"">MERKEZ: Ateşbaz Veli Mah. Eski Meram Cad. No:93 - Meram/KONYA +90 332 322 70 03</td>
          <br>
          <td style=""margin-left: 160px;"">ŞUBE: Fevzi Çakmak Mah. Demirkapı Cad. No:27/C - Karatay/KONYA +90 332 345 22 11
          </td>
          <br>
          
          <td  style=""margin-left: 160px;"">FABRİKA: 1. OSB. İstikamet Cad. No:19 - Selçuklu/KONYA +90 332 251 51 15
          </td>
          
        </div>
       
     
        <table style=""width: 100%;"">
          <tr>
            <td  style=""float: left;margin-left: 68px;""><a href=""info@tmrgroup.com.tr"">info@tmrgroup.com.tr</a></td>  
            <td style=""float:right; margin-right: 44px;""><a href=""tmrmutfak.com.tr"">tmrmutfak.com.tr</a></td>
          </tr>
        </table>
        
      </footer> ");

                        }
                        else if (kactane == 2)
                        {
                            sb.Append(@" <footer>

        <div style=""border-style: inset;margin-left: 78px;"">

          <td style=""margin-left: 100px;width: 400px;"">TMR ENDÜSTRİYEL MUTFAK ve HİJYEN EKİPMANLARI - TAMER ERVURAL / Mevlana Vd. 115 527 02632</td>
          <br>
          <td  style=""margin-left: 160px;"">MERKEZ: Ateşbaz Veli Mah. Eski Meram Cad. No:93 - Meram/KONYA +90 332 322 70 03</td>
          <br>
          <td style=""margin-left: 160px;"">ŞUBE: Fevzi Çakmak Mah. Demirkapı Cad. No:27/C - Karatay/KONYA +90 332 345 22 11
          </td>
          <br>
          
          <td  style=""margin-left: 160px;"">FABRİKA: 1. OSB. İstikamet Cad. No:19 - Selçuklu/KONYA +90 332 251 51 15
          </td>
          
        </div>
       
     
        <table style=""width: 100%;"">
          <tr>
            <td  style=""float: left;margin-left: 68px;""><a href=""info@tmrgroup.com.tr"">info@tmrgroup.com.tr</a></td>  
            <td style=""float:right; margin-right: 44px;""><a href=""tmrmutfak.com.tr"">tmrmutfak.com.tr</a></td>
          </tr>
        </table>
        
      </footer> ");


                        }
                        else if (kactane == 3)
                        {
                            sb.Append(@" <footer>

        <div style=""border-style: inset;margin-left: 78px;"">

          <td style=""margin-left: 100px;width: 400px;"">TMR ENDÜSTRİYEL MUTFAK ve HİJYEN EKİPMANLARI - TAMER ERVURAL / Mevlana Vd. 115 527 02632</td>
          <br>
          <td  style=""margin-left: 160px;"">MERKEZ: Ateşbaz Veli Mah. Eski Meram Cad. No:93 - Meram/KONYA +90 332 322 70 03</td>
          <br>
          <td style=""margin-left: 160px;"">ŞUBE: Fevzi Çakmak Mah. Demirkapı Cad. No:27/C - Karatay/KONYA +90 332 345 22 11
          </td>
          <br>
          
          <td  style=""margin-left: 160px;"">FABRİKA: 1. OSB. İstikamet Cad. No:19 - Selçuklu/KONYA +90 332 251 51 15
          </td>
          
        </div>
       
     
        <table style=""width: 100%;"">
          <tr>
            <td  style=""float: left;margin-left: 68px;""><a href=""info@tmrgroup.com.tr"">info@tmrgroup.com.tr</a></td>  
            <td style=""float:right; margin-right: 44px;""><a href=""tmrmutfak.com.tr"">tmrmutfak.com.tr</a></td>
          </tr>
        </table>
        
      </footer> ");

                        }
                        else if (kactane == 4)
                        {
                            sb.Append(@" <footer>

        <div style=""border-style: inset;margin-left: 78px;"">

          <td style=""margin-left: 100px;width: 400px;"">TMR ENDÜSTRİYEL MUTFAK ve HİJYEN EKİPMANLARI - TAMER ERVURAL / Mevlana Vd. 115 527 02632</td>
          <br>
          <td  style=""margin-left: 160px;"">MERKEZ: Ateşbaz Veli Mah. Eski Meram Cad. No:93 - Meram/KONYA +90 332 322 70 03</td>
          <br>
          <td style=""margin-left: 160px;"">ŞUBE: Fevzi Çakmak Mah. Demirkapı Cad. No:27/C - Karatay/KONYA +90 332 345 22 11
          </td>
          <br>
          
          <td  style=""margin-left: 160px;"">FABRİKA: 1. OSB. İstikamet Cad. No:19 - Selçuklu/KONYA +90 332 251 51 15
          </td>
          
        </div>
       
     
        <table style=""width: 100%;"">
          <tr>
            <td  style=""float: left;margin-left: 68px;""><a href=""info@tmrgroup.com.tr"">info@tmrgroup.com.tr</a></td>  
            <td style=""float:right; margin-right: 44px;""><a href=""tmrmutfak.com.tr"">tmrmutfak.com.tr</a></td>
          </tr>
        </table>
        
      </footer> ");

                        }

                        sb.Append(@"           </table>
    </div>
  </body>
</html>");

                        //var globalSettings = new GlobalSettings
                        //{

                        //    ColorMode = ColorMode.Color,
                        //    Orientation = Orientation.Portrait,
                        //    PaperSize = DinkToPdf.PaperKind.A4,
                        //    DocumentTitle = "PDF Report",
                        //    Out = @$"Pdf/{kelime + DateTime.Now.Date.ToString("yyyy-MM-dd") + TeklifId + FirmaId + KullanıcıId}.pdf", //rastgele kelime+datetime+Teklifid+FirmaId+kulanıcıid
                        //    Margins = new MarginSettings { Right = 10, Left = 10,Bottom=30 },
                        //    UseCompression = true,
                        //    DPI = 72,



                        //};

                        //var objectSettings = new ObjectSettings
                        //{
                        //    IncludeInOutline = true,
                        //    PagesCount = true,
                        //    HtmlContent = sb.ToString(),
                        //    WebSettings = { DefaultEncoding = "utf-8", PrintMediaType = true },
                        //    HeaderSettings = { FontName = "Arial", FontSize = 7, Right = "Page [page] of [toPage]" },


                        //};
                        //var pdf = new HtmlToPdfDocument()
                        //{
                        //    GlobalSettings = globalSettings,
                        //    Objects = { objectSettings }
                        //};

                        //_converter.Convert(pdf);


                    }


                }
            }
            return Ok(html + sb.ToString());

        }


        [HttpPost("TeklifResim"), Authorize]
        public async Task<ActionResult<TeklifDTO>> TeklifResim([FromForm] CizimA T, int TeklifId)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var html = await _tk.Cizim(T, TeklifId, CompanyId, KullanıcıId);
            return Ok(html);


        }


        [HttpGet("DenemeHtml"), Authorize]
        public async Task<ActionResult<PdfTeklifResponse>> DenemeHtml(int TeklifId, int AltFirmaId, int Dil, string? html)
        {
            var getlist = _userservice.GetId();
            int FirmaId = getlist[0];
            int KullanıcıId = getlist[1];
            string slq = $@"select Id from Teklifler tk          
            where tk.Id={TeklifId} and tk.FirmaId={FirmaId}";
            var varmi = await _db.QueryAsync<PdfTeklifResponse>(slq);
            if (varmi.Count() == 0)
            {
                return BadRequest("Boyle bir id yok");
            }



            var sqsl = @$"Update Teklifler set  AltFirmaId={AltFirmaId},Dil_id={Dil} where Id={TeklifId} and FirmaId={FirmaId} ";
            await _db.ExecuteAsync(sqsl);


            string sqqlq = $@"select tk.AltFirmaId,tk.MusteriId,Musteriler.Unvan,Musteriler.Telefon,Musteriler.Mail,Musteriler.Adres,tk.TeklifTarihi,tk.GecerlilikTarihi,
            FirmaOzellikler.Logo,FirmaOzellikler.AltMetin,IskontoOrani,IskontoBirim,IskontoGoster,MarkaGoster,StokKoduGoster
            from Teklifler tk 
            left join FirmaOzellikler on FirmaOzellikler.AltFirmaId=tk.AltFirmaId
            left join Musteriler on Musteriler.Id=TK.MusteriId
            where tk.Id={TeklifId} and tk.FirmaId={FirmaId}";
            var teklif = await _db.QueryAsync<PdfTeklifResponse>(sqqlq);



            string kelime = "";
            var rnd = new Random();
            for (int i = 0; i < 13; i++)
            {
                kelime += ((char)rnd.Next('A', 'Z')).ToString();
            }

            string sqlquery = $@"select DISTINCT(se.Grup),TeklifGrup.Anlami as GrupKarisilik
            from TeklifDetay se
			left join TeklifGrup on TeklifGrup.Harf=se.Grup
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId}
			Order By se.Grup";
            var kategorisimi = _db.Query<TeklifDetayInsertResponse>(sqlquery);


            string sqql = $@"select Count(*) from(
            select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.StokId,
            se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,StokResim.Resim
            from TeklifDetay se
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId} and se.Aktif=1
			)as kayitsayisi";
            var dongu = await _db.QueryFirstAsync<decimal>(sqql);
            var kacadet = dongu / 10;
            kacadet = Math.Ceiling(kacadet);

            DynamicParameters prm = new();
            var sb = new StringBuilder();
            int sayac = 0;
            var kategori = 0;
            decimal? toplamkackaldı = 0;
            decimal? geneltoplam = 0;
            decimal? ToplamKdvTutari = 0;
            decimal? KdvsizToplam = 0;
            decimal? kactane = 0;
            decimal? kdvlitoplam = 0;
            decimal? header = 0;
            var isim = 0;
            decimal? iskontobirimtoplam = 0;
            decimal? aratoplam = 0;
            bool stokkodugoster = true;
            bool markagoster = false;
            bool iskontogoster = true;
            bool iskontobirim = true;
            decimal? iskontooran = 0;
            foreach (var aa in kategorisimi)
            {
                string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.StokId,
            se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,TeklifGrup.Anlami as GrupKarisilik,se.IskontoOrani,Stoklar.ParaBirimiId,se.Grup,ParaBirimleri.Kod as ParaBirimiIsmi,StokResim.Resim,se.MarkaAdi
            from TeklifDetay se
			left join TeklifGrup on TeklifGrup.Harf=se.Grup

			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId} and se.Grup='{aa.Grup}'
			Order By se.Grup";
                kategori++;
                isim = 0;
                var detay = _db.Query<TeklifDetayInsertResponse>(sql);
                if (aa.KategoriIsmi == null)
                {
                    aa.KategoriIsmi = "";
                }

                if (sayac != 0)
                {



                    sb.Append(@$" 
             <tr style=""page-break-inside: avoid;"">
        <td><td>
        <td>
          <div></div>
          <div></div>
          <div></div>
        </td>
        <td></td>
        <td></td>
        <td></td>
         <td  class=""tdk"" style=""color:blue"">Total:{kdvlitoplam?.ToString("N")}</td>

      </tr>");




                    kdvlitoplam = 0;
                    if (kactane == 7)
                    {
                        sb.Append(@$"<tr  style=""page-break-before: always;""><td class=""category""  style=""color:blue"">{aa.Grup + " " + aa.GrupKarisilik}+</td></tr>");
                        kactane = 0;

                    }
                    else
                    {
                        sb.Append(@$"<tr  style=""page-break-inside: avoid;""><td class=""category""  style=""color:blue"">{aa.Grup + " " + aa.GrupKarisilik}</td></tr>");

                    }

                }

                foreach (var icerik in detay.Select((value, index) => new { value, index }))
                {
                    string path = _host.WebRootPath + $"\\{icerik.value.Resim}";
                    prm.Add("@Resim", path);
                    decimal? birim = 0;
                    if (icerik.value.IskontoBirim == true)
                    {
                        iskontobirimtoplam += icerik.value.IskontoOrani;
                        birim = (icerik.value.BirimFiyat - icerik.value.IskontoOrani);

                    }
                    else
                    {
                        iskontobirimtoplam += icerik.value.BirimFiyat * (icerik.value.IskontoOrani / 100);
                        birim = (icerik.value.BirimFiyat - (icerik.value.BirimFiyat * icerik.value.IskontoOrani / 100));

                    }
                    var KdvTutari = birim * (icerik.value.KDVOrani / 100);
                    ToplamKdvTutari += KdvTutari*icerik.value.Miktar;
                    decimal? toplamkdvsiz = birim * icerik.value.Miktar;
                    KdvsizToplam += toplamkdvsiz;
                    decimal? ToplamKdvli = (birim + KdvTutari) * icerik.value.Miktar;
                    kdvlitoplam += ToplamKdvli;
                    geneltoplam += kdvlitoplam;
                    icerik.value.BirimFiyat = birim;

                    if (header == 0)
                    {
                        foreach (var item in teklif)
                        {
                            iskontobirim = item.IskontoBirim;
                            iskontooran = item.IskontoOrani;
                            stokkodugoster = item.StokKoduGoster;
                            markagoster = item.MarkaGoster;
                            iskontogoster = item.IskontoGoster;
                            string resim = _host.WebRootPath + $"\\{item.Logo}";
                            if (item.MarkaGoster == true && item.StokKoduGoster == true)
                            {
                                sb.Append(@$"<!DOCTYPE html>
<html lang=""tr"">

<head>
  <meta charset=""UTF-8"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Document</title>
 <style>
    @media print {{
      body {{
        width: 21cm;
        height: 29.7cm;
        /* change the margins as you want them to be. */
      }}
    }}

    body {{
      width: 21cm;
      height: 29.7cm;
      font-family: Arial, sans-serif;
      font-size: 14px;
      margin: 0;
      padding: 0;
      /* change the margins as you want them to be. */
    }}

    .w100 {{
      width: 100%;
    }}
     .tdk
    {{text-align:left;
      padding: 0.5rem 0.5rem;
      width: 80px;

    }}
    table {{
      border-collapse: collapse;
    }}

    th,
    .category {{
      color: #ff0000;
      font-weight: 600;
    }}

    th,
    td {{
      text-align: left;
      padding: 0.5rem 0.5rem;
    }}

    table tr th {{
      border-bottom: 1px solid black;
      padding: 0px 0.5rem;
    }}

   

    table tr td {{
      width: 60px;
      height: 20px;
    }}

    .title {{
      text-align: center;
      font-weight: 600;
      font-size: larger;
    }}
    main{{
      min-height: 100vh;
    }}

    .info div div:nth-child(2n + 1) {{
      float: right;
    }}
  </style>
  <style type=""text/css"" media=""print"">
    @page
    {{
        size: auto;
        margin: 2mm 6mm 0mm 16mm;
    }}
    body{{
      position: relative;
       
    }}
   


  .spacer {{height: 2em;}} /* height of footer + a little extra */
   footer {{left: 0;
            width: 100%;
            position: running(footer);
            height: 170px;
  }}
    thead
    {{
        display: table-header-group;
    }}
 
    /*
        tekrar etmesini istersen
        tfoot
        {{
            display: table-footer-group;
        }}
    */
    tfoot
    {{
        display: table-row-group;
        bottom: 0;

    }}
    
    
</style>
</head>
<body>
    <table id=""customers"">
        <thead>
            <tr>
                <td colspan=""10"">
                    <img src=""{resim}"" style=""width: 850px;height: 150px;""/>
                    <div>
                        <div class=""title"">Teklif Formu</div>
                        <div class=""info"">
                            <div>
                          <div><span>Tarih:</span> <b>{item.TeklifTarihi}</b></div>
                            <div><span>Firma:</span> <span>{item.Unvan}</span></div>
                          </div>
                          <div>
                            <div><span>Teklif No:</span></div>
                            <div><span>Adres No:</span> <span>{item.Adres}</span></div>
                          </div>
                          <div>
                            <div><span>Vergi Dairesi:</span></div>
                            <div><span>İlgili:</span></div>
                          </div>
                           <div>
                            <div><span>Tel:</span>{item.Telefon}</div>
                            <div><span>Vergi No:</span></div>
                          </div>
                  
                        </div>
                        <tr style=""border-bottom: 6px solid #dbdbdb;"">
                            <th>Marka</th>
                            <th>Kod</th>
                            <th>Ürün</th>
                            <th style=""width: 400px;"">Aciklama</th>
                            <th>Miktar</th>
                            <th style=""width: 45px;"">Birim Fiyat</th>
                            <th style=""width: 50px;"">KDV Orani</th>
                            <th>Tutar</th>
                           
                          </tr>
                          <tr>
                </td>
            </tr>
            
        </thead>
        <tbody>");
                            }
                            else if (item.MarkaGoster == true && item.StokKoduGoster == false)
                            {
                                sb.Append(@$"<!DOCTYPE html>
<html lang=""tr"">

<head>
  <meta charset=""UTF-8"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Document</title>
 <style>
    @media print {{
      body {{
        width: 21cm;
        height: 29.7cm;
        /* change the margins as you want them to be. */
      }}
    }}

    body {{
      width: 21cm;
      height: 29.7cm;
      font-family: Arial, sans-serif;
      font-size: 14px;
      margin: 0;
      padding: 0;
      /* change the margins as you want them to be. */
    }}

    .w100 {{
      width: 100%;
    }}
     .tdk
    {{text-align:left;
      padding: 0.5rem 0.5rem;
      width: 80px;

    }}
    table {{
      border-collapse: collapse;
    }}

    th,
    .category {{
      color: #ff0000;
      font-weight: 600;
    }}

    th,
    td {{
      text-align: left;
      padding: 0.5rem 0.5rem;
    }}

    table tr th {{
      border-bottom: 1px solid black;
      padding: 0px 0.5rem;
    }}

   

    table tr td {{
      width: 60px;
      height: 20px;
    }}

    .title {{
      text-align: center;
      font-weight: 600;
      font-size: larger;
    }}
    main{{
      min-height: 100vh;
    }}

    .info div div:nth-child(2n + 1) {{
      float: right;
    }}
  </style>
  <style type=""text/css"" media=""print"">
    @page
    {{
        size: auto;
        margin: 2mm 6mm 0mm 16mm;
    }}
    body{{
      position: relative;
       
    }}
   


  .spacer {{height: 2em;}} /* height of footer + a little extra */
   footer {{left: 0;
            width: 100%;
            position: running(footer);
            height: 170px;
  }}
    thead
    {{
        display: table-header-group;
    }}
 
    /*
        tekrar etmesini istersen
        tfoot
        {{
            display: table-footer-group;
        }}
    */
    tfoot
    {{
        display: table-row-group;
        bottom: 0;

    }}
    
    
</style>
</head>
<body>
    <table id=""customers"">
        <thead>
            <tr>
                <td colspan=""10"">
                    <img src=""{resim}"" style=""width: 850px;height: 150px;""/>
                    <div>
                        <div class=""title"">Teklif Formu</div>
                        <div class=""info"">
                            <div>
                          <div><span>Tarih:</span> <b>{item.TeklifTarihi}</b></div>
                            <div><span>Firma:</span> <span>{item.Unvan}</span></div>
                          </div>
                          <div>
                            <div><span>Teklif No:</span></div>
                            <div><span>Adres No:</span> <span>{item.Adres}</span></div>
                          </div>
                          <div>
                            <div><span>Vergi Dairesi:</span></div>
                            <div><span>İlgili:</span></div>
                          </div>
                           <div>
                            <div><span>Tel:</span>{item.Telefon}</div>
                            <div><span>Vergi No:</span></div>
                          </div>
                  
                        </div>
                        <tr style=""border-bottom: 6px solid #dbdbdb;"">
                            <th>Marka</th>
                            <th>Ürün</th>
                            <th style=""width: 400px;"">Aciklama</th>
                            <th>Miktar</th>
                            <th style=""width: 45px;"">Birim Fiyat</th>
                            <th style=""width: 50px;"">KDV Orani</th>
                            <th>Tutar</th>
                            <th></th>
                          </tr>
                          <tr>
                </td>
            </tr>
            
        </thead>
        <tbody>");
                            }
                            else if (item.MarkaGoster == false && item.StokKoduGoster == true)
                            {
                                sb.Append(@$"<!DOCTYPE html>
<html lang=""tr"">

<head>
  <meta charset=""UTF-8"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Document</title>
 <style>
    @media print {{
      body {{
        width: 21cm;
        height: 29.7cm;
        /* change the margins as you want them to be. */
      }}
    }}

    body {{
      width: 21cm;
      height: 29.7cm;
      font-family: Arial, sans-serif;
      font-size: 14px;
      margin: 0;
      padding: 0;
      /* change the margins as you want them to be. */
    }}

    .w100 {{
      width: 100%;
    }}
     .tdk
    {{text-align:left;
      padding: 0.5rem 0.5rem;
      width: 80px;

    }}
    table {{
      border-collapse: collapse;
    }}

    th,
    .category {{
      color: #ff0000;
      font-weight: 600;
    }}

    th,
    td {{
      text-align: left;
      padding: 0.5rem 0.5rem;
    }}

    table tr th {{
      border-bottom: 1px solid black;
      padding: 0px 0.5rem;
    }}

   

    table tr td {{
      width: 60px;
      height: 20px;
    }}

    .title {{
      text-align: center;
      font-weight: 600;
      font-size: larger;
    }}
    main{{
      min-height: 100vh;
    }}

    .info div div:nth-child(2n + 1) {{
      float: right;
    }}
  </style>
  <style type=""text/css"" media=""print"">
    @page
    {{
        size: auto;
        margin: 2mm 6mm 0mm 16mm;
    }}
    body{{
      position: relative;
       
    }}
   


  .spacer {{height: 2em;}} /* height of footer + a little extra */
   footer {{left: 0;
            width: 100%;
            position: running(footer);
            height: 170px;
  }}
    thead
    {{
        display: table-header-group;
    }}
 
    /*
        tekrar etmesini istersen
        tfoot
        {{
            display: table-footer-group;
        }}
    */
    tfoot
    {{
        display: table-row-group;
        bottom: 0;

    }}
    
    
</style>
</head>
<body>
    <table id=""customers"">
        <thead>
            <tr>
                <td colspan=""10"">
                    <img src=""{resim}"" style=""width: 850px;height: 150px;""/>
                    <div>
                        <div class=""title"">Teklif Formu</div>
                        <div class=""info"">
                            <div>
                          <div><span>Tarih:</span> <b>{item.TeklifTarihi}</b></div>
                            <div><span>Firma:</span> <span>{item.Unvan}</span></div>
                          </div>
                          <div>
                            <div><span>Teklif No:</span></div>
                            <div><span>Adres No:</span> <span>{item.Adres}</span></div>
                          </div>
                          <div>
                            <div><span>Vergi Dairesi:</span></div>
                            <div><span>İlgili:</span></div>
                          </div>
                           <div>
                            <div><span>Tel:</span>{item.Telefon}</div>
                            <div><span>Vergi No:</span></div>
                          </div>
                  
                        </div>
                        <tr style=""border-bottom: 6px solid #dbdbdb;"">
                            <th>Kod</th>
                            <th>Ürün</th>
                            <th style=""width: 400px;"">Aciklama</th>
                            <th>Miktar</th>
                            <th style=""width: 45px;"">Birim Fiyat</th>
                            <th style=""width: 50px;"">KDV Orani</th>
                            <th>Tutar</th>
                            <th></th>
                          </tr>
                          <tr>
                </td>
            </tr>
            
        </thead>
        <tbody>");
                            }
                            else if (item.MarkaGoster == false && item.StokKoduGoster == false)
                            {
                                sb.Append(@$"<!DOCTYPE html>
<html lang=""tr"">

<head>
  <meta charset=""UTF-8"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Document</title>
 <style>
    @media print {{
      body {{
        width: 21cm;
        height: 29.7cm;
        /* change the margins as you want them to be. */
      }}
    }}

    body {{
      width: 21cm;
      height: 29.7cm;
      font-family: Arial, sans-serif;
      font-size: 14px;
      margin: 0;
      padding: 0;
      /* change the margins as you want them to be. */
    }}

    .w100 {{
      width: 100%;
    }}
     .tdk
    {{text-align:left;
      padding: 0.5rem 0.5rem;
      width: 80px;

    }}
    table {{
      border-collapse: collapse;
    }}

    th,
    .category {{
      color: #ff0000;
      font-weight: 600;
    }}

    th,
    td {{
      text-align: left;
      padding: 0.5rem 0.5rem;
    }}

    table tr th {{
      border-bottom: 1px solid black;
      padding: 0px 0.5rem;
    }}

   

    table tr td {{
      width: 60px;
      height: 20px;
    }}

    .title {{
      text-align: center;
      font-weight: 600;
      font-size: larger;
    }}
    main{{
      min-height: 100vh;
    }}

    .info div div:nth-child(2n + 1) {{
      float: right;
    }}
  </style>
  <style type=""text/css"" media=""print"">
    @page
    {{
        size: auto;
        margin: 2mm 6mm 0mm 16mm;
    }}
    body{{
      position: relative;
       
    }}
   


  .spacer {{height: 2em;}} /* height of footer + a little extra */
   footer {{left: 0;
            width: 100%;
            position: running(footer);
            height: 170px;
  }}
    thead
    {{
        display: table-header-group;
    }}
 
    /*
        tekrar etmesini istersen
        tfoot
        {{
            display: table-footer-group;
        }}
    */
    tfoot
    {{
        display: table-row-group;
        bottom: 0;

    }}
    
    
</style>
</head>
<body>
    <table id=""customers"">
        <thead>
            <tr>
                <td colspan=""10"">
                    <img src=""{resim}"" style=""width: 850px;height: 150px;""/>
                    <div>
                        <div class=""title"">Teklif Formu</div>
                        <div class=""info"">
                            <div>
                          <div><span>Tarih:</span> <b>{item.TeklifTarihi}</b></div>
                            <div><span>Firma:</span> <span>{item.Unvan}</span></div>
                          </div>
                          <div>
                            <div><span>Teklif No:</span></div>
                            <div><span>Adres No:</span> <span>{item.Adres}</span></div>
                          </div>
                          <div>
                            <div><span>Vergi Dairesi:</span></div>
                            <div><span>İlgili:</span></div>
                          </div>
                           <div>
                            <div><span>Tel:</span>{item.Telefon}</div>
                            <div><span>Vergi No:</span></div>
                          </div>
                  
                        </div>
                        <tr style=""border-bottom: 6px solid #dbdbdb;"">
                            <th>Ürün</th>
                            <th style=""width: 400px;"">Aciklama</th>
                            <th>Miktar</th>
                            <th style=""width: 45px;"">Birim Fiyat</th>
                            <th style=""width: 50px;"">KDV Orani</th>
                            <th>Tutar</th>
                            <th></th>
                          </tr>
                          <tr>
                </td>
            </tr>
            
        </thead>
        <tbody>");
                            }

                        }

                    }
                    if (sayac == 0)
                    {


                        if (isim == 0)
                        {
                            sb.Append(@$"<tr><td class=""category""  style=""color:blue;page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">{aa.Grup + " " + aa.GrupKarisilik}</td></tr>");
                            isim++;
                        }




                    }
                    sayac++;
                    header++;


                    if (markagoster == true && stokkodugoster == true)
                    {
                        sb.Append(@$" 
    
          <tr style=""page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">
                <td>{icerik.value.MarkaAdi}</td>
                <td>{icerik.value.StokKodu}</td>
                <td><img style=""width: 65px;height:65px"" src=""{path}"" alt=""Patates Soyma Makinesi"" /></td>
                <td>
                  {icerik.value.StokAdi}<br>
                  50*70*92<br>
                  {icerik.value.Aciklama}
                </td>
                <td>{icerik.value.Miktar?.ToString("F")}</td>
                <td>{icerik.value.BirimFiyat?.ToString("F")}</td>
                <td>{icerik.value.KDVOrani?.ToString("F")}</td>
                <td>{ToplamKdvli?.ToString("F")}</td>
              </tr>
            ");
                    }
                    else if (markagoster == true && stokkodugoster == false)
                    {
                        sb.Append(@$" 
    
          <tr style=""page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">
                <td>{icerik.value.MarkaAdi}</td>
                <td><img style=""width: 65px;height:65px"" src=""{path}"" alt=""Patates Soyma Makinesi"" /></td>
                <td>
                  {icerik.value.StokAdi}<br>
                  50*70*92<br>
                  {icerik.value.Aciklama}
                </td>
                <td>{icerik.value.Miktar?.ToString("F")}</td>
                <td>{icerik.value.BirimFiyat?.ToString("F")}</td>
                <td>{icerik.value.KDVOrani?.ToString("F")}</td>
                <td>{ToplamKdvli?.ToString("F")}</td>
              </tr>
            ");
                    }
                    else if (markagoster == false && stokkodugoster == true)
                    {
                        sb.Append(@$" 
    
          <tr style=""page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">
                <td>{icerik.value.StokKodu}</td>
                <td><img style=""width: 65px;height:65px"" src=""{path}"" alt=""Patates Soyma Makinesi"" /></td>
                <td>
                  {icerik.value.StokAdi}<br>
                  50*70*92<br>
                  {icerik.value.Aciklama}
                </td>
                <td>{icerik.value.Miktar?.ToString("F")}</td>
                <td>{icerik.value.BirimFiyat?.ToString("F")}</td>
                <td>{icerik.value.KDVOrani?.ToString("F")}</td>
                <td>{ToplamKdvli?.ToString("F")}</td>
              </tr>
            ");
                    }
                    else if (markagoster == false && stokkodugoster == false)
                    {
                        sb.Append(@$" 
    
          <tr style=""page-break-inside: avoid;border-bottom: 6px solid #dbdbdb;"">
               
                <td><img style=""width: 65px;height:65px"" src=""{path}"" alt=""Patates Soyma Makinesi"" /></td>
                <td>
                  {icerik.value.StokAdi}<br>
                  50*70*92<br>
                  {icerik.value.Aciklama}
                </td>
                <td>{icerik.value.Miktar?.ToString("F")}</td>
                <td>{icerik.value.BirimFiyat?.ToString("F")}</td>
                <td>{icerik.value.KDVOrani?.ToString("F")}</td>
                <td>{ToplamKdvli?.ToString("F")}</td>
              </tr>
            ");
                    }

                    kactane = 0;





                    kactane++;



                    if (sayac == dongu)
                    {
                        if (iskontobirim==true)
                        {
                            geneltoplam -= iskontooran;
                            iskontobirimtoplam += iskontooran;
                        }
                        else
                        {
                            var iskontos= geneltoplam * (iskontooran / 100);
                            geneltoplam = geneltoplam - iskontos;
                            iskontobirimtoplam += iskontos;
                        }
                        
                     

                        sb.Append(@$" 
             <tr style=""page-break-inside: avoid;"" >
        <td><td>
        <td>
          <div></div>
          <div></div>
          <div></div>
        </td>
        <td></td>
        <td></td>
        <td></td>
        <td class=""tdk"" style=""color:blue"">Total:{kdvlitoplam?.ToString("F")}</td>
      </tr>");
                        if (iskontogoster == true)
                        {
                            sb.Append(@$"<tr style=""page-break-before: always;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Liste Toplam:{KdvsizToplam?.ToString("F")}</td>
          </tr>
     <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Ozel Iskonto:{iskontobirimtoplam?.ToString("F")}</td>
          </tr>
       </tr>
     <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Ara Toplam:{(iskontobirimtoplam+KdvsizToplam)?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">KDV Tutarı:{ToplamKdvTutari?.ToString("F")}</td>
          </tr>
    
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Genel Toplam:{geneltoplam?.ToString("F")}</td>
          </tr>

</tbody>");
                        }
                        else if (iskontogoster == false)
                        {
                            sb.Append(@$"<tr style=""page-break-before: always;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Toplam:{KdvsizToplam?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">KDV Tutarı:{KdvTutari?.ToString("F")}</td>
          </tr>
          <tr style=""page-break-inside: avoid;"">
              <td>
              <td>
              <td>
                  <div></div>
                  <div></div>
                  <div></div>
              </td>
              <td></td>
              <td></td>
              <td></td>
              <td style=""color:red"">Genel Toplam:{geneltoplam?.ToString("F")}</td>
          </tr>
</tbody>");
                        }


                        sb.Append(@"         <tfoot >
            <trstyle=""page-break-inside:avoid;"">
                <td colspan=""10"">
                    <p style=""margin-left: 20px;"">
                        Nakliye, nakliye sigortası ve montaj KONYA şehir içi firmamıza aittir. Şehir dışı alıcıya aittir.
                    </p>
                    <p style=""margin-left: 20px;"">
                        Her türlü inşaat işleri (duvar, yer, dekorasyon, elektrik, su, doğalgaz, havalandırma tesisatları) alıcıya aittir.
                    </p>
                    <p style=""margin-left: 20px;"">
                        Montaj esnasında taşıma için vinç ihtiyacı olduğu takdirde ALICI tarafından karşılanacaktır.
                    </p>
                    <p style=""margin-left: 20px;"">Fiyatlar 1 gün süre ile geçerlidir</p>
                    <p style=""margin-left: 20px;"">Teslim tarihi …/…/……</p>
                    <p style=""margin-left: 20px;"">Ürünlerin miktar değişikliklerinde karşılıklı mutabakat halinde proforma yeniden güncellenir.</p>
                    <h2 style=""text-align: center;"">Onay</h2>
                    <table style=""width: 100%;"">
                        <tr>
                          <td  style=""float: left;margin-left: 68px;font-size: 25px;"">Alıcı</td>  
                        <td style=""float: right;margin-right: 68px; font-size: 25px; "">Satıcı</td>
                        </tr>
                    </table>
                </td>
            </tr>
        </tfoot>
</table>");

                        sb.Append(@" <footer>

        <div style=""border-style: inset;margin-left: 78px;"">

          <td style=""margin-left: 100px;width: 400px;"">TMR ENDÜSTRİYEL MUTFAK ve HİJYEN EKİPMANLARI - TAMER ERVURAL / Mevlana Vd. 115 527 02632</td>
          <br>
          <td  style=""margin-left: 160px;"">MERKEZ: Ateşbaz Veli Mah. Eski Meram Cad. No:93 - Meram/KONYA +90 332 322 70 03</td>
          <br>
          <td style=""margin-left: 160px;"">ŞUBE: Fevzi Çakmak Mah. Demirkapı Cad. No:27/C - Karatay/KONYA +90 332 345 22 11
          </td>
          <br>
          
          <td  style=""margin-left: 160px;"">FABRİKA: 1. OSB. İstikamet Cad. No:19 - Selçuklu/KONYA +90 332 251 51 15
          </td>
          
        </div>
       
     
        <table style=""width: 100%;"">
          <tr>
            <td  style=""float: left;margin-left: 68px;""><a href=""info@tmrgroup.com.tr"">info@tmrgroup.com.tr</a></td>  
            <td style=""float:right; margin-right: 44px;""><a href=""tmrmutfak.com.tr"">tmrmutfak.com.tr</a></td>
          </tr>
        </table>
        
      </footer> ");

                        sb.Append(@"           </table>
    </div>
  </body>
</html>");

                    }


                    //var globalSettings = new GlobalSettings
                    //{

                    //    ColorMode = ColorMode.Color,
                    //    Orientation = Orientation.Portrait,
                    //    PaperSize = DinkToPdf.PaperKind.A4,
                    //    DocumentTitle = "PDF Report",
                    //    Out = @$"Pdf/{kelime + DateTime.Now.Date.ToString("yyyy-MM-dd") + TeklifId + FirmaId + KullanıcıId}.pdf", //rastgele kelime+datetime+Teklifid+FirmaId+kulanıcıid
                    //    Margins = new MarginSettings { Right = 10, Left = 10,Bottom=30 },
                    //    UseCompression = true,
                    //    DPI = 72,



                    //};

                    //var objectSettings = new ObjectSettings
                    //{
                    //    IncludeInOutline = true,
                    //    PagesCount = true,
                    //    HtmlContent = sb.ToString(),
                    //    WebSettings = { DefaultEncoding = "utf-8", PrintMediaType = true },
                    //    HeaderSettings = { FontName = "Arial", FontSize = 7, Right = "Page [page] of [toPage]" },


                    //};
                    //var pdf = new HtmlToPdfDocument()
                    //{
                    //    GlobalSettings = globalSettings,
                    //    Objects = { objectSettings }
                    //};

                    //_converter.Convert(pdf);


                }
            }
            return Ok(html + sb.ToString());

        }
    }
        

}














