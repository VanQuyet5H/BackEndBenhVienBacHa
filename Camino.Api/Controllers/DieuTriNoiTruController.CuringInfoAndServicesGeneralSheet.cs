using System;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpGet("GetCuringInfoAndServicesGeneralSheet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<CuringInfoAndServicesGeneralSheetViewModel> GetCuringInfoAndServicesGeneralSheet(long yeuCauTiepNhanId)
        {
            var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri);

            var model =  noiTruHoSoKhac != null ? JsonConvert.DeserializeObject<CuringInfoAndServicesGeneralSheetViewModel>(noiTruHoSoKhac.ThongTinHoSo) :
                new CuringInfoAndServicesGeneralSheetViewModel();
            if(noiTruHoSoKhac != null)
            {
                model.NoiTruHoSoKhacId = noiTruHoSoKhac.Id;
            }
            return model;
        }

        [HttpPost("UpdateCuringInfoAndServicesGeneralSheet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateCuringInfoAndServicesGeneralSheet([FromBody] CuringInfoAndServicesGeneralSheetViewModel curingInfo, long yeuCauTiepNhanId)
        {
            var existCuringInfo = await _noiTruHoSoKhacService.IsThisExistForCuringInfo(yeuCauTiepNhanId);
            var thongTinHoSo = JsonConvert.SerializeObject(curingInfo);
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (existCuringInfo.IsExist)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(existCuringInfo.Id.GetValueOrDefault());
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                return Ok();
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };
            await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInThongTinDieuTriVaCacDichVu")]
        public async Task<ActionResult> PhieuInThongTinDieuTriVaCacDichVu(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInThongTinDieuTriVaCacDichVu(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
