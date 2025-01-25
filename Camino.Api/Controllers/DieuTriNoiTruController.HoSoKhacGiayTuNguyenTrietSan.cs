using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoSoKhacGiayTuNguyenTrietSan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacGiayTuNguyenTrietSanViewModel>> GetThongTinHoSoKhacGiayTuNguyenTrietSanAsync(long yeuCauTiepNhanId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinHoSoKhacGiayTuNguyenTrietSan(yeuCauTiepNhanId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacGiayTuNguyenTrietSanViewModel>() ?? new HoSoKhacGiayTuNguyenTrietSanViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            }

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacGiayTuNguyenTrietSan")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacGiayTuNguyenTrietSan([FromBody] HoSoKhacGiayTuNguyenTrietSanViewModel hoSoKhacGiayTuNguyenTrietSanViewModel)
        {
            hoSoKhacGiayTuNguyenTrietSanViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacGiayTuNguyenTrietSanViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacGiayTuNguyenTrietSanViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacGiayTuNguyenTrietSanViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                         .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacGiayTuNguyenTrietSanViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacGiayTuNguyenTrietSanViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(c => c.Id == hoSoKhacGiayTuNguyenTrietSanViewModel.Id);
                hoSoKhac = hoSoKhacGiayTuNguyenTrietSanViewModel.ToEntity(hoSoKhac);

                foreach (var item in hoSoKhac.NoiTruHoSoKhacFileDinhKems)
                {
                    if (item.WillDelete != true)
                    {
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                    }
                }
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("InGiayTuNguyenTrietSan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InGiayTuNguyenTrietSan(long yeuCauTiepNhanId, string hostingName)
        {
            var html = await _dieuTriNoiTruService.InGiayTuNguyenTrietSan(yeuCauTiepNhanId, hostingName, false);

            return html;
        }
    }
}
