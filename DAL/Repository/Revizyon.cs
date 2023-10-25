using DAL.DTO;
using DAL.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class Revizyon : IRevizyon
    {
        private readonly IDbConnection _db;
        private readonly ITeklifler _tk;


        public Revizyon(IDbConnection db, ITeklifler tk)
        {
            _db = db;
            _tk = tk;
        }

        public async Task<int> RevizyonInsert(int TeklifId, int FirmaId, int KullanıcıId)
        {
            DynamicParameters prm = new DynamicParameters();

            var list = await _db.QueryAsync<TeklifInsert>(@$"select * from Teklifler where Id={TeklifId} and Aktif=1");
            foreach (var T in list)
            {
                prm.Add("@Aktif", true);
                prm.Add("@MusteriId", T.MusteriId);
                prm.Add("@FirmaId", FirmaId);
                prm.Add("@KullanıcıId", KullanıcıId);
                prm.Add("@ParaBirimiId", T.ParaBirimiId);
                prm.Add("@TeklifTarihi", T.TeklifTarihi);
                prm.Add("@RevizyonId", TeklifId);

                prm.Add("@IslemTarihi", DateTime.Now);
                prm.Add("@GecerlilikTarihi", T.GecerlilikTarihi);
                prm.Add("@TeklifAdi", T.SepetAdi);
                prm.Add("@Kur", T.Kur);
                prm.Add("@Durum", 1);

                prm.Add("@IskontoOrani", T.IskontoOrani);
            }
       

            var sqla = @"Insert into Teklifler (MusteriId,RevizyonId,TeklifAdi,ParaBirimiId,Durum,TeklifTarihi,IslemTarihi,GecerlilikTarihi,Dil_id,Kur,IskontoOrani,KullanıcıId,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@MusteriId,@RevizyonId,@TeklifAdi,@ParaBirimiId,@Durum,@TeklifTarihi,@IslemTarihi,@GecerlilikTarihi,@Dil_id,@Kur,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif)";
            int id= await _db.QuerySingleAsync<int>(sqla, prm);


            var detaylist = await _db.QueryAsync<TeklifDetayInsert>(@$"select * from TeklifDetay where TeklifId={TeklifId} and Aktif=1");            
            foreach (var T in detaylist)
            {
                DynamicParameters prm1 = new();
                prm1.Add("@Aktif", true);
                prm1.Add("@FirmaId", FirmaId);
                prm1.Add("@KullanıcıId", KullanıcıId);
                prm1.Add("@TeklifId", id);
                prm1.Add("@StokId", T.StokId);
                prm1.Add("@StokKodu", T.StokKodu);
                prm1.Add("@Miktar", T.Miktar);
                prm1.Add("@BirimId", T.BirimId);
                prm1.Add("@StokAdı", T.StokAdi);
                prm1.Add("@Aciklama", T.Aciklama);
                prm1.Add("@BirimFiyat", T.BirimFiyat);
                prm1.Add("@KDVOrani", T.KDVOrani);
                prm1.Add("@IskontoOrani", T.IskontoOrani);

                var sql = @"Insert into TeklifDetay (TeklifId,BirimId,StokAdi,Aciklama,StokId,StokKodu,Miktar,BirimFiyat,KDVOrani,IskontoOrani,KullanıcıId,FirmaId,Aktif) OUTPUT INSERTED.[Id]  values  (@TeklifId,@BirimId,@StokAdı,@Aciklama,@StokId,@StokKodu,@Miktar,@BirimFiyat,@KDVOrani,@IskontoOrani,@KullanıcıId,@FirmaId,@Aktif)";
                await _db.ExecuteAsync(sql, prm1);
            }
            return id;

        }
    }
}
