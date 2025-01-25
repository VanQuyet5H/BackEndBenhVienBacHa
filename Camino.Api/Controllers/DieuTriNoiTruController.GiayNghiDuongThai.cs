using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.GiayNghiDuongThaiNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai;
using Camino.Core.Domain.ValueObject.Grid;
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
        [HttpPost("GetThongTinGiayChungNhanNghiDuongThai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public GiayChungNhanNghiDuongThaiGrid GetThongTinGiayChungNhanNghiDuongThai(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinGiayChungNhanNghiDuongThai(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetDataChungNhanNghiDuongThai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ThongTinChungNhanNghiDuongThai GetDataChungNhanNghiDuongThai(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetDataChungNhanNghiDuongThai(yeuCauTiepNhanId);
            return lookup;
        }
        #region save / update
        [HttpPost("LuuGiayChungNhanNghiDuongThai")]
        public async Task<ActionResult<GiayChungNhanNghiDuongThaiViewModel>> LuuGiayChungNhanNghiDuongThai([FromBody] GiayChungNhanNghiDuongThaiViewModel giayChungNhanNghiDuongThaiViewModel)
        {
            if (giayChungNhanNghiDuongThaiViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new GiayChungNhanNghiDuongThaiViewModel()
                {
                    YeuCauTiepNhanId = giayChungNhanNghiDuongThaiViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = giayChungNhanNghiDuongThaiViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = giayChungNhanNghiDuongThaiViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var giayChungNhanNghiDuongThai = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.AddAsync(giayChungNhanNghiDuongThai);
                return CreatedAtAction(nameof(Get), new { id = giayChungNhanNghiDuongThai.Id }, giayChungNhanNghiDuongThai.ToModel<GiayChungNhanNghiDuongThaiViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(giayChungNhanNghiDuongThaiViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = giayChungNhanNghiDuongThaiViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = giayChungNhanNghiDuongThaiViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = giayChungNhanNghiDuongThaiViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                giayChungNhanNghiDuongThaiViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #endregion
        #region In 
        [HttpPost("InGiayChungNhanNghiDuongThai")]
        public async Task<ActionResult<string>> InGiayChungNhanNghiDuongThai([FromBody]XacNhanInPhieuGiayChungNhanNghiDuongThai xacNhanIn)
        {
            //XacNhanInPhieuGiayChungNhanNghiDuongThai
            var html = await _dieuTriNoiTruService.InGiayChungNhanNghiDuongThai(xacNhanIn);
            return html;
        }
        [HttpPost("InTatCaGiayNghiDuongThai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachXuatChungTuExcel)]
        public async Task<ActionResult<string>> GetPhieuDuongThaiNgoaiTruVaNoiTru([FromBody]GiayChungNhanNghiDuongThaiQueryInfo info)
        {
            var html = string.Empty;
            if(info.YeuCauTiepNhanNgoaiTruId != null)
            {
                var yckbIds = _yeuCauKhamBenhService.GetYeuCauKhamBenhKhamThaiIds((long)info.YeuCauTiepNhanNgoaiTruId);
                var slPhieu = 0;
                foreach (var item in yckbIds.Result.ToList())
                {
                    var htmlNgoai = await _yeuCauKhamBenhService.PhieuNghiDuongThai(item);

                    if (!string.IsNullOrEmpty(htmlNgoai))
                    {
                        html += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>Giấy nghỉ dưỡng thai ngoại trú</th></tr></table>" + htmlNgoai;
                    }
                    if (slPhieu < yckbIds.Result.Count())
                    {
                        html += "<div class=\"pagebreak\"> </div>";
                    }
                        
                }
            }
            if (info.YeuCauTiepNhanNoiTruId != null)
            {
                var infoIn = new XacNhanInPhieuGiayChungNhanNghiDuongThai
                {
                    LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai,
                    YeuCauTiepNhanId = (long)info.YeuCauTiepNhanNoiTruId
                };
                var htmlNoiTru = await _dieuTriNoiTruService.InGiayChungNhanNghiDuongThai(infoIn);
                if (!string.IsNullOrEmpty(html))
                {
                    html += "<div class=\"pagebreak\"> </div>";
                }
                if (!string.IsNullOrEmpty(htmlNoiTru))
                {
                    html += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>Giấy nghỉ dưỡng thai nội trú</th></tr></table>" + htmlNoiTru;
                }
            }
            
            return Ok(html);
        }
        #endregion
    }
}
