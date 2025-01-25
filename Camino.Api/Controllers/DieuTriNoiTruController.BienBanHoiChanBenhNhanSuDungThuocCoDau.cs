using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BienBanHoiChanSuDungThuocCoDau;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetThuocPhieuDieuTriCuaBN")]
        public async Task<ActionResult<ICollection<ThuocPhieuDieuTriBenhNhanItemVo>>> GetThuocPhieuDieuTriCuaBN([FromBody]DropDownListRequestModel queryInfo,long yeuCauTiepNhanId)
        {
            var lookup =  await _dieuTriNoiTruService.GetDuocPhamBenhNhanNoiTru(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }
        [HttpPost("GetDuocPhamCoDau")]
        public async Task<ActionResult<ICollection<ThuocPhieuDieuTriBenhNhanItemVo>>> GetDuocPhamCoDau([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _dieuTriNoiTruService.GetDuocPhamCoDau(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetDuocPhamCoDauVo")]
        public async Task<ActionResult<ICollection<ThuocPhieuDieuTriBenhNhanItemVo>>> GetDuocPhamCoDauVo( long duocPhamId)
        {
            var lookup = await _dieuTriNoiTruService.GetDuocPhamCoDauVo(duocPhamId);
            return Ok(lookup);
        }
        #region CRUD
        [HttpPost("LuuBienBanHoiChanSuDungThuocCoDau")]
        public async Task<ActionResult<BienBanHoiChanSuDungThuocCoDauViewModel>> LuuTrichBienBanHoiChan([FromBody] BienBanHoiChanSuDungThuocCoDauViewModel bienBanHoiChanSuDungThuocCoDauViewModel)
        {
            if (bienBanHoiChanSuDungThuocCoDauViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var bienBan= new BienBanHoiChanSuDungThuocCoDauViewModel()
                {
                    YeuCauTiepNhanId = bienBanHoiChanSuDungThuocCoDauViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = bienBanHoiChanSuDungThuocCoDauViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = bienBanHoiChanSuDungThuocCoDauViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var bienBanSuDungThuocCoDau = bienBan.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.AddAsync(bienBanSuDungThuocCoDau);
                return CreatedAtAction(nameof(Get), new { id = bienBanSuDungThuocCoDau.Id }, bienBanSuDungThuocCoDau.ToModel<BienBanHoiChanSuDungThuocCoDauViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(bienBanHoiChanSuDungThuocCoDauViewModel.Id);
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = bienBanHoiChanSuDungThuocCoDauViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = bienBanHoiChanSuDungThuocCoDauViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = bienBanHoiChanSuDungThuocCoDauViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                bienBanHoiChanSuDungThuocCoDauViewModel.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                foreach (var itemNoiTruHoSoKhacFileDinhKem in update.NoiTruHoSoKhacFileDinhKems.ToList())
                {
                    var soket = await _noiDuTruHoSoKhacFileDinhKemService.GetByIdAsync(itemNoiTruHoSoKhacFileDinhKem.Id);
                    if (soket == null)
                    {
                        return NotFound();
                    }
                    await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(soket.Id);
                }
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        [HttpGet("GetBienBanHcBenhNhanSuDungThuocCoDau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BienBanHoiChanSuDungThuocCoDauVo> GetBienBanHc( long yeuCauTiepNhanId)
        {
            var noitruHoSoKhacids = _noiTruHoSoKhacService.HoSoKhacIds(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuatSuDungThuocCoDau);
            if(noitruHoSoKhacids.Result.Any())
            {
                var bienBanHoiChan = await _noiTruHoSoKhacService.GetByIdAsync(noitruHoSoKhacids.Result.FirstOrDefault());

                if (bienBanHoiChan != null)
                {
                    var bienBan = JsonConvert.DeserializeObject<BienBanHoiChanSuDungThuocCoDauVo>(bienBanHoiChan.ThongTinHoSo);
                    bienBan.Id = noitruHoSoKhacids.Result.FirstOrDefault();
                    return bienBan;
                }
            }
            return new BienBanHoiChanSuDungThuocCoDauVo();
        }
        #endregion
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInBienBanHoiChanPhauThuatCoDau")]
        public async Task<ActionResult> PhieuInBienBanHoiChanPhauThuatCoDau(BangTheoDoiHoiTinhHttpParamsRequest phieuInBienBanHoiChan)
        {
            var phieuIns = await _dieuTriNoiTruService.PhieuInBienBanHoiChanPhauThuatCoDau(phieuInBienBanHoiChan);
            return Ok(phieuIns);
        }
    }
}
