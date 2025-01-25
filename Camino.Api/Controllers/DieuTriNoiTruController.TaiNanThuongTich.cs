using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinTaiNanThuongTich")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<DieuTriNoiTruTaiNanThuongTichViewModel>> GetThongTinTaiNanThuongTich(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn));

            var dieuTriNoiTruTaiNanThuongTich = new DieuTriNoiTruTaiNanThuongTichViewModel
            {
                Id = yeuCauTiepNhan.Id,
                ThongTinTaiNanThuongTich = yeuCauTiepNhan.NoiTruBenhAn.ThongTinTaiNanThuongTich
            };

            return dieuTriNoiTruTaiNanThuongTich;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("CapNhatTaiNanThuongTich")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatTaiNanThuongTich([FromBody] DieuTriNoiTruTaiNanThuongTichViewModel dieuTriNoiTruTaiNanThuongTichViewModel)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(dieuTriNoiTruTaiNanThuongTichViewModel.Id, o => o.Include(p => p.NoiTruBenhAn));

            // kết thúc bệnh án => không cho them
            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            yeuCauTiepNhan.NoiTruBenhAn.ThongTinTaiNanThuongTich = dieuTriNoiTruTaiNanThuongTichViewModel.ThongTinTaiNanThuongTich;

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }
    }
}