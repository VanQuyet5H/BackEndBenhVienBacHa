using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
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
        [HttpPost("GetThongTinGiayChungNhanNghiViecHuongBHXH")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public GiayChungNhanNghiViecHuongBHXHGrid GetThongTinGiayChungNhanNghiViecHuongBHXH(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinGiayChungNhanNghiViecHuongBHXH(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetDataChungNhanNghiViecHuongBHXH")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ThongTinChungNhanNghiViecHuongBHXH GetDataChungNhanNghiViecHuongBHXH(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetDataChungNhanNghiViecHuongBHXH(yeuCauTiepNhanId);
            return lookup;
        }
        #region save / update
        [HttpPost("LuuGiayChungNhanNghiViecHuongBHXH")]
        public async Task<ActionResult<GiayChungNhanNghiViecHuongBHXHViewModel>> LuuGiayChungNhanNghiViecHuongBHXH([FromBody] GiayChungNhanNghiViecHuongBHXHViewModel giayChungNhanNghiViecHuongBHXHViewModel)
        {
            if (giayChungNhanNghiViecHuongBHXHViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new GiayChungNhanNghiViecHuongBHXHViewModel()
                {
                    YeuCauTiepNhanId = giayChungNhanNghiViecHuongBHXHViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = giayChungNhanNghiViecHuongBHXHViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = giayChungNhanNghiViecHuongBHXHViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var giayChungNhanNghiViecHuongBHXH = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.AddAsync(giayChungNhanNghiViecHuongBHXH);
                return CreatedAtAction(nameof(Get), new { id = giayChungNhanNghiViecHuongBHXH.Id }, giayChungNhanNghiViecHuongBHXH.ToModel<GiayChungNhanNghiViecHuongBHXHViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(giayChungNhanNghiViecHuongBHXHViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = giayChungNhanNghiViecHuongBHXHViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = giayChungNhanNghiViecHuongBHXHViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = giayChungNhanNghiViecHuongBHXHViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                giayChungNhanNghiViecHuongBHXHViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #endregion
        #region In 
        [HttpPost("InGiayChungNhanNghiViecHuongBHXH")]
        public async Task<ActionResult<string>> InGiayChungNhanNghiViecHuongBHXH([FromBody]XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH xacNhanIn)
        {
            //XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH
            var html = await _dieuTriNoiTruService.InGiayChungNhanNghiViecHuongBHXH(xacNhanIn);
            return html;
        }
        [HttpPost("GetMaBacSi")]
        public async Task<ActionResult<string>> GetMaBacSi([FromBody]GiayChungNhanNghiViecHuongBHXHQueryInfo xacNhanIn)
        {
            var maBS = await _dieuTriNoiTruService.GetMaBS(xacNhanIn.Searching);
            return maBS;
        }
        [HttpPost("InGiayChungNhanNghiViecHuongBHXHNgoaTruVaNoTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachXuatChungTuExcel)]
        public async Task<ActionResult<string>> InGiayChungNhanNghiViecHuongBHXHNgoaTruVaNoTru([FromBody]GiayNghiHuongBHXHQueryInfo info)
        {
            var html = string.Empty;
            if (info.YeuCauTiepNhanNgoaiTruId != null)
            {
                var yckbIds = _yeuCauKhamBenhService.GetYeuCauKhamBenhNghiHuongBHXH((long)info.YeuCauTiepNhanNgoaiTruId);
                var slPhieu = 0;
                foreach (var item in yckbIds.Result.ToList())
                {
                    ThongTinNgayNghiHuongBHYT thongTin = new ThongTinNgayNghiHuongBHYT
                    {
                        YeuCauKhamBenhId = item.Id,
                        ThoiDiemTiepNhan = item.NghiHuongBHXHTuNgay,
                        DenNgay = item.NghiHuongBHXHDenNgay,

                        PhuongPhapDieuTriNghiHuongBHYT = item.PhuongPhapDieuTriNghiHuongBHYT,
                        ICDChinhNghiHuongBHYT = item.ICDChinhNghiHuongBHYT,
                        TenICDChinhNghiHuongBHYT = item.TenICDChinhNghiHuongBHYT,
                    };

                    var htmlNgoai = _yeuCauKhamBenhService.XemGiayNghiHuongBHYTLien1(thongTin);

                    if (!string.IsNullOrEmpty(htmlNgoai))
                    {
                        html += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>Giấy nghỉ hưởng BHXH ngoại trú</th></tr></table>" + htmlNgoai;
                    }
                    if (slPhieu < yckbIds.Result.Count())
                    {
                        html += "<div class=\"pagebreak\"> </div>";
                    }

                }
            }

            var infoIn = new XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH
            {
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH,
                YeuCauTiepNhanId = info.YeuCauTiepNhanNoiTruId ?? 0
            };
            var htmlNoiTru = await _dieuTriNoiTruService.InGiayChungNhanNghiViecHuongBHXH(infoIn);

            if (!string.IsNullOrEmpty(htmlNoiTru))
            {
                if (!string.IsNullOrEmpty(html))
                {
                    html += "<div class=\"pagebreak\"> </div>";
                }
                html += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>Giấy nghỉ hưởng BHXH nội trú</th></tr></table>" + htmlNoiTru;
            }

            return Ok(html);
        }
        #endregion
    }
}
