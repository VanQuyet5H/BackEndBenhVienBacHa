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

        [HttpPost("ThemHoacCapNhatPhieuChamSocSoSinh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemHoacCapNhatPhieuChamSocSoSinh(PhieuChamSocSoSinhViewModel hoSoKhacViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoSoKhacViewModel.YeuCauTiepNhanId);
            if (hoSoKhacViewModel.Id == 0) // Them
            {
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuChamSoc;
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                hoSoKhacViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
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
                var noiTruHoSoKhac = _noiTruHoSoKhacService.GetById(hoSoKhacViewModel.Id);
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuChamSoc;
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

        [HttpGet("GetPhieuChamSocSoSinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<PhieuChamSocSoSinhViewModel>> GetPhieuChamSocSoSinh(long id)
        {
            var ycTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs)
                                                                        .ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User));

            var hoSoGayMe = ycTiepNhan.NoiTruHoSoKhacs.FirstOrDefault(p => p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuChamSoc);
            if (hoSoGayMe == null)
            {
                return NoContent();
            }
            var model = hoSoGayMe.ToModel<PhieuChamSocSoSinhViewModel>();
            return Ok(model);
        }

        //[HttpPost("InBienKiemTruocTiemChungTE")]
        //public ActionResult InBienKiemTruocTiemChungTE(long noiTruHoSoKhacId)
        //{
        //    var result = _dieuTriNoiTruService.InBienKiemTruocTiemChungTE(noiTruHoSoKhacId);
        //    return Ok(result);
        //}
    }
}
