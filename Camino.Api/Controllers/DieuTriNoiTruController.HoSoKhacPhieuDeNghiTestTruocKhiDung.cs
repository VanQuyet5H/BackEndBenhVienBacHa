using Camino.Api.Extensions;
using Camino.Api.Models.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetThongTinPhieuDeNghiTestTruocKhiDung")]
        public PhieuDeNghiTestTruocKhiDungThuocGridVo GetThongTinPhieuDeNghiTestTruocKhiDung(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuDeNghiTestTruocKhiDung(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("ValidatorPhieuDeNghiTestTruocKhiDung")]
        public ActionResult CheckValidationForQuayThuoc([FromBody]ValiDatorPhieuDeNghiTestTruocKhiDung validationCheck)
        {
            return Ok();
        }
        [HttpPost("LuuPhieuDeNghiTestTruocKhiDungThuoc")]
        public async Task<ActionResult<PhieuDeNghiTestTruocKhiDungThuocViewModel>> LuuPhieuDeNghiTestTruocKhiDungThuoc([FromBody] PhieuDeNghiTestTruocKhiDungThuocViewModel phieuDeNghiTestTruocKhiDungThuocViewModel)
        {
            if (phieuDeNghiTestTruocKhiDungThuocViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phieuKhaiThacTienSuBenh = new PhieuDeNghiTestTruocKhiDungThuocViewModel()
                {
                    YeuCauTiepNhanId = phieuDeNghiTestTruocKhiDungThuocViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuDeNghiTestTruocKhiDungThuocViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuDeNghiTestTruocKhiDungThuocViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = phieuKhaiThacTienSuBenh.ToEntity<NoiTruHoSoKhac>();
                if (phieuDeNghiTestTruocKhiDungThuocViewModel.FileChuKy != null)
                {
                    foreach (var itemfileChuKy in phieuDeNghiTestTruocKhiDungThuocViewModel.FileChuKy)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileChuKy.Uid,
                            Ten = itemfileChuKy.Ten,
                            TenGuid = itemfileChuKy.TenGuid,
                            DuongDan = itemfileChuKy.DuongDan,
                            LoaiTapTin = itemfileChuKy.LoaiTapTin,
                            MoTa = itemfileChuKy.MoTa,
                            KichThuoc = itemfileChuKy.KichThuoc
                        };
                        user.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuDeNghiTestTruocKhiDungThuocViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuDeNghiTestTruocKhiDungThuocViewModel.Id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuDeNghiTestTruocKhiDungThuocViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuDeNghiTestTruocKhiDungThuocViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuDeNghiTestTruocKhiDungThuocViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuDeNghiTestTruocKhiDungThuocViewModel.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                if (update.NoiTruHoSoKhacFileDinhKems.Any())
                {
                    foreach (var itemNoiTruHoSoKhacFileDinhKem in update.NoiTruHoSoKhacFileDinhKems.ToList())
                    {
                        var soket = await _noiDuTruHoSoKhacFileDinhKemService.GetByIdAsync(itemNoiTruHoSoKhacFileDinhKem.Id);
                        if (soket == null)
                        {
                            return NotFound();
                        }
                        await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(soket.Id);
                    }
                }
                if (phieuDeNghiTestTruocKhiDungThuocViewModel.Id != 0)
                {
                    if (phieuDeNghiTestTruocKhiDungThuocViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuDeNghiTestTruocKhiDungThuocViewModel.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Uid,
                                Ten = itemfileChuKy.Ten,
                                TenGuid = itemfileChuKy.TenGuid,
                                DuongDan = itemfileChuKy.DuongDan,
                                LoaiTapTin = itemfileChuKy.LoaiTapTin,
                                MoTa = itemfileChuKy.MoTa,
                                KichThuoc = itemfileChuKy.KichThuoc
                            };
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        }
                    }
                }
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #region In 
        [HttpPost("InPhieuDeNghiTestTruocKhiDungThuoc")]
        public async Task<ActionResult<string>> InPhieuDeNghiTestTruocKhiDungThuoc([FromBody]InPhieuDeNghiTestTruocKhiDungThuoc inPhieuDeNghiTestTruocKhiDungThuoc)
        {
            var html = await _dieuTriNoiTruService.InPhieuDeNghiTestTruocKhiDungThuoc(inPhieuDeNghiTestTruocKhiDungThuoc);
            return html;
        }
        #endregion
    }
}
