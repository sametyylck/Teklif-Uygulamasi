using BL.Control.IdControl;
using BL.Control.Stoklar;
using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace TMRApi.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class StoklarController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IStoklar _stoklar;
        private readonly IDbConnection _db;
        private readonly IStoklarService _stokservice;
        private readonly IDControl _idcontrol;

        public StoklarController(IUserService userservice, IStoklar stoklar, IDbConnection db, IStoklarService stokservice, IDControl idcontrol)
        {
            _userservice = userservice;
            _stoklar = stoklar;
            _db = db;
            _stokservice = stokservice;
            _idcontrol = idcontrol;
        }

        [HttpPost("Insert"), Authorize]
        public async Task<ActionResult<StoklarInsert>> Insert(StoklarInsert request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var hata = await _stokservice.Insert(request, CompanyId);
            if (hata == "true")
            {
                int KullanıcıId = getlist[1];
                int id = await _stoklar.Insert(request, CompanyId, KullanıcıId);
                string sql = $@"Select st.Id,st.KategoriId,StokKategori.Isim as KatagoriIsmi,st.StokKodu,st.StokAdi,st.Aciklama,st.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi
            ,st.BirimId,Birim.Isim as BirimIsmi,st.BirimFiyat,st.KDVOranı,st.Dil_id,Dil.Isim as DilIsmi,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik  
            from Stoklar st
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on  ParaBirimleri.Id=ParaBirimiId
            left join Dil on Dil.Id=st.Dil_id
            left join Birim on Birim.Id=BirimId
            where st.Id={id}";
                var list = await _db.QueryAsync<StoklarInsertResponse>(sql);
                return Ok(list);
            }
            else
            {
                return BadRequest(hata);
            }

        }

        [HttpPut("Update"), Authorize]
        public async Task<ActionResult<StoklarUpdate>> Update(StoklarUpdate request)
        {

            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            int KullanıcıId = getlist[1];
            var hata = await _stokservice.Update(request, CompanyId);
            if (hata == "true")
            {
                await _stoklar.Update(request, CompanyId, KullanıcıId);

                var stokid = await _db.QueryFirstAsync<int>(@$"select ISNULL((select ISNULL(id,0) from Stoklar where Id={request.Id} and Aktif=1 and Dil_id={request.Dil_id}),0)");
                string sql = string.Empty;
                if (stokid != 0)
                {
                    sql = $@"Select st.Id,st.KategoriId,StokKategori.Isim as KatagoriIsmi,st.StokKodu,st.StokAdi,st.Aciklama,st.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi
            ,st.BirimId,Birim.Isim as BirimIsmi,st.BirimFiyat,st.KDVOranı,st.Dil_id,Dil.Isim as DilIsmi,Marka,AmbalajAgirlik,AmbalajsizAgirlik,Desi,En,Boy,Yukseklik  
            from Stoklar st
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on  ParaBirimleri.Id=ParaBirimiId
            left join Dil on Dil.Id=st.Dil_id
            left join Birim on Birim.Id=BirimId
            where st.Id={request.Id}";
                }
                else
                {
                    sql = $@"Select st.Id,st.KategoriId,StokKategori.Isim as KatagoriIsmi,st.StokKodu,st.StokAdi,st.Aciklama,st.ParaBirimiId,ParaBirimleri.Kod as ParaBirimiIsmi
            ,st.BirimId,Birim.Isim as BirimIsmi,st.BirimFiyat,st.KDVOranı,st.Dil_id,Dil.Isim as DilIsmi
            from Stoklar st
            left join StokKategori on StokKategori.Id=KategoriId
            left join ParaBirimleri on  ParaBirimleri.Id=ParaBirimiId
            left join Dil on Dil.Id=st.Dil_id
            left join Birim on Birim.Id=BirimId
            where st.StokId={request.Id} and st.Dil_id={request.Dil_id}";
                }
                var list = await _db.QueryAsync<StoklarInsertResponse>(sql);

                return Ok(list);
            }
            else
            {
                return BadRequest(hata);
            }



        }
        [HttpDelete("Delete"), Authorize]
        public async Task<ActionResult<StoklarInsert>> Delete(int Id)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var hata = await _idcontrol.IdControl(Id, "Stoklar", CompanyId);
            if (hata == "true")
            {
                int KullanıcıId = getlist[1];
                await _stoklar.Delete(Id, CompanyId, KullanıcıId);
                return Ok("Silme islemi basarili");
            }
            else
            {
                return BadRequest(hata);
            }

        }
        [HttpPost("Details"), Authorize]
        public async Task<ActionResult<RegisterDTO>> Details(int id, int dil)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _stoklar.Details(id, dil, CompanyId);
            return Ok(list);

        }

        [HttpPost("List"), Authorize]
        public async Task<ActionResult<RegisterDTO>> List(int? dil, string? T, int? KAYITSAYISI, int? SAYFA)
        {
            var getlist = _userservice.GetId();
            int CompanyId = getlist[0];
            var list = await _stoklar.List(dil, T, CompanyId, KAYITSAYISI, SAYFA);
            var count = await _stoklar.Count(dil, T, CompanyId, KAYITSAYISI, SAYFA);
            return Ok(new { list, count });

        }
    }
}
