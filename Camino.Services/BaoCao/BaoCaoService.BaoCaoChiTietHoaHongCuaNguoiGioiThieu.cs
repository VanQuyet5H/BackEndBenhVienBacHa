using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGrid3961Async(QueryInfo queryInfo)
        {            
            var thongTinGioiThieus = new List<BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo>();
            var timKiemNangCaoObj = new BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo>(queryInfo.AdditionalSearchString);
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

            if (tuNgay != null && denNgay != null)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                var dataHinhThucDen = _hinhThucDenRepository.TableNoTracking
                .Select(o => new { o.Id, o.Ten })
                .ToList();

                var dataNoiGioiThieu = _noiGioiThieuRepository.TableNoTracking
                    .Select(o => new { o.Id, o.Ten, o.DonVi })
                    .ToList();

                var phieuThuDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking
                    .Where(o => o.DaHuy != true && o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && tuNgay <= o.NgayThu && o.NgayThu <= denNgay);

                if (!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan))
                {
                    phieuThuDataQuery = phieuThuDataQuery.Where(o => o.YeuCauTiepNhan.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan));
                }
                if(timKiemNangCaoObj.HinhThucDenId.GetValueOrDefault() != 0)
                {
                    if(timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                    {
                        phieuThuDataQuery = phieuThuDataQuery.Where(o => o.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId);
                    }
                    else
                    {
                        phieuThuDataQuery = phieuThuDataQuery.Where(o => timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || o.YeuCauTiepNhan.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId);
                    }
                }

                var phieuThus = phieuThuDataQuery.Select(o => new
                {
                    PhieuThuId = o.Id,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    NgayThu = o.NgayThu,
                    SoPhieuHienThi = o.SoPhieuHienThi,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    ThoiDiemTiepNhan = o.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    BenhNhanId = o.YeuCauTiepNhan.BenhNhanId,
                    MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
                    NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                    HinhThucDenId = o.YeuCauTiepNhan.HinhThucDenId,
                    CongNo = o.CongNo,
                    DataPhieuChis = o.TaiKhoanBenhNhanChis
                                    .Where(chi => chi.DaHuy != true && chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                                    .Select(chi => new
                                    {
                                        Id = chi.Id,
                                        NgayChi = chi.NgayChi,
                                        TienChiPhi = chi.TienChiPhi,
                                        SoTienBaoHiemTuNhanChiTra = chi.SoTienBaoHiemTuNhanChiTra,
                                        TienMat = 0,
                                        ChuyenKhoan = 0,
                                        Gia = chi.Gia,
                                        SoLuong = chi.SoLuong,
                                        DonGiaBaoHiem = chi.DonGiaBaoHiem,
                                        MucHuongBaoHiem = chi.MucHuongBaoHiem,
                                        TiLeBaoHiemThanhToan = chi.TiLeBaoHiemThanhToan,
                                        SoTienMienGiam = chi.SoTienMienGiam,
                                        YeuCauKhamBenhId = chi.YeuCauKhamBenhId,
                                        YeuCauDichVuKyThuatId = chi.YeuCauDichVuKyThuatId,
                                        YeuCauDuocPhamBenhVienId = chi.YeuCauDuocPhamBenhVienId,
                                        YeuCauVatTuBenhVienId = chi.YeuCauVatTuBenhVienId,
                                        DonThuocThanhToanChiTietId = chi.DonThuocThanhToanChiTietId,
                                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = chi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                                        YeuCauTruyenMauId = chi.YeuCauTruyenMauId,
                                        YeuCauGoiDichVuId = chi.YeuCauGoiDichVuId,
                                    }).ToList(),
                }).ToList();

                var dichVuKhamBenhIds = phieuThus.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauKhamBenhId != null).Select(o => o.YeuCauKhamBenhId).Distinct().ToList();
                var dichVuKyThuatIds = phieuThus.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuKyThuatId != null).Select(o => o.YeuCauDichVuKyThuatId).Distinct().ToList();
                var dichVuGiuongIds = phieuThus.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null).Select(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId).Distinct().ToList();
                var truyenMauIds = phieuThus.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauTruyenMauId != null).Select(o => o.YeuCauTruyenMauId).Distinct().ToList();
                var yeuCauDuocPhamIds = phieuThus.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(o => o.YeuCauDuocPhamBenhVienId).Distinct().ToList();
                var yeuCauVatTuIds = phieuThus.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauVatTuBenhVienId != null).Select(o => o.YeuCauVatTuBenhVienId).Distinct().ToList();

                var maxTake = 18000;

                var dataDichVuKhamBenh = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
                for (int i = 0; i < dichVuKhamBenhIds.Count; i = i + maxTake)
                {
                    var takeIds = dichVuKhamBenhIds.Skip(i).Take(maxTake).ToList();

                    var info = _yeuCauKhamBenhRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                    {
                        Id = o.Id,
                        TenDichVu = o.TenDichVu,
                        BacSiKham = o.BacSiThucHienId != null ? o.BacSiThucHien.User.HoTen : "",
                        DichVuKhamBenhBenhVienId = o.DichVuKhamBenhBenhVienId,
                        NhomGiaDichVuKhamBenhBenhVienId = o.NhomGiaDichVuKhamBenhBenhVienId,
                        YeuCauKhamBenh = true,
                        ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    }).ToList();

                    dataDichVuKhamBenh.AddRange(info);
                }

                var dataDichVuKyThuat = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
                for (int i = 0; i < dichVuKyThuatIds.Count; i = i + maxTake)
                {
                    var takeIds = dichVuKyThuatIds.Skip(i).Take(maxTake).ToList();

                    var info = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                    {
                        Id = o.Id,
                        TenDichVu = o.TenDichVu,
                        BacSiKham = o.YeuCauKhamBenhId != null && o.YeuCauKhamBenh.BacSiThucHienId != null ? o.YeuCauKhamBenh.BacSiThucHien.User.HoTen : "",
                        DichVuKyThuatBenhVienId = o.DichVuKyThuatBenhVienId,
                        NhomGiaDichVuKyThuatBenhVienId = o.NhomGiaDichVuKyThuatBenhVienId,
                        YeuCauDichVuKyThuat = true,
                        LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                        NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                        ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    }).ToList();

                    dataDichVuKyThuat.AddRange(info);
                }

                var dataDichVuGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking.Where(o => dichVuGiuongIds.Contains(o.Id))
                    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                    {
                        Id = o.Id,
                        TenDichVu = o.Ten,
                        DichVuGiuongBenhVienId = o.DichVuGiuongBenhVienId,
                        NhomGiaDichVuGiuongBenhVienId = o.NhomGiaDichVuGiuongBenhVienId,
                        YeuCauGiuong = true,
                        ThoiDiemChiDinh = o.NgayPhatSinh
                    }).ToList();

                var dataTruyenMau = _yeuCauTruyenMauRepository.TableNoTracking.Where(o => truyenMauIds.Contains(o.Id))
                    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                    {
                        Id = o.Id,
                        TenDichVu = o.TenDichVu,
                        MauVaChePhamId = o.MauVaChePhamId,
                        YeuCauTruyenMau = true,
                        NoiThucHienId = o.NoiThucHienId,
                        NoiChiDinhId = o.NoiChiDinhId
                    }).ToList();

                var dataYeuCauDuocPham = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
                for (int i = 0; i < yeuCauDuocPhamIds.Count; i = i + maxTake)
                {
                    var takeIds = yeuCauDuocPhamIds.Skip(i).Take(maxTake).ToList();

                    var info = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                    {
                        Id = o.Id,
                        TenDichVu = o.Ten,
                        BacSiKham = o.YeuCauKhamBenhId != null && o.YeuCauKhamBenh.BacSiThucHienId != null ? o.YeuCauKhamBenh.BacSiThucHien.User.HoTen : "",
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        DonGiaNhapDuocPham = o.DonGiaNhap,
                        YeuCauDuocPham = true,                        
                        ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    }).ToList();

                    dataYeuCauDuocPham.AddRange(info);
                }

                var dataYeuCauVatTu = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
                for (int i = 0; i < yeuCauVatTuIds.Count; i = i + maxTake)
                {
                    var takeIds = yeuCauVatTuIds.Skip(i).Take(maxTake).ToList();

                    var info = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                    {
                        Id = o.Id,
                        TenDichVu = o.Ten,
                        BacSiKham = o.YeuCauKhamBenhId != null && o.YeuCauKhamBenh.BacSiThucHienId != null ? o.YeuCauKhamBenh.BacSiThucHien.User.HoTen : "",
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        DonGiaNhapVatTu = o.DonGiaNhap,
                        YeuCauVatTu = true,
                        ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    }).ToList();

                    dataYeuCauVatTu.AddRange(info);
                }

                var thongTinHopDongs = _noiGioiThieuHopDongRepository.TableNoTracking
                .Select(o => new
                {
                    o.NoiGioiThieuId,
                    o.NgayBatDau,
                    o.NgayKetThuc,
                    ChiTietHoaHongDichVuKhamBenhs = o.NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs.Select(k => new
                    {
                        k.DichVuKhamBenhBenhVienId,
                        k.NhomGiaDichVuKhamBenhBenhVienId,
                        k.LoaiHoaHong,
                        k.SoTienHoaHong,
                        k.TiLeHoaHong,
                        k.ApDungTuLan,
                        k.ApDungDenLan
                    }).ToList(),
                    ChiTietHoaHongDichVuKyThuats = o.NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats.Select(k => new
                    {
                        k.DichVuKyThuatBenhVienId,
                        k.NhomGiaDichVuKyThuatBenhVienId,
                        k.LoaiHoaHong,
                        k.SoTienHoaHong,
                        k.TiLeHoaHong,
                        k.ApDungTuLan,
                        k.ApDungDenLan
                    }).ToList(),
                    ChiTietHoaHongDichVuGiuongs = o.NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs.Select(k => new
                    {
                        k.DichVuGiuongBenhVienId,
                        k.NhomGiaDichVuGiuongBenhVienId,
                        k.LoaiHoaHong,
                        k.SoTienHoaHong,
                        k.TiLeHoaHong,
                        k.ApDungTuLan,
                        k.ApDungDenLan
                    }).ToList(),
                    ChiTietHoaHongDuocPhams = o.NoiGioiThieuHopDongChiTietHoaHongDuocPhams.Select(k => new
                    {
                        k.DuocPhamBenhVienId,
                        k.TiLeHoaHong
                    }).ToList(),
                    ChiTietHoaHongVatTus = o.NoiGioiThieuHopDongChiTietHoaHongVatTus.Select(k => new
                    {
                        k.VatTuBenhVienId,
                        k.TiLeHoaHong
                    }).ToList(),
                }).ToList();
                var thongTinDichVuKhamBenhTinhHoaHongs = new List<ThongTinDichVuKhamBenhTinhHoaHong>(); 
                var thongTinDichVuKyThuatTinhHoaHongs = new List<ThongTinDichVuKyThuatTinhHoaHong>();
                var thongTinDichVuGiuongTinhHoaHongs = new List<ThongTinDichVuGiuongTinhHoaHong>();

                var chiTietHoaHongCuaNguoiGioiThieuGridVos = new List<BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo>();
                foreach (var dataPhieuThu in phieuThus.OrderBy(o=>o.NgayThu))
                {
                    foreach (var dataPhieuChi in dataPhieuThu.DataPhieuChis
                        .Where(o=>o.YeuCauKhamBenhId != null || o.YeuCauDichVuKyThuatId != null || o.YeuCauDuocPhamBenhVienId != null || o.YeuCauVatTuBenhVienId != null || o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null || o.YeuCauTruyenMauId != null))
                    {
                        var baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo = new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                        {
                            NgayKham = dataPhieuThu.ThoiDiemTiepNhan,
                            MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                            YeuCauTiepNhanId = dataPhieuThu.YeuCauTiepNhanId,
                            BenhNhanId = dataPhieuThu.BenhNhanId.Value,
                            TenBN = dataPhieuThu.HoTen,
                            NgaySinh = dataPhieuThu.NgaySinh,
                            ThangSinh = dataPhieuThu.ThangSinh,
                            NamSinh = dataPhieuThu.NamSinh,
                            DiaChi = dataPhieuThu.DiaChi,

                            //BSKham = item.YeuCauKhamBenh.BacSiThucHien.User.HoTen ?? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //    ?? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //    ?? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //    ?? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //    ?? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen,

                            //TenDV = item.YeuCauKhamBenh.TenDichVu ?? item.YeuCauDichVuKyThuat.TenDichVu
                            //    ?? item.YeuCauDuocPhamBenhVien.Ten
                            //    ?? item.YeuCauVatTuBenhVien.Ten
                            //    ?? item.YeuCauDichVuGiuongBenhVien.Ten
                            //    ?? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ten
                            //    ?? item.YeuCauTruyenMau.TenDichVu,
                            //NhomDichVuConId = item.YeuCauDichVuKyThuat.NhomDichVuBenhVienId,

                            YeuCauKhamBenhId = dataPhieuChi.YeuCauKhamBenhId,
                            YeuCauDichVuKyThuatId = dataPhieuChi.YeuCauDichVuKyThuatId,
                            YeuCauDuocPhamBenhVienId = dataPhieuChi.YeuCauDuocPhamBenhVienId,
                            YeuCauVatTuBenhVienId = dataPhieuChi.YeuCauVatTuBenhVienId,
                            YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = dataPhieuChi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                            YeuCauTruyenMauId = dataPhieuChi.YeuCauTruyenMauId,


                            DonGiaBenhVien = dataPhieuChi.Gia.GetValueOrDefault(),
                            SoLuong = dataPhieuChi.SoLuong,
                            SoTienDV = Math.Round((decimal)dataPhieuChi.SoLuong.GetValueOrDefault() * (dataPhieuChi.Gia.GetValueOrDefault()), 2, MidpointRounding.AwayFromZero) - dataPhieuChi.SoTienMienGiam.GetValueOrDefault(),


                            MaNguoiBenh = dataPhieuThu.MaBN,
                            SoBienLaiThuTien = dataPhieuThu.SoPhieuHienThi,
                            TinhTrangThanhToan = !(dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() != 0 || dataPhieuThu.CongNo.GetValueOrDefault() != 0),


                            //DichVuKhamBenhId = item.YeuCauKhamBenhId != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : (long?)null,
                            //DichVuKyThuatId = item.YeuCauDichVuKyThuatId != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : (long?)null,
                            //DuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId != null ? item.YeuCauDuocPhamBenhVien.DuocPhamBenhVienId : (long?)null,
                            //VatTuBenhVienId = item.YeuCauVatTuBenhVienId != null ? item.YeuCauVatTuBenhVien.VatTuBenhVienId : (long?)null,
                            //DichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId != null
                            //? item.YeuCauDichVuGiuongBenhVien.DichVuGiuongBenhVienId
                            //: (item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null ? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId : (long?)null),
                            //TruyenMauId = item.YeuCauTruyenMauId != null ? item.YeuCauTruyenMau.MauVaChePhamId : (long?)null
                        };

                        BaoCaoDoanhThuTheoNhomDichVuDataDichVu thongTinDichVu = null;                        
                        if (dataPhieuChi.YeuCauKhamBenhId != null)
                        {
                            thongTinDichVu = dataDichVuKhamBenh.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauKhamBenhId);
                            baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.BSKham = thongTinDichVu?.BacSiKham;
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHoaHongDichVuKhamBenhs = hopDong.ChiTietHoaHongDichVuKhamBenhs
                                        .Where(o => o.DichVuKhamBenhBenhVienId == thongTinDichVu.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVu.NhomGiaDichVuKhamBenhBenhVienId)
                                        .ToList();
                                    if (chiTietHoaHongDichVuKhamBenhs.Any())
                                    {
                                        //tinh so lan dv có hoa hồng 
                                        var soLanDaTinhHoaHong = thongTinDichVuKhamBenhTinhHoaHongs
                                            .Where(o=>o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId && dataPhieuThu.NgayThu.Month == o.Thang && o.DichVuKhamBenhBenhVienId == thongTinDichVu.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVu.NhomGiaDichVuKhamBenhBenhVienId)
                                            .Select(o=>o.SoLuong).DefaultIfEmpty().Sum();
                                        var lanTinhHoaHong = (int)Math.Floor(soLanDaTinhHoaHong) + 1;
                                        var chiTietHoaHongDichVuKhamBenh = chiTietHoaHongDichVuKhamBenhs
                                            .Where(o => o.ApDungTuLan <= lanTinhHoaHong && (o.ApDungDenLan == null || lanTinhHoaHong <= o.ApDungDenLan))
                                            .OrderBy(o => o.ApDungTuLan)
                                            .FirstOrDefault();
                                        if(chiTietHoaHongDichVuKhamBenh != null)
                                        {
                                            if(chiTietHoaHongDichVuKhamBenh.LoaiHoaHong == Enums.LoaiHoaHong.TiLe && chiTietHoaHongDichVuKhamBenh.TiLeHoaHong != null)
                                            {
                                                baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.HoaHong = $"{chiTietHoaHongDichVuKhamBenh.TiLeHoaHong.Value.ToString("#.##")}%";
                                                baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = Math.Round(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV * chiTietHoaHongDichVuKhamBenh.TiLeHoaHong.GetValueOrDefault() / 100, 2, MidpointRounding.AwayFromZero);
                                            }
                                            else if (chiTietHoaHongDichVuKhamBenh.LoaiHoaHong == Enums.LoaiHoaHong.SoTien && chiTietHoaHongDichVuKhamBenh.SoTienHoaHong != null)
                                            {
                                                var soTienHoaHong = chiTietHoaHongDichVuKhamBenh.SoTienHoaHong.Value * (decimal)baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoLuong.GetValueOrDefault();
                                                if(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV <= soTienHoaHong)
                                                {
                                                    baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = 0;
                                                }
                                                else
                                                {
                                                    baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = soTienHoaHong;
                                                }
                                            }
                                        }
                                        //lưu đã tính hoa hồng
                                        thongTinDichVuKhamBenhTinhHoaHongs.Add(new ThongTinDichVuKhamBenhTinhHoaHong
                                        {
                                            NoiGioiThieuId = dataPhieuThu.NoiGioiThieuId.Value,
                                            DichVuKhamBenhBenhVienId = thongTinDichVu.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                            NhomGiaDichVuKhamBenhBenhVienId = thongTinDichVu.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                            Thang = dataPhieuThu.NgayThu.Month,
                                            SoLuong = baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoLuong.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                        }
                        else if (dataPhieuChi.YeuCauDichVuKyThuatId != null)
                        {
                            thongTinDichVu = dataDichVuKyThuat.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDichVuKyThuatId);
                            baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.BSKham = thongTinDichVu?.BacSiKham;
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHoaHongDichVuKyThuats = hopDong.ChiTietHoaHongDichVuKyThuats
                                        .Where(o => o.DichVuKyThuatBenhVienId == thongTinDichVu.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == thongTinDichVu.NhomGiaDichVuKyThuatBenhVienId)
                                        .ToList();
                                    if (chiTietHoaHongDichVuKyThuats.Any())
                                    {
                                        //tinh so lan dv có hoa hồng 
                                        var soLanDaTinhHoaHong = thongTinDichVuKyThuatTinhHoaHongs
                                            .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId && dataPhieuThu.NgayThu.Month == o.Thang && o.DichVuKyThuatBenhVienId == thongTinDichVu.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == thongTinDichVu.NhomGiaDichVuKyThuatBenhVienId)
                                            .Select(o => o.SoLuong).DefaultIfEmpty().Sum();
                                        var lanTinhHoaHong = (int)Math.Floor(soLanDaTinhHoaHong) + 1;
                                        var chiTietHoaHongDichVuKyThuat = chiTietHoaHongDichVuKyThuats
                                            .Where(o => o.ApDungTuLan <= lanTinhHoaHong && (o.ApDungDenLan == null || lanTinhHoaHong <= o.ApDungDenLan))
                                            .OrderBy(o => o.ApDungTuLan)
                                            .FirstOrDefault();
                                        if (chiTietHoaHongDichVuKyThuat != null)
                                        {
                                            if (chiTietHoaHongDichVuKyThuat.LoaiHoaHong == Enums.LoaiHoaHong.TiLe && chiTietHoaHongDichVuKyThuat.TiLeHoaHong != null)
                                            {
                                                baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.HoaHong = $"{chiTietHoaHongDichVuKyThuat.TiLeHoaHong.Value.ToString("#.##")}%";
                                                baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = Math.Round(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV * chiTietHoaHongDichVuKyThuat.TiLeHoaHong.GetValueOrDefault() / 100, 2, MidpointRounding.AwayFromZero);
                                            }
                                            else if (chiTietHoaHongDichVuKyThuat.LoaiHoaHong == Enums.LoaiHoaHong.SoTien && chiTietHoaHongDichVuKyThuat.SoTienHoaHong != null)
                                            {
                                                var soTienHoaHong = chiTietHoaHongDichVuKyThuat.SoTienHoaHong.Value * (decimal)baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoLuong.GetValueOrDefault();
                                                if (baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV <= soTienHoaHong)
                                                {
                                                    baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = 0;
                                                }
                                                else
                                                {
                                                    baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = soTienHoaHong;
                                                }
                                            }
                                        }
                                        //lưu đã tính hoa hồng
                                        thongTinDichVuKyThuatTinhHoaHongs.Add(new ThongTinDichVuKyThuatTinhHoaHong
                                        {
                                            NoiGioiThieuId = dataPhieuThu.NoiGioiThieuId.Value,
                                            DichVuKyThuatBenhVienId = thongTinDichVu.DichVuKyThuatBenhVienId.GetValueOrDefault(),
                                            NhomGiaDichVuKyThuatBenhVienId = thongTinDichVu.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault(),
                                            Thang = dataPhieuThu.NgayThu.Month,
                                            SoLuong = baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoLuong.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                        }
                        else if (dataPhieuChi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null)
                        {
                            thongTinDichVu = dataDichVuGiuong.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId);
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHoaHongDichVuGiuongs = hopDong.ChiTietHoaHongDichVuGiuongs
                                        .Where(o => o.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaDichVuGiuongBenhVienId)
                                        .ToList();
                                    if (chiTietHoaHongDichVuGiuongs.Any())
                                    {
                                        //tinh so lan dv có hoa hồng 
                                        var soLanDaTinhHoaHong = thongTinDichVuGiuongTinhHoaHongs
                                            .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId && dataPhieuThu.NgayThu.Month == o.Thang && o.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaDichVuGiuongBenhVienId)
                                            .Select(o => o.SoLuong).DefaultIfEmpty().Sum();
                                        var lanTinhHoaHong = (int)Math.Floor(soLanDaTinhHoaHong) + 1;
                                        var chiTietHoaHongDichVuGiuong = chiTietHoaHongDichVuGiuongs
                                            .Where(o => o.ApDungTuLan <= lanTinhHoaHong && (o.ApDungDenLan == null || lanTinhHoaHong <= o.ApDungDenLan))
                                            .OrderBy(o => o.ApDungTuLan)
                                            .FirstOrDefault();
                                        if (chiTietHoaHongDichVuGiuong != null)
                                        {
                                            if (chiTietHoaHongDichVuGiuong.LoaiHoaHong == Enums.LoaiHoaHong.TiLe && chiTietHoaHongDichVuGiuong.TiLeHoaHong != null)
                                            {
                                                baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.HoaHong = $"{chiTietHoaHongDichVuGiuong.TiLeHoaHong.Value.ToString("#.##")}%";
                                                baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = Math.Round(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV * chiTietHoaHongDichVuGiuong.TiLeHoaHong.GetValueOrDefault() / 100, 2, MidpointRounding.AwayFromZero);
                                            }
                                            else if (chiTietHoaHongDichVuGiuong.LoaiHoaHong == Enums.LoaiHoaHong.SoTien && chiTietHoaHongDichVuGiuong.SoTienHoaHong != null)
                                            {
                                                var soTienHoaHong = chiTietHoaHongDichVuGiuong.SoTienHoaHong.Value * (decimal)baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoLuong.GetValueOrDefault();
                                                if (baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV <= soTienHoaHong)
                                                {
                                                    baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = 0;
                                                }
                                                else
                                                {
                                                    baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = soTienHoaHong;
                                                }
                                            }
                                        }
                                        //lưu đã tính hoa hồng
                                        thongTinDichVuGiuongTinhHoaHongs.Add(new ThongTinDichVuGiuongTinhHoaHong
                                        {
                                            NoiGioiThieuId = dataPhieuThu.NoiGioiThieuId.Value,
                                            DichVuGiuongBenhVienId = thongTinDichVu.DichVuGiuongBenhVienId.GetValueOrDefault(),
                                            NhomGiaDichVuGiuongBenhVienId = thongTinDichVu.NhomGiaDichVuGiuongBenhVienId.GetValueOrDefault(),
                                            Thang = dataPhieuThu.NgayThu.Month,
                                            SoLuong = baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoLuong.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                        }
                        else if (dataPhieuChi.YeuCauTruyenMauId != null)
                        {
                            thongTinDichVu = dataTruyenMau.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauTruyenMauId);
                        }
                        else if (dataPhieuChi.YeuCauDuocPhamBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauDuocPham.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDuocPhamBenhVienId);
                            baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.BSKham = thongTinDichVu?.BacSiKham;
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHoaHongDuocPham = hopDong.ChiTietHoaHongDuocPhams
                                        .Where(o => o.DuocPhamBenhVienId == thongTinDichVu.DuocPhamBenhVienId)
                                        .OrderBy(o=>o.TiLeHoaHong)
                                        .FirstOrDefault();
                                    if (chiTietHoaHongDuocPham != null)
                                    {                                        
                                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.HoaHong = $"{chiTietHoaHongDuocPham.TiLeHoaHong.ToString("#.##")}%";
                                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = Math.Round(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV * chiTietHoaHongDuocPham.TiLeHoaHong / 100, 2, MidpointRounding.AwayFromZero);                                        
                                    }
                                }
                            }
                        }
                        else if (dataPhieuChi.YeuCauVatTuBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauVatTu.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauVatTuBenhVienId);
                            baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.BSKham = thongTinDichVu?.BacSiKham;
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHoaHongVatTu = hopDong.ChiTietHoaHongVatTus
                                        .Where(o => o.VatTuBenhVienId == thongTinDichVu.VatTuBenhVienId)
                                        .OrderBy(o => o.TiLeHoaHong)
                                        .FirstOrDefault();
                                    if (chiTietHoaHongVatTu != null)
                                    {
                                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.HoaHong = $"{chiTietHoaHongVatTu.TiLeHoaHong.ToString("#.##")}%";
                                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.ThanhTienHoaHong = Math.Round(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.SoTienDV * chiTietHoaHongVatTu.TiLeHoaHong / 100, 2, MidpointRounding.AwayFromZero);
                                    }
                                }
                            }
                        }                        
                        if (thongTinDichVu == null)
                        {
                            thongTinDichVu = new BaoCaoDoanhThuTheoNhomDichVuDataDichVu()
                            {
                                TenDichVu = "",
                                YeuCauGiuong = true
                            };
                        }
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.TenDV = thongTinDichVu.TenDichVu;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.DichVuKhamBenhId = thongTinDichVu.DichVuKhamBenhBenhVienId;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.DichVuKyThuatId = thongTinDichVu.DichVuKyThuatBenhVienId;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.DuocPhamBenhVienId = thongTinDichVu.DuocPhamBenhVienId;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.VatTuBenhVienId = thongTinDichVu.VatTuBenhVienId;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.DichVuGiuongBenhVienId = thongTinDichVu.DichVuGiuongBenhVienId;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.TruyenMauId = thongTinDichVu.MauVaChePhamId;
                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.NhomDichVuConId = thongTinDichVu.NhomDichVuBenhVienId;

                        var hinhThucDen = "";
                        if (dataPhieuThu.NoiGioiThieuId != null)
                        {
                            var noiGioiThieu = dataNoiGioiThieu.First(o => o.Id == dataPhieuThu.NoiGioiThieuId);
                            hinhThucDen = $"{dataHinhThucDen.FirstOrDefault(o => o.Id == dataPhieuThu.HinhThucDenId)?.Ten}/ {noiGioiThieu.Ten + (!string.IsNullOrEmpty(noiGioiThieu.DonVi) ? $" - {noiGioiThieu.DonVi}" : "")}";
                        }
                        else
                        {
                            hinhThucDen = dataHinhThucDen.FirstOrDefault(o => o.Id == dataPhieuThu.HinhThucDenId)?.Ten;
                        }

                        baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo.HinhThucDenDisplay = hinhThucDen;

                        chiTietHoaHongCuaNguoiGioiThieuGridVos.Add(baoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo);
                    }
                }
                thongTinGioiThieus = chiTietHoaHongCuaNguoiGioiThieuGridVos
                        .GroupBy(x => new { x.NgayKham, x.MaTN, x.BenhNhanId, x.NhomDichVuTruocGroup, x.DichVuBenhVienId, x.SoBienLaiThuTien, x.DonGiaBenhVien, x.HoaHong }) //x.YeuCauDichVuBenhVienId,
                        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                        {
                            NgayKham = item.Key.NgayKham,
                            MaTN = item.Key.MaTN,
                            YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                            BenhNhanId = item.Key.BenhNhanId,
                            TenBN = item.First().TenBN,
                            NgaySinh = item.First().NgaySinh,
                            ThangSinh = item.First().ThangSinh,
                            NamSinh = item.First().NamSinh,
                            DiaChi = item.First().DiaChi,
                            BSKham = item.First().BSKham,
                            TenDV = item.First().TenDV,
                            SoTienDV = item.Sum(a => a.SoTienDV),
                            HoaHong = item.First().HoaHong,
                            ThanhTienHoaHong = item.Sum(a => a.ThanhTienHoaHong.GetValueOrDefault()),
                            NhomDichVuConId = item.First().NhomDichVuConId,
                            NhomDichVu = item.First().NhomDichVuTruocGroup,
                            MaNguoiBenh = item.First().MaNguoiBenh,
                            SoBienLaiThuTien = item.First().SoBienLaiThuTien,
                            TinhTrangThanhToan = item.First().TinhTrangThanhToan,
                            HinhThucDenDisplay = item.First().HinhThucDenDisplay,
                        })
                        .OrderBy(x => x.TenBN).ThenBy(x => x.NgayKham)
                        .ToList();

                var lstNhomBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
                long? nguoiBenhId = null;
                var stt = 1;
                foreach (var thongTin in thongTinGioiThieus)
                {
                    if (thongTin.BenhNhanId != nguoiBenhId)
                    {
                        thongTin.STT = stt++;
                        nguoiBenhId = thongTin.BenhNhanId;
                    }

                    if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                    {
                        thongTin.NhomDV = "Khám bệnh";
                    }
                    //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                    else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                    {
                        thongTin.NhomDV = "Giường";
                    }
                    else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham)
                    {
                        thongTin.NhomDV = "Thuốc";
                    }
                    else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao)
                    {
                        thongTin.NhomDV = "VTYT";
                    }
                    else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.TruyenMau)
                    {
                        thongTin.NhomDV = "Truyền máu";
                    }
                    //=======================================================================
                    else
                    {
                        if (thongTin.NhomDichVuConId != null)
                        {
                            thongTin.NhomDV = GetTenPhanNhom(lstNhomBenhVien, thongTin.NhomDichVuConId);
                        }
                    }                    
                }

                thongTinGioiThieus = thongTinGioiThieus
                    .Skip(queryInfo.Skip)
                    .Take(queryInfo.Take)
                    .ToList();

                //if (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                //{
                //    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                //    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                //                && x.BenhNhanId != null
                //                && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                //                && (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)
                //                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                //                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                //                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                //                                                                                      && b.DaHuy != true
                //                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                //                                                                                      && b.NgayChi >= tuNgay
                //                                                                                      && b.NgayChi <= denNgay)))
                //    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                //    {
                //        YeucauTiepNhanId = x.Id,
                //        BenhNhanId = x.BenhNhanId.Value,
                //        NoiGioiThieuId = x.NoiGioiThieuId,
                //        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                //        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                //        TenHinhThucDen = x.HinhThucDen.Ten,
                //        LaGioiThieu = x.NoiGioiThieuId != null
                //    })
                //    .OrderBy(x => x.ThoiDiemTiepNhan)
                //    .ToList();
                //}
                //else
                //{
                //    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                //    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                //                && x.NoiGioiThieuId != null
                //                && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                //                && (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || (x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))
                //                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                //                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                //                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                //                                                                                      && b.DaHuy != true
                //                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                //                                                                                      && b.NgayChi >= tuNgay
                //                                                                                      && b.NgayChi <= denNgay)))
                //    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                //    {
                //        YeucauTiepNhanId = x.Id,
                //        BenhNhanId = x.BenhNhanId.Value,
                //        NoiGioiThieuId = x.NoiGioiThieuId,
                //        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                //        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                //        TenHinhThucDen = x.HinhThucDen.Ten,
                //        LaGioiThieu = x.NoiGioiThieuId != null
                //    })
                //    .OrderBy(x => x.ThoiDiemTiepNhan)
                //    .ToList();
                //}

                //if (lstTiepNhanTheoNoiGioiThieu.Any())
                //{
                //    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();

                //    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                //        .Where(x => x.BenhNhanId != null
                //                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                //                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                //        .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                //        {
                //            YeucauTiepNhanId = x.Id,
                //            BenhNhanId = x.BenhNhanId.Value,
                //            NoiGioiThieuId = x.NoiGioiThieuId,
                //            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                //            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                //            TenHinhThucDen = x.HinhThucDen.Ten,
                //            LaGioiThieu = x.NoiGioiThieuId != null,

                //            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                //            LaDataTheoDieuKienTimKiem = (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                //                                        && (x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                //                                                                            && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                //                                                                            && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                //                                                                                                               && b.DaHuy != true
                //                                                                                                               && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                //                                                                                                               && b.NgayChi >= tuNgay
                //                                                                                                               && b.NgayChi <= denNgay)))
                //        })
                //        .OrderBy(x => x.ThoiDiemTiepNhan)
                //        .ToList();

                //    foreach (var benhNhanId in lstBenhNhanId)
                //    {
                //        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                //            .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                //            .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                //        if (lanTiepNhanDauTienCoGioiThieu == null)
                //        {
                //            lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                //                .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                //                .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();

                //            if (lanTiepNhanDauTienCoGioiThieu == null)
                //            {
                //                continue;
                //            }
                //        }

                //        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                //            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId
                //                        && x.BenhNhanId == benhNhanId)
                //            .ToList();

                //        var khongThemNguoiGioiThieu = false;
                //        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuId;
                //        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuDisplay;
                //        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                //        {
                //            if (lanTiepNhan.NoiGioiThieuId != null)
                //            {
                //                if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId)
                //                    || ((timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0) && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId))
                //                {
                //                    // trường hợp tìm kiếm theo người giới thiệu thì chỉ thêm người giới thiệu đang tìm kiếm thôi
                //                    if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId))
                //                    {
                //                        khongThemNguoiGioiThieu = true;
                //                    }
                //                    else
                //                    {
                //                        khongThemNguoiGioiThieu = false;
                //                    }
                //                    nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                //                    tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                //                }
                //                else
                //                {
                //                    khongThemNguoiGioiThieu = false;
                //                }
                //            }

                //            if (!khongThemNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null)
                //            {
                //                if (lstTiepNhanTheoNoiGioiThieu.All(x => x.YeucauTiepNhanId != lanTiepNhan.YeucauTiepNhanId) && lanTiepNhan.LaDataTheoDieuKienTimKiem == true)
                //                {
                //                    lanTiepNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                //                    lanTiepNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                //                    lanTiepNhan.LaGioiThieu = true;
                //                    lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                //                }
                //                else
                //                {
                //                    var lanTiepNhanDaThem = lstTiepNhanTheoNoiGioiThieu.FirstOrDefault(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId);
                //                    if (lanTiepNhanDaThem != null)
                //                    {
                //                        lanTiepNhanDaThem.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                //                        lanTiepNhanDaThem.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                //                        lanTiepNhanDaThem.LaGioiThieu = true;
                //                    }

                //                }
                //            }
                //        }
                //    }

                //var lstTiepNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                //    thongTinGioiThieus = _taiKhoanBenhNhanChiRepository.TableNoTracking
                //        .Where(x => x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                //                    && lstTiepNhanId.Contains(x.TaiKhoanBenhNhanThu.YeuCauTiepNhanId)
                //                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                //                    && x.TaiKhoanBenhNhanThu.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                //                    && x.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId != null
                //                    && x.NgayChi >= tuNgay
                //                    && x.NgayChi <= denNgay
                //                    && x.DaHuy != true
                //                    && (x.YeuCauKhamBenhId != null
                //                        || x.YeuCauDichVuKyThuatId != null
                //                        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //                        || x.YeuCauDuocPhamBenhVienId != null
                //                        || x.YeuCauVatTuBenhVienId != null
                //                        || x.YeuCauDichVuGiuongBenhVienId != null
                //                        || x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null
                //                        || x.YeuCauTruyenMauId != null))
                //        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                //        {
                //            NgayKham = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThoiDiemTiepNhan,
                //            MaTN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                //            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                //            BenhNhanId = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId.Value,
                //            TenBN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.HoTen,
                //            NgaySinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NgaySinh,
                //            ThangSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThangSinh,
                //            NamSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NamSinh,
                //            DiaChi = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.DiaChiDayDu,
                //            BSKham = item.YeuCauKhamBenh.BacSiThucHien.User.HoTen ?? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                //                    //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //                    ?? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                //                    ?? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                //                    ?? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                //                    ?? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen,

                //            TenDV = item.YeuCauKhamBenh.TenDichVu ?? item.YeuCauDichVuKyThuat.TenDichVu
                //                    //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //                    ?? item.YeuCauDuocPhamBenhVien.Ten
                //                    ?? item.YeuCauVatTuBenhVien.Ten
                //                    ?? item.YeuCauDichVuGiuongBenhVien.Ten
                //                    ?? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ten
                //                    ?? item.YeuCauTruyenMau.TenDichVu,
                //            NhomDichVuConId = item.YeuCauDichVuKyThuat.NhomDichVuBenhVienId,
                //            //NhomDichVu = item.YeuCauKhamBenhId != null ? Enums.EnumNhomGoiDichVu.DichVuKhamBenh : Enums.EnumNhomGoiDichVu.DichVuKyThuat,

                //            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //            YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                //            YeuCauDichVuKyThuatId = item.YeuCauDichVuKyThuatId,
                //            YeuCauDuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId,
                //            YeuCauVatTuBenhVienId = item.YeuCauVatTuBenhVienId,
                //            YeuCauDichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId,
                //            YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                //            YeuCauTruyenMauId = item.YeuCauTruyenMauId,

                //            //YeuCauDichVuBenhVienId = item.YeuCauKhamBenhId ?? item.YeuCauDichVuKyThuatId.Value,
                //            //SoTienDV = item.TienChiPhi ?? 0,
                //            DonGiaBenhVien = item.Gia.GetValueOrDefault(),
                //            SoTienDV = Convert.ToDecimal(item.SoLuong.GetValueOrDefault() * (double)(item.Gia.GetValueOrDefault())) - item.SoTienMienGiam.GetValueOrDefault(),

                //            //Cập nhật: sau khi quyery list data, thì foreach gán value vào sau
                //            //HinhThucDenDisplay = lstTiepNhanTheoNoiGioiThieu.Any(x => x.YeucauTiepNhanId == item.YeuCauTiepNhanId) 
                //            //    ? lstTiepNhanTheoNoiGioiThieu.Where(x => x.YeucauTiepNhanId == item.TaiKhoanBenhNhanThu.YeuCauTiepNhanId).Select(a => a.HinhThucDenDisplay).First() : null,
                //            MaNguoiBenh = item.YeuCauTiepNhan.BenhNhan.MaBN,
                //            SoBienLaiThuTien = item.SoPhieuHienThi,
                //            TinhTrangThanhToan = !(item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() != 0 || item.TaiKhoanBenhNhanThu.CongNo.GetValueOrDefault() != 0),

                //            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //            //DichVuBenhVienId = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                //            DichVuKhamBenhId = item.YeuCauKhamBenhId != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : (long?)null,
                //            DichVuKyThuatId = item.YeuCauDichVuKyThuatId != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : (long?)null,
                //            DuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId != null ? item.YeuCauDuocPhamBenhVien.DuocPhamBenhVienId : (long?)null,
                //            VatTuBenhVienId = item.YeuCauVatTuBenhVienId != null ? item.YeuCauVatTuBenhVien.VatTuBenhVienId : (long?)null,
                //            DichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId != null
                //                ? item.YeuCauDichVuGiuongBenhVien.DichVuGiuongBenhVienId
                //                : (item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null ? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId : (long?)null),
                //            TruyenMauId = item.YeuCauTruyenMauId != null ? item.YeuCauTruyenMau.MauVaChePhamId : (long?)null
                //        }).ToList();

                //    thongTinGioiThieus = thongTinGioiThieus
                //        .GroupBy(x => new { x.NgayKham, x.MaTN, x.BenhNhanId, x.NhomDichVuTruocGroup, x.DichVuBenhVienId, x.SoBienLaiThuTien, x.DonGiaBenhVien }) //x.YeuCauDichVuBenhVienId,
                //        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                //        {
                //            NgayKham = item.Key.NgayKham,
                //            MaTN = item.Key.MaTN,
                //            YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                //            BenhNhanId = item.Key.BenhNhanId,
                //            TenBN = item.First().TenBN,
                //            NgaySinh = item.First().NgaySinh,
                //            ThangSinh = item.First().ThangSinh,
                //            NamSinh = item.First().NamSinh,
                //            DiaChi = item.First().DiaChi,
                //            BSKham = item.First().BSKham,
                //            TenDV = item.First().TenDV,
                //            SoTienDV = item.Sum(a => a.SoTienDV),
                //            NhomDichVuConId = item.First().NhomDichVuConId,

                //            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //            NhomDichVu = item.First().NhomDichVuTruocGroup,

                //            //Cập nhật: sau khi quyery list data, thì foreach gán value vào sau
                //            //HinhThucDenDisplay = item.First().HinhThucDenDisplay,
                //            MaNguoiBenh = item.First().MaNguoiBenh,
                //            SoBienLaiThuTien = item.First().SoBienLaiThuTien,
                //            TinhTrangThanhToan = item.First().TinhTrangThanhToan
                //        })
                //        .OrderBy(x => x.TenBN).ThenBy(x => x.NgayKham)
                //        //.Skip(queryInfo.Skip)
                //        //.Take(queryInfo.Take)
                //        .ToList();

                //    var lstNhomBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
                //    long? nguoiBenhId = null;
                //    var stt = 1;
                //    foreach (var thongTin in thongTinGioiThieus)
                //    {
                //        if (thongTin.BenhNhanId != nguoiBenhId)
                //        {
                //            thongTin.STT = stt++;
                //            nguoiBenhId = thongTin.BenhNhanId;
                //        }

                //        if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                //        {
                //            thongTin.NhomDV = "Khám bệnh";
                //        }
                //        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                //        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                //        {
                //            thongTin.NhomDV = "Giường";
                //        }
                //        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham)
                //        {
                //            thongTin.NhomDV = "Thuốc";
                //        }
                //        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao)
                //        {
                //            thongTin.NhomDV = "VTYT";
                //        }
                //        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.TruyenMau)
                //        {
                //            thongTin.NhomDV = "Truyền máu";
                //        }
                //        //=======================================================================
                //        else
                //        {
                //            if (thongTin.NhomDichVuConId != null)
                //            {
                //                thongTin.NhomDV = GetTenPhanNhom(lstNhomBenhVien, thongTin.NhomDichVuConId);
                //            }
                //        }


                //        //Cập nhật: sau khi quyery list data, thì foreach gán value vào sau
                //        var hinhThucDen = lstTiepNhanTheoNoiGioiThieu
                //            .Where(x => x.YeucauTiepNhanId == thongTin.YeuCauTiepNhanId)
                //            .Select(a => a.HinhThucDenDisplay).FirstOrDefault();
                //        thongTin.HinhThucDenDisplay = hinhThucDen;
                //    }

                //    thongTinGioiThieus = thongTinGioiThieus
                //        .Skip(queryInfo.Skip)
                //        .Take(queryInfo.Take)
                //        .ToList();                
            }

            return new GridDataSource
            {
                Data = thongTinGioiThieus.ToArray(),
                TotalRowCount = thongTinGioiThieus.Count()
            };
        }
        public async Task<GridDataSource> GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync(QueryInfo queryInfo)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<ThongTinTiepNhanCoGioiThieuVo>();
            var thongTinGioiThieus = new List<BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo>();
            var timKiemNangCaoObj = new BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo>(queryInfo.AdditionalSearchString);
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

            if (tuNgay != null && denNgay != null)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                if (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.BenhNhanId != null
                                && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                && (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)
                                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true 
                                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi 
                                                                                                      && b.DaHuy != true 
                                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                                                                                                      && b.NgayChi >= tuNgay 
                                                                                                      && b.NgayChi <= denNgay)))
                    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        BenhNhanId = x.BenhNhanId.Value,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                }
                else
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.NoiGioiThieuId != null
                                && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                && (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || (x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))
                                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                                                                                      && b.DaHuy != true
                                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                                                                                                      && b.NgayChi >= tuNgay
                                                                                                      && b.NgayChi <= denNgay)))
                    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        BenhNhanId = x.BenhNhanId.Value,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                }

                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    //var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    //    .Where(x => x.BenhNhanId != null 
                    //                && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                    //                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                    //                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                    //                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                    //                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                    //                                                                                      && b.DaHuy != true
                    //                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                    //                                                                                      && b.NgayChi >= tuNgay
                    //                                                                                      && b.NgayChi <= denNgay)))
                    //    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                    //    {
                    //        YeucauTiepNhanId = x.Id,
                    //        BenhNhanId = x.BenhNhanId.Value,
                    //        NoiGioiThieuId = x.NoiGioiThieuId,
                    //        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                    //    })
                    //    .ToList();

                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            BenhNhanId = x.BenhNhanId.Value,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,

                            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                            LaDataTheoDieuKienTimKiem = (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                                        && (x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                                                                            && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                                                            && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                                                                                                               && b.DaHuy != true
                                                                                                                               && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                                                                                                                               && b.NgayChi >= tuNgay
                                                                                                                               && b.NgayChi <= denNgay)))
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

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

                    var lstTiepNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                    thongTinGioiThieus = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        .Where(x => x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi 
                                    && lstTiepNhanId.Contains(x.TaiKhoanBenhNhanThu.YeuCauTiepNhanId)
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                    && x.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId != null
                                    && x.NgayChi >= tuNgay
                                    && x.NgayChi <= denNgay
                                    && x.DaHuy != true
                                    && (x.YeuCauKhamBenhId != null 
                                        || x.YeuCauDichVuKyThuatId != null
                                        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                                        || x.YeuCauDuocPhamBenhVienId != null 
                                        || x.YeuCauVatTuBenhVienId != null 
                                        || x.YeuCauDichVuGiuongBenhVienId != null 
                                        || x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null
                                        || x.YeuCauTruyenMauId != null))
                        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                        {
                            NgayKham = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThoiDiemTiepNhan,
                            MaTN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                            BenhNhanId = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId.Value,
                            TenBN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.HoTen,
                            NgaySinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NgaySinh,
                            ThangSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThangSinh,
                            NamSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NamSinh,
                            DiaChi = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.DiaChiDayDu,
                            BSKham = item.YeuCauKhamBenh.BacSiThucHien.User.HoTen ?? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                                    //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                                    ?? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                                    ?? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                                    ?? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                                    ?? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen,

                            TenDV = item.YeuCauKhamBenh.TenDichVu ?? item.YeuCauDichVuKyThuat.TenDichVu
                                    //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                                    ?? item.YeuCauDuocPhamBenhVien.Ten
                                    ?? item.YeuCauVatTuBenhVien.Ten
                                    ?? item.YeuCauDichVuGiuongBenhVien.Ten
                                    ?? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ten
                                    ?? item.YeuCauTruyenMau.TenDichVu,
                            NhomDichVuConId = item.YeuCauDichVuKyThuat.NhomDichVuBenhVienId,
                            //NhomDichVu = item.YeuCauKhamBenhId != null ? Enums.EnumNhomGoiDichVu.DichVuKhamBenh : Enums.EnumNhomGoiDichVu.DichVuKyThuat,

                            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                            YeuCauDichVuKyThuatId = item.YeuCauDichVuKyThuatId,
                            YeuCauDuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId,
                            YeuCauVatTuBenhVienId = item.YeuCauVatTuBenhVienId,
                            YeuCauDichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId,
                            YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                            YeuCauTruyenMauId = item.YeuCauTruyenMauId,

                            //YeuCauDichVuBenhVienId = item.YeuCauKhamBenhId ?? item.YeuCauDichVuKyThuatId.Value,
                            //SoTienDV = item.TienChiPhi ?? 0,
                            DonGiaBenhVien = item.Gia.GetValueOrDefault(),
                            SoTienDV = Convert.ToDecimal(item.SoLuong.GetValueOrDefault() * (double)(item.Gia.GetValueOrDefault())) - item.SoTienMienGiam.GetValueOrDefault(),

                            //Cập nhật: sau khi quyery list data, thì foreach gán value vào sau
                            //HinhThucDenDisplay = lstTiepNhanTheoNoiGioiThieu.Any(x => x.YeucauTiepNhanId == item.YeuCauTiepNhanId) 
                            //    ? lstTiepNhanTheoNoiGioiThieu.Where(x => x.YeucauTiepNhanId == item.TaiKhoanBenhNhanThu.YeuCauTiepNhanId).Select(a => a.HinhThucDenDisplay).First() : null,
                            MaNguoiBenh = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            SoBienLaiThuTien = item.SoPhieuHienThi,
                            TinhTrangThanhToan = !(item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() != 0 || item.TaiKhoanBenhNhanThu.CongNo.GetValueOrDefault() != 0),

                            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            //DichVuBenhVienId = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                            DichVuKhamBenhId = item.YeuCauKhamBenhId != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : (long?)null,
                            DichVuKyThuatId = item.YeuCauDichVuKyThuatId != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : (long?)null,
                            DuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId != null ? item.YeuCauDuocPhamBenhVien.DuocPhamBenhVienId : (long?)null,
                            VatTuBenhVienId = item.YeuCauVatTuBenhVienId != null ? item.YeuCauVatTuBenhVien.VatTuBenhVienId : (long?)null,
                            DichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId != null 
                                ? item.YeuCauDichVuGiuongBenhVien.DichVuGiuongBenhVienId 
                                : (item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null ? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId : (long?)null),
                            TruyenMauId = item.YeuCauTruyenMauId != null ? item.YeuCauTruyenMau.MauVaChePhamId : (long?)null
                        }).ToList();

                    thongTinGioiThieus = thongTinGioiThieus
                        .GroupBy(x => new {x.NgayKham, x.MaTN, x.BenhNhanId, x.NhomDichVuTruocGroup, x.DichVuBenhVienId, x.SoBienLaiThuTien, x.DonGiaBenhVien}) //x.YeuCauDichVuBenhVienId,
                        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                        {
                            NgayKham = item.Key.NgayKham,
                            MaTN = item.Key.MaTN,
                            YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                            BenhNhanId = item.Key.BenhNhanId,
                            TenBN = item.First().TenBN,
                            NgaySinh = item.First().NgaySinh,
                            ThangSinh = item.First().ThangSinh,
                            NamSinh = item.First().NamSinh,
                            DiaChi = item.First().DiaChi,
                            BSKham = item.First().BSKham,
                            TenDV = item.First().TenDV,
                            SoTienDV = item.Sum(a => a.SoTienDV),
                            NhomDichVuConId = item.First().NhomDichVuConId,

                            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            NhomDichVu = item.First().NhomDichVuTruocGroup,

                            //Cập nhật: sau khi quyery list data, thì foreach gán value vào sau
                            //HinhThucDenDisplay = item.First().HinhThucDenDisplay,
                            MaNguoiBenh = item.First().MaNguoiBenh,
                            SoBienLaiThuTien = item.First().SoBienLaiThuTien,
                            TinhTrangThanhToan = item.First().TinhTrangThanhToan
                        })
                        .OrderBy(x => x.TenBN).ThenBy(x => x.NgayKham)
                        //.Skip(queryInfo.Skip)
                        //.Take(queryInfo.Take)
                        .ToList();

                    var lstNhomBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
                    long? nguoiBenhId = null;
                    var stt = 1;
                    foreach (var thongTin in thongTinGioiThieus)
                    {
                        if (thongTin.BenhNhanId != nguoiBenhId)
                        {
                            thongTin.STT = stt++;
                            nguoiBenhId = thongTin.BenhNhanId;
                        }

                        if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                        {
                            thongTin.NhomDV = "Khám bệnh";
                        }
                        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                        else if(thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                        {
                            thongTin.NhomDV = "Giường";
                        }
                        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham)
                        {
                            thongTin.NhomDV = "Thuốc";
                        }
                        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao)
                        {
                            thongTin.NhomDV = "VTYT";
                        }
                        else if (thongTin.NhomDichVu == Enums.EnumNhomGoiDichVu.TruyenMau)
                        {
                            thongTin.NhomDV = "Truyền máu";
                        }
                        //=======================================================================
                        else
                        {
                            if (thongTin.NhomDichVuConId != null)
                            {
                                thongTin.NhomDV = GetTenPhanNhom(lstNhomBenhVien, thongTin.NhomDichVuConId);
                            }
                        }


                        //Cập nhật: sau khi quyery list data, thì foreach gán value vào sau
                        var hinhThucDen = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.YeucauTiepNhanId == thongTin.YeuCauTiepNhanId)
                            .Select(a => a.HinhThucDenDisplay).FirstOrDefault();
                        thongTin.HinhThucDenDisplay = hinhThucDen;
                    }

                    thongTinGioiThieus = thongTinGioiThieus
                        .Skip(queryInfo.Skip)
                        .Take(queryInfo.Take)
                        .ToList();

                }
            }

            return new GridDataSource
            {
                Data = thongTinGioiThieus.ToArray(),
                TotalRowCount = thongTinGioiThieus.Count()
            };
        }

        private string GetTenPhanNhom(List<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> lstNhomDichVuBenhViens, long? phanNhomId)
        {
            if (phanNhomId != null)
            {
                var phanNhom = lstNhomDichVuBenhViens.Where(x => x.Id == phanNhomId).FirstOrDefault();
                if (phanNhom != null)
                {
                    if (phanNhom.NhomDichVuBenhVienChaId == null)
                    {
                        return phanNhom.Ten;
                    }
                    else
                    {
                        return GetTenPhanNhom(lstNhomDichVuBenhViens, phanNhom.NhomDichVuBenhVienChaId ?? 0);
                    }
                }
            }

            return "Khác";
        }

        public async Task<GridDataSource> GetDataTotalPageBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync(QueryInfo queryInfo)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<ThongTinTiepNhanCoGioiThieuVo>();
            var timKiemNangCaoObj = new BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo>(queryInfo.AdditionalSearchString);
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

            //if (timKiemNangCaoObj.NoiGioiThieuId != null && tuNgay != null && denNgay != null)
            if (tuNgay != null && denNgay != null)
            {
                //var lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                //    .Where(x => x.NoiGioiThieuId != null
                //                && x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId
                //                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                //                && x.BenhNhanId != null
                //                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                //                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                //                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                //                                                                                      && b.DaHuy != true
                //                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                //                                                                                      && b.NgayChi >= tuNgay
                //                                                                                      && b.NgayChi <= denNgay)))
                //    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                //    {
                //        YeucauTiepNhanId = x.Id,
                //        BenhNhanId = x.BenhNhanId.Value,
                //        NoiGioiThieuId = x.NoiGioiThieuId,
                //        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                //    })
                //    .ToList();
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                if (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.BenhNhanId != null
                                && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                && (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)
                                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                                                                                      && b.DaHuy != true
                                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                                                                                                      && b.NgayChi >= tuNgay
                                                                                                      && b.NgayChi <= denNgay)))
                    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        BenhNhanId = x.BenhNhanId.Value,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                }
                else
                {
                    lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.NoiGioiThieuId != null
                                && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                && (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || (x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))
                                && x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                                                   && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                                   && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                                                                                      && b.DaHuy != true
                                                                                                      && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                                                                                                      && b.NgayChi >= tuNgay
                                                                                                      && b.NgayChi <= denNgay)))
                    .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                    {
                        YeucauTiepNhanId = x.Id,
                        BenhNhanId = x.BenhNhanId.Value,
                        NoiGioiThieuId = x.NoiGioiThieuId,
                        ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                        NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                        TenHinhThucDen = x.HinhThucDen.Ten,
                        LaGioiThieu = x.NoiGioiThieuId != null
                    })
                    .OrderBy(x => x.ThoiDiemTiepNhan)
                    .ToList();
                }

                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            BenhNhanId = x.BenhNhanId.Value,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,

                            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                            LaDataTheoDieuKienTimKiem = (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                                        && (x.TaiKhoanBenhNhanThus.Any(a => a.DaHuy != true
                                                                                            && a.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                                                            && a.TaiKhoanBenhNhanChis.Any(b => b.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                                                                                                               && b.DaHuy != true
                                                                                                                               && (b.YeuCauKhamBenhId != null || b.YeuCauDichVuKyThuatId != null)
                                                                                                                               && b.NgayChi >= tuNgay
                                                                                                                               && b.NgayChi <= denNgay)))
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

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

                    var lstTiepNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                    //var thongTinGioiThieus = _taiKhoanBenhNhanChiRepository.TableNoTracking
                    //    .Where(x => x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                    //                && lstTiepNhanId.Contains(x.TaiKhoanBenhNhanThu.YeuCauTiepNhanId)
                    //                && x.TaiKhoanBenhNhanThu.DaHuy != true
                    //                && x.TaiKhoanBenhNhanThu.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                    //                && x.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId != null
                    //                && x.NgayChi >= tuNgay
                    //                && x.NgayChi <= denNgay
                    //                && x.DaHuy != true
                    //                && (x.YeuCauKhamBenhId != null || x.YeuCauDichVuKyThuatId != null))
                    //    .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                    //    {
                    //        NgayKham = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    //        MaTN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //        BenhNhanId = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId.Value,
                    //        TenBN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.HoTen,
                    //        NgaySinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NgaySinh,
                    //        ThangSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThangSinh,
                    //        NamSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NamSinh,
                    //        DiaChi = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.DiaChiDayDu,
                    //        BSKham = item.YeuCauKhamBenh.BacSiThucHien.User.HoTen ?? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen,
                    //        TenDV = item.YeuCauKhamBenh.TenDichVu ?? item.YeuCauDichVuKyThuat.TenDichVu,
                    //        NhomDichVuConId = item.YeuCauDichVuKyThuat.NhomDichVuBenhVienId,
                    //        NhomDichVu = item.YeuCauKhamBenh != null ? Enums.EnumNhomGoiDichVu.DichVuKhamBenh : Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    //        YeuCauDichVuBenhVienId = item.YeuCauKhamBenhId ?? item.YeuCauDichVuKyThuatId.Value,
                    //        //SoTienDV = item.TienChiPhi ?? 0,
                    //        DonGiaBenhVien = item.Gia.GetValueOrDefault(),
                    //        SoTienDV = Convert.ToDecimal(item.SoLuong.GetValueOrDefault() * (double)(item.Gia.GetValueOrDefault())) - item.SoTienMienGiam.GetValueOrDefault(),

                    //        HinhThucDenDisplay = lstTiepNhanTheoNoiGioiThieu.Any(x => x.YeucauTiepNhanId == item.YeuCauTiepNhanId)
                    //            ? lstTiepNhanTheoNoiGioiThieu.Where(x => x.YeucauTiepNhanId == item.TaiKhoanBenhNhanThu.YeuCauTiepNhanId).Select(a => a.HinhThucDenDisplay).First() : null,
                    //        MaNguoiBenh = item.YeuCauTiepNhan.BenhNhan.MaBN,
                    //        SoBienLaiThuTien = item.SoPhieuHienThi,
                    //        TinhTrangThanhToan = !(item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() != 0 || item.TaiKhoanBenhNhanThu.CongNo.GetValueOrDefault() != 0),
                    //        //DichVuBenhVienId = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                    //    })
                    //    .GroupBy(x => new { x.NgayKham, x.MaTN, x.BenhNhanId, x.NhomDichVu, x.DichVuBenhVienId, x.SoBienLaiThuTien, x.DonGiaBenhVien }) //x.YeuCauDichVuBenhVienId,
                    //    .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                    //    {
                    //        NgayKham = item.Key.NgayKham,
                    //        MaTN = item.Key.MaTN,
                    //        BenhNhanId = item.Key.BenhNhanId,
                    //        TenBN = item.First().TenBN,
                    //        NgaySinh = item.First().NgaySinh,
                    //        ThangSinh = item.First().ThangSinh,
                    //        NamSinh = item.First().NamSinh,
                    //        DiaChi = item.First().DiaChi,
                    //        BSKham = item.First().BSKham,
                    //        TenDV = item.First().TenDV,
                    //        SoTienDV = item.Sum(a => a.SoTienDV),
                    //        NhomDichVuConId = item.First().NhomDichVuConId,
                    //        NhomDichVu = item.First().NhomDichVu,

                    //        HinhThucDenDisplay = item.First().HinhThucDenDisplay,
                    //        MaNguoiBenh = item.First().MaNguoiBenh,
                    //        SoBienLaiThuTien = item.First().SoBienLaiThuTien,
                    //        TinhTrangThanhToan = item.First().TinhTrangThanhToan
                    //    });

                    var thongTinGioiThieus = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        .Where(x => x.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi
                                    && lstTiepNhanId.Contains(x.TaiKhoanBenhNhanThu.YeuCauTiepNhanId)
                                    && x.TaiKhoanBenhNhanThu.DaHuy != true
                                    && x.TaiKhoanBenhNhanThu.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                    && x.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId != null
                                    && x.NgayChi >= tuNgay
                                    && x.NgayChi <= denNgay
                                    && x.DaHuy != true
                                    && (x.YeuCauKhamBenhId != null
                                        || x.YeuCauDichVuKyThuatId != null
                                        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                                        || x.YeuCauDuocPhamBenhVienId != null
                                        || x.YeuCauVatTuBenhVienId != null
                                        || x.YeuCauDichVuGiuongBenhVienId != null
                                        || x.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null
                                        || x.YeuCauTruyenMauId != null))
                        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                        {
                            NgayKham = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThoiDiemTiepNhan,
                            MaTN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            BenhNhanId = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.BenhNhanId.Value,
                            TenBN = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.HoTen,
                            NgaySinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NgaySinh,
                            ThangSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.ThangSinh,
                            NamSinh = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.NamSinh,
                            DiaChi = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.DiaChiDayDu,
                            //BSKham = item.YeuCauKhamBenh.BacSiThucHien.User.HoTen ?? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            //        ?? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //        ?? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //        ?? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.BacSiThucHien.User.HoTen
                            //        ?? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.BacSiThucHien.User.HoTen,

                            //TenDV = item.YeuCauKhamBenh.TenDichVu ?? item.YeuCauDichVuKyThuat.TenDichVu
                            //        //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            //        ?? item.YeuCauDuocPhamBenhVien.Ten
                            //        ?? item.YeuCauVatTuBenhVien.Ten
                            //        ?? item.YeuCauDichVuGiuongBenhVien.Ten
                            //        ?? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ten
                            //        ?? item.YeuCauTruyenMau.TenDichVu,
                            NhomDichVuConId = item.YeuCauDichVuKyThuat.NhomDichVuBenhVienId,
                            //NhomDichVu = item.YeuCauKhamBenhId != null ? Enums.EnumNhomGoiDichVu.DichVuKhamBenh : Enums.EnumNhomGoiDichVu.DichVuKyThuat,

                            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                            YeuCauDichVuKyThuatId = item.YeuCauDichVuKyThuatId,
                            YeuCauDuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId,
                            YeuCauVatTuBenhVienId = item.YeuCauVatTuBenhVienId,
                            YeuCauDichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId,
                            YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                            YeuCauTruyenMauId = item.YeuCauTruyenMauId,

                            //YeuCauDichVuBenhVienId = item.YeuCauKhamBenhId ?? item.YeuCauDichVuKyThuatId.Value,
                            //SoTienDV = item.TienChiPhi ?? 0,
                            DonGiaBenhVien = item.Gia.GetValueOrDefault(),
                            SoTienDV = Convert.ToDecimal(item.SoLuong.GetValueOrDefault() * (double)(item.Gia.GetValueOrDefault())) - item.SoTienMienGiam.GetValueOrDefault(),

                            //HinhThucDenDisplay = lstTiepNhanTheoNoiGioiThieu.Any(x => x.YeucauTiepNhanId == item.YeuCauTiepNhanId)
                            //    ? lstTiepNhanTheoNoiGioiThieu.Where(x => x.YeucauTiepNhanId == item.TaiKhoanBenhNhanThu.YeuCauTiepNhanId).Select(a => a.HinhThucDenDisplay).First() : null,
                            MaNguoiBenh = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            SoBienLaiThuTien = item.SoPhieuHienThi,
                            TinhTrangThanhToan = !(item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() != 0 || item.TaiKhoanBenhNhanThu.CongNo.GetValueOrDefault() != 0),

                            //30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            //DichVuBenhVienId = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                            DichVuKhamBenhId = item.YeuCauKhamBenhId != null ? item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId : (long?)null,
                            DichVuKyThuatId = item.YeuCauDichVuKyThuatId != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : (long?)null,
                            DuocPhamBenhVienId = item.YeuCauDuocPhamBenhVienId != null ? item.YeuCauDuocPhamBenhVien.DuocPhamBenhVienId : (long?)null,
                            VatTuBenhVienId = item.YeuCauVatTuBenhVienId != null ? item.YeuCauVatTuBenhVien.VatTuBenhVienId : (long?)null,
                            DichVuGiuongBenhVienId = item.YeuCauDichVuGiuongBenhVienId != null
                                ? item.YeuCauDichVuGiuongBenhVien.DichVuGiuongBenhVienId
                                : (item.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null ? item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId : (long?)null),
                            TruyenMauId = item.YeuCauTruyenMauId != null ? item.YeuCauTruyenMau.MauVaChePhamId : (long?)null
                        })
                        .GroupBy(x => new { x.NgayKham, x.MaTN, x.BenhNhanId, x.NhomDichVuTruocGroup, x.DichVuBenhVienId, x.SoBienLaiThuTien, x.DonGiaBenhVien }) //x.YeuCauDichVuBenhVienId,
                        .Select(item => new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo
                        {
                            NgayKham = item.Key.NgayKham,
                            MaTN = item.Key.MaTN,
                            BenhNhanId = item.Key.BenhNhanId,
                            //TenBN = item.First().TenBN,
                            //NgaySinh = item.First().NgaySinh,
                            //ThangSinh = item.First().ThangSinh,
                            //NamSinh = item.First().NamSinh,
                            //DiaChi = item.First().DiaChi,
                            //BSKham = item.First().BSKham,
                            //TenDV = item.First().TenDV,
                            //SoTienDV = item.Sum(a => a.SoTienDV),
                            //NhomDichVuConId = item.First().NhomDichVuConId,

                            ////30/11/2021: cập nhật theo yêu cầu: lấy tất cả dv trừ đơn thuốc ra về
                            //NhomDichVu = item.First().NhomDichVuTruocGroup,

                            //HinhThucDenDisplay = item.First().HinhThucDenDisplay,
                            //MaNguoiBenh = item.First().MaNguoiBenh,
                            //SoBienLaiThuTien = item.First().SoBienLaiThuTien,
                            //TinhTrangThanhToan = item.First().TinhTrangThanhToan
                        });
                    var countTask = thongTinGioiThieus.Count();
                    return new GridDataSource { TotalRowCount = countTask };
                }
                return new GridDataSource { TotalRowCount = 0 };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo>(query.AdditionalSearchString);
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

            //var noiGioiThieu = _noiGioiThieuRepository.TableNoTracking.FirstOrDefault(x => x.Id == timKiemNangCaoObj.NoiGioiThieuId);
            var tenHinhThucDen = "Tất cả";
            if (timKiemNangCaoObj.HinhThucDenId != null && timKiemNangCaoObj.HinhThucDenId != 0)
            {
                var hinhThucDen = _hinhThucDenRepository.TableNoTracking.FirstOrDefault(x => x.Id == timKiemNangCaoObj.HinhThucDenId);
                tenHinhThucDen = hinhThucDen?.Ten;
            }

            var datas = (ICollection<BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO CHI TIẾT HOA HỒNG CỦA NGƯỜI GIỚI THIỆU");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 10;
                    worksheet.Column(9).Width = 40;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 40;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;

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
                        range.Worksheet.Cells["A3:L3"].Value = "BÁO CÁO CHI TIẾT HOA HỒNG CỦA NGƯỜI GIỚI THIỆU";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        range.Worksheet.Cells["A4:L4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                      + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }
                    
                    using (var range = worksheet.Cells["A5:L5"])
                    {
                        range.Worksheet.Cells["A5:L5"].Merge = true;
                        //range.Worksheet.Cells["A5:L5"].Value = "Người giới thiệu: " + (noiGioiThieu != null ? $"{noiGioiThieu.Ten} - {noiGioiThieu.DonVi}" : string.Empty);
                        range.Worksheet.Cells["A5:L5"].Value = $"Hình thức đến: {tenHinhThucDen}";
                        range.Worksheet.Cells["A5:L5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:L5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:L5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:P7"])
                    {
                        range.Worksheet.Cells["A7:P7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:P7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:P7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:P7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:P7"].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A7:P7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Hình thức đến";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //del ?
                        range.Worksheet.Cells["C7"].Value = "Ngày khám";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Mã NB";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Mã TN";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Số biên lai thu tiền";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Tên BN";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Năm sinh";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Địa chỉ";

                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Bác sỹ khám (nếu có)";

                        range.Worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7"].Value = "Tên dịch vụ";

                        range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7"].Value = "Nhóm dịch vụ";

                        range.Worksheet.Cells["M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M7"].Value = "Số tiền dịch vụ (chưa trừ BHYT)";

                        range.Worksheet.Cells["N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N7"].Value = "(%) hoa hồng";

                        range.Worksheet.Cells["O7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O7"].Value = "Thành tiền hoa hồng";

                        range.Worksheet.Cells["P7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P7"].Value = "Tình trạng thanh toán";
                    }

                    //write data from line 8
                    int index = 8;
                    int stt = 1;
                    if (datas.Any())
                    {
                        var startIndex = index;
                        var endIndex = index;
                        foreach (var item in datas)
                        {
                            //// format border, font chữ,....
                            //worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            //worksheet.Cells["A" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            //worksheet.Cells["A" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                            //worksheet.Cells["A" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = item.STT;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.HinhThucDenDisplay;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.NgayKhamDisplay;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index].Value = item.MaNguoiBenh;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["E" + index].Value = item.MaTN;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["F" + index].Value = item.SoBienLaiThuTien;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["G" + index].Value = item.TenBN;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["H" + index].Value = item.NamSinh;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["I" + index].Value = item.DiaChi;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["J" + index].Value = item.BSKham;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["K" + index].Value = item.TenDV;

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["L" + index].Value = item.NhomDV;

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Value = item.SoTienDV;
                            //worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Value = item.HoaHong;

                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["O" + index].Value = item.ThanhTienHoaHong;

                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["P" + index].Value = item.TinhTrangThanhToan == true ? "X" : string.Empty;
                            stt++;
                            index++;
                        }

                        endIndex = index;
                        // format border, font chữ,.... => cho toàn bộ table chưa data
                        worksheet.Cells["A" + startIndex + ":P" + endIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + startIndex + ":P" + endIndex].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + startIndex + ":P" + endIndex].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + startIndex + ":P" + endIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + startIndex + ":P" + endIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        worksheet.Cells["M" + startIndex + ":M" + endIndex].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["O" + startIndex + ":O" + endIndex].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + startIndex + ":O" + endIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        //total
                        //worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        //worksheet.Cells["A" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        //worksheet.Cells["A" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                        //worksheet.Cells["A" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["O" + index].Style.Font.Bold = true;
                        worksheet.Cells["O" + index].Value = datas.Sum(x => x.ThanhTienHoaHong.GetValueOrDefault());
                        worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;

                        using (var range = worksheet.Cells["A" + index + ":L" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":L" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":L" + index].Value = "Tổng cộng";
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //range.Worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }


                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["M" + index].Style.Font.Bold = true;
                        worksheet.Cells["M" + index].Value = datas.Sum(x => x.SoTienDV);
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        public async Task<List<LookupItemTemplateVo>> GetNoiGioiThieuDaCoNguoiBenhAsync(DropDownListRequestModel queryInfo, bool theoHinhThucDen = false)
        {
            var loadData = true;
            if (theoHinhThucDen)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                var hinhThucDenId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

                loadData = hinhThucDenId == 0 || hinhThucDenId == hinhThucDenGioiThieuId;
            }


            if (loadData)
            {
                if (theoHinhThucDen)
                {
                    var noiGioiThieus = await _noiGioiThieuRepository.TableNoTracking
                        .Where(x => x.IsDisabled != true)
                        .Select(item => new LookupItemTemplateVo()
                        {
                            KeyId = item.Id,
                            Ma = item.DonVi,
                            Ten = item.Ten,
                            DisplayName = item.Ten
                        })
                        .ApplyLike(queryInfo.Query?.Trim(), x => x.Ten, x => x.Ma)
                        .Take(queryInfo.Take)
                        .ToListAsync();
                    return noiGioiThieus;
                }
                else
                {
                    var noiGioiThieus = await _noiGioiThieuRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhans.Any(a =>
                            a.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && a.TaiKhoanBenhNhanThus.Any(b => b.DaHuy != true
                                                               && b.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi
                                                               && b.TaiKhoanBenhNhanChis.Any(c => c.DaHuy != true
                                                                                                  && (c.YeuCauKhamBenhId != null || c.YeuCauDichVuKyThuatId != null)
                                                                                                  && c.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))))
                        .Select(item => new LookupItemTemplateVo()
                        {
                            KeyId = item.Id,
                            Ma = item.DonVi,
                            Ten = item.Ten,
                            DisplayName = item.Ten
                        })
                        .ApplyLike(queryInfo.Query?.Trim(), x => x.Ten, x => x.Ma)
                        .Take(queryInfo.Take)
                        .ToListAsync();
                    return noiGioiThieus;
                }
            }
            else
            {
                return new List<LookupItemTemplateVo>();
            }
        }

        public async Task<List<LookupItemTemplateVo>> GetHinhThucDenCoTatCaAsync(DropDownListRequestModel queryInfo)
        {
            var hinhThucDens = await _hinhThucDenRepository.TableNoTracking
                .Where(x => x.IsDisabled != true)
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.Id,
                    Ten = item.Ten,
                    DisplayName = item.Ten
                })
                .ApplyLike(queryInfo.Query?.Trim(), x => x.Ten)
                .Take(queryInfo.Take)
                .ToListAsync();
            return hinhThucDens;
        }

        public async Task<List<MaTiepNhanTheoHinhThucDenLookupItemVo>> GetMaYeuCauTiepNhanTheoHinhThucDenAsync(DropDownListRequestModel queryInfo)
        {
            var yeuCauTiepNhans = new List<MaTiepNhanTheoHinhThucDenLookupItemVo>();
            var timKiemNangCaoObj = new MaTiepNhanTheoHinhThucDenQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies) && queryInfo.ParameterDependencies.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<MaTiepNhanTheoHinhThucDenQueryInfoVo>(queryInfo.ParameterDependencies);
            }

            if (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0)
            {
                yeuCauTiepNhans = await _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                && (timKiemNangCaoObj.HinhThucDenId == null
                                    || timKiemNangCaoObj.HinhThucDenId == 0
                                    || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId))
                    .Select(item => new MaTiepNhanTheoHinhThucDenLookupItemVo()
                    {
                        KeyId = item.MaYeuCauTiepNhan,
                        MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                        TenNguoiBenh = item.HoTen,
                        MaNguoiBenh = item.BenhNhan.MaBN,
                        DisplayName = $"{item.MaYeuCauTiepNhan} - {item.HoTen}"
                    })
                    .ApplyLike(queryInfo.Query?.Trim(), x => x.MaYeuCauTiepNhan, x => x.TenNguoiBenh, x => x.MaNguoiBenh)
                    .Take(queryInfo.Take)
                    .ToListAsync();
            }
            else
            {
                var lstTiepNhanTheoNoiGioiThieu = await _yeuCauTiepNhanRepository.TableNoTracking
                   .Where(x => x.NoiGioiThieuId != null
                               && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                               && x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId
                               && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                               && x.BenhNhanId != null)
                   .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                   {
                       YeucauTiepNhanId = x.Id,
                       BenhNhanId = x.BenhNhanId.Value,
                       NoiGioiThieuId = x.NoiGioiThieuId,
                       ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                   })
                   .ToListAsync();
                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .Select(x => new ThongTinTiepNhanCoGioiThieuVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            BenhNhanId = x.BenhNhanId.Value,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                        })
                        .ToList();
                    foreach (var benhNhanId in lstBenhNhanId)
                    {
                        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.BenhNhanId == benhNhanId)
                            .OrderBy(x => x.YeucauTiepNhanId).First();
                        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId
                                        && x.BenhNhanId == benhNhanId)
                            .ToList();

                        var doiNguoiGioiThieu = false;
                        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                        {
                            if (lanTiepNhan.NoiGioiThieuId != null)
                            {
                                if (lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId)
                                {
                                    doiNguoiGioiThieu = true;
                                }
                                else
                                {
                                    doiNguoiGioiThieu = false;
                                }
                            }

                            if (!doiNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null)
                            {
                                lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                            }
                        }
                    }

                    var lstTiepNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                    yeuCauTiepNhans = await _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => lstTiepNhanId.Contains(x.Id))
                        .Select(item => new MaTiepNhanTheoHinhThucDenLookupItemVo()
                        {
                            KeyId = item.MaYeuCauTiepNhan,
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            TenNguoiBenh = item.HoTen,
                            MaNguoiBenh = item.BenhNhan.MaBN,
                            DisplayName = $"{item.MaYeuCauTiepNhan} - {item.HoTen}"
                        })
                        .ApplyLike(queryInfo.Query?.Trim(), x => x.MaYeuCauTiepNhan, x => x.TenNguoiBenh, x => x.MaNguoiBenh)
                        .Take(queryInfo.Take)
                        .ToListAsync();
                }
            }
           
            return yeuCauTiepNhans;
        }
    }
}
