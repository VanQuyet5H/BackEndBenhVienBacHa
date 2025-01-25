using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetThongTinHoSoChamSocDieuDuongHoSinh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public HoSoChamSocDieuDuongHoSinhVo GetThongTinHoSoChamSocDieuDuongHoSinh(long yeuCauTiepNhanId)
        {
            var kiemTraPhieuTonTai = _noiTruHoSoKhacService.HoSoKhacIds(yeuCauTiepNhanId, LoaiHoSoDieuTriNoiTru.HoSoChamSocDieuDuongHoSinh);
            if (kiemTraPhieuTonTai.Result.Any())
            {
                var hoSoKhac = _dieuTriNoiTruService.GetPhieuHoSoKhacJson(yeuCauTiepNhanId, kiemTraPhieuTonTai.Result.FirstOrDefault()); // du yêu cầu tiêp nhận
                var jsonData = JsonConvert.DeserializeObject<HoSoChamSocDieuDuongHoSinhVo>(hoSoKhac);
                jsonData.Id = kiemTraPhieuTonTai.Result.FirstOrDefault();
                return jsonData;
            }
            else
            {
                // getThongTin tư 1 tới 7 là thông tin theo yêu cầu tiếp nhận
                var getThongTin = _dieuTriNoiTruService.GetDataTheoYCTN(yeuCauTiepNhanId);
                var newHoSoChamSocDieuDuongHoSinhVo = new HoSoChamSocDieuDuongHoSinhVo();
                newHoSoChamSocDieuDuongHoSinhVo = getThongTin;
                return newHoSoChamSocDieuDuongHoSinhVo;
            }
        }
        [HttpPost("LuuHoSoDieuDtriChamSocHoSinh")]
        public async Task<ActionResult<HoSoChamSocDieuDuongHoSinhViewModel>> LuuHoSoDieuDtriChamSocHoSinh([FromBody] HoSoChamSocDieuDuongHoSinhViewModel hoSoChamSocDieuDuongHoSinhViewModel)
        {
            if (hoSoChamSocDieuDuongHoSinhViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phieuKhaiThacTienSuBenh = new HoSoChamSocDieuDuongHoSinhViewModel()
                {
                    YeuCauTiepNhanId = hoSoChamSocDieuDuongHoSinhViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = hoSoChamSocDieuDuongHoSinhViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = hoSoChamSocDieuDuongHoSinhViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = phieuKhaiThacTienSuBenh.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<HoSoChamSocDieuDuongHoSinhViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(hoSoChamSocDieuDuongHoSinhViewModel.Id);
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = hoSoChamSocDieuDuongHoSinhViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = hoSoChamSocDieuDuongHoSinhViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = hoSoChamSocDieuDuongHoSinhViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                hoSoChamSocDieuDuongHoSinhViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #region In 
        [HttpPost("InHoSoDieuDtriChamSocHoSinh")]
        public async Task<ActionResult<string>> InHoSoDieuDtriChamSocHoSinh([FromBody]XacNhanInHoSoChamSocDieuDuongHoSinh xacNhanInHoSoChamSocDieuDuongHoSinh)
        {
            var html = await _dieuTriNoiTruService.InHoSoDieuDtriChamSocHoSinh(xacNhanInHoSoChamSocDieuDuongHoSinh);
            return html;
        }
        #endregion
    }
}
