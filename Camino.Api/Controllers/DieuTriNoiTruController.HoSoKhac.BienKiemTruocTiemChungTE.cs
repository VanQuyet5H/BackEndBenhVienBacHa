using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("ThemHoacCapNhatTiemChungTreEm")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemHoacCapNhatTiemChungTreEm(HoSoKhacBanKiemTiemChungTreEmViewModel hoSoKhacViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoSoKhacViewModel.YeuCauTiepNhanId);
            if (hoSoKhacViewModel.Id == 0) // Them
            {
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BanKiemTruocTiemChungTreEm;
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                foreach (var item in hoSoKhacViewModel.NoiTruHoSoKhacFileDinhKems)
                {
                    item.Ma = Guid.NewGuid().ToString();
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                }
                var noiTruHoSoKhac = hoSoKhacViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhac);
                var resul = new
                {
                    noiTruHoSoKhac.Id,
                    noiTruHoSoKhac.LastModified,
                };
                return Ok(resul);
            }
            else // Update
            {
                var noiTruHoSoKhac = _noiTruHoSoKhacService.GetById(hoSoKhacViewModel.Id, s => s.Include(p => p.NoiTruHoSoKhacFileDinhKems));
                foreach (var item in hoSoKhacViewModel.NoiTruHoSoKhacFileDinhKems)
                {
                    item.Ma = Guid.NewGuid().ToString();
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                }
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BanKiemTruocTiemChungTreEm;
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                hoSoKhacViewModel.ToEntity(noiTruHoSoKhac);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                var resul = new
                {
                    noiTruHoSoKhac.Id,
                    noiTruHoSoKhac.LastModified,
                };
                return Ok(resul);
            }
        }

        [HttpGet("GetHoSoKhacTiemChungTreEm")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacBanKiemTiemChungTreEmViewModel>> GetHoSoKhacTiemChungTreEm(long id)
        {
            var ycTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                                 .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            var hoSoGayMe = ycTiepNhan.NoiTruHoSoKhacs.FirstOrDefault(p => p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BanKiemTruocTiemChungTreEm);
            if (hoSoGayMe == null)
            {
                return NoContent();
            }
            var model = hoSoGayMe.ToModel<HoSoKhacBanKiemTiemChungTreEmViewModel>();
            return Ok(model);
        }

        [HttpPost("InBienKiemTruocTiemChungTE")]
        public ActionResult InBienKiemTruocTiemChungTE(long noiTruHoSoKhacId)
        {
            var result = _dieuTriNoiTruService.InBienKiemTruocTiemChungTE(noiTruHoSoKhacId);
            return Ok(result);
        }
    }
}
