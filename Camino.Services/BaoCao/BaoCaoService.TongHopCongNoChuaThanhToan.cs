using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoTongHopCongNoChuaThanhToanForGridAsync(QueryInfo queryInfo)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<BaoCaoTongHopCongNoChuaThanhToanGridVo>();
            var timKiemNangCaoObj = new BaoCaoTongHopCongNoChuaThanhToanTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTongHopCongNoChuaThanhToanTimKiemVo>(queryInfo.AdditionalSearchString);
                if (timKiemNangCaoObj.MaYeuCauTiepNhan == null 
                    || timKiemNangCaoObj.MaYeuCauTiepNhan.Contains("undefined") 
                    || timKiemNangCaoObj.MaYeuCauTiepNhan == "0")
                {
                    timKiemNangCaoObj.MaYeuCauTiepNhan = null;
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if ((tuNgay != null && denNgay != null) || !string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan))
            {
                var tenDichVuCovid = "SARS-CoV";
                var tenDichVuGiamDau = "[GD]";
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                if (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                        .Where(x => 
                                    //BVHD-3917
                                    ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                        || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru))
                                    && ((!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) 
                                         && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                        || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) 
                                            && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)

                                            //BVHD-3917
                                            && ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true 
                                                        && x.ThoiDiemTiepNhan >= tuNgay && x.ThoiDiemTiepNhan <= denNgay))
                                                || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true 
                                                        && x.NoiTruBenhAn.ThoiDiemRaVien != null && x.NoiTruBenhAn.ThoiDiemRaVien.Value >= tuNgay && x.NoiTruBenhAn.ThoiDiemRaVien <= denNgay))
                                            )
                                    && (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && x.BenhNhanId != null)
                        .Select(x => new BaoCaoTongHopCongNoChuaThanhToanGridVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoTen = x.HoTen,
                            MaBN = x.BenhNhan.MaBN,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();
                }
                else
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                    .Where(x => x.NoiGioiThieuId != null

                                    //BVHD-3917
                                && ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                     || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru))

                                && ((!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan)
                                     && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)
                                        && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)
                                        //&& x.ThoiDiemTiepNhan >= tuNgay
                                        //&& x.ThoiDiemTiepNhan <= denNgay))

                                        //BVHD-3917
                                        && ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true
                                                        && x.ThoiDiemTiepNhan >= tuNgay && x.ThoiDiemTiepNhan <= denNgay))
                                            || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true
                                                        && x.NoiTruBenhAn.ThoiDiemRaVien != null && x.NoiTruBenhAn.ThoiDiemRaVien.Value >= tuNgay && x.NoiTruBenhAn.ThoiDiemRaVien <= denNgay))
                                            )

                                 && (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || (x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))
                                 && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                 && x.BenhNhanId != null)
                                 //&& x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                 //                                  && a.TaiKhoanBenhNhanChis.Any(b => b.DaHuy != true)))
                    .Select(x => new BaoCaoTongHopCongNoChuaThanhToanGridVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                        TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                        BenhNhanId = x.BenhNhanId.Value,
                        HoTen = x.HoTen,
                        MaBN = x.BenhNhan.MaBN,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                }
                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    //&& ((!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan)
                                    //     && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    //    || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)
                                    //        && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)
                                    //        && x.ThoiDiemTiepNhan >= tuNgay
                                    //        && x.ThoiDiemTiepNhan <= denNgay))
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                                    //&& x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                    //                                   && a.TaiKhoanBenhNhanChis.Any(b => b.DaHuy != true)))
                        .Select(x => new BaoCaoTongHopCongNoChuaThanhToanGridVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoTen = x.HoTen,
                            MaBN = x.BenhNhan.MaBN,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,

                            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                            LaDataTheoDieuKienTimKiem = (!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) 
                                                            && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                                        || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) 
                                                                && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) 
                                                                && x.ThoiDiemTiepNhan >= tuNgay 
                                                                && x.ThoiDiemTiepNhan <= denNgay)
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

                    //todo: update nơi giới thiệu
                    foreach (var benhNhanId in lstBenhNhanId)
                    {
                        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                            .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        if (lanTiepNhanDauTienCoGioiThieu == null)
                        {
                            lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                                .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                                .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();

                            if (lanTiepNhanDauTienCoGioiThieu == null)
                            {
                                continue;
                            }
                        }

                        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId
                                        && x.BenhNhanId == benhNhanId)
                            .ToList();

                        var khongThemNguoiGioiThieu = false;
                        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuId;
                        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuDisplay;
                        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                        {
                            if (lanTiepNhan.NoiGioiThieuId != null)
                            {
                                if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId) 
                                    || ((timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0) && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId))
                                {
                                    // trường hợp tìm kiếm theo người giới thiệu thì chỉ thêm người giới thiệu đang tìm kiếm thôi
                                    if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId))
                                    {
                                        khongThemNguoiGioiThieu = true;
                                    }
                                    else
                                    {
                                        khongThemNguoiGioiThieu = false;
                                    }
                                    nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                                    tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                                }
                                else
                                {
                                    khongThemNguoiGioiThieu = false;
                                }
                            }

                            if (!khongThemNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null)
                            {
                                if (lstTiepNhanTheoNoiGioiThieu.All(x => x.YeucauTiepNhanId != lanTiepNhan.YeucauTiepNhanId) && lanTiepNhan.LaDataTheoDieuKienTimKiem == true)
                                {
                                    lanTiepNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                    lanTiepNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                    lanTiepNhan.LaGioiThieu = true;
                                    lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                                }
                                else
                                {
                                    var lanTiepNhanDaThem = lstTiepNhanTheoNoiGioiThieu.FirstOrDefault(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId);
                                    if (lanTiepNhanDaThem != null)
                                    {
                                        lanTiepNhanDaThem.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                        lanTiepNhanDaThem.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                        lanTiepNhanDaThem.LaGioiThieu = true;
                                    }

                                }
                            }
                        }
                    }

                    #region BVHD-3917 xử lý check trường hợp chọn ngoại trú -> chỉ lấy các YCTN ngoại trú của người bệnh mà không có nhập viện (chưa tạo bệnh án)
                    var lstYeuCauChiTiepNhanNgoaiTruId = new List<long>();
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        var lstMaTiepNhanQuery = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();
                        lstYeuCauChiTiepNhanNgoaiTruId = _yeuCauTiepNhanRepository.TableNoTracking
                            .Where(x => lstMaTiepNhanQuery.Contains(x.MaYeuCauTiepNhan))
                            .Select(x => new
                            {
                                YeuCauTiepNhanId = x.Id,
                                MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                                CoNoiTru = x.NoiTruBenhAn != null
                            })
                            .GroupBy(x => new { x.MaYeuCauTiepNhan })
                            .Where(x => !x.Any(a => a.CoNoiTru))
                            .Select(x => x.First().YeuCauTiepNhanId)
                            .Distinct().ToList();

                        lstTiepNhanTheoNoiGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeucauTiepNhanId)).ToList();
                    }

                    lstTiepNhanTheoNoiGioiThieu =
                        lstTiepNhanTheoNoiGioiThieu
                            .Distinct()
                            .OrderBy(x => x.ThoiDiemTiepNhan)
                            .Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                    var lstMaTiepNhan = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();

                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        lstYeuCauChiTiepNhanNgoaiTruId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                    }

                    #endregion

                    // tham khảo từ GetDataBaoCaoChiTietMienPhiTronVienForGridAsync

                    #region Code cũ
                    //var lstChiPhi = _taiKhoanBenhNhanThuRepository.TableNoTracking
                    //    .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                    //                && x.DaHuy != true
                    //                //&& x.YeuCauTiepNhan.BenhNhanId != null
                    //                && ((x.LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuTamUng && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true))
                    //                    || (x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.LoaiChiTienBenhNhan != Enums.LoaiChiTienBenhNhan.HoanUng))))
                    //    .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                    //    {
                    //        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        ChiPhi = item.TienMat.GetValueOrDefault() + item.ChuyenKhoan.GetValueOrDefault() + item.POS.GetValueOrDefault() + item.CongNo.GetValueOrDefault() + item.TamUng.GetValueOrDefault(),
                    //        LaBenhNhanThu = true
                    //    })
                    //    .Union(
                    //        _yeuCauKhamBenhRepository.TableNoTracking
                    //        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                    //                    && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                    //        {
                    //            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //            ChiPhi = item.KhongTinhPhi != true ? ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                    //            TenDichVu = item.TenDichVu,
                    //            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    //            DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham,
                    //            LaNoiTru = false
                    //        })
                    //    ).Union(
                    //        _yeuCauDichVuKyThuatRepository.TableNoTracking
                    //        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                    //                    && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                    //        {
                    //            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //            ChiPhi = item.KhongTinhPhi != true ? (((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan) - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                    //            TenDichVu = item.TenDichVu,
                    //            LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    //            NoiThucHien = item.NoiThucHien.Ten,
                    //            NgayPhauThuat = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat,
                    //            ThoiGianGayMe = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe,
                    //            ThoiGianBanGiao = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat, //todo: có thể cần kiểm tra lại
                    //            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    //            DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                    //            LaNoiTru = item.NoiTruPhieuDieuTriId != null
                    //        })
                    //    )
                    //    .Union(
                    //        _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                    //        .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                    //        {
                    //            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //            TenDichVu = item.Ten,
                    //            ChiPhi = (Convert.ToDecimal(item.SoLuong * (double)(item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau ?? 0 : item.Gia))) - item.SoTienMienGiam.GetValueOrDefault(),
                    //            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                    //            DaThucHien = true
                    //        })
                    //    )
                    //    .Union(
                    //        _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    //        .Where(x => x.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                    //                    && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //            .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                    //            {
                    //                MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //                TenDichVu = item.Ten,
                    //                ChiPhi = item.KhongTinhPhi != true ? (item.GiaBan - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                    //                NhomDichVu = Enums.EnumNhomGoiDichVu.DuocPham,
                    //                DaThucHien = item.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                    //            })
                    //    )
                    //    .Union(
                    //        _yeuCauVatTuBenhVienRepository.TableNoTracking
                    //            .Where(x => x.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                    //                        && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                    //            .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                    //            {
                    //                MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //                TenDichVu = item.Ten,
                    //                ChiPhi = item.KhongTinhPhi != true ? (item.GiaBan - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                    //                NhomDichVu = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                    //                DaThucHien = item.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                    //            })
                    //    )
                    //    .ToList();
                    #endregion

                    #region Code cập nhật 08/04/2022

                    #region Chi phí người bệnh đã đóng
                    //var queryBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
                    //.Where(x => x.DaHuy != true
                    //            //&& x.YeuCauTiepNhan.BenhNhanId != null
                    //            && ((x.LoaiThuTienBenhNhan != Enums.LoaiThuTienBenhNhan.ThuTamUng && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true))
                    //                || (x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng && x.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true && a.LoaiChiTienBenhNhan != Enums.LoaiChiTienBenhNhan.HoanUng))));

                    //var queryBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
                    //        .Where(x => x.DaHuy != true);

                    IQueryable<TaiKhoanBenhNhanThu> queryBenhNhanThu = null;
                    //var taiKhoanChis = new List<ThongTinNguoiBenhChiVo>();

                    //BVHD-3917
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        //taiKhoanChis = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        //    .Where(x => x.DaHuy != true
                        //                && x.YeuCauTiepNhanId != null
                        //                && lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId.Value)
                        //                && x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi)
                        //    .Select(x => new ThongTinNguoiBenhChiVo()
                        //    {
                        //        YeuCauTiepNhanId = x.YeuCauTiepNhanId.Value,
                        //        ChiPhi = x.TienChiPhi
                        //    })
                        //    .Distinct().ToList();
                        //var lstTiepNhanIdCoThuTien = taiKhoanChis.Select(x => x.YeuCauTiepNhanId).Distinct().ToList();

                        queryBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(x =>
                            x.DaHuy != true && x.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan
                            && lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                            && ((x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi) // && lstTiepNhanIdCoThuTien.Contains(x.YeuCauTiepNhanId))
                                || (x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng && x.ThuTienGoiDichVu != true && x.PhieuHoanUngId == null)));
                    }
                    else
                    {
                        //taiKhoanChis = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        //    .Where(x => x.DaHuy != true
                        //                && x.YeuCauTiepNhanId != null
                        //                && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                        //                && x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi)
                        //    .Select(x => new ThongTinNguoiBenhChiVo()
                        //    {
                        //        YeuCauTiepNhanId = x.YeuCauTiepNhanId.Value,
                        //        ChiPhi = x.TienChiPhi
                        //    })
                        //    .Distinct().ToList();
                        //var lstTiepNhanIdCoThuTien = taiKhoanChis.Select(x => x.YeuCauTiepNhanId).Distinct().ToList();

                        queryBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(x =>
                            x.DaHuy != true && x.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan
                            && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                            && ((x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi) // && lstTiepNhanIdCoThuTien.Contains(x.YeuCauTiepNhanId))
                                || (x.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng && x.ThuTienGoiDichVu != true && x.PhieuHoanUngId == null)));
                    }

                    var lstChiPhi = queryBenhNhanThu
                        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                        {
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            //ChiPhi = item.TienMat.GetValueOrDefault() + item.ChuyenKhoan.GetValueOrDefault() + item.POS.GetValueOrDefault() + item.CongNo.GetValueOrDefault() + item.TamUng.GetValueOrDefault(),

                            //BVHD-3917: chỉ tính tiền mặt, chuyển khoản, pos và tạm ứng chưa hoàn ứng
                            ChiPhi = item.TienMat.GetValueOrDefault() + item.ChuyenKhoan.GetValueOrDefault() + item.POS.GetValueOrDefault(),
                            LaBenhNhanThu = true,

                            LaThuTheoChiPhi = item.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi,
                            //TongChiPhi = taiKhoanChis.Where(a => a.YeuCauTiepNhanId == item.YeuCauTiepNhanId).Sum(a => a.ChiPhi ?? 0),
                            CongNo = item.CongNo,
                            TienChuyenKhoan = item.ChuyenKhoan,
                            TienPOS = item.POS,
                            PhieuChis = item.TaiKhoanBenhNhanChis.Select(a => new ThongTinPhieuChiVo()
                            {
                                LoaiChiTienBenhNhan = a.LoaiChiTienBenhNhan,
                                TienChiPhi = a.TienChiPhi
                            }).ToList()
                        }).ToList();
                    #endregion

                    #region Chi phí khám
                    var queryChiPhiKham = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => 
                                        //x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                        x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);

                    //BVHD-3917
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        queryChiPhiKham = queryChiPhiKham.Where(x =>
                            lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId));
                    }
                    else
                    {
                        queryChiPhiKham = queryChiPhiKham.Where(x =>
                            lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan));
                    }

                    var lstChiPhiKham = queryChiPhiKham
                        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                        {
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ChiPhi = item.KhongTinhPhi != true ? ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                            TenDichVu = item.TenDichVu,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham,
                            LaNoiTru = false,

                            //BVHD-3917
                            SoLuong = 1,
                            DuocHuongBHYT = item.DuocHuongBaoHiem,
                            BaoHiemChiTra = item.BaoHiemChiTra,
                            DonGiaBHYT = item.DonGiaBaoHiem ?? 0,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan ?? 0,
                            MucHuongBaoHiem = item.MucHuongBaoHiem ?? 0
                        }).ToList();
                    if (lstChiPhiKham.Any())
                    {
                        lstChiPhi.AddRange(lstChiPhiKham);
                    }
                    #endregion

                    #region Chi phí kỹ thuật
                    var queryChiPhiKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => 
                                    //x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
                    var lstYeuCauKyThuatIdCoThuePhong = new List<long>();
                    //BVHD-3917
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        queryChiPhiKyThuat = queryChiPhiKyThuat.Where(x =>
                            lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId));
                        lstYeuCauKyThuatIdCoThuePhong = _thuePhongRepository.TableNoTracking
                            .Where(x => lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId))
                            .Select(x => x.YeuCauDichVuKyThuatTinhChiPhiId)
                            .Distinct().ToList();
                    }
                    else
                    {
                        queryChiPhiKyThuat = queryChiPhiKyThuat.Where(x =>
                            lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan));
                        lstYeuCauKyThuatIdCoThuePhong = _thuePhongRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(x => x.YeuCauDichVuKyThuatTinhChiPhiId)
                            .Distinct().ToList();
                    }

                    var lstChiPhiKyThuat = queryChiPhiKyThuat
                        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                        {
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ChiPhi = item.KhongTinhPhi != true ? (((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * item.SoLan) - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                            TenDichVu = item.TenDichVu,
                            LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                            NoiThucHien = item.NoiThucHien.Ten,
                            NgayPhauThuat = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat,
                            ThoiGianGayMe = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe,
                            ThoiGianBanGiao = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat, //todo: có thể cần kiểm tra lại
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                            DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                            LaNoiTru = item.NoiTruPhieuDieuTriId != null,

                            //BVHD-3917
                            CoThuePhong = lstYeuCauKyThuatIdCoThuePhong.Contains(item.Id),
                            YeuCauDichVuKyThuatChiPhiThuePhongId = item.Id,
                            SoLuong = item.SoLan,
                            DuocHuongBHYT = item.DuocHuongBaoHiem,
                            BaoHiemChiTra = item.BaoHiemChiTra,
                            DonGiaBHYT = item.DonGiaBaoHiem ?? 0,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan ?? 0,
                            MucHuongBaoHiem = item.MucHuongBaoHiem ?? 0
                        }).ToList();

                    #region BVHD-3917 get thông tin thuê phòng theo dịch vụ PTTT
                    var lstYeuCauDichVuKyThuatChiPhiThuePhongId = lstChiPhiKyThuat.Where(x => x.CoThuePhong)
                        .Select(x => x.YeuCauDichVuKyThuatChiPhiThuePhongId.Value).Distinct().ToList();
                    if (lstYeuCauDichVuKyThuatChiPhiThuePhongId.Any())
                    {
                        var thuePhongs = _thuePhongRepository.TableNoTracking
                            .Where(x => lstYeuCauDichVuKyThuatChiPhiThuePhongId.Contains(x.YeuCauDichVuKyThuatTinhChiPhiId))
                            .Select(x => new ChiPhiThuePhongVo()
                            {
                                YeuCauDichVuKyThuatThuePhongId = x.YeuCauDichVuKyThuatThuePhongId,
                                YeuCauDichVuKyThuatChiPhiThuePhongId = x.YeuCauDichVuKyThuatTinhChiPhiId,
                                GiaThuePhong = x.YeuCauDichVuKyThuatTinhChiPhi.Gia,
                                BatDauThuePhong = x.ThoiDiemBatDau,
                                KetThucThuePhong = x.ThoiDiemKetThuc
                            }).ToList();
                        foreach (var chiPhiKyThuat in lstChiPhiKyThuat)
                        {
                            if (chiPhiKyThuat.CoThuePhong && chiPhiKyThuat.YeuCauDichVuKyThuatChiPhiThuePhongId != null)
                            {
                                var thuePhong = thuePhongs.FirstOrDefault(x => x.YeuCauDichVuKyThuatChiPhiThuePhongId == chiPhiKyThuat.YeuCauDichVuKyThuatChiPhiThuePhongId);
                                if (thuePhong != null)
                                {
                                    chiPhiKyThuat.ChiPhiThuePhong = thuePhong;
                                }
                            }
                        }
                    }
                    #endregion


                    if (lstChiPhiKyThuat.Any())
                    {
                        lstChiPhi.AddRange(lstChiPhiKyThuat);
                    }
                    #endregion

                    #region Chi phí giường
                    //var queryChiPhiGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                    //    .Where(x =>
                    //        //BVHD-3917
                    //        ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true && lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId))
                    //         || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))));

                    IQueryable<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> queryChiPhiGiuong = null;
                    //BVHD-3917
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        queryChiPhiGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                            .Where(x => lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId));
                    }
                    else
                    {
                        queryChiPhiGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan));
                    }

                    var lstChiPhiGiuong = queryChiPhiGiuong
                        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                        {
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenDichVu = item.Ten,
                            ChiPhi = (Convert.ToDecimal(item.SoLuong * (double)(item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau ?? 0 : item.Gia))) - item.SoTienMienGiam.GetValueOrDefault(),
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                            DaThucHien = true,
                            ChiPhiBHYTDichVuGiuongs = item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Select(a => new ThongTinChiPhiBHYTDichVuVo()
                            {
                                DuocHuongBaoHiem = a.DuocHuongBaoHiem,
                                BaoHiemChiTra = a.BaoHiemChiTra,
                                DonGiaBaoHiem = a.DonGiaBaoHiem,
                                MucHuongBaoHiem = a.MucHuongBaoHiem,
                                TiLeBaoHiemThanhToan = a.TiLeBaoHiemThanhToan
                            }).ToList(),
                            SoLuong = item.SoLuong
                        }).ToList();
                    if (lstChiPhiGiuong.Any())
                    {
                        foreach (var chiPhiGiuong in lstChiPhiGiuong)
                        {
                            if (chiPhiGiuong.ChiPhiBHYTDichVuGiuongs.Any())
                            {
                                var bhyt = chiPhiGiuong.ChiPhiBHYTDichVuGiuongs.First();
                                chiPhiGiuong.DuocHuongBHYT = bhyt.DuocHuongBaoHiem;
                                chiPhiGiuong.BaoHiemChiTra = bhyt.BaoHiemChiTra;
                                chiPhiGiuong.DonGiaBHYT = bhyt.DonGiaBaoHiem.GetValueOrDefault();
                                chiPhiGiuong.MucHuongBaoHiem = bhyt.MucHuongBaoHiem.GetValueOrDefault();
                                chiPhiGiuong.TiLeBaoHiemThanhToan = bhyt.TiLeBaoHiemThanhToan.GetValueOrDefault();
                            }
                        }

                        lstChiPhi.AddRange(lstChiPhiGiuong);
                    }
                    #endregion

                    #region Chi phí dược phẩm
                    var queryChiPhiDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(x =>
                                    //x.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                    x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy);

                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        queryChiPhiDuocPham = queryChiPhiDuocPham
                            .Where(x => lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId));
                    }
                    else
                    {
                        queryChiPhiDuocPham = queryChiPhiDuocPham
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan));
                    }

                    var lstChiPhiDuocPham = queryChiPhiDuocPham
                        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                        {
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenDichVu = item.Ten,
                            //ChiPhi = item.KhongTinhPhi != true ? (item.GiaBan - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DuocPham,
                            DaThucHien = item.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien,

                            //BVHD-3917
                            SoLuong = item.SoLuong,
                            DuocHuongBHYT = item.DuocHuongBaoHiem,
                            BaoHiemChiTra = item.BaoHiemChiTra,
                            DonGiaBHYT = item.DonGiaBaoHiem ?? 0,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan ?? 0,
                            MucHuongBaoHiem = item.MucHuongBaoHiem ?? 0,

                            // xử lý tự tính giá bán
                            LaTuTinhGiaBan = true,
                            XuatKhoChiTietId = item.XuatKhoDuocPhamChiTietId,
                            KhongTinhPhi = item.KhongTinhPhi,
                            SoTienMienGiam = item.SoTienMienGiam,
                            DonGiaNhap = item.DonGiaNhap,
                            VAT = item.VAT,
                            TiLeTheoThapGia = item.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKhos = (item.KhongTinhPhi != true && item.XuatKhoDuocPhamChiTietId != null)
                                ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                        }).ToList();
                    if (lstChiPhiDuocPham.Any())
                    {
                        //var lstXuatKhoChiTietId = lstChiPhiDuocPham
                        //    .Where(x => x.KhongTinhPhi != true && x.XuatKhoChiTietId != null)
                        //    .Select(x => x.XuatKhoChiTietId.Value)
                        //    .ToList();
                        //if (lstXuatKhoChiTietId.Any())
                        //{
                        //    var lstPhuongPhapTinhGiaTriTonKho = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                        //        .Where(x => lstXuatKhoChiTietId.Contains(x.XuatKhoDuocPhamChiTietId))
                        //        .Select(x => new PhuongPhapTinhGiaTriTonKhoTheoXuatVo()
                        //        {
                        //            XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId,
                        //            PhuongPhapTinhGiaTriTonKho = x.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho
                        //        })
                        //        .Distinct()
                        //        .ToList();
                        //    if (lstPhuongPhapTinhGiaTriTonKho.Any())
                        //    {
                        //        foreach (var chiPhiDuocPham in lstChiPhiDuocPham)
                        //        {
                        //            if (chiPhiDuocPham.KhongTinhPhi != true && chiPhiDuocPham.XuatKhoChiTietId != null)
                        //            {
                        //                var lstPhuongPhapTinhGiaTriTonKhoTheoXuat = lstPhuongPhapTinhGiaTriTonKho
                        //                    .Where(x => x.XuatKhoChiTietId == chiPhiDuocPham.XuatKhoChiTietId)
                        //                    .Select(x => x.PhuongPhapTinhGiaTriTonKho)
                        //                    .Distinct()
                        //                    .ToList();
                        //                chiPhiDuocPham.PhuongPhapTinhGiaTriTonKhos = lstPhuongPhapTinhGiaTriTonKhoTheoXuat;
                        //            }
                        //        }
                        //    }
                        //}

                        lstChiPhi.AddRange(lstChiPhiDuocPham);
                    }
                    #endregion

                    #region Chi phí vật tư
                    var queryChiPhiVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x =>
                                    //x.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                                    x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy);
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        queryChiPhiVatTu = queryChiPhiVatTu
                            .Where(x => lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId));
                    }
                    else
                    {
                        queryChiPhiVatTu = queryChiPhiVatTu
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan));
                    }

                    var lstChiPhiVatTu = queryChiPhiVatTu
                        .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                        {
                            MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TenDichVu = item.Ten,
                            //ChiPhi = item.KhongTinhPhi != true ? (item.GiaBan - item.SoTienMienGiam.GetValueOrDefault()) : 0,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                            DaThucHien = item.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien,

                            //BVHD-3917
                            SoLuong = item.SoLuong,
                            DuocHuongBHYT = item.DuocHuongBaoHiem,
                            BaoHiemChiTra = item.BaoHiemChiTra,
                            DonGiaBHYT = item.DonGiaBaoHiem ?? 0,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan ?? 0,
                            MucHuongBaoHiem = item.MucHuongBaoHiem ?? 0,

                            // xử lý tự tính giá bán
                            LaTuTinhGiaBan = true,
                            XuatKhoChiTietId = item.XuatKhoVatTuChiTietId,
                            KhongTinhPhi = item.KhongTinhPhi,
                            SoTienMienGiam = item.SoTienMienGiam,
                            DonGiaNhap = item.DonGiaNhap,
                            VAT = item.VAT,
                            TiLeTheoThapGia = item.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKhos = (item.KhongTinhPhi != true && item.XuatKhoVatTuChiTietId != null)
                                ? item.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Select(a => a.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                        }).ToList();
                    if (lstChiPhiVatTu.Any())
                    {
                        //var lstXuatKhoChiTietId = lstChiPhiVatTu
                        //    .Where(x => x.KhongTinhPhi != true && x.XuatKhoChiTietId != null)
                        //    .Select(x => x.XuatKhoChiTietId.Value)
                        //    .ToList();
                        //if (lstXuatKhoChiTietId.Any())
                        //{
                        //    var lstPhuongPhapTinhGiaTriTonKho = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                        //        .Where(x => lstXuatKhoChiTietId.Contains(x.XuatKhoVatTuChiTietId))
                        //        .Select(x => new PhuongPhapTinhGiaTriTonKhoTheoXuatVo()
                        //        {
                        //            XuatKhoChiTietId = x.XuatKhoVatTuChiTietId,
                        //            PhuongPhapTinhGiaTriTonKho = x.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho
                        //        })
                        //        .Distinct()
                        //        .ToList();
                        //    if (lstPhuongPhapTinhGiaTriTonKho.Any())
                        //    {
                        //        foreach (var chiPhiVatTu in lstChiPhiVatTu)
                        //        {
                        //            if (chiPhiVatTu.KhongTinhPhi != true && chiPhiVatTu.XuatKhoChiTietId != null)
                        //            {
                        //                var lstPhuongPhapTinhGiaTriTonKhoTheoXuat = lstPhuongPhapTinhGiaTriTonKho
                        //                    .Where(x => x.XuatKhoChiTietId == chiPhiVatTu.XuatKhoChiTietId)
                        //                    .Select(x => x.PhuongPhapTinhGiaTriTonKho)
                        //                    .Distinct()
                        //                    .ToList();
                        //                chiPhiVatTu.PhuongPhapTinhGiaTriTonKhos = lstPhuongPhapTinhGiaTriTonKhoTheoXuat;
                        //            }
                        //        }
                        //    }
                        //}

                        lstChiPhi.AddRange(lstChiPhiVatTu);
                    }
                    #endregion

                    #region Cập nhật 13/07/2022: Chi phí truyền máu
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true)
                    {
                        var lstChiPhiTruyenMau = _yeuCauTruyenMauRepository.TableNoTracking
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy
                                        && lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(item => new BaoCaoTongHopCongNoChuaThanhToanChiPhiTungDichVuVo()
                            {
                                MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                ChiPhi = item.DonGiaBan,
                                TenDichVu = item.TenDichVu,
                                NhomDichVu = Enums.EnumNhomGoiDichVu.TruyenMau,
                                DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien,
                                LaNoiTru = true,

                                SoLuong = 1,
                                DuocHuongBHYT = item.DuocHuongBaoHiem,
                                BaoHiemChiTra = item.BaoHiemChiTra,
                                DonGiaBHYT = item.DonGiaBaoHiem ?? 0,
                                TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan ?? 0,
                                MucHuongBaoHiem = item.MucHuongBaoHiem ?? 0
                            }).ToList();
                        if (lstChiPhiTruyenMau.Any())
                        {
                            lstChiPhi.AddRange(lstChiPhiTruyenMau);
                        }
                    }
                    #endregion

                    #endregion
                    
                    foreach (var yeuCauTiepNhan in lstTiepNhanTheoNoiGioiThieu)
                    {
                        var lstChiPhiTheoMaTiepNhan = lstChiPhi.Where(x => x.MaYeuCauTiepNhan.Contains(yeuCauTiepNhan.MaYeuCauTiepNhan)).ToList();

                        // chi phí cls
                        var lstDichVuPTTT = lstChiPhiTheoMaTiepNhan.Where(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && x.DaThucHien).ToList();
                        if (lstDichVuPTTT.Any())
                        {
                            yeuCauTiepNhan.PhongPhauThuat = string.Join(", ", lstDichVuPTTT.Select(x => x.NoiThucHien));
                            yeuCauTiepNhan.NgayPhauThuatDisplay = string.Join(", ", lstDichVuPTTT.Select(x => x.NgayPhauThuat?.ApplyFormatDate()));
                            yeuCauTiepNhan.DichVuKyThuat = string.Join(", ", lstDichVuPTTT.Select(x => x.TenDichVu));
                            //yeuCauTiepNhan.ThoiGianGayMeDisplay = string.Join(", ", lstDichVuPTTT.Select(x => x.ThoiGianGayMe?.FormatNgayGioTimKiemTrenBaoCao()));
                            //yeuCauTiepNhan.ThoiGianBanGiaoDisplay = string.Join(", ", lstDichVuPTTT.Select(x => x.ThoiGianBanGiao?.FormatNgayGioTimKiemTrenBaoCao()));

                            //BVHD-3917
                            var lstThongTinThuePhong = lstChiPhiTheoMaTiepNhan.Where(x => x.DaThucHien && x.CoThuePhong && x.ChiPhiThuePhong != null).ToList();
                            if (lstThongTinThuePhong.Any())
                            {
                                yeuCauTiepNhan.ThoiGianBatDauThuePhongDisplay =
                                    string.Join(", ", lstThongTinThuePhong
                                        .Select(x => x.ChiPhiThuePhong.BatDauThuePhong?.FormatNgayGioTimKiemTrenBaoCao()));
                                yeuCauTiepNhan.ThoiGianKetThucThuePhongDisplay =
                                    string.Join(", ", lstThongTinThuePhong
                                        .Select(x => x.ChiPhiThuePhong.KetThucThuePhong?.FormatNgayGioTimKiemTrenBaoCao()));
                            }
                        }

                        // chi phí DV kỹ thuật
                        //var chiPhiDichVuCLS = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu 
                        //                                                         && (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh || x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                        //                                                      && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                        //                                                      && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn
                        //                                                      && !string.IsNullOrEmpty(x.TenDichVu)
                        //                                                      && !x.TenDichVu.Contains(tenDichVuCovid)
                        //                                                      && x.ChiPhi != null
                        //                                                      && x.ChiPhi > 0
                        //                                                      && x.DaThucHien)
                        //                                            .Sum(x => x.ChiPhi ?? 0);
                        //yeuCauTiepNhan.ChiPhiCanLamSan = chiPhiDichVuCLS == 0 ? (decimal?) null : chiPhiDichVuCLS;

                        // chi phí cls
                        var chiPhiDichVuCLS = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                                 && (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh || x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                                                                                 && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                                 && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn
                                                                                 && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.Khac
                                                                                 && !string.IsNullOrEmpty(x.TenDichVu)
                                                                                 && !x.TenDichVu.Contains(tenDichVuCovid)
                                                                                 && (x.TenDichVu.RemoveVietnameseDiacritics().Length < 4 || !x.TenDichVu.RemoveVietnameseDiacritics().Substring(0, 4).ToLower().Equals(tenDichVuGiamDau.ToLower()))
                                                                                 && !x.CoThuePhong
                                                                                 && x.ChiPhi != null
                                                                                 && x.ChiPhi > 0
                                                                                 && x.DaThucHien)
                            .ToList();
                        var chiPhiCLSNgoaiTru = chiPhiDichVuCLS.Where(x => x.LaNoiTru != true).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiCanLamSanNgoaiTru = chiPhiCLSNgoaiTru == 0 ? (decimal?)null : chiPhiCLSNgoaiTru;

                        var chiPhiCLSNoiTru = chiPhiDichVuCLS.Where(x => x.LaNoiTru == true).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiCanLamSanNoiTru = chiPhiCLSNoiTru == 0 ? (decimal?)null : chiPhiCLSNoiTru;

                        #region BVHD-3917 Chi phí dịch vụ khám, kỹ thuật chưa thực hiện
                        var chiPhiDichVuChuaThucHien = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                                          && (x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh || x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                                                                                          && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                                          && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn
                                                                                          && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.Khac
                                                                                          && !string.IsNullOrEmpty(x.TenDichVu)
                                                                                          && !x.TenDichVu.Contains(tenDichVuCovid)
                                                                                          && (x.TenDichVu.RemoveVietnameseDiacritics().Length < 4 || !x.TenDichVu.RemoveVietnameseDiacritics().Substring(0, 4).ToLower().Equals(tenDichVuGiamDau.ToLower()))
                                                                                          && !x.CoThuePhong
                                                                                          && x.ChiPhi != null
                                                                                          && x.ChiPhi > 0
                                                                                          && !x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiChuaThucHien = chiPhiDichVuChuaThucHien == 0 ? (decimal?)null : chiPhiDichVuChuaThucHien;

                        #endregion

                        // chi phí giường
                        var chiPhiDichVuGiuong = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                              && x.LoaiDichVuKyThuat == null
                                                                              && x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
                                                                              && x.ChiPhi != null
                                                                              && x.ChiPhi > 0
                                                                              && x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiGiuong = chiPhiDichVuGiuong == 0 ? (decimal?)null : chiPhiDichVuGiuong;

                        // chi phí thuốc, vật tư
                        var chhiPhiThuoc = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                                   && x.LoaiDichVuKyThuat == null
                                                                                   && x.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham
                                                                                   //&& x.ChiPhi != null
                                                                                   //&& x.ChiPhi > 0
                                                                                   && x.ChiPhiThucTe != null
                                                                                   && x.ChiPhiThucTe > 0
                                                                                   && x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiThuoc = chhiPhiThuoc == 0 ? (decimal?)null : chhiPhiThuoc;

                        var chiPhiVatTu = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                             && x.LoaiDichVuKyThuat == null
                                                                             && x.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao
                                                                             //&& x.ChiPhi != null
                                                                             //&& x.ChiPhi > 0
                                                                             && x.ChiPhiThucTe != null
                                                                             && x.ChiPhiThucTe > 0
                                                                             && x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiVTYT = chiPhiVatTu == 0 ? (decimal?)null : chiPhiVatTu;

                        #region BVHD-3917: chi phí thuốc và vật tư chưa thực hiện -> dùng để bổ sung cho chỗ tính số tiền dịch vụ khác
                        var chiPhiThuocChuaThucHien = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                                          && x.LoaiDichVuKyThuat == null
                                                                                          && x.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham
                                                                                          && x.ChiPhi != null
                                                                                          && x.ChiPhi > 0
                                                                                          && !x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiThuocChuaThucHien = chiPhiThuocChuaThucHien == 0 ? (decimal?)null : chiPhiThuocChuaThucHien;

                        var chiPhiVatTuChuaThucHien = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                             && x.LoaiDichVuKyThuat == null
                                                                             && x.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao
                                                                             && x.ChiPhi != null
                                                                             && x.ChiPhi > 0
                                                                             && !x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiVTYTChuaThucHien = chiPhiVatTuChuaThucHien == 0 ? (decimal?)null : chiPhiVatTuChuaThucHien;
                        #endregion

                        #region BVHD-3917: chi phí thuê phòng mổ
                        var chiPhiThuePhongMo = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                                 && x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat
                                                                                 && x.CoThuePhong).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.ChiPhiThuePhongMo = chiPhiThuePhongMo == 0 ? (decimal?)null : chiPhiThuePhongMo;
                        #endregion

                        #region BVHD-3917: chi phí giảm đau
                        var chiPhiGiamDau = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                                                                                   && x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat
                                                                                   && (x.TenDichVu.RemoveVietnameseDiacritics().Length >= 4 && x.TenDichVu.RemoveVietnameseDiacritics().Substring(0, 4).ToLower().Equals(tenDichVuGiamDau.ToLower()))
                                                                                   && x.ChiPhi != null
                                                                                   && x.ChiPhi > 0
                                                                                   && x.DaThucHien).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.GiamDau = chiPhiGiamDau == 0 ? (decimal?)null : chiPhiGiamDau;
                        #endregion

                        //// chi phí dv covid
                        //var chiPhiDichVuCovid = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                        //                                                             && x.TenDichVu.Contains(tenDichVuCovid)
                        //                                                             && x.ChiPhi != null
                        //                                                             && x.ChiPhi > 0
                        //                                                             && x.DaThucHien).Sum(x => x.ChiPhi ?? 0);
                        //yeuCauTiepNhan.TestCovid = chiPhiDichVuCovid == 0 ? (decimal?) null : chiPhiDichVuCovid;

                        //// chi phí suát ăn
                        //var chiPhiSuatAn = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu
                        //                                                         && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn
                        //                                                         && x.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat
                        //                                                         && x.ChiPhi != null
                        //                                                         && x.ChiPhi > 0
                        //                                                         && x.DaThucHien).Sum(x => x.ChiPhi ?? 0);
                        //yeuCauTiepNhan.SuatAn = chiPhiSuatAn == 0 ? (decimal?) null : chiPhiSuatAn;

                        // chi phí kh đã thanh tóan
                        //var chiPhiDaThanhToan = lstChiPhiTheoMaTiepNhan.Where(x => x.LaBenhNhanThu
                        //                                                         && x.LoaiDichVuKyThuat == null
                        //                                                         && x.NhomDichVu == null
                        //                                                         && x.ChiPhi != null
                        //                                                         && x.ChiPhi > 0).Sum(x => x.ChiPhi ?? 0);

                        //tổng tiền người bệnh đã thanh toán, riêng phiếu chi thì check theo ChiPhiThucTe vì cách tính khác
                        var chiPhiDaThanhToan = lstChiPhiTheoMaTiepNhan.Where(x => x.LaBenhNhanThu
                                                                                   && x.ChiPhiThucTe != null
                                                                                   && x.ChiPhiThucTe > 0).Sum(x => x.ChiPhiThucTe ?? 0);
                        yeuCauTiepNhan.NguoiBenhDaThanhToan = chiPhiDaThanhToan == 0 ? (decimal?) null : chiPhiDaThanhToan;

                        // tổng chi phí ca PT
                        yeuCauTiepNhan.ChiPhiCaPhauThuat = lstChiPhiTheoMaTiepNhan.Where(x => !x.LaBenhNhanThu).Sum(x => x.ChiPhiThucTe);

                        #region BVHD-3917 Tổng công nợ chưa thanh toán
                        yeuCauTiepNhan.CongNoChuaThanhToan = yeuCauTiepNhan.ChiPhiCaPhauThuat - yeuCauTiepNhan.NguoiBenhDaThanhToan.GetValueOrDefault();

                        #endregion
                    }
                }
            }

            return new GridDataSource
            {
                Data = lstTiepNhanTheoNoiGioiThieu.ToArray(),
                TotalRowCount = lstTiepNhanTheoNoiGioiThieu.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageBaoCaoTongHopCongNoChuaThanhToanForGridAsync(QueryInfo queryInfo)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<BaoCaoTongHopCongNoChuaThanhToanGridVo>();
            var timKiemNangCaoObj = new BaoCaoTongHopCongNoChuaThanhToanTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTongHopCongNoChuaThanhToanTimKiemVo>(queryInfo.AdditionalSearchString);
                if (timKiemNangCaoObj.MaYeuCauTiepNhan == null
                    || timKiemNangCaoObj.MaYeuCauTiepNhan.Contains("undefined")
                    || timKiemNangCaoObj.MaYeuCauTiepNhan == "0")
                {
                    timKiemNangCaoObj.MaYeuCauTiepNhan = null;
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if ((tuNgay != null && denNgay != null) || !string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan))
            {
                var tenDichVuCovid = "SARS-CoV";
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                if (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                        .Where(x =>
                                    //BVHD-3917
                                    ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                     || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru))

                                    && ((!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan)
                                         && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                        || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)
                                            && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)
                                            //&& x.ThoiDiemTiepNhan >= tuNgay
                                            //&& x.ThoiDiemTiepNhan <= denNgay))

                                            //BVHD-3917
                                            && ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true
                                                            && x.ThoiDiemTiepNhan >= tuNgay && x.ThoiDiemTiepNhan <= denNgay))
                                                || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true
                                                            && x.NoiTruBenhAn.ThoiDiemRaVien != null && x.NoiTruBenhAn.ThoiDiemRaVien.Value >= tuNgay && x.NoiTruBenhAn.ThoiDiemRaVien <= denNgay))
                                                )
                                    && (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && x.BenhNhanId != null)
                        .Select(x => new BaoCaoTongHopCongNoChuaThanhToanGridVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoTen = x.HoTen,
                            MaBN = x.BenhNhan.MaBN,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                        })
                        .ToList();
                }
                else
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                    .Where(x => x.NoiGioiThieuId != null

                                //BVHD-3917
                                && ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                     || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru))

                                && ((!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan)
                                     && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)
                                        && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)
                                        //&& x.ThoiDiemTiepNhan >= tuNgay
                                        //&& x.ThoiDiemTiepNhan <= denNgay))

                                        //BVHD-3917
                                        && ((timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true
                                                    && x.ThoiDiemTiepNhan >= tuNgay && x.ThoiDiemTiepNhan <= denNgay))
                                            || (timKiemNangCaoObj.LaNguoiBenhNgoaiTru != true
                                                    && x.NoiTruBenhAn.ThoiDiemRaVien != null && x.NoiTruBenhAn.ThoiDiemRaVien.Value >= tuNgay && x.NoiTruBenhAn.ThoiDiemRaVien <= denNgay))
                                            )
                                 && (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || (x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))
                                 && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                 && x.BenhNhanId != null)
                    //&& x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                    //                                  && a.TaiKhoanBenhNhanChis.Any(b => b.DaHuy != true)))
                    .Select(x => new BaoCaoTongHopCongNoChuaThanhToanGridVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                        TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                        BenhNhanId = x.BenhNhanId.Value,
                        HoTen = x.HoTen,
                        MaBN = x.BenhNhan.MaBN,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    })
                    .ToList();
                }
                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    //&& ((!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan)
                                    //     && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    //    || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)
                                    //        && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)
                                    //        && x.ThoiDiemTiepNhan >= tuNgay
                                    //        && x.ThoiDiemTiepNhan <= denNgay))
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        //&& x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                        //                                   && a.TaiKhoanBenhNhanChis.Any(b => b.DaHuy != true)))
                        .Select(x => new BaoCaoTongHopCongNoChuaThanhToanGridVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.BenhNhanId.Value,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                        })
                        .ToList();

                    //todo: update nơi giới thiệu
                    foreach (var benhNhanId in lstBenhNhanId)
                    {
                        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                            .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        if (lanTiepNhanDauTienCoGioiThieu == null)
                        {
                            continue;
                        }
                        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId
                                        && x.BenhNhanId == benhNhanId)
                            .ToList();

                        var khongThemNguoiGioiThieu = false;
                        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuId;
                        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuDisplay;
                        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                        {
                            if (lanTiepNhan.NoiGioiThieuId != null)
                            {
                                if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId)
                                    || ((timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0) && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId))
                                {
                                    // trường hợp tìm kiếm theo người giới thiệu thì chỉ thêm người giới thiệu đang tìm kiếm thôi
                                    if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId))
                                    {
                                        khongThemNguoiGioiThieu = true;
                                    }
                                    else
                                    {
                                        khongThemNguoiGioiThieu = false;
                                    }
                                    nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                                    tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                                }
                                else
                                {
                                    khongThemNguoiGioiThieu = false;
                                }
                            }

                            if (!khongThemNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null && lstTiepNhanTheoNoiGioiThieu.All(x => x.YeucauTiepNhanId != lanTiepNhan.YeucauTiepNhanId))
                            {
                                lanTiepNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                lanTiepNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                            }
                        }
                    }

                    #region BVHD-3917 xử lý check trường hợp chọn ngoại trú -> chỉ lấy các YCTN ngoại trú của người bệnh mà không có nhập viện (chưa tạo bệnh án)
                    var lstYeuCauChiTiepNhanNgoaiTruId = new List<long>();
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        var lstMaTiepNhanQuery = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();
                        lstYeuCauChiTiepNhanNgoaiTruId = _yeuCauTiepNhanRepository.TableNoTracking
                            .Where(x => lstMaTiepNhanQuery.Contains(x.MaYeuCauTiepNhan))
                            .Select(x => new
                            {
                                YeuCauTiepNhanId = x.Id,
                                MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                                CoNoiTru = x.NoiTruBenhAn != null
                            })
                            .GroupBy(x => new { x.MaYeuCauTiepNhan })
                            .Where(x => !x.Any(a => a.CoNoiTru))
                            .Select(x => x.First().YeuCauTiepNhanId)
                            .Distinct().ToList();

                        lstTiepNhanTheoNoiGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => lstYeuCauChiTiepNhanNgoaiTruId.Contains(x.YeucauTiepNhanId)).ToList();
                    }
                    #endregion

                    var countTask = lstTiepNhanTheoNoiGioiThieu.Count();
                    return new GridDataSource { TotalRowCount = countTask };
                }
                return new GridDataSource { TotalRowCount = 0 };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoTongHopCongNoChuaThanhToan(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoTongHopCongNoChuaThanhToanTimKiemVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTongHopCongNoChuaThanhToanTimKiemVo>(query.AdditionalSearchString);
                if (timKiemNangCaoObj.MaYeuCauTiepNhan == null
                    || timKiemNangCaoObj.MaYeuCauTiepNhan.Contains("undefined")
                    || timKiemNangCaoObj.MaYeuCauTiepNhan == "0")
                {
                    timKiemNangCaoObj.MaYeuCauTiepNhan = null;
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            

           // var noiGioiThieu = _noiGioiThieuRepository.TableNoTracking.FirstOrDefault(x => x.Id == timKiemNangCaoObj.NoiGioiThieuId);

            var datas = (ICollection<BaoCaoTongHopCongNoChuaThanhToanGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỔNG HỢP CÔNG NỢ CHƯA THANH TOÁN");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 40;
                    worksheet.Column(8).Width = 40;
                    worksheet.Column(9).Width = 40;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.DefaultColWidth = 25;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "TỔNG HỢP CÔNG NỢ CHƯA THANH TOÁN";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        if (timKiemNangCaoObj.MaYeuCauTiepNhan == null)
                        {
                            range.Worksheet.Cells["A4:L4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();

                        }
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:W7"])
                    {
                        range.Worksheet.Cells["A7:W7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:W7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:W7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:W7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:W7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:W7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Ngày Tiếp Nhận";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Hình Thức Đến";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Mã Y Tế";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Mã Tiếp Nhận";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Họ Và Tên";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Phòng PT";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Ngày PT";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Nội dung PTTT";

                        //range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["J7"].Value = "Chi Phí CLS";

                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Chi Phí CLS Ngoại Trú";

                        range.Worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7"].Value = "Chi Phí CLS Nội Trú";

                        range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7"].Value = "DV chưa thực hiện";

                        range.Worksheet.Cells["M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M7"].Value = "Chi Phí Giường";

                        //range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["L7"].Value = "Chi Phí Thuốc, VTYT";

                        range.Worksheet.Cells["N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N7"].Value = "Chi Phí Thuốc";
                        range.Worksheet.Cells["O7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O7"].Value = "Chi Phí VTYT";

                        range.Worksheet.Cells["P7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P7"].Value = "Chi Phí Thuê Phòng Mổ";

                        range.Worksheet.Cells["Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q7"].Value = "Giảm Đau";

                        //range.Worksheet.Cells["Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["Q7"].Value = "Test Covid";

                        range.Worksheet.Cells["R7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R7"].Value = "Dịch vụ khác";

                        range.Worksheet.Cells["S7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S7"].Value = "KH Đã Thanh Toán";

                        range.Worksheet.Cells["T7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["T7"].Value = "Tổng Công Nợ Chưa Thanh Toán";

                        range.Worksheet.Cells["U7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["U7"].Value = "Tổng Chi Phí Ca PT";

                        range.Worksheet.Cells["V7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["V7"].Value = "Thời gian bắt đầu thuê PM";

                        range.Worksheet.Cells["W7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["W7"].Value = "Thời gian kết thúc thuê PM";

                        //range.Worksheet.Cells["X7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //range.Worksheet.Cells["X7"].Value = "Ra Viện";
                    }

                    //write data from line 8
                    int index = 8;
                    int stt = 1;
                    var formatCurrency = "#,##0.00";
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":W" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":W" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":W" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":W" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":W" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.ThoiDiemTiepNhanDisplay;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = item.HinhThucDenDisplay;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = item.MaBN;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = item.MaYeuCauTiepNhan;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = item.HoTen;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = item.PhongPhauThuat;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = item.NgayPhauThuatDisplay;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = item.DichVuKyThuat;

                            //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //worksheet.Cells["J" + index].Value = item.ChiPhiCanLamSan;
                            //worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Value = item.ChiPhiCanLamSanNgoaiTru;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Value = item.ChiPhiCanLamSanNoiTru;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Value = item.ChiPhiChuaThucHien;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Value = item.ChiPhiGiuong;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = formatCurrency;

                            //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //worksheet.Cells["M" + index].Value = item.ChiPhiThuocVTYT;
                            //worksheet.Cells["M" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Value = item.ChiPhiThuoc;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["O" + index].Value = item.ChiPhiVTYT;
                            worksheet.Cells["O" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["P" + index].Value = item.ChiPhiThuePhongMo;
                            worksheet.Cells["P" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Q" + index].Value = item.GiamDau;
                            worksheet.Cells["Q" + index].Style.Numberformat.Format = formatCurrency;

                            //worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //worksheet.Cells["Q" + index].Value = item.TestCovid;
                            //worksheet.Cells["Q" + index].Style.Numberformat.Format = formatCurrency;

                            //worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //worksheet.Cells["R" + index].Value = item.SuatAn;
                            //worksheet.Cells["R" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["R" + index].Value = item.DichVuKhac;
                            worksheet.Cells["R" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["S" + index].Value = item.NguoiBenhDaThanhToan;
                            worksheet.Cells["S" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["T" + index].Value = item.CongNoChuaThanhToan;
                            worksheet.Cells["T" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["U" + index].Value = item.ChiPhiCaPhauThuat;
                            worksheet.Cells["U" + index].Style.Numberformat.Format = formatCurrency;

                            //worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["V" + index].Value = item.ThoiGianGayMeDisplay;

                            //worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["W" + index].Value = item.ThoiGianBanGiaoDisplay;

                            worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["V" + index].Value = item.ThoiGianBatDauThuePhongDisplay;

                            worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["W" + index].Value = item.ThoiGianKetThucThuePhongDisplay;

                            //worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["X" + index].Value = item.RaVien ? "X" : string.Empty;

                            stt++;
                            index++;
                        }

                        //total
                        worksheet.Cells["A" + index + ":W" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":W" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + index + ":W" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":W" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":W" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;

                        using (var range = worksheet.Cells["A" + index + ":I" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":I" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":I" + index].Value = "Tổng TT";
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                        }


                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["J" + index].Style.Font.Bold = true;
                        //worksheet.Cells["J" + index].Value = datas.Sum(x => x.ChiPhiCanLamSan ?? 0);
                        //worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.Font.Bold = true;
                        worksheet.Cells["J" + index].Value = datas.Sum(x => x.ChiPhiCanLamSanNgoaiTru ?? 0);
                        worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Style.Font.Bold = true;
                        worksheet.Cells["K" + index].Value = datas.Sum(x => x.ChiPhiCanLamSanNoiTru ?? 0);
                        worksheet.Cells["K" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Style.Font.Bold = true;
                        worksheet.Cells["L" + index].Value = datas.Sum(x => x.ChiPhiChuaThucHien ?? 0);
                        worksheet.Cells["L" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["M" + index].Style.Font.Bold = true;
                        worksheet.Cells["M" + index].Value = datas.Sum(x => x.ChiPhiGiuong ?? 0);
                        worksheet.Cells["M" + index].Style.Numberformat.Format = formatCurrency;

                        //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["L" + index].Style.Font.Bold = true;
                        //worksheet.Cells["L" + index].Value = datas.Sum(x => x.ChiPhiThuocVTYT ?? 0);
                        //worksheet.Cells["L" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["N" + index].Style.Font.Bold = true;
                        worksheet.Cells["N" + index].Value = datas.Sum(x => x.ChiPhiThuoc ?? 0);
                        worksheet.Cells["N" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["O" + index].Style.Font.Bold = true;
                        worksheet.Cells["O" + index].Value = datas.Sum(x => x.ChiPhiVTYT ?? 0);
                        worksheet.Cells["O" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["P" + index].Style.Font.Bold = true;
                        worksheet.Cells["P" + index].Value = datas.Sum(x => x.ChiPhiThuePhongMo ?? 0);
                        worksheet.Cells["P" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["Q" + index].Style.Font.Bold = true;
                        worksheet.Cells["Q" + index].Value = datas.Sum(x => x.GiamDau ?? 0);
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = formatCurrency;

                        //worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["Q" + index].Style.Font.Bold = true;
                        //worksheet.Cells["Q" + index].Value = datas.Sum(x => x.TestCovid ?? 0);
                        //worksheet.Cells["Q" + index].Style.Numberformat.Format = formatCurrency;

                        //worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["R" + index].Style.Font.Bold = true;
                        //worksheet.Cells["R" + index].Value = datas.Sum(x => x.SuatAn ?? 0);
                        //worksheet.Cells["R" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["R" + index].Style.Font.Bold = true;
                        worksheet.Cells["R" + index].Value = datas.Sum(x => x.DichVuKhac ?? 0);
                        worksheet.Cells["R" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["S" + index].Style.Font.Bold = true;
                        worksheet.Cells["S" + index].Value = datas.Sum(x => x.NguoiBenhDaThanhToan ?? 0);
                        worksheet.Cells["S" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["T" + index].Style.Font.Bold = true;
                        worksheet.Cells["T" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["U" + index].Style.Font.Bold = true;
                        worksheet.Cells["U" + index].Value = datas.Sum(x => x.ChiPhiCaPhauThuat ?? 0);
                        worksheet.Cells["U" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["V" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["V" + index].Style.Font.Bold = true;
                        //worksheet.Cells["V" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["W" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["W" + index].Style.Font.Bold = true;
                        //worksheet.Cells["W" + index].Style.Numberformat.Format = formatCurrency;

                        //worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["X" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["X" + index].Style.Font.Bold = true;
                        //worksheet.Cells["X" + index].Style.Numberformat.Format = formatCurrency;
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
