using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpGet("GetDanhSachChoPhauThuatThuThuatHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChoPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var danhSachPhauThuatThuThuats = await _phauThuatThuThuatService.GetDanhSachChoPhauThuatThuThuatHienTaiAsync(phongKhamHienTaiId, searchString, yeuCauTiepNhanHienTaiId);
            return Ok(danhSachPhauThuatThuThuats);
        }

        [HttpGet("GetDanhSachDangPhauThuatThuThuatHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDangPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var danhSachPhauThuatThuThuats = await _phauThuatThuThuatService.GetDanhSachDangPhauThuatThuThuatHienTaiAsync(phongKhamHienTaiId, searchString, yeuCauTiepNhanHienTaiId);
            return Ok(danhSachPhauThuatThuThuats);
        }

        [HttpGet("GetDanhSachTheoDoiPhauThuatThuThuatHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachTheoDoiPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var danhSachPhauThuatThuThuats = await _phauThuatThuThuatService.GetDanhSachTheoDoiPhauThuatThuThuatHienTaiAsync(phongKhamHienTaiId, searchString, yeuCauTiepNhanHienTaiId);
            return Ok(danhSachPhauThuatThuThuats);
        }

        [HttpGet("GetTrangThaiPhauThuatThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<EnumTrangThaiPhauThuatThuThuat>> GetTrangThaiPhauThuatThuThuat(long phongKhamHienTaiId, long yeuCauTiepNhanId)
        {
            return await _phauThuatThuThuatService.GetTrangThaiPhauThuatThuThuat(yeuCauTiepNhanId, phongKhamHienTaiId);
        }

        [HttpGet("GetThongTinBenhNhanDangTuongTrinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<PhauThuatThuThuatThongTinBenhNhanViewModel>> GetThongTinBenhNhanDangTuongTrinh(long phongKhamHienTaiId)
        {
            //var isCoBenhNhanKhacDangKham = await _phauThuatThuThuatService.KiemTraCoBenhNhanKhacDangKhamTrongPhong(0, phongKhamHienTaiId);

            var benhNhanDangTuongTrinh = await _phauThuatThuThuatService.GetThongTinBenhNhanDangTuongTrinh(phongKhamHienTaiId);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (benhNhanDangTuongTrinh != null)
            {
                if(benhNhanDangTuongTrinh.NoiTruBenhAn != null || benhNhanDangTuongTrinh.QuyetToanTheoNoiTru == true)
                {
                    //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(benhNhanDangTuongTrinh.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                    var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(benhNhanDangTuongTrinh.Id).Result;

                    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(benhNhanDangTuongTrinh.Id);
                    soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
                }
                else
                {
                    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(benhNhanDangTuongTrinh.Id);
                    soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(benhNhanDangTuongTrinh.Id);
                }
            }

            var resultViewModel = benhNhanDangTuongTrinh?.ToModel<PhauThuatThuThuatThongTinBenhNhanViewModel>();

            #region BVHD-3882
            if (benhNhanDangTuongTrinh?.HinhThucDenId != null)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);
                resultViewModel.LaHinhThucDenGioiThieu = benhNhanDangTuongTrinh.HinhThucDenId == hinhThucDenGioiThieuId;
            }
            #endregion

            if (resultViewModel != null)
            {
                resultViewModel.TrangThaiPhauThuatThuThuat = await _phauThuatThuThuatService.GetTrangThaiPhauThuatThuThuat(resultViewModel.YeuCauTiepNhanId, phongKhamHienTaiId);
                resultViewModel.SoDuTaiKhoan = soDuTk;
                resultViewModel.SoDuTaiKhoanUocLuongConLai = soDuUocLuongConLai;
                //resultViewModel.TrangThaiDichVuKyThuat = trangThai;

                //update tường trình lại
                var phongBenhVienHangDoiTuongTrinhLai = await _phauThuatThuThuatService.GetPhongBenhVienHangDoiTuongTrinhLai(resultViewModel.YeuCauTiepNhanId, phongKhamHienTaiId);
                resultViewModel.IsTuongTrinhLai = phongBenhVienHangDoiTuongTrinhLai.Any();
                resultViewModel.IsTuongTrinhLaiCoTheoDoi = phongBenhVienHangDoiTuongTrinhLai.Any(p => p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                                                                                                      p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat.ThoiDiemBatDauTheoDoi != null &&
                                                                                                      p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi != null &&
                                                                                                      _phauThuatThuThuatService.IsPhauThuat(p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId));
                resultViewModel.IsTuongTrinhLaiCoKetLuan = phongBenhVienHangDoiTuongTrinhLai.Any(p => p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                                                                                                      _phauThuatThuThuatService.IsPhauThuat(p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId));
                //resultViewModel.IsTuongTrinhLaiCoTheoDoi = phongBenhVienHangDoiTuongTrinhLai.Any(p => p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null && _phauThuatThuThuatService.IsPhauThuat(p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId));
            }

            #region cập nhật 12/12/2022 hiển thị gói dịch vụ
            if (benhNhanDangTuongTrinh != null)
            {
                //if (benhNhanDangTuongTrinh!=null && ((benhNhanDangTuongTrinh.BenhNhan.YeuCauGoiDichVus.Any() && benhNhanDangTuongTrinh.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
                // || (benhNhanDangTuongTrinh.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && benhNhanDangTuongTrinh.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))))
                //{
                //    resultViewModel.CoDichVuKhuyenMai = true;
                //}
                var kiemTraGoi = _yeuCauKhamBenhService.KiemTraCoGoiVaKhuyenMaiTheoNguoiBenhId(benhNhanDangTuongTrinh.BenhNhanId ?? 0);
                var nguoiBenhCoGoi = kiemTraGoi.Item1;
                resultViewModel.CoDichVuKhuyenMai = kiemTraGoi.Item2;


                //get thông tin gói marketing của người bệnh
                //if (benhNhanDangTuongTrinh != null && (benhNhanDangTuongTrinh.BenhNhan.YeuCauGoiDichVus.Any() || benhNhanDangTuongTrinh.BenhNhan.YeuCauGoiDichVuSoSinhs.Any()))
                if (nguoiBenhCoGoi)
                {
                    var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(new QueryInfo
                    {
                        AdditionalSearchString = $"{benhNhanDangTuongTrinh.BenhNhanId} ; false",
                        Take = Int32.MaxValue
                    });

                    var lstGoi = gridData.Data.Select(p => (GoiDichVuTheoBenhNhanGridVo)p).ToList();
                    if (lstGoi.Any())
                    {
                        resultViewModel.GoiDichVus = lstGoi;
                    }
                }
            }
            #endregion

            #region BVHD-3941
            if (resultViewModel != null && benhNhanDangTuongTrinh?.CoBHTN == true)
            {
                resultViewModel.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(benhNhanDangTuongTrinh.Id).Result;
            }
            #endregion
            #region Cập nhật 07/06/2022: Cập nhật hiển thị thời điểm tiếp nhận. 1. Lấy thời điểm tiếp nhận ngoại trú; 2. YCTN Nội trú nếu không có tiếp nhận ngoại trú thì lấy thời điểm tạo BA
            if (resultViewModel != null && benhNhanDangTuongTrinh?.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                if (benhNhanDangTuongTrinh.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                {
                    var yeuCauTiepNhanNgoaiTru = _yeuCauTiepNhanService.GetById(benhNhanDangTuongTrinh.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                    resultViewModel.NgayTN = yeuCauTiepNhanNgoaiTru.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH();
                }
                else
                {
                    resultViewModel.NgayTN = benhNhanDangTuongTrinh.NoiTruBenhAn.ThoiDiemTaoBenhAn.ApplyFormatDateTimeSACH();
                }
            }
            #endregion

            return resultViewModel;
        }

        [HttpGet("GetThongTinBenhNhanTheoYeuCauTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<PhauThuatThuThuatThongTinBenhNhanViewModel> GetThongTinBenhNhanTheoYeuCauDichTiepNhan(long yeuCauTiepNhanId, long phongKhamHienTai)
        {
            var result = await _phauThuatThuThuatService.GetThongTinBenhNhanTheoYeuCauTiepNhan(yeuCauTiepNhanId);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            //if (result != null)
            //{
            //    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanId);
            //    soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanId);
            //}

            if (result != null)
            {
                if (result.NoiTruBenhAn != null || result.QuyetToanTheoNoiTru == true)
                {
                    //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(result.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                    var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(result.Id).Result;

                    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(result.Id);
                    soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
                }
                else
                {
                    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(result.Id);
                    soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(result.Id);
                }
            }

            var resultViewModel = result?.ToModel<PhauThuatThuThuatThongTinBenhNhanViewModel>();

            #region BVHD-3882
            if (result?.HinhThucDenId != null)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);
                resultViewModel.LaHinhThucDenGioiThieu = result.HinhThucDenId == hinhThucDenGioiThieuId;
            }
            #endregion

            if (resultViewModel != null)
            {
                resultViewModel.TrangThaiPhauThuatThuThuat = await _phauThuatThuThuatService.GetTrangThaiPhauThuatThuThuat(yeuCauTiepNhanId, phongKhamHienTai);
                resultViewModel.SoDuTaiKhoan = soDuTk;
                resultViewModel.SoDuTaiKhoanUocLuongConLai = soDuUocLuongConLai;
                //resultViewModel.TrangThaiDichVuKyThuat = trangThai;

                //update tường trình lại
                var phongBenhVienHangDoiTuongTrinhLai = await _phauThuatThuThuatService.GetPhongBenhVienHangDoiTuongTrinhLai(resultViewModel.YeuCauTiepNhanId, phongKhamHienTai);
                resultViewModel.IsTuongTrinhLai = phongBenhVienHangDoiTuongTrinhLai.Any();
                resultViewModel.IsTuongTrinhLaiCoTheoDoi = phongBenhVienHangDoiTuongTrinhLai.Any(p => p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                                                                                                      p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat.ThoiDiemBatDauTheoDoi != null &&
                                                                                                      p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi != null &&
                                                                                                      _phauThuatThuThuatService.IsPhauThuat(p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId));
                resultViewModel.IsTuongTrinhLaiCoKetLuan = phongBenhVienHangDoiTuongTrinhLai.Any(p => p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                                                                                                      _phauThuatThuThuatService.IsPhauThuat(p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId));
                //resultViewModel.IsTuongTrinhLaiCoTheoDoi = phongBenhVienHangDoiTuongTrinhLai.Any(p => p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null && _phauThuatThuThuatService.IsPhauThuat(p.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId));
            }

            #region cập nhật 12/12/2022 hiển thị gói dịch vụ
            if (result != null)
            {
                //dich vu khuyen mai
                //if ((result.BenhNhan.YeuCauGoiDichVus.Any() && result.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                //                                                                          z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
                // || (result.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && result.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any())))
                //{
                //    resultViewModel.CoDichVuKhuyenMai = true;
                //}

                var kiemTraGoi = _yeuCauKhamBenhService.KiemTraCoGoiVaKhuyenMaiTheoNguoiBenhId(result.BenhNhanId ?? 0);
                var nguoiBenhCoGoi = kiemTraGoi.Item1;
                resultViewModel.CoDichVuKhuyenMai = kiemTraGoi.Item2;

                //get thông tin gói marketing của người bệnh
                //if (result != null && (result.BenhNhan.YeuCauGoiDichVus.Any() || result.BenhNhan.YeuCauGoiDichVuSoSinhs.Any()))
                if (nguoiBenhCoGoi)
                {
                    var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(new QueryInfo
                    {
                        AdditionalSearchString = $"{result.BenhNhanId} ; false",
                        Take = Int32.MaxValue
                    });

                    var lstGoi = gridData.Data.Select(p => (GoiDichVuTheoBenhNhanGridVo)p).ToList();
                    if (lstGoi.Any())
                    {
                        resultViewModel.GoiDichVus = lstGoi;
                    }
                }
            }
            #endregion

            #region BVHD-3941
            if (resultViewModel != null && result?.CoBHTN == true)
            {
                resultViewModel.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.Id).Result;
            }
            #endregion
            #region Cập nhật 07/06/2022: Cập nhật hiển thị thời điểm tiếp nhận. 1. Lấy thời điểm tiếp nhận ngoại trú; 2. YCTN Nội trú nếu không có tiếp nhận ngoại trú thì lấy thời điểm tạo BA
            if (resultViewModel != null && result?.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                if (result.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                {
                    var yeuCauTiepNhanNgoaiTru = _yeuCauTiepNhanService.GetById(result.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                    resultViewModel.NgayTN = yeuCauTiepNhanNgoaiTru.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH();
                }
                else
                {
                    resultViewModel.NgayTN = result.NoiTruBenhAn.ThoiDiemTaoBenhAn.ApplyFormatDateTimeSACH();
                }
            }
            #endregion

            return resultViewModel;
        }

        [HttpGet("GetThongTinBenhNhanTiepTheo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<PhauThuatThuThuatThongTinBenhNhanViewModel> GetThongTinBenhNhanTiepTheo(long phongKhamHienTaiId, long yeuCauTiepNhanHienTaiId)
        {
            var result = await _phauThuatThuThuatService.GetThongTinBenhNhanTiepTheo(phongKhamHienTaiId, yeuCauTiepNhanHienTaiId);

            decimal soDuTk = 0;

            if (result != null)
            {
                soDuTk = _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(result.Id).Result;
            }

            var resultViewModel = result?.ToModel<PhauThuatThuThuatThongTinBenhNhanViewModel>();

            if (resultViewModel != null)
            {
                resultViewModel.TrangThaiPhauThuatThuThuat = await _phauThuatThuThuatService.GetTrangThaiPhauThuatThuThuat(result.Id, phongKhamHienTaiId);
                resultViewModel.SoDuTaiKhoan = soDuTk;
            }

            return resultViewModel;
        }

        [HttpGet("KiemTraConYeuCauDichVuKyThuatTaiPhong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<bool> KiemTraConYeuCauDichVuKyThuatTaiPhong(long phongBenhVienId, long yeuCauTiepNhanId)
        {
            return await _phauThuatThuThuatService.KiemTraConYeuCauDichVuKyThuatTaiPhong(phongBenhVienId, yeuCauTiepNhanId);
        }

        [HttpPost("BatDauKhamBenhNhanPTTT")]
        public async Task<ActionResult> BatDauKhamBenhNhanPTTT([FromBody] HangDoiPhauThuatThuThuatInputViewModel hangDoiPhauThuatThuThuatInputViewModel)
        {
            await _phauThuatThuThuatService.BatDauKhamBenhNhanPTTT(hangDoiPhauThuatThuThuatInputViewModel.YeuCauTiepNhanDangKhamId ?? 0, hangDoiPhauThuatThuThuatInputViewModel.YeuCauTiepNhanBatDauKhamId, hangDoiPhauThuatThuThuatInputViewModel.PhongBenhVienId);

            return NoContent();
        }

        [HttpPost("HuyKhamBenhNhanPTTT")]
        public async Task<ActionResult> HuyKhamBenhNhanPTTT([FromBody] HangDoiPhauThuatThuThuatInputViewModel hangDoiPhauThuatThuThuatInputViewModel)
        {
            await _phauThuatThuThuatService.HuyKhamBenhNhanPTTT(hangDoiPhauThuatThuThuatInputViewModel.PhongBenhVienId);

            return NoContent();
        }

        [HttpGet("CoDuocHuongBHYT")]
        public async Task<bool> CoDuocHuongBHYT(long yeuCauDichVuKyThuatId)
        {
            return await _phauThuatThuThuatService.CoDuocHuongBHYT(yeuCauDichVuKyThuatId);
        }
    }
}
