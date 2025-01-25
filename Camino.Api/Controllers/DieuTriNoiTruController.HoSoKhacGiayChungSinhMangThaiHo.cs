using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.GiayChungSinhMangThaiHo;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetThongTinGiayChungSinhMangThaiHo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public GiayChungNhanSinhMangThaiHoGrid GetThongTinGiayChungSinhMangThaiHo(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinGiayChungSinhMangThaiHo(yeuCauTiepNhanId);
            return lookup;
        }
        #region get data Create
        [HttpPost("GetDataGiayChungSinhMangThaiHoCreate")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ThongTinGiayChungNhanSinhMangThaiHo GetDataGiayChungSinhMangThaiHoCreate(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetDataGiayChungSinhMangThaiHoCreate(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetDacDiemTreSoSinh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<ThongTinDacDiemTreSoSinhGridVo> GetDacDiemTreSoSinh(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetDacDiemTreSoSinh(yeuCauTiepNhanId);
            return lookup;
        }
        #endregion
        #region save / update
        [HttpPost("LuuGiayChungSinhMangThaiHo")]
        public async Task<ActionResult<GiayChungSinhMangThaiHoViewModel>> LuuGiayChungSinhMangThaiHo([FromBody] GiayChungSinhMangThaiHoViewModel giayChungSinhMangThaiHoViewModel)
        {
            if (giayChungSinhMangThaiHoViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new GiayChungSinhMangThaiHoViewModel()
                {
                    YeuCauTiepNhanId = giayChungSinhMangThaiHoViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = giayChungSinhMangThaiHoViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = giayChungSinhMangThaiHoViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<GiayChungSinhMangThaiHoViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(giayChungSinhMangThaiHoViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = giayChungSinhMangThaiHoViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = giayChungSinhMangThaiHoViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = giayChungSinhMangThaiHoViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                giayChungSinhMangThaiHoViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #endregion
        #region In 
        [HttpPost("InGiayChungSinhMangThaiHo")]
        public async Task<ActionResult<string>> InGiayChungSinhMangThaiHo([FromBody]XacNhanInPhieuGiaySinhMangThaiHo xacNhanIn)
        {
            var html = await _dieuTriNoiTruService.InGiayChungSinhMangThaiHo(xacNhanIn);
            return html;
        }
        #endregion
    }
}
