using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.PhieuTheoDoiTruyenMau;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;
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
        [HttpPost("GetDanhSachMaDonViMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachMaDonViMau([FromBody]DropDownListRequestModel queryInfo,long? yeuCauTiepNhanId, long? MaDVMID)
        {
            var lookup = await _dieuTriNoiTruService.GetDanhSachMaDonViMau(queryInfo, yeuCauTiepNhanId, MaDVMID);
            return Ok(lookup);
        }
        [HttpPost("GetThongTinPhieuTheoDoiTruyenMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public PhieuTheoDoiTruyenMauGrid GetThongTinPhieuTheoDoiTruyenMau(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuTheoDoiTruyenMau(yeuCauTiepNhanId);
            return lookup;
        }
        [HttpPost("GetThongTinPhieuTheoDoiTruyenMauSoSanhMaDonViMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public PhieuTheoDoiTruyenMauGrid GetThongTinPhieuTheoDoiTruyenMauSoSanhMaDonViMau(long yeuCauTiepNhanId,long maDonViMauId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinPhieuTheoDoiTruyenMauSoSanhMaDonViMau(yeuCauTiepNhanId, maDonViMauId);
            return lookup;
        }
        #region Save // update
        [HttpPost("LuuPhieuTheoDoiTruyenMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<PhieuTheoDoiTruyenMauViewModel>> LuuPhieuTheoDoiTruyenMau([FromBody] PhieuTheoDoiTruyenMauViewModel phieuTheoDoiTruyenMauViewModel)
        {
            if (phieuTheoDoiTruyenMauViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phieu = new PhieuTheoDoiTruyenMauViewModel()
                {
                    YeuCauTiepNhanId = phieuTheoDoiTruyenMauViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = phieuTheoDoiTruyenMauViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = phieuTheoDoiTruyenMauViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var phieuEntity = phieu.ToEntity<NoiTruHoSoKhac>();

                CreatedAtAction(nameof(Get), new { id = phieuEntity.Id }, phieuEntity.ToModel<PhieuTheoDoiTruyenMauViewModel>());
                if (phieuTheoDoiTruyenMauViewModel.FileChuKy != null)
                {
                    foreach (var itemfileChuKy in phieuTheoDoiTruyenMauViewModel.FileChuKy)
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
                        phieuEntity.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }

                //Required 2 field: DD truyền máu,Bắt đầu truyền
                //Nếu chưa nhập Ngừng truyền hồi thì cập nhật: NhanVienThucHienId = DD truyền máu, ThoiDiemThucHien = Bắt đầu truyền
                //Nếu có nhập Ngừng truyền hồi thì cập nhật: TrangThai = Da thực hiện, ThoiDiemHoanThanh = Ngừng truyền hồi, NhanVienThucHienId = DD truyền máu,
                //ThoiDiemThucHien = Bắt đầu truyền

                var yctn = await _yeuCauTiepNhanService.GetByIdAsync(phieuTheoDoiTruyenMauViewModel.YeuCauTiepNhanId,s=>s.Include(z => z.YeuCauTruyenMaus).ThenInclude(p=>p.NhapKhoMauChiTiets));

                if (phieuTheoDoiTruyenMauViewModel.NgungTruyenHoi == null)
                {

                    var listYCTM = yctn.YeuCauTruyenMaus.Where(item => item.NhapKhoMauChiTiets.Any(p => p.Id == phieuTheoDoiTruyenMauViewModel.MaDonViMauTruyenId)).ToList();
                    foreach (var item in listYCTM)
                    {
                        item.NhanVienThucHienId = phieuTheoDoiTruyenMauViewModel.DDTruyenMauId;
                        item.ThoiDiemThucHien = phieuTheoDoiTruyenMauViewModel.BatDauTruyenHoi;
                        item.TrangThai = Enums.EnumTrangThaiYeuCauTruyenMau.ChuaThucHien;
                        item.ThoiDiemHoanThanh = phieuTheoDoiTruyenMauViewModel.NgungTruyenHoi;

                    }

                    phieuEntity.YeuCauTiepNhan = yctn; 

                }
                else
                {
                    var listYCTM = yctn.YeuCauTruyenMaus.Where(item => item.NhapKhoMauChiTiets.Any(p => p.Id == phieuTheoDoiTruyenMauViewModel.MaDonViMauTruyenId)).ToList();
                    foreach (var item in listYCTM)
                    {
                        item.TrangThai = Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien;
                        item.ThoiDiemHoanThanh = phieuTheoDoiTruyenMauViewModel.NgungTruyenHoi;
                        item.NhanVienThucHienId = phieuTheoDoiTruyenMauViewModel.DDTruyenMauId;
                        item.ThoiDiemThucHien = phieuTheoDoiTruyenMauViewModel.BatDauTruyenHoi;
                    }
                    phieuEntity.YeuCauTiepNhan = yctn;
                }
                await _noiDuTruHoSoKhacService.UpdateAsync(phieuEntity);
                return Ok(phieuEntity.Id);
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(phieuTheoDoiTruyenMauViewModel.Id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems).Include(j => j.YeuCauTiepNhan).ThenInclude(p => p.KetQuaSinhHieus).Include(j => j.YeuCauTiepNhan).ThenInclude(p=>p.YeuCauTruyenMaus));
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = phieuTheoDoiTruyenMauViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = phieuTheoDoiTruyenMauViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = phieuTheoDoiTruyenMauViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                phieuTheoDoiTruyenMauViewModel.ToEntity<NoiTruHoSoKhac>();

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
                if (phieuTheoDoiTruyenMauViewModel.Id != 0)
                {
                    if (phieuTheoDoiTruyenMauViewModel.FileChuKy != null)
                    {
                        foreach (var itemfileChuKy in phieuTheoDoiTruyenMauViewModel.FileChuKy)
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
                var yctn = await _yeuCauTiepNhanService.GetByIdAsync(phieuTheoDoiTruyenMauViewModel.YeuCauTiepNhanId, s => s.Include(z => z.YeuCauTruyenMaus).ThenInclude(p => p.NhapKhoMauChiTiets));

                if (phieuTheoDoiTruyenMauViewModel.NgungTruyenHoi == null)
                {

                    var listYCTM = yctn.YeuCauTruyenMaus.Where(item => item.NhapKhoMauChiTiets.Any(p => p.Id == phieuTheoDoiTruyenMauViewModel.MaDonViMauTruyenId)).ToList();
                    foreach (var item in listYCTM)
                    {
                        item.NhanVienThucHienId = phieuTheoDoiTruyenMauViewModel.DDTruyenMauId;
                        item.ThoiDiemThucHien = phieuTheoDoiTruyenMauViewModel.BatDauTruyenHoi;
                        item.TrangThai = Enums.EnumTrangThaiYeuCauTruyenMau.ChuaThucHien;
                        item.ThoiDiemHoanThanh = phieuTheoDoiTruyenMauViewModel.NgungTruyenHoi;
                    }

                    update.YeuCauTiepNhan = yctn;

                }
                else
                {
                    var listYCTM = yctn.YeuCauTruyenMaus.Where(item => item.NhapKhoMauChiTiets.Any(p => p.Id == phieuTheoDoiTruyenMauViewModel.MaDonViMauTruyenId)).ToList();
                    foreach (var item in listYCTM)
                    {
                        item.TrangThai = Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien;
                        item.ThoiDiemHoanThanh = phieuTheoDoiTruyenMauViewModel.NgungTruyenHoi;
                        item.NhanVienThucHienId = phieuTheoDoiTruyenMauViewModel.DDTruyenMauId;
                        item.ThoiDiemThucHien = phieuTheoDoiTruyenMauViewModel.BatDauTruyenHoi;
                    }
                    update.YeuCauTiepNhan = yctn;
                }
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        #endregion
        [HttpPost("GetThongTinDefaultPhieuTheoDoiTruyenMauCreate")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ThongTinDefaultPhieuTheoDoiTruyenMauCreate GetThongTinDefaultPhieuTheoDoiTruyenMauCreate(long yeuCauTiepNhanId)
        {
            var lookup = _dieuTriNoiTruService.GetThongTinDefaultPhieuTheoDoiTruyenMauCreate(yeuCauTiepNhanId);
            return lookup;
        }
        #region In 
        [HttpPost("InPhieuTheoDoiTruyenMau")]
        public async Task<ActionResult<string>> InPhieuTheoDoiTruyenMau([FromBody]XacNhanInPhieuTheoDoiTruyenMau xacNhanIn)
        {
            var html = await _dieuTriNoiTruService.InPhieuTheoDoiTruyenMau(xacNhanIn);
            return html;
        }
        #endregion
    }
}
