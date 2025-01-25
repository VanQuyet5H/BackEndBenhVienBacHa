using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpGet("GetGiay15Ngay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<PhieuSoKet15NgayDieuTriNewViewModel> GetGiay15Ngay(long yeuCauTiepNhanId)
        {
            var giay15NgayEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri);

            if (giay15NgayEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(giay15NgayEntity.Id);
                var giay15s = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriNewViewModel>(giay15NgayEntity.ThongTinHoSo);
                giay15s.INoiTruHoSoKhacId = giay15NgayEntity.Id;

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (giay15s.FileChuKy != null && giay15s.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        giay15s.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        giay15s.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }

                return giay15s;
            }

            var giay15 = new PhieuSoKet15NgayDieuTriNewViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giay15.NgayThucHienText = DateTime.Now.ApplyFormatDateTime();
            giay15.NgayThucHien = DateTime.Now;
            giay15.NguoiThucHienText = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            giay15.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien15Ngay(yeuCauTiepNhanId);
            giay15.NhanVienTrongBVHayNgoaiBV = true;
            giay15.NhanVienTrongBVHayNgoaiBVTruongKhoa = true;
            return giay15;
        }
        [HttpGet("GetGiay15NgayNew")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<PhieuSoKet15NgayDieuTriNewViewModel> GetGiay15NgayNew(long yeuCauTiepNhanId)
        {
            var giay15 = new PhieuSoKet15NgayDieuTriNewViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giay15.NgayThucHienText = DateTime.Now.ApplyFormatDateTime();
            giay15.NguoiThucHienText = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            giay15.NgayThucHien = DateTime.Now;
            giay15.ChanDoan = _dieuTriNoiTruService.GetChanDoanVaoVien15Ngay(yeuCauTiepNhanId);
            giay15.NhanVienTrongBVHayNgoaiBV = true;
            giay15.NhanVienTrongBVHayNgoaiBVTruongKhoa = true;
            return giay15;
        }
        [HttpGet("GetGiay15NgayView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<PhieuSoKet15NgayDieuTriNewViewModel> GetGiay15NgayView(long id)
        {
            var giay15NgayEntity = await _dieuTriNoiTruService.GetNoiTruHoSoKhac15Ngay(id, Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri);

            if (giay15NgayEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(giay15NgayEntity.Id);
                var giay15s = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriNewViewModel>(giay15NgayEntity.ThongTinHoSo);
                giay15s.INoiTruHoSoKhacId = giay15NgayEntity.Id;

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (giay15s.FileChuKy != null && giay15s.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        giay15s.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        giay15s.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }

                return giay15s;
            }
            return new PhieuSoKet15NgayDieuTriNewViewModel() ;
        }
        [HttpGet("GetDanhSach15Ngay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<List<DsPhieu15Ngay>> GetDanhSach15Ngay(long id)
        {
            var dsPhieu15Ngays = new List<DsPhieu15Ngay>();
            var giay15NgayEntitys = await _dieuTriNoiTruService.GetNoiTruHoSoKhac15Ngays(id, Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri);
            if(giay15NgayEntitys != null)
            {
                foreach (var item in giay15NgayEntitys)
                {
                    var dsPhieu15Ngay = new DsPhieu15Ngay();
                    var giay15s = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriNewViewModel>(item.ThongTinHoSo);

                    if(!string.IsNullOrEmpty(giay15s.TuNgayString))
                    {
                        DateTime ngay = DateTime.Now;
                        DateTime.TryParseExact(giay15s.TuNgayString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);

                        dsPhieu15Ngay.TuNgay = ngay.ApplyFormatDate();
                    }
                    if (!string.IsNullOrEmpty(giay15s.DenNgayString))
                    {
                        DateTime ngay = DateTime.Now;
                        DateTime.TryParseExact(giay15s.DenNgayString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);

                        dsPhieu15Ngay.DenNgay = ngay.ApplyFormatDate();
                    }
                    dsPhieu15Ngay.NoiTruHoSoKhacId = item.Id;

                    dsPhieu15Ngays.Add(dsPhieu15Ngay);
                }
            }

            return dsPhieu15Ngays;
        }

        [HttpPost("UpdateGiay15Ngay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> Giay15Ngay([FromBody] PhieuSoKet15NgayDieuTriNewViewModel viewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            viewModel.NgayThucHienText = DateTime.Now.ApplyFormatDateTime();
            viewModel.NguoiThucHienText = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(viewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (viewModel.INoiTruHoSoKhacId != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(viewModel.INoiTruHoSoKhacId.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (viewModel.FileChuKy != null && viewModel.FileChuKy.Any())
                {
                    foreach (var file in viewModel.FileChuKy)
                    {
                        if (noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.All(w => w.TenGuid != file.TenGuid))
                        {
                            var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                            {
                                Id = 0,
                                Ten = file.Ten,
                                DuongDan = file.DuongDan,
                                Ma = file.Uid,
                                KichThuoc = file.KichThuoc,
                                TenGuid = file.TenGuid,
                                NoiTruHoSoKhacId = noiTruHoSoKhac.Id,
                                LoaiTapTin = file.LoaiTapTin.GetValueOrDefault()
                            };
                            noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileChuKy);
                            await _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileChuKy.DuongDan, noiTruHoSoKhacFileChuKy.TenGuid);
                        }
                    }
                }
                if (noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems != null && noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.Any())
                {
                    foreach (var noiTruHoSoKhacKemFileEntity in noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems)
                    {
                        if (viewModel.FileChuKy != null && viewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                viewModel.CheckCreateOrCapNhat = true; // true là cập nhat
                return Ok(viewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (viewModel.FileChuKy.Any())
            {
                foreach (var file in viewModel.FileChuKy)
                {
                    var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                    {
                        Id = 0,
                        Ma = file.Uid,
                        Ten = file.Ten,
                        DuongDan = file.DuongDan,
                        KichThuoc = file.KichThuoc,
                        TenGuid = file.TenGuid,
                        NoiTruHoSoKhacId = noiTruHoSoKhacNew.Id,
                        LoaiTapTin = file.LoaiTapTin.GetValueOrDefault()
                    };
                    noiTruHoSoKhacNew.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileChuKy);
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileChuKy.DuongDan, noiTruHoSoKhacFileChuKy.TenGuid);
                }
            }

            await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);
            viewModel.INoiTruHoSoKhacId = noiTruHoSoKhacNew.Id;
            viewModel.CheckCreateOrCapNhat = false; // false là tao moi
            return Ok(viewModel);
        }

        #region xóa sơ kết
        [HttpPost("xoaSoKet")]
        public async Task<ActionResult> Delete(long id)
        {
            var soket = await _noiDuTruHoSoKhacService.GetByIdAsync(id,s=>s.Include(k=>k.NoiTruHoSoKhacFileDinhKems));
            if (soket == null)
            {
                return NotFound();
            }
            await _noiDuTruHoSoKhacService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion
        #region In 
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInPhieu15Ngay")]
        public async Task<ActionResult> PhieuInPhieu15Ngay(PhieuDieuTriVaServicesHttpParams15Ngay dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _dieuTriNoiTruService.PhieuSoKet15NgayDieuTriUpdate(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
        #endregion

        #region check BA còn hạn sử dụng phiếu sơ kết        
        [HttpGet("KiemTraTaoPhieuSoKetConHanSuDung")]
        public async Task<bool> KiemTraTaoPhieuSoKetConHanSuDung(long yeuCauTiepNhanId)
        {
            var result = true;


            var yeuCauTiepNhan = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId, g => g.Include(d => d.NoiTruHoSoKhacs).Include(d => d.NoiTruBenhAn));


            var ngayVaoVien = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien;

            var phieuSoKets= yeuCauTiepNhan.NoiTruHoSoKhacs.Where(d => d.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri).Select(d => d.ThongTinHoSo).ToList();

            var listNgayThucHienPhieu = new List<DateTime>();
            foreach (var item in phieuSoKets)
            {
                var queryString = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriVo>(item);
                if(!string.IsNullOrEmpty(queryString.NgayThucHienString))
                {
                    DateTime ngay = DateTime.Now;
                    DateTime.TryParseExact(queryString.NgayThucHienString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);
                    listNgayThucHienPhieu.Add(ngay);
                }
            }

            var ngayCuoi = listNgayThucHienPhieu.OrderBy(d => d).ToList();





            if (phieuSoKets.Count() != 0)
            {
               if(ngayCuoi.Count() != 0)
                {
                    DateTime start = new DateTime(ngayCuoi.First().Year, ngayCuoi.First().Month, ngayCuoi.First().Day);

                    DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    TimeSpan difference = end - start;

                    // TH2 : ngày hiện tại - thời gian nhập viện  > 15 ngày=>  show text yêu cầu tạo phiếu sơ kết
                    if (difference.TotalDays > 15)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                // chưa có phiếu sơ kết
                DateTime start = new DateTime(ngayVaoVien.Year, ngayVaoVien.Month, ngayVaoVien.Day);

                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                TimeSpan difference = end - start; 

                // TH1 : ngày hiện tại - thời gian nhập viện  < 15 ngày => không show text yêu cầu tạo phiếu sơ kết
                // TH2 : ngày hiện tại - thời gian nhập viện  > 15 ngày=>  show text yêu cầu tạo phiếu sơ kết
                if (difference.TotalDays > 15)
                {
                    result = false;
                }
                
            }
            return result;
        }
        #endregion
    }
}
