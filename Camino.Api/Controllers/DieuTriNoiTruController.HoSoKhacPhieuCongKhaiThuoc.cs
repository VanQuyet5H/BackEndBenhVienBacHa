using Camino.Api.Extensions;
using Camino.Api.Models.PhieuCongKhaiThuoc;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc;
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
        [HttpPost("GetDataPhieuCongKhaiThuoc")]
        public  List<string> GetDataPhieuCongKhaiThuoc(PhieuCongKhaiThuocViewModel phieuCongKhaiThuocViewModel)
        {
            var phieuCongKhaiThuoc = new PhieuCongKhaiThuoc
            { 
                NgayVaoVien = (DateTime)phieuCongKhaiThuocViewModel.NgayVaoVien,
                NgayRaVien = (DateTime)phieuCongKhaiThuocViewModel.NgayRaVien,
                YeuCauTiepNhanId = phieuCongKhaiThuocViewModel.YeuCauTiepNhanId
            };
            var lookup =  _dieuTriNoiTruService.GetDataPhieuCongKhaiThuoc(phieuCongKhaiThuoc);
            return lookup;
        }
        [HttpPost("GetThongTinPhieuCongKhaiThuoc")]
        public PhieuCongKhaiThuocObject GetThongTinPhieuCongKhaiThuoc(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuCongKhaiThuoc(yeuCauTiepNhanId);
            return lookup;
        }
        #region save / update
        [HttpPost("LuuPhieuCongKhaiThuoc")]
        public async Task<ActionResult<PhieuCongKhaiThuocViewModel>> LuuPhieuCongKhaiThuoc([FromBody] PhieuCongKhaiThuocViewModel phieuCongKhaiThuocViewModel)
        {
            if (phieuCongKhaiThuocViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new PhieuCongKhaiThuocViewModel()
                {
                    YeuCauTiepNhanId = phieuCongKhaiThuocViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuCongKhaiThuocViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuCongKhaiThuocViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (phieuCongKhaiThuocViewModel.FileChuKy.Count() > 0)
                {
                    foreach (var itemfileChuKy in phieuCongKhaiThuocViewModel.FileChuKy)
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
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        user.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuCongKhaiThuocViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuCongKhaiThuocViewModel.Id, s => s.Include(x => x.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuCongKhaiThuocViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuCongKhaiThuocViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuCongKhaiThuocViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuCongKhaiThuocViewModel.ToEntity<NoiTruHoSoKhac>();
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
                if (phieuCongKhaiThuocViewModel.Id != 0)
                {
                    if (phieuCongKhaiThuocViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuCongKhaiThuocViewModel.FileChuKy)
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
        [HttpPost("InPhieuCongKhaiThuoc")]
        public async Task<ActionResult<string>> InPhieuCongKhaiThuoc([FromBody]XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var html = await _dieuTriNoiTruService.InPhieuCongKhaiThuoc(xacNhanInTrichBienBanHoiChan);
            return html;
        }
        #endregion
    }
}
