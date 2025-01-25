using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
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
        [HttpGet("GetThongTinBienBanCamKetGayTeGiamDauTrongDeSauMo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>> GetThongTinBienBanCamKetGayTeGiamDauTrongDeSauMoAsync(long yeuCauTiepNhanId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinBienBanCamKetGayTeGiamDauTrongDeSauMo(yeuCauTiepNhanId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>() ?? new HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            }

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMo")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMo([FromBody] HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel)
        {
            hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                                          .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(c => c.Id == hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.Id);
                hoSoKhac = hoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel.ToEntity(hoSoKhac);

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
        [HttpGet("InBienBanCamKetGayTeGiamDauTrongDeSauMo")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InBienBanCamKetGayTeGiamDauTrongDeSauMo(long yeuCauTiepNhanId)
        {
            var html = await _dieuTriNoiTruService.InBienBanCamKetGayTeGiamDauTrongDeSauMo(yeuCauTiepNhanId, false);

            return html;
        }
    }
}