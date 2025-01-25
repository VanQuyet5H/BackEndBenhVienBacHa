using Camino.Api.Extensions;
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

        [HttpPost("GetDataPhieuCongKhaiThuocVatTu")]
        public List<string> GetDataPhieuCongKhaiThuocVatTu(
                        PhieuCongKhaiThuocVatTuViewModel phieuCongKhaiThuocVatTuViewModel)
        {
            var phieuCongKhaiThuocVatTu = new PhieuCongKhaiThuocVatTu
            {
                YeuCauTiepNhanId = phieuCongKhaiThuocVatTuViewModel.YeuCauTiepNhanId
            };
            var lookup = _dieuTriNoiTruService.GetDataPhieuCongKhaiThuocVatTu(phieuCongKhaiThuocVatTu);
            return lookup;
        }

        [HttpPost("GetThongTinPhieuCongKhaiThuocVatTu")]
        public PhieuCongKhaiThuocVatTuObject GetThongTinPhieuCongKhaiThuocVatTu(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuCongKhaiThuocVatTu(yeuCauTiepNhanId);
            return lookup;
        }

        #endregion

        #region SAVE / UPDATE

        [HttpPost("LuuPhieuCongKhaiThuocVatTu")]
        public async Task<ActionResult<PhieuCongKhaiThuocVatTuViewModel>> LuuPhieuCongKhaiThuocVatTu([FromBody] PhieuCongKhaiThuocVatTuViewModel phieuCongKhaiThuocVatTuViewModel)
        {
            var dieuTriNoiTru = await _dieuTriNoiTruService.GetByIdAsync(phieuCongKhaiThuocVatTuViewModel.YeuCauTiepNhanId);
            if (dieuTriNoiTru.CoBHYT == false)
            {
                throw new Exception("Bệnh nhân không có BHYT.");
            }

            if (phieuCongKhaiThuocVatTuViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new PhieuCongKhaiThuocVatTuViewModel()
                {
                    YeuCauTiepNhanId = phieuCongKhaiThuocVatTuViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuCongKhaiThuocVatTuViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuCongKhaiThuocVatTuViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (phieuCongKhaiThuocVatTuViewModel.FileChuKy.Count() > 0)
                {
                    foreach (var itemfileChuKy in phieuCongKhaiThuocVatTuViewModel.FileChuKy)
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
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuCongKhaiThuocVatTuViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuCongKhaiThuocVatTuViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuCongKhaiThuocVatTuViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuCongKhaiThuocVatTuViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuCongKhaiThuocVatTuViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuCongKhaiThuocVatTuViewModel.ToEntity<NoiTruHoSoKhac>();
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
                if (phieuCongKhaiThuocVatTuViewModel.Id != 0)
                {
                    if (phieuCongKhaiThuocVatTuViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuCongKhaiThuocVatTuViewModel.FileChuKy)
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

        [HttpPost("InPhieuCongKhaiThuocVatTuYte")]
        public async Task<ActionResult<string>> InPhieuCongKhaiThuocVatTuYte([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.InPhieuCongKhaiThuocVatTu(xacNhanInTrichBienBanHoiChan);
            return html;
        }

        #endregion
    }
}
