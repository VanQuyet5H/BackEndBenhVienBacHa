using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.CauHinh;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioLamViecController : CaminoBaseController
    {
        private readonly ICauHinhService _cauHinhService;
        public GioLamViecController(ICauHinhService cauHinhService)
        {
            _cauHinhService = cauHinhService;
        }

        [HttpGet("GetLichLamViec")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichLamViec)]
        public async Task<ActionResult> GetLichLamViec(long id)
        {
            var ngayLamViecViewModel = new Camino.Api.Models.GioLamViec.GioLamViecViewModel();
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var ngayLamViec = cauHinhChung.NgayLamViec.Select(str => str.ToString()).ToArray();

            if (ngayLamViec.Length > 0)
            {
                ngayLamViecViewModel.ThuHai = ngayLamViec[0] == "1" ? true : false;
                ngayLamViecViewModel.ThuBa = ngayLamViec[1] == "1" ? true : false;
                ngayLamViecViewModel.ThuTu = ngayLamViec[2] == "1" ? true : false;
                ngayLamViecViewModel.ThuNam = ngayLamViec[3] == "1" ? true : false;
                ngayLamViecViewModel.ThuSau = ngayLamViec[4] == "1" ? true : false;
                ngayLamViecViewModel.ThuBay = ngayLamViec[5] == "1" ? true : false;
                ngayLamViecViewModel.ChuNhat = ngayLamViec[6] == "1" ? true : false;
            }

            ngayLamViecViewModel.BatDau = cauHinhChung.GioBatDauLamViec;
            ngayLamViecViewModel.KetThuc = cauHinhChung.GioKetThucLamViec;

            return Ok(ngayLamViecViewModel);
        }

        [HttpPost("SaveGioLamViec")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichLamViec)]
        public async Task<ActionResult> SaveGioLamViec([FromBody] Camino.Api.Models.GioLamViec.GioLamViecViewModel model)
        {
            var ngayLamViecs = string.Empty;
            ngayLamViecs = model.ThuHai == null || model.ThuHai == true ? "1" : "0";
            ngayLamViecs += model.ThuBa == null || model.ThuBa == true ? "1" : "0";
            ngayLamViecs += model.ThuTu == null || model.ThuTu == true ? "1" : "0";
            ngayLamViecs += model.ThuNam == null || model.ThuNam == true ? "1" : "0";
            ngayLamViecs += model.ThuSau == null || model.ThuSau == true ? "1" : "0";
            ngayLamViecs += model.ThuBay == null || model.ThuBay == true ? "1" : "0";
            ngayLamViecs += model.ChuNhat == null || model.ChuNhat == true ? "1" : "0";

            var cauHinhs = new List<CauHinh>();

            var ngayLamViecEntity = _cauHinhService.GetSetting("CauHinhChung.NgayLamViec");
            var gioBatDauLamViecEntity = _cauHinhService.GetSetting("CauHinhChung.GioBatDauLamViec");
            var gioKetThucLamViecEntity = _cauHinhService.GetSetting("CauHinhChung.GioKetThucLamViec");

            if (ngayLamViecEntity != null)
            {
                ngayLamViecEntity.Value = ngayLamViecs.ToString();
                cauHinhs.Add(ngayLamViecEntity);
            }

            if (gioBatDauLamViecEntity != null)
            {
                gioBatDauLamViecEntity.Value = model.BatDau.ToString();
                cauHinhs.Add(gioBatDauLamViecEntity);
            }

            if (gioKetThucLamViecEntity != null)
            {
                gioKetThucLamViecEntity.Value = model.KetThuc.ToString();
                cauHinhs.Add(gioKetThucLamViecEntity);
            }

            await _cauHinhService.UpdateAsync(cauHinhs);

            return NoContent();
        }
    }
}
