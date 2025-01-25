using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.PhieuTheoDoiTruyenDich;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetDataDanhSachTruyenDichForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public PhieuTheoDoiTruyenDichGrid GetDataDanhSachTruyenDichForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var lookup = _dieuTriNoiTruService.GetDataDanhSachTruyenDichForGridAsync(queryInfo);
            return lookup;
        }
        #region get DanhSach tên thuốc truyền dịch theo ngày
        [HttpPost("GetDataDanhSachTruyenDichTheoNgayForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDataDanhSachTruyenDichTheoNgayForGridAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = _dieuTriNoiTruService.GetDataDanhSachTruyenDichTheoNgayForGridAsync(queryInfo);
            return Ok(lookup);
        }
        // data da chon
        [HttpPost("GetDataBindTruyenDichTheoNgayForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public DuocPhamPhieuDieuTriTheoNgay GetDataBindTruyenDichTheoNgayForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var lookup = _dieuTriNoiTruService.GetDataBindTruyenDichTheoNgayForGridAsync(queryInfo);
            return lookup;
        }
        #endregion
        #region Save // update
        [HttpPost("LuuPhieuTheoDoiTruyenDich")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<PhieuTheoDoiTruyenDichViewModel>> LuuPhieuTheoDoiTruyenDich([FromBody] PhieuTheoDoiTruyenDichViewModel phieuTheoDoiTruyenDichViewModel)
        {
            if (phieuTheoDoiTruyenDichViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phieusoket15ngay = new PhieuTheoDoiTruyenDichViewModel()
                {
                    YeuCauTiepNhanId = phieuTheoDoiTruyenDichViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuTheoDoiTruyenDichViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuTheoDoiTruyenDichViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = phieusoket15ngay.ToEntity<NoiTruHoSoKhac>();

                CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<PhieuTheoDoiTruyenDichViewModel>());
                if (phieuTheoDoiTruyenDichViewModel.FileChuKy != null)
                {
                    foreach (var itemfileChuKy in phieuTheoDoiTruyenDichViewModel.FileChuKy)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileChuKy.Ma,
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
                return Ok(user.Id);
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuTheoDoiTruyenDichViewModel.Id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems).Include(j => j.YeuCauTiepNhan).ThenInclude(p => p.KetQuaSinhHieus));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuTheoDoiTruyenDichViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuTheoDoiTruyenDichViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuTheoDoiTruyenDichViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuTheoDoiTruyenDichViewModel.ToEntity<NoiTruHoSoKhac>();

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
                if (phieuTheoDoiTruyenDichViewModel.Id != 0)
                {
                    if (phieuTheoDoiTruyenDichViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuTheoDoiTruyenDichViewModel.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Ma,
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
        [HttpPost("GetThongTinPhieuTheoDoiTruyenDich")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public PhieuTheoDoiTruyenDichGridInfo GetThongTinPhieuTheoDoiTruyenDich(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuTheoDoiTruyenDich(yeuCauTiepNhanId);
            return lookup;
        }
        #region In 
        [HttpPost("InPhieuTheoDoiTruyenDich")]
        public async Task<ActionResult<string>> InPhieuTheoDoiTruyenDich([FromBody]XacNhanInPhieuTheoDoiTruyenDich xacNhanIn)
        {
            var html = await _dieuTriNoiTruService.InPhieuTheoDoiTruyenDich(xacNhanIn);
            return html;
        }
        #endregion
        #region validate change sl
        [HttpPost("ValidateSoLuongChangeDichTruyen")]
        public ActionResult ValidateSoLuongChangeDichTruyen(ValidatetorTruyenDich validationCheck)
        {
            return Ok();
        }
        [HttpPost("ValidatorTotalSlKhongVuotTongBanDau")]
        public async Task<bool> ValidatorTotalSlKhongVuotTongBanDau(ValidatetorTruyenDichVo validationCheck)
        {
            var value =  await _dieuTriNoiTruService.ValidatorTotalSlKhongVuotTongBanDau(validationCheck);
            return value;
        }
        #endregion
    }
}
