using DAL.DTO;
using DAL.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BL.Control.Teklif
{
   
    public class TeklifControl : ITeklifControl
    {
        private readonly IDbConnection _db;
        private readonly ISepet _sepet;

        public TeklifControl(IDbConnection db, ISepet sepet)
        {
            _db = db;
            _sepet = sepet;
        }

        public async Task<List<string>> Control(int TeklifId,int dil, int FirmaId)
        {
            string sqqlq = $@"select tk.AltFirmaId,tk.MusteriId,Musteriler.Unvan,Musteriler.Telefon,Musteriler.Mail,Musteriler.Adres,tk.TeklifTarihi,tk.GecerlilikTarihi,
            FirmaOzellikler.Logo,FirmaOzellikler.AltMetin
            from Teklifler tk 
            left join FirmaOzellikler on FirmaOzellikler.AltFirmaId=tk.AltFirmaId
            left join Musteriler on Musteriler.Id=TK.MusteriId
            where tk.Id={TeklifId} and tk.FirmaId={FirmaId}";
            var teklif = await _db.QueryAsync<PdfTeklifResponse>(sqqlq);
            string dilismi = $@"select Isim from Dil where Id={dil}";
            var Dili = await _db.QueryFirstAsync<string>(dilismi);
            List<string> dogru = new List<string>();

            List<string> hatalar=new List<string>();
            string sql = $@"select se.Id,se.TeklifId,se.StokAdi,se.Aciklama,se.KategoriIsmi,se.StokId,
            se.StokKodu,se.Miktar,se.BirimFiyat,se.KdvOrani,se.IskontoOrani,Stoklar.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi,StokResim.Resim,se.Hediye
            from TeklifDetay se
			left join Stoklar on Stoklar.Id=se.StokId
			left join StokResim on Stokresim.StokId=se.StokId  and StokResim.Aktif=1
			left join ParaBirimleri on ParaBirimleri.Id=Stoklar.ParaBirimiId
            where se.TeklifId={TeklifId} and SE.FirmaId={FirmaId}
			Order By se.KategoriIsmi";
            var detay = _db.Query<TeklifDetayInsertResponse>(sql);
            foreach (var item  in detay)
            {
                var kontrol = await _sepet.DilKontrol(item.StokId, dil, FirmaId);
                if (kontrol.Count()==0)
                {
                    string hata = @$"{item.StokAdi}'nın {Dili} bir karşılığı yok";
                    hatalar.Add(hata);
                }
                if (item.BirimFiyat==0 && item.Hediye==false)
                {
                    string hata = @$"{item.StokAdi}'nın Birimfiyatı 0 olamaz";
                    hatalar.Add(hata);

                }
                if (kontrol.Count()!=0)
                {
                    if (kontrol.First().KategoriIsmi==null)
                    {
                        string hata = @$"{item.StokAdi}'nın KategoriIsmi {Dili}'de bir karşılıgı yok ";
                        hatalar.Add(hata);
                    }
                }



            }
            if (hatalar.Count()==0)
            {
                return (dogru);
            }
            else
            {
                return (hatalar);

            }








        }
    }
}

