using Camino.Api.Extensions;
using Camino.Api.Models.PhieuKhaiThacTienSuBenh;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuKhaiThacTienSuDiUng;
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
        [HttpPost("LuuPhieuKhaiThacTienSuBenh")]
        public async Task<ActionResult<PhieuKhaiThacTienSuBenhViewModel>> LuuPhieuKhaiThacTienSuBenh([FromBody] PhieuKhaiThacTienSuBenhViewModel phieuKhaiThacTienSuBenhViewModel)
        {
            if (phieuKhaiThacTienSuBenhViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phieuKhaiThacTienSuBenh = new PhieuKhaiThacTienSuBenhViewModel()
                {
                    YeuCauTiepNhanId = phieuKhaiThacTienSuBenhViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuKhaiThacTienSuBenhViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuKhaiThacTienSuBenhViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = phieuKhaiThacTienSuBenh.ToEntity<NoiTruHoSoKhac>();
                if (phieuKhaiThacTienSuBenhViewModel.FileChuKy.Count() > 0)
                {
                    foreach (var itemfileChuKy in phieuKhaiThacTienSuBenhViewModel.FileChuKy)
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
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuKhaiThacTienSuBenhViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuKhaiThacTienSuBenhViewModel.Id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuKhaiThacTienSuBenhViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuKhaiThacTienSuBenhViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuKhaiThacTienSuBenhViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuKhaiThacTienSuBenhViewModel.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                if(update.NoiTruHoSoKhacFileDinhKems.Count() > 0)
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
                if (phieuKhaiThacTienSuBenhViewModel.Id != 0)
                {
                    if (phieuKhaiThacTienSuBenhViewModel.FileChuKy.Count() > 0)
                    {
                        foreach (var itemfileChuKy in phieuKhaiThacTienSuBenhViewModel.FileChuKy)
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
        [HttpPost("GetPhieuKhaiThacTienSuDiUngConfig")]
        public PhieuKhaiThacTienSuDiUngConfig GetPhieuKhaiThacTienSuDiUngConfig()
        {
            var lookup = _dieuTriNoiTruService.PhieuKhaiThacTienSuDiUngConfig();
            return lookup;
        }
        [HttpPost("GetDanhSachPhieuKhaiThacTienSuBenh")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachPhieuKhaiThacTienSuBenh()
        {
            var lookup = await _dieuTriNoiTruService.GetListTenNhanVienmAsync();
            return Ok(lookup);
        }
        [HttpPost("GetThongTinPhieuKhaiThacTienSuBenh")]
        public PhieuKhaiThacTienSuDiUngGridVo GetThongTinPhieuKhaiThacTienSuBenh(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuKhaiThacTienSuBenh(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetDanhSachDuocPhamQuocGia")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachDuocPhamQuocGia()
        {
            var lookup =  _dieuTriNoiTruService.GetDanhSachDuocPhamQuocGia();
            return Ok(lookup);
        }
        [HttpPost("GetDanhSachDuocPhamQuocGiaDeNghiTest")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachDuocPhamQuocGiaDeNghiTest()
        {
            var lookup = _dieuTriNoiTruService.GetDanhSachDuocPhamQuocGiaDeNghiTest();
            return Ok(lookup);
        }
        #region In 
        [HttpPost("InPhieuKhaiThacTienSuBenh")]
        public async Task<ActionResult<string>> InTrichBienBanHoiChan([FromBody]XacNhanInTienSu xacNhanIn)
        {
            var html = await _dieuTriNoiTruService.PhieuKhaiThacTienSuBenh(xacNhanIn);
            return html;
        }
        #endregion
    }
}
