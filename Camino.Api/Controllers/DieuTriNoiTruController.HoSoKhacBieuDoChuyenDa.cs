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
        [HttpGet("GetThongTinHoSoKhacBieuDoChuyenDa")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacBieuDoChuyenDaViewModel>> GetThongTinHoSoKhacBieuDoChuyenDa(long yeuCauTiepNhanId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinHoSoKhacBieuDoChuyenDa(yeuCauTiepNhanId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacBieuDoChuyenDaViewModel>() ?? new HoSoKhacBieuDoChuyenDaViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            }

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacBieuDoChuyenDa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacBieuDoChuyenDa([FromBody] HoSoKhacBieuDoChuyenDaViewModel hoSoKhacBieuDoChuyenDaViewModel)
        {
            hoSoKhacBieuDoChuyenDaViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacBieuDoChuyenDaViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacBieuDoChuyenDaViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacBieuDoChuyenDaViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                   .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacBieuDoChuyenDaViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacBieuDoChuyenDaViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(c => c.Id == hoSoKhacBieuDoChuyenDaViewModel.Id);
                hoSoKhac = hoSoKhacBieuDoChuyenDaViewModel.ToEntity(hoSoKhac);

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
        [HttpGet("InBieuDoChuyenDa")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InBieuDoChuyenDa(long yeuCauTiepNhanId)
        {
            var html = await _dieuTriNoiTruService.InBieuDoChuyenDa(yeuCauTiepNhanId, false);

            return html;
        }
    }
}
