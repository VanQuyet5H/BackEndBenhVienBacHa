using Camino.Api.Extensions;
using Camino.Api.Models.PhieuCongKhaiDichVuKyThuat;
using Camino.Api.Models.PhieuCongKhaiDichVuKyThuat;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat;
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
        #region GET DATA

        [HttpPost("GetDataPhieuCongKhaiDichVuKyThuat")]
        public List<string> GetDataPhieuCongKhaiDichVuKyThuat(
                        PhieuCongKhaiDichVuKyThuatViewModel phieuCongKhaiDichVuKyThuatViewModel)
        {
            var phieuCongKhaiDichVuKyThuat = new PhieuCongKhaiDichVuKyThuat
            {
                YeuCauTiepNhanId = phieuCongKhaiDichVuKyThuatViewModel.YeuCauTiepNhanId
            };
            var lookup = _dieuTriNoiTruService.GetDataPhieuCongKhaiDichVuKyThuat(phieuCongKhaiDichVuKyThuat);
            return lookup;
        }

        [HttpPost("GetThongTinPhieuCongKhaiDichVuKyThuat")]
        public PhieuCongKhaiDichVuKyThuatObject GetThongTinPhieuCongKhaiDichVuKyThuat(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuCongKhaiDichVuKyThuat(yeuCauTiepNhanId);
            return lookup;
        }

        #endregion

        #region SAVE / UPDATE

        [HttpPost("LuuPhieuCongKhaiDichVuKyThuat")]
        public async Task<ActionResult<PhieuCongKhaiDichVuKyThuatViewModel>> LuuPhieuCongKhaiDichVuKyThuat([FromBody] PhieuCongKhaiDichVuKyThuatViewModel phieuCongKhaiDichVuKyThuatViewModel)
        {
            if (phieuCongKhaiDichVuKyThuatViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new PhieuCongKhaiDichVuKyThuatViewModel()
                {
                    YeuCauTiepNhanId = phieuCongKhaiDichVuKyThuatViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuCongKhaiDichVuKyThuatViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuCongKhaiDichVuKyThuatViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (phieuCongKhaiDichVuKyThuatViewModel.FileChuKy.Count() > 0)
                {
                    foreach (var itemfileChuKy in phieuCongKhaiDichVuKyThuatViewModel.FileChuKy)
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
                            KichThuoc = itemfileChuKy.KichVatTu
                        };
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        user.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuCongKhaiDichVuKyThuatViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuCongKhaiDichVuKyThuatViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuCongKhaiDichVuKyThuatViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuCongKhaiDichVuKyThuatViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuCongKhaiDichVuKyThuatViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuCongKhaiDichVuKyThuatViewModel.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                foreach (var itemNoiTruHoSoKhacFileDinhKem in update.NoiTruHoSoKhacFileDinhKems.ToList())
                {
                    var soket = await _noiDuTruHoSoKhacFileDinhKemService.GetByIdAsync(itemNoiTruHoSoKhacFileDinhKem.Id);
                    if (soket == null)
                    {
                        return NotFound();
                    }
                    await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(soket.Id);
                }
                if (phieuCongKhaiDichVuKyThuatViewModel.Id != 0)
                {
                    if (phieuCongKhaiDichVuKyThuatViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuCongKhaiDichVuKyThuatViewModel.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Uid,
                                Ten = itemfileChuKy.Ten,
                                TenGuid = itemfileChuKy.TenGuid,
                                DuongDan = itemfileChuKy.DuongDan,
                                LoaiTapTin = itemfileChuKy.LoaiTapTin,
                                MoTa = itemfileChuKy.MoTa,
                                KichThuoc = itemfileChuKy.KichVatTu
                            };
                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        }
                    }

                }
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
        }

        #endregion

        #region PRINTF 

        [HttpPost("InPhieuCongKhaiDichVuKyThuat")]
        public async Task<ActionResult<string>> InPhieuCongKhaiDichVuKyThuat([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.InPhieuCongKhaiDichVuKyThuat(xacNhanInTrichBienBanHoiChan);
            return html;
        }

        #endregion
    }
}
