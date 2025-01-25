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
        [HttpGet("GetThongTinHoSoKhacPhieuTheoDoiChucNangSong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacPhieuTheoDoiChucNangSongViewModel>> GetThongTinHoSoKhacPhieuTheoDoiChucNangSongAsync(long yeuCauTiepNhanId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinHoSoKhacPhieuTheoDoiChucNangSong(yeuCauTiepNhanId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacPhieuTheoDoiChucNangSongViewModel>() ?? new HoSoKhacPhieuTheoDoiChucNangSongViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            }

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacPhieuTheoDoiChucNangSong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacPhieuTheoDoiChucNangSong([FromBody] HoSoKhacPhieuTheoDoiChucNangSongViewModel hoSoKhacPhieuTheoDoiChucNangSongViewModel)
        {
            hoSoKhacPhieuTheoDoiChucNangSongViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacPhieuTheoDoiChucNangSongViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacPhieuTheoDoiChucNangSongViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacPhieuTheoDoiChucNangSongViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                             .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacPhieuTheoDoiChucNangSongViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacPhieuTheoDoiChucNangSongViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(c => c.Id == hoSoKhacPhieuTheoDoiChucNangSongViewModel.Id);
                hoSoKhac = hoSoKhacPhieuTheoDoiChucNangSongViewModel.ToEntity(hoSoKhac);

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
        [HttpGet("InPhieuTheoDoiChucNangSong")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InPhieuTheoDoiChucNangSong(long yeuCauTiepNhanId)
        {
            var html = await _dieuTriNoiTruService.InPhieuTheoDoiChucNangSong(yeuCauTiepNhanId, false);

            return html;
        }
    }
}
