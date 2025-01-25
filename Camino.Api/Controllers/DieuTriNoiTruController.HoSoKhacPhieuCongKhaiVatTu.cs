using Camino.Api.Extensions;
using Camino.Api.Models.PhieuCongKhaiVatTu;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiVatTu;
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
        [HttpPost("GetDataPhieuCongKhaiVatTu")]
        public List<string> GetDataPhieuCongKhaiVatTu(PhieuCongKhaiVatTuViewModel phieuCongKhaiVatTuViewModel)
        {
            var phieuCongKhaiVatTu = new PhieuCongKhaiVatTu
            {
                NgayVaoVien = (DateTime)phieuCongKhaiVatTuViewModel.NgayVaoVien,
                NgayRaVien = (DateTime)phieuCongKhaiVatTuViewModel.NgayRaVien,
                YeuCauTiepNhanId = phieuCongKhaiVatTuViewModel.YeuCauTiepNhanId
            };
            var lookup = _dieuTriNoiTruService.GetDataPhieuCongKhaiVatTu(phieuCongKhaiVatTu);
            return lookup;
        }
        [HttpPost("GetThongTinPhieuCongKhaiVatTu")]
        public PhieuCongKhaiVatTuObject GetThongTinPhieuCongKhaiVatTu(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuCongKhaiVatTu(yeuCauTiepNhanId);
            return lookup;
        }
        #region save / update
        [HttpPost("LuuPhieuCongKhaiVatTu")]
        public async Task<ActionResult<PhieuCongKhaiVatTuViewModel>> LuuPhieuCongKhaiVatTu([FromBody] PhieuCongKhaiVatTuViewModel phieuCongKhaiVatTuViewModel)
        {
            if (phieuCongKhaiVatTuViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new PhieuCongKhaiVatTuViewModel()
                {
                    YeuCauTiepNhanId = phieuCongKhaiVatTuViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuCongKhaiVatTuViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuCongKhaiVatTuViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (phieuCongKhaiVatTuViewModel.FileChuKy.Count() > 0)
                {
                    foreach (var itemfileChuKy in phieuCongKhaiVatTuViewModel.FileChuKy)
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
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuCongKhaiVatTuViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuCongKhaiVatTuViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuCongKhaiVatTuViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuCongKhaiVatTuViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuCongKhaiVatTuViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuCongKhaiVatTuViewModel.ToEntity<NoiTruHoSoKhac>();
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
                if (phieuCongKhaiVatTuViewModel.Id != 0)
                {
                    if (phieuCongKhaiVatTuViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuCongKhaiVatTuViewModel.FileChuKy)
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
            return null;
        }
        #endregion
        #region In 
        [HttpPost("InPhieuCongKhaiVatTu")]
        public async Task<ActionResult<string>> InPhieuCongKhaiVatTu([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.InPhieuCongKhaiVatTu(xacNhanInTrichBienBanHoiChan);
            return html;
        }
        #endregion
    }
}
