using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private readonly long[] NhomDichVuNoiSoi = new long[] { 247 };
        private readonly long[] NhomDichVuNoiSoiTMH = new long[] { 241 };
        private readonly long[] NhomDichVuSieuAm = new long[] { 202 };
        private readonly long[] NhomDichVuXQuang = new long[] { 226, 227};
        private readonly long[] NhomDichVuCTScan = new long[] { 230 };
        private readonly long[] NhomDichVuMRI = new long[] { 231 };
        private readonly long[] NhomDichVuDienTimDienNao = new long[] { 248, 249 };
        private readonly long[] NhomDichVuTDCNDoLoang = new long[] { 250, 251 };
        private readonly long[] NhomDichVuThuThuat = new long[] { 240, 201, 206, 207, 209, 210, 214, 216, 218, 220, 221, 225, 228, 233, 239, 242, 243 };
        private readonly long[] NhomDichVuPhauThuat = new long[] { 245, 203, 211, 212, 213, 215, 217, 219, 222, 223, 224, 229, 232, 254, };
        private readonly long KhoaKhamBenhId = 4;

        public async Task<GridDataSource> GetDataBaoCaoDoanhThuTheoNhomDichVu3961Async(BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay;
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToList();

            var phieuThuDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan &&
                o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && tuNgay < o.NgayThu && o.NgayThu <= denNgay);

            var phieuHuyDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.DaHuy == true &&
                o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && tuNgay < o.NgayHuy && o.NgayHuy <= denNgay);

            var phieuChiDataQuery = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu &&
                tuNgay < o.NgayChi && o.NgayChi <= denNgay);

            var phieuThus = phieuThuDataQuery.Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
            {
                PhieuThuId = o.Id,
                NgayThu = o.NgayThu,
                LaPhieuHuy = false,
                MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                DataPhieuChis = o.TaiKhoanBenhNhanChis
                                    .Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                                    .Select(chi => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
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
            var phieuHuys = phieuHuyDataQuery.Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
            {
                PhieuThuId = o.Id,
                NgayThu = o.NgayHuy ?? o.NgayThu,
                LaPhieuHuy = true,
                MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                DataPhieuChis = o.TaiKhoanBenhNhanChis
                                    .Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                                    .Select(chi => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
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
            var phieuChis = phieuChiDataQuery.Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
            {
                PhieuChiId = o.Id,
                NgayThu = o.NgayChi,
                LaPhieuHuy = false,
                MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                GoiDichVu = o.TaiKhoanBenhNhanThu != null && o.TaiKhoanBenhNhanThu.ThuTienGoiDichVu != null && o.TaiKhoanBenhNhanThu.ThuTienGoiDichVu == true,
                DataPhieuChis = new List<BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi>(){ new BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
                                    {
                                        Id = o.Id,
                                        NgayChi = o.NgayChi,
                                        TienChiPhi = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.TienChiPhi : 0,
                                        SoTienBaoHiemTuNhanChiTra = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.SoTienBaoHiemTuNhanChiTra : 0,
                                        TienMat = 0,
                                        ChuyenKhoan = 0,

                                        Gia = o.Gia,
                                        SoLuong = o.SoLuong,
                                        DonGiaBaoHiem = o.DonGiaBaoHiem,
                                        MucHuongBaoHiem = o.MucHuongBaoHiem,
                                        TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan,
                                        SoTienMienGiam = o.SoTienMienGiam,

                                        YeuCauKhamBenhId = o.YeuCauKhamBenhId,
                                        YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId,
                                        YeuCauDuocPhamBenhVienId = o.YeuCauDuocPhamBenhVienId,
                                        YeuCauVatTuBenhVienId = o.YeuCauVatTuBenhVienId,
                                        DonThuocThanhToanChiTietId = o.DonThuocThanhToanChiTietId,
                                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                                        YeuCauTruyenMauId = o.YeuCauTruyenMauId,
                                    } },
            }).ToList();

            var allDataThuChi = phieuThus.Concat(phieuHuys).Concat(phieuChis).ToList();

            var dichVuKhamBenhIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauKhamBenhId != null).Select(o => o.YeuCauKhamBenhId).Distinct().ToList();
            var dichVuKyThuatIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuKyThuatId != null).Select(o => o.YeuCauDichVuKyThuatId).Distinct().ToList();
            var dichVuGiuongIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null).Select(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId).Distinct().ToList();
            var truyenMauIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauTruyenMauId != null).Select(o => o.YeuCauTruyenMauId).Distinct().ToList();
            var yeuCauDuocPhamIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(o => o.YeuCauDuocPhamBenhVienId).Distinct().ToList();
            var yeuCauVatTuIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauVatTuBenhVienId != null).Select(o => o.YeuCauVatTuBenhVienId).Distinct().ToList();
            var donThuocIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTietId).Distinct().ToList();
            var thongTinQuyetToanGoiDichVuIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauGoiDichVuId != null).Select(o => o.Id).Distinct().ToList();

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
                    DichVuKhamBenhBenhVienId = o.DichVuKhamBenhBenhVienId,
                    NhomGiaDichVuKhamBenhBenhVienId = o.NhomGiaDichVuKhamBenhBenhVienId,
                    YeuCauKhamBenh = true,
                    NoiThucHienId = o.NoiThucHienId ?? o.NoiDangKyId,
                    NoiChiDinhId = o.NoiChiDinhId,
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
                    DichVuKyThuatBenhVienId = o.DichVuKyThuatBenhVienId,
                    NhomGiaDichVuKyThuatBenhVienId = o.NhomGiaDichVuKyThuatBenhVienId,
                    YeuCauDichVuKyThuat = true,
                    LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    NoiThucHienId = o.NoiThucHienId,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId,
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
                    NoiThucHienId = o.PhongBenhVienId,
                    NoiChiDinhId = o.PhongBenhVienId,
                    ThoiDiemChiDinh = o.NgayPhatSinh
                }).ToList();

            var dataTruyenMau = _yeuCauTruyenMauRepository.TableNoTracking.Where(o => truyenMauIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
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
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    DonGiaNhapDuocPham = o.DonGiaNhap,
                    YeuCauDuocPham = true,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId,
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
                    VatTuBenhVienId = o.VatTuBenhVienId,
                    DonGiaNhapVatTu = o.DonGiaNhap,
                    YeuCauVatTu = true,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                }).ToList();

                dataYeuCauVatTu.AddRange(info);
            }

            var dataDonThuoc = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
            for (int i = 0; i < donThuocIds.Count; i = i + maxTake)
            {
                var takeIds = donThuocIds.Skip(i).Take(maxTake).ToList();

                var info = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    DuocPhamBenhVienId = o.DuocPhamId,
                    DonGiaNhapDuocPham = o.DonGiaNhap,
                    DonThuocBHYT = true,
                    NoiThucHienKhamBenhId = o.DonThuocThanhToan.YeuCauKhamBenhId != null ? o.DonThuocThanhToan.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiChiDinhId = o.DonThuocThanhToan.NoiTruDonThuocId != null ? o.DonThuocThanhToan.NoiTruDonThuoc.NoiKeDonId : (long?)null,
                }).ToList();

                dataDonThuoc.AddRange(info);
            }

            var taiKhoanBenhNhanChiThongTins = _taiKhoanBenhNhanChiThongTinRepository.TableNoTracking.Where(o => thongTinQuyetToanGoiDichVuIds.Contains(o.Id)).ToList();

            var dsMaYCTNChuaCoSoBenhAns = allDataThuChi.Where(o => string.IsNullOrEmpty(o.SoBenhAn)).Select(o => o.MaYeuCauTiepNhan).Distinct().ToList();

            var dsSoBenhAn = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null && dsMaYCTNChuaCoSoBenhAns.Contains(o.MaYeuCauTiepNhan))
                .Select(o => new { o.MaYeuCauTiepNhan, o.NoiTruBenhAn.SoBenhAn }).ToList();

            //tinh lan thuc hien dv
            var maYeuCauTiepNhans = allDataThuChi.Where(o => o.NoiGioiThieuId != null).Select(o => o.MaYeuCauTiepNhan).Distinct().ToList();
            var yeuCauTiepNhanBSGioiThieus = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => maYeuCauTiepNhans.Contains(o.MaYeuCauTiepNhan))
                .Select(o => new { o.Id, o.MaYeuCauTiepNhan }).ToList();
            var yeuCauTiepNhanBSGioiThieuIds = yeuCauTiepNhanBSGioiThieus.Select(o => o.Id).ToList();

            var dichVuKhamBSGioiThieus = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanBSGioiThieuIds.Contains(o.YeuCauTiepNhanId) && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVuBSGioiThieu 
                {
                    Id = o.Id,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    DichVuBenhVienId = o.DichVuKhamBenhBenhVienId,
                    NhomGiaId = o.NhomGiaDichVuKhamBenhBenhVienId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh
                })
                .ToList();
            dichVuKhamBSGioiThieus.ForEach(o => o.MaYeuCauTiepNhan = yeuCauTiepNhanBSGioiThieus.FirstOrDefault(tn => tn.Id == o.YeuCauTiepNhanId)?.MaYeuCauTiepNhan);
            var dichVuKyThuatBSGioiThieus = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanBSGioiThieuIds.Contains(o.YeuCauTiepNhanId) && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVuBSGioiThieu 
                { 
                    Id = o.Id, 
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    DichVuBenhVienId = o.DichVuKyThuatBenhVienId,
                    NhomGiaId = o.NhomGiaDichVuKyThuatBenhVienId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    SoLan = o.SoLan
                })
                .ToList();
            dichVuKyThuatBSGioiThieus.ForEach(o => o.MaYeuCauTiepNhan = yeuCauTiepNhanBSGioiThieus.FirstOrDefault(tn => tn.Id == o.YeuCauTiepNhanId)?.MaYeuCauTiepNhan);

            var thongTinHopDongs = _noiGioiThieuHopDongRepository.TableNoTracking
                .Select(o => new
                {
                    o.NoiGioiThieuId,
                    o.NgayBatDau,
                    o.NgayKetThuc,
                    ChiTietHeSoDichVuKhamBenhs = o.NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs.Select(k => new
                    {
                        k.DichVuKhamBenhBenhVienId,
                        k.NhomGiaDichVuKhamBenhBenhVienId,
                        k.HeSoGioiThieuTuLan1,
                        k.HeSoGioiThieuTuLan2,
                        k.HeSoGioiThieuTuLan3
                    }).ToList(),
                    ChiTietHeSoDichVuKyThuats = o.NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats.Select(k => new
                    {
                        k.DichVuKyThuatBenhVienId,
                        k.NhomGiaDichVuKyThuatBenhVienId,
                        k.HeSoGioiThieuTuLan1,
                        k.HeSoGioiThieuTuLan2,
                        k.HeSoGioiThieuTuLan3
                    }).ToList(),
                    ChiTietHeSoDichVuGiuongs = o.NoiGioiThieuHopDongChiTietHeSoDichVuGiuongs.Select(k => new
                    {
                        k.DichVuGiuongBenhVienId,
                        k.NhomGiaDichVuGiuongBenhVienId,
                        k.HeSoGioiThieuTuLan1,
                        k.HeSoGioiThieuTuLan2,
                        k.HeSoGioiThieuTuLan3
                    }).ToList(),
                    ChiTietHeSoDuocPhams = o.NoiGioiThieuHopDongChiTietHeSoDuocPhams.Select(k => new
                    {
                        k.DuocPhamBenhVienId,
                        k.LoaiGia,
                        k.HeSo
                    }).ToList(),
                    ChiTietHeSoVatTus = o.NoiGioiThieuHopDongChiTietHeSoVatTus.Select(k => new
                    {
                        k.VatTuBenhVienId,
                        k.LoaiGia,
                        k.HeSo
                    }).ToList(),
                }).ToList();

            var baoCaoDoanhThuTheoNhomDichVuGridVos = new List<BaoCaoDoanhThuTheoNhomDichVuGridVo>();
            foreach (var dataPhieuThu in allDataThuChi)
            {
                foreach (var dataPhieuChi in dataPhieuThu.DataPhieuChis)
                {
                    if (dataPhieuChi.YeuCauGoiDichVuId != null)
                    {
                        var taiKhoanBenhNhanChiThongTin = taiKhoanBenhNhanChiThongTins.FirstOrDefault(o => o.Id == dataPhieuChi.Id);
                        if (taiKhoanBenhNhanChiThongTin != null)
                        {
                            List<NoiDungQuyetToanGoiMarketing> noiDungQuyetToanGoiMarketings = null;
                            try
                            {
                                noiDungQuyetToanGoiMarketings = JsonConvert.DeserializeObject<List<NoiDungQuyetToanGoiMarketing>>(taiKhoanBenhNhanChiThongTin.NoiDung);
                            }
                            catch (Exception ex)
                            {

                            }

                            if (noiDungQuyetToanGoiMarketings != null && noiDungQuyetToanGoiMarketings.Count > 0)
                            {
                                foreach (var noiDungQuyetToanGoiMarketing in noiDungQuyetToanGoiMarketings)
                                {
                                    List<decimal> heSos = new List<decimal>();
                                    var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoDoanhThuTheoNhomDichVuGridVo
                                    {
                                        Id = dataPhieuChi.Id,
                                        MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                                        MaNB = dataPhieuThu.MaNB,
                                        HoVaTen = dataPhieuThu.HoTen,
                                        NamSinh = dataPhieuThu.NamSinh?.ToString(),
                                        GioiTinh = dataPhieuThu.GioiTinh?.GetDescription(),
                                        SoBenhAn = dataPhieuThu.SoBenhAn,
                                        NgayThu = dataPhieuThu.NgayThu,
                                        NguoiGioiThieu = dataPhieuThu.NguoiGioiThieu
                                    };
                                    if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                                    }
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = noiDungQuyetToanGoiMarketing.NoiDung;

                                    if (dataPhieuThu.NoiGioiThieuId != null)
                                    {
                                        var hopDong = thongTinHopDongs
                                            .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                        && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                            .OrderBy(o => o.NgayBatDau)
                                            .LastOrDefault();
                                        if (hopDong != null)
                                        {
                                            if(noiDungQuyetToanGoiMarketing.DichVuBenhVienId != null && noiDungQuyetToanGoiMarketing.NhomGiaDichVuId != null && noiDungQuyetToanGoiMarketing.SoLuong != null && noiDungQuyetToanGoiMarketing.DonGia != null)
                                            {
                                                //kham
                                                if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauKhamBenh)
                                                {
                                                    var chiTietHeSoDichVuKham = hopDong.ChiTietHeSoDichVuKhamBenhs
                                                            .Where(o => o.DichVuKhamBenhBenhVienId == noiDungQuyetToanGoiMarketing.DichVuBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == noiDungQuyetToanGoiMarketing.NhomGiaDichVuId)
                                                            .FirstOrDefault();
                                                    if (chiTietHeSoDichVuKham != null)
                                                    {
                                                        if (chiTietHeSoDichVuKham.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuKham.HeSoGioiThieuTuLan2 == null)
                                                        {
                                                            heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                        }
                                                        else
                                                        {
                                                            //tinh lan theo matn
                                                            var yeuCauTiepNhanDaYeuCauIds = yeuCauTiepNhanBSGioiThieus.Where(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan).Select(o => o.Id).ToList();
                                                            var yeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                                                                .Where(o => yeuCauTiepNhanDaYeuCauIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                            && o.DichVuKhamBenhBenhVienId == noiDungQuyetToanGoiMarketing.DichVuBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == noiDungQuyetToanGoiMarketing.NhomGiaDichVuId)
                                                                .Select(o => o.Id).ToList();                                                            
                                                            var solanDaYeuCau = yeuCauKhamBenhs.Count;

                                                            if (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() == 1)
                                                            {
                                                                if (solanDaYeuCau == 0)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (solanDaYeuCau == 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (solanDaYeuCau > 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                }
                                                            }
                                                            else if (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() > 1)
                                                            {
                                                                if (solanDaYeuCau > 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                }
                                                                else
                                                                {
                                                                    for (int i = solanDaYeuCau; i < (solanDaYeuCau + noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault()); i++)
                                                                    {
                                                                        if (i == 0)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                        }
                                                                        else if (i == 1)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                        }
                                                                        else if (i > 1)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuKham.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKham.HeSoGioiThieuTuLan1);
                                                                        }
                                                                    }
                                                                    if (heSos.Distinct().Count() == 1)
                                                                    {
                                                                        heSos = new List<decimal> { heSos.First() };
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                //dvkt
                                                if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat)
                                                {
                                                    var chiTietHeSoDichVuKyThuat = hopDong.ChiTietHeSoDichVuKyThuats
                                                            .Where(o => o.DichVuKyThuatBenhVienId == noiDungQuyetToanGoiMarketing.DichVuBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == noiDungQuyetToanGoiMarketing.NhomGiaDichVuId)
                                                            .FirstOrDefault();
                                                    if (chiTietHeSoDichVuKyThuat != null)
                                                    {
                                                        if (chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 == null)
                                                        {
                                                            heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                        }
                                                        else
                                                        {
                                                            //tinh lan theo matn
                                                            var yeuCauTiepNhanDaYeuCauIds = yeuCauTiepNhanBSGioiThieus.Where(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan).Select(o => o.Id).ToList();
                                                            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                                                .Where(o => yeuCauTiepNhanDaYeuCauIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                            && o.DichVuKyThuatBenhVienId == noiDungQuyetToanGoiMarketing.DichVuBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == noiDungQuyetToanGoiMarketing.NhomGiaDichVuId)
                                                                .Select(o => o.SoLan).ToList();
                                                            var solanDaYeuCau = yeuCauDichVuKyThuats.Sum();

                                                            if (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() == 1)
                                                            {
                                                                if (solanDaYeuCau == 0)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (solanDaYeuCau == 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (solanDaYeuCau > 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                            }
                                                            else if (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() > 1)
                                                            {
                                                                if (solanDaYeuCau > 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                                else
                                                                {
                                                                    for (int i = solanDaYeuCau; i < (solanDaYeuCau + noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault()); i++)
                                                                    {
                                                                        if (i == 0)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                        }
                                                                        else if (i == 1)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                        }
                                                                        else if (i > 1)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                        }
                                                                    }
                                                                    if (heSos.Distinct().Count() == 1)
                                                                    {
                                                                        heSos = new List<decimal> { heSos.First() };
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                //giuong
                                                if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuGiuong)
                                                {
                                                    var chiTietHeSoDichVuGiuong = hopDong.ChiTietHeSoDichVuGiuongs
                                                            .Where(o => o.DichVuGiuongBenhVienId == noiDungQuyetToanGoiMarketing.DichVuBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == noiDungQuyetToanGoiMarketing.NhomGiaDichVuId)
                                                            .FirstOrDefault();
                                                    if (chiTietHeSoDichVuGiuong != null)
                                                    {
                                                        if (chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 == null)
                                                        {
                                                            heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                        }
                                                        else
                                                        {
                                                            //tinh lan theo matn
                                                            var yeuCauTiepNhanDaYeuCauIds = yeuCauTiepNhanBSGioiThieus.Where(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan).Select(o => o.Id).ToList();
                                                            var yeuCauGiuongs = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                                                                .Where(o => yeuCauTiepNhanDaYeuCauIds.Contains(o.YeuCauTiepNhanId) 
                                                                            && o.DichVuGiuongBenhVienId == noiDungQuyetToanGoiMarketing.DichVuBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == noiDungQuyetToanGoiMarketing.NhomGiaDichVuId)
                                                                .Select(o => o.SoLuong).ToList();
                                                            var solanDaYeuCau = (int)Math.Floor(yeuCauGiuongs.Sum());

                                                            if (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() == 1)
                                                            {
                                                                if (solanDaYeuCau == 0)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (solanDaYeuCau == 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (solanDaYeuCau > 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                }
                                                            }
                                                            else if (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() > 1)
                                                            {
                                                                if (solanDaYeuCau > 1)
                                                                {
                                                                    heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                }
                                                                else
                                                                {
                                                                    for (int i = solanDaYeuCau; i < (solanDaYeuCau + noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault()); i++)
                                                                    {
                                                                        if (i == 0)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                        }
                                                                        else if (i == 1)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                        }
                                                                        else if (i > 1)
                                                                        {
                                                                            heSos.Add(chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1);
                                                                        }
                                                                    }
                                                                    if (heSos.Distinct().Count() == 1)
                                                                    {
                                                                        heSos = new List<decimal> { heSos.First() };
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                
                                        }
                                    }
                                    if (heSos.Count == 0)
                                    {
                                        heSos.Add(1);
                                    }
                                    if (heSos.Count == 1)
                                    {                                        
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet = noiDungQuyetToanGoiMarketing.ThanhTien;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo = heSos.First();

                                        var thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault();

                                        thanhTien = Math.Round(thanhTien * baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo, 2);

                                        baoCaoDoanhThuTheoNhomDichVuGridVo.TongCong = thanhTien;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauKhamBenh ? thanhTien : 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuGiuong ? thanhTien : 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = 0;

                                        baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                                        if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat)
                                        {
                                            if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = thanhTien;
                                            }
                                            else if (NhomDichVuNoiSoi.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = thanhTien;
                                            }
                                            else if (NhomDichVuNoiSoiTMH.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = thanhTien;
                                            }
                                            else if (NhomDichVuSieuAm.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = thanhTien;
                                            }
                                            else if (NhomDichVuXQuang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = thanhTien;
                                            }
                                            else if (NhomDichVuCTScan.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = thanhTien;
                                            }
                                            else if (NhomDichVuMRI.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = thanhTien;
                                            }
                                            else if (NhomDichVuDienTimDienNao.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = thanhTien;
                                            }
                                            else if (NhomDichVuTDCNDoLoang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = thanhTien;
                                            }
                                            else if (NhomDichVuPhauThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                            }
                                            else if (NhomDichVuThuThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                            }
                                            else if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                            {
                                                if (noiDungQuyetToanGoiMarketing.NoiDung != null &&
                                                    noiDungQuyetToanGoiMarketing.NoiDung.ToLower().Contains("phẫu thuật"))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                                }
                                                else
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                                }

                                            }
                                            else
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thanhTien;
                                            }
                                        }

                                        baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < heSos.Count; i++)
                                        {
                                            var heSoTheoLan = heSos[i];
                                            var soLuong = (i == (heSos.Count - 1)) ? (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() - i) : 1;

                                            var baoCaoDoanhThuTheoNhomDichVuGridVoClone = baoCaoDoanhThuTheoNhomDichVuGridVo.CopyObject<BaoCaoDoanhThuTheoNhomDichVuGridVo>();

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet = Math.Round(noiDungQuyetToanGoiMarketing.DonGia.GetValueOrDefault() * (decimal)soLuong, 2);

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo = heSoTheoLan;

                                            var thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet.GetValueOrDefault();

                                            thanhTien = Math.Round(thanhTien * baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo, 2);

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.TongCong = thanhTien;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.KhamBenh = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauKhamBenh ? thanhTien : 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.NgayGiuong = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuGiuong ? thanhTien : 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.Thuoc = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.VTYT = 0;

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = 0;
                                            if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat)
                                            {
                                                if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = thanhTien;
                                                }
                                                else if (NhomDichVuNoiSoi.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = thanhTien;
                                                }
                                                else if (NhomDichVuNoiSoiTMH.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = thanhTien;
                                                }
                                                else if (NhomDichVuSieuAm.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = thanhTien;
                                                }
                                                else if (NhomDichVuXQuang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = thanhTien;
                                                }
                                                else if (NhomDichVuCTScan.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = thanhTien;
                                                }
                                                else if (NhomDichVuMRI.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = thanhTien;
                                                }
                                                else if (NhomDichVuDienTimDienNao.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = thanhTien;
                                                }
                                                else if (NhomDichVuTDCNDoLoang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = thanhTien;
                                                }
                                                else if (NhomDichVuPhauThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                                }
                                                else if (NhomDichVuThuThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                                }
                                                else if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                                {
                                                    if (noiDungQuyetToanGoiMarketing.NoiDung != null &&
                                                        noiDungQuyetToanGoiMarketing.NoiDung.ToLower().Contains("phẫu thuật"))
                                                    {
                                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                                    }
                                                    else
                                                    {
                                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                                    }

                                                }
                                                else
                                                {
                                                    baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = thanhTien;
                                                }
                                            }

                                            baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVoClone);
                                        }
                                    }                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoDoanhThuTheoNhomDichVuGridVo
                        {
                            Id = dataPhieuChi.Id,
                            MaNB = dataPhieuThu.MaNB,
                            MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                            HoVaTen = dataPhieuThu.HoTen,
                            NamSinh = dataPhieuThu.NamSinh?.ToString(),
                            GioiTinh = dataPhieuThu.GioiTinh?.GetDescription(),
                            SoBenhAn = dataPhieuThu.SoBenhAn,
                            NgayThu = dataPhieuThu.NgayThu,
                            NguoiGioiThieu = dataPhieuThu.NguoiGioiThieu
                        };
                        if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                        }

                        BaoCaoDoanhThuTheoNhomDichVuDataDichVu thongTinDichVu = null;
                        decimal heSo = 1;
                        var loaiGia = LoaiGiaNoiGioiThieuHopDong.GiaBan;
                        List<decimal> heSoDichVuKyThuats = new List<decimal>();
                        if (dataPhieuChi.YeuCauKhamBenhId != null)
                        {
                            thongTinDichVu = dataDichVuKhamBenh.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauKhamBenhId);
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= thongTinDichVu.ThoiDiemChiDinh.Date && (o.NgayKetThuc == null || thongTinDichVu.ThoiDiemChiDinh.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHeSoDichVuKhamBenh = hopDong.ChiTietHeSoDichVuKhamBenhs
                                        .Where(o => o.DichVuKhamBenhBenhVienId == thongTinDichVu.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVu.NhomGiaDichVuKhamBenhBenhVienId)
                                        .FirstOrDefault();
                                    if (chiTietHeSoDichVuKhamBenh != null)
                                    {
                                        if(chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan2 == null)
                                        {
                                            heSo = chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan1;
                                        }
                                        else
                                        {
                                            //tinh lan theo matn
                                            var yeuCauKhamIds = dichVuKhamBSGioiThieus
                                                .Where(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan && o.DichVuBenhVienId == thongTinDichVu.DichVuKhamBenhBenhVienId && o.NhomGiaId == thongTinDichVu.NhomGiaDichVuKhamBenhBenhVienId)
                                                .OrderBy(o => o.ThoiDiemChiDinh)
                                                .Select(o => o.Id).ToList();
                                            var lanYeuCau = yeuCauKhamIds.IndexOf(dataPhieuChi.YeuCauKhamBenhId.Value) + 1;
                                            if(lanYeuCau == 1)
                                            {
                                                heSo = chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan1;
                                            }
                                            else if(lanYeuCau == 2)
                                            {
                                                heSo = chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan1;
                                            }
                                            else if (lanYeuCau > 2)
                                            {
                                                heSo = chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (dataPhieuChi.YeuCauDichVuKyThuatId != null)
                        {
                            thongTinDichVu = dataDichVuKyThuat.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDichVuKyThuatId);

                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= thongTinDichVu.ThoiDiemChiDinh.Date && (o.NgayKetThuc == null || thongTinDichVu.ThoiDiemChiDinh.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHeSoDichVuKyThuat = hopDong.ChiTietHeSoDichVuKyThuats
                                        .Where(o => o.DichVuKyThuatBenhVienId == thongTinDichVu.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == thongTinDichVu.NhomGiaDichVuKyThuatBenhVienId)
                                        .FirstOrDefault();
                                    if (chiTietHeSoDichVuKyThuat != null)
                                    {
                                        if (chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 == null)
                                        {
                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                        }
                                        else
                                        {
                                            //tinh lan theo matn
                                            var yeuCauDichVuKyThuats = dichVuKyThuatBSGioiThieus
                                                .Where(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan && o.DichVuBenhVienId == thongTinDichVu.DichVuKyThuatBenhVienId && o.NhomGiaId == thongTinDichVu.NhomGiaDichVuKyThuatBenhVienId)
                                                .OrderBy(o => o.ThoiDiemChiDinh).ToList();
                                            var solanDaYeuCau = 0;

                                            foreach(var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
                                            {
                                                if(yeuCauDichVuKyThuat.Id != dataPhieuChi.YeuCauDichVuKyThuatId)
                                                {
                                                    solanDaYeuCau += yeuCauDichVuKyThuat.SoLan;
                                                }
                                                else
                                                {
                                                    if(dataPhieuChi.SoLuong.GetValueOrDefault() == 1)
                                                    {
                                                        if (solanDaYeuCau == 0)
                                                        {
                                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                                        }
                                                        else if (solanDaYeuCau == 1)
                                                        {
                                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                                        }
                                                        else if (solanDaYeuCau > 1)
                                                        {
                                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                                        }
                                                    }
                                                    else if (dataPhieuChi.SoLuong.GetValueOrDefault() > 1)
                                                    {
                                                        if(solanDaYeuCau > 1)
                                                        {
                                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                                        }
                                                        else
                                                        {
                                                            for(int i = solanDaYeuCau; i < (solanDaYeuCau + dataPhieuChi.SoLuong.GetValueOrDefault()); i++)
                                                            {
                                                                if (i == 0)
                                                                {
                                                                    heSoDichVuKyThuats.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (i == 1)
                                                                {
                                                                    heSoDichVuKyThuats.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                                else if (i > 1)
                                                                {
                                                                    heSoDichVuKyThuats.Add(chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1);
                                                                }
                                                            }
                                                            if(heSoDichVuKyThuats.Distinct().Count() == 1)
                                                            {
                                                                heSo = heSoDichVuKyThuats.First();
                                                                heSoDichVuKyThuats = new List<decimal>();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
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
                                                && o.NgayBatDau.Date <= thongTinDichVu.ThoiDiemChiDinh.Date && (o.NgayKetThuc == null || thongTinDichVu.ThoiDiemChiDinh.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHeSoDichVuGiuong = hopDong.ChiTietHeSoDichVuGiuongs
                                        .Where(o => o.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaDichVuGiuongBenhVienId)
                                        .FirstOrDefault();
                                    if (chiTietHeSoDichVuGiuong != null)
                                    {
                                        if (chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 == null)
                                        {
                                            heSo = chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1;
                                        }
                                        else
                                        {
                                            var yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds = dataPhieuThu.DataPhieuChis
                                                .Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null)
                                                .Select(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.GetValueOrDefault()).ToList();

                                            var yeuCauGiuongIds = dataDichVuGiuong
                                                .Where(o=> yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Contains(o.Id) && o.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaDichVuGiuongBenhVienId)
                                                .OrderBy(o => o.ThoiDiemChiDinh)
                                                .Select(o=>o.Id).ToList();

                                            var lanYeuCau = yeuCauGiuongIds.IndexOf(dataPhieuChi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value) + 1;
                                            if (lanYeuCau == 1)
                                            {
                                                heSo = chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1;
                                            }
                                            else if (lanYeuCau == 2)
                                            {
                                                heSo = chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1;
                                            }
                                            else if (lanYeuCau > 2)
                                            {
                                                heSo = chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuGiuong.HeSoGioiThieuTuLan1;
                                            }
                                        }
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
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= thongTinDichVu.ThoiDiemChiDinh.Date && (o.NgayKetThuc == null || thongTinDichVu.ThoiDiemChiDinh.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if(hopDong != null)
                                {
                                    var chiTietHeSoDuocPham = hopDong.ChiTietHeSoDuocPhams.Where(o => o.DuocPhamBenhVienId == thongTinDichVu.DuocPhamBenhVienId).FirstOrDefault();
                                    if(chiTietHeSoDuocPham != null)
                                    {
                                        heSo = chiTietHeSoDuocPham.HeSo;
                                        loaiGia = chiTietHeSoDuocPham.LoaiGia;
                                    }
                                }
                            }
                        }
                        else if (dataPhieuChi.YeuCauVatTuBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauVatTu.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauVatTuBenhVienId);
                            if (dataPhieuThu.NoiGioiThieuId != null && thongTinDichVu != null)
                            {
                                var hopDong = thongTinHopDongs
                                    .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                && o.NgayBatDau.Date <= thongTinDichVu.ThoiDiemChiDinh.Date && (o.NgayKetThuc == null || thongTinDichVu.ThoiDiemChiDinh.Date <= o.NgayKetThuc.Value.Date))
                                    .OrderBy(o => o.NgayBatDau)
                                    .LastOrDefault();
                                if (hopDong != null)
                                {
                                    var chiTietHeSoVatTu = hopDong.ChiTietHeSoVatTus.Where(o => o.VatTuBenhVienId == thongTinDichVu.VatTuBenhVienId).FirstOrDefault();
                                    if (chiTietHeSoVatTu != null)
                                    {
                                        heSo = chiTietHeSoVatTu.HeSo;
                                        loaiGia = chiTietHeSoVatTu.LoaiGia;
                                    }
                                }
                            }

                        }
                        else if (dataPhieuChi.DonThuocThanhToanChiTietId != null)
                        {
                            thongTinDichVu = dataDonThuoc.FirstOrDefault(o => o.Id == dataPhieuChi.DonThuocThanhToanChiTietId);
                        }
                        if (thongTinDichVu == null)
                        {
                            thongTinDichVu = new BaoCaoDoanhThuTheoNhomDichVuDataDichVu()
                            {
                                TenDichVu = "Dịch vụ giường",
                                YeuCauGiuong = true
                            };
                        }
                        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = thongTinDichVu.TenDichVu;

                        if (dataPhieuThu.GoiDichVu)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                        }
                        else
                        {
                            long? noiThucHienDuocTinhId = null;
                            if (thongTinDichVu.YeuCauKhamBenh)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId;
                            }
                            if (thongTinDichVu.YeuCauDichVuKyThuat)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId ?? thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauGiuong)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauTruyenMau)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauDuocPham)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauVatTu)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.DonThuocBHYT)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                            }
                            var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == noiThucHienDuocTinhId)?.KhoaPhong.Ten;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = tenKhoaPhong ?? "Không xác định";
                        }


                        if (thongTinDichVu.DonGiaNhapDuocPham != null || thongTinDichVu.DonGiaNhapVatTu != null)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho = Math.Round((thongTinDichVu.DonGiaNhapDuocPham ?? thongTinDichVu.DonGiaNhapVatTu).GetValueOrDefault() * (decimal)dataPhieuChi.SoLuong.GetValueOrDefault(), 2);
                        }

                        if(!heSoDichVuKyThuats.Any())
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam = dataPhieuChi.SoTienMienGiam.GetValueOrDefault();
                            baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra = Math.Round(((decimal)dataPhieuChi.SoLuong.GetValueOrDefault() * dataPhieuChi.DonGiaBaoHiem.GetValueOrDefault() * dataPhieuChi.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * dataPhieuChi.MucHuongBaoHiem.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet = dataPhieuChi.TienChiPhi.GetValueOrDefault() + dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() + dataPhieuChi.SoTienMienGiam.GetValueOrDefault() + baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra.GetValueOrDefault();
                            //if(!baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault().AlmostEqual(Math.Round(dataPhieuChi.Gia.GetValueOrDefault() * (decimal)dataPhieuChi.SoLuong.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)))
                            //{

                            //}    
                            baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo = heSo;
                            decimal thanhTien = 0;
                            if(baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho != null && loaiGia == LoaiGiaNoiGioiThieuHopDong.GiaNhap)
                            {
                                thanhTien = Math.Round(baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho.GetValueOrDefault() * baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo, 2) 
                                    - baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam.GetValueOrDefault() 
                                    - baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra.GetValueOrDefault();
                            }
                            else
                            {
                                thanhTien = Math.Round(baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault() * baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo, 2)
                                    - baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam.GetValueOrDefault()
                                    - baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra.GetValueOrDefault();

                                //thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * (dataPhieuChi.TienChiPhi.GetValueOrDefault() + dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault());
                                //thanhTien = Math.Round(thanhTien * baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo, 2);
                            }
                            if (thanhTien < 0 && baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo < 1)
                            {
                                thanhTien = 0;
                            }
                            else
                            {
                                thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * thanhTien;
                            }
                            baoCaoDoanhThuTheoNhomDichVuGridVo.TongCong = thanhTien;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = thongTinDichVu.YeuCauKhamBenh ? thanhTien : 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = thongTinDichVu.YeuCauGiuong ? thanhTien : 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thongTinDichVu.YeuCauTruyenMau ? thanhTien : 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = thongTinDichVu.YeuCauDuocPham || thongTinDichVu.DonThuocBHYT ? thanhTien : 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = thongTinDichVu.YeuCauVatTu ? thanhTien : 0;

                            baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                            if (thongTinDichVu.YeuCauDichVuKyThuat)
                            {
                                if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = thanhTien;
                                }
                                else if (NhomDichVuNoiSoi.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = thanhTien;
                                }
                                else if (NhomDichVuNoiSoiTMH.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = thanhTien;
                                }
                                else if (NhomDichVuSieuAm.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = thanhTien;
                                }
                                else if (NhomDichVuXQuang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = thanhTien;
                                }
                                else if (NhomDichVuCTScan.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = thanhTien;
                                }
                                else if (NhomDichVuMRI.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = thanhTien;
                                }
                                else if (NhomDichVuDienTimDienNao.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = thanhTien;
                                }
                                else if (NhomDichVuTDCNDoLoang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = thanhTien;
                                }
                                else if (NhomDichVuPhauThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                }
                                else if (NhomDichVuThuThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                }
                                else if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                {
                                    if (thongTinDichVu.TenDichVu != null &&
                                        thongTinDichVu.TenDichVu.ToLower().Contains("phẫu thuật"))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                    }
                                    else
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                    }

                                }
                                else
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thanhTien;
                                }
                            }

                            baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                        }
                        else
                        {
                            foreach(var heSoDichVuKyThuat in heSoDichVuKyThuats)
                            {
                                var baoCaoDoanhThuTheoNhomDichVuGridVoClone = baoCaoDoanhThuTheoNhomDichVuGridVo.CopyObject<BaoCaoDoanhThuTheoNhomDichVuGridVo>();

                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam = Math.Round(dataPhieuChi.SoTienMienGiam.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2);
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra = Math.Round((dataPhieuChi.DonGiaBaoHiem.GetValueOrDefault() * dataPhieuChi.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * dataPhieuChi.MucHuongBaoHiem.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet = Math.Round(dataPhieuChi.TienChiPhi.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2)
                                                                                + Math.Round(dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2)
                                                                                + baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam.GetValueOrDefault()
                                                                                + baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra.GetValueOrDefault();

                                //if (!baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault().AlmostEqual(Math.Round(dataPhieuChi.Gia.GetValueOrDefault() * (decimal)dataPhieuChi.SoLuong.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)))
                                //{

                                //}

                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo = heSoDichVuKyThuat;

                                var thanhTien = Math.Round(baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet.GetValueOrDefault() * baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo, 2)
                                    - baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam.GetValueOrDefault()
                                    - baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra.GetValueOrDefault();

                                if (thanhTien < 0 && baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo < 1)
                                {
                                    thanhTien = 0;
                                }
                                else
                                {
                                    thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * thanhTien;
                                }

                                //var thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) *
                                //    (Math.Round(dataPhieuChi.TienChiPhi.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2) + Math.Round(dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2));
                                //thanhTien = Math.Round(thanhTien * baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo, 2);

                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.TongCong = thanhTien;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.KhamBenh = thongTinDichVu.YeuCauKhamBenh ? thanhTien : 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.NgayGiuong = thongTinDichVu.YeuCauGiuong ? thanhTien : 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = thongTinDichVu.YeuCauTruyenMau ? thanhTien : 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.Thuoc = thongTinDichVu.YeuCauDuocPham || thongTinDichVu.DonThuocBHYT ? thanhTien : 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.VTYT = thongTinDichVu.YeuCauVatTu ? thanhTien : 0;

                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = 0;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = 0;
                                if (thongTinDichVu.YeuCauDichVuKyThuat)
                                {
                                    if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = thanhTien;
                                    }
                                    else if (NhomDichVuNoiSoi.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = thanhTien;
                                    }
                                    else if (NhomDichVuNoiSoiTMH.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = thanhTien;
                                    }
                                    else if (NhomDichVuSieuAm.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = thanhTien;
                                    }
                                    else if (NhomDichVuXQuang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = thanhTien;
                                    }
                                    else if (NhomDichVuCTScan.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = thanhTien;
                                    }
                                    else if (NhomDichVuMRI.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = thanhTien;
                                    }
                                    else if (NhomDichVuDienTimDienNao.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = thanhTien;
                                    }
                                    else if (NhomDichVuTDCNDoLoang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = thanhTien;
                                    }
                                    else if (NhomDichVuPhauThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                    }
                                    else if (NhomDichVuThuThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                    }
                                    else if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                    {
                                        if (thongTinDichVu.TenDichVu != null &&
                                            thongTinDichVu.TenDichVu.ToLower().Contains("phẫu thuật"))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                        }
                                        else
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                        }

                                    }
                                    else
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = thanhTien;
                                    }
                                }

                                baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVoClone);
                            }                            
                        }
                    }
                }
            }
            return new GridDataSource { Data = baoCaoDoanhThuTheoNhomDichVuGridVos.OrderBy(o => o.Nhom.Equals("Gói dịch vụ") ? 1 : 2).ThenBy(o => o.Nhom).ThenBy(o => o.NgayThu).ToArray(), TotalRowCount = baoCaoDoanhThuTheoNhomDichVuGridVos.Count };
        }

        public async Task<GridDataSource> GetDataBaoCaoDoanhThuTheoNhomDichVuAsync(BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay;
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToList();

            var phieuThuDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan &&
                o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && tuNgay < o.NgayThu && o.NgayThu <= denNgay);

            var phieuHuyDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.DaHuy == true &&
                o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && tuNgay < o.NgayHuy && o.NgayHuy <= denNgay);

            var phieuChiDataQuery = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu &&
                tuNgay < o.NgayChi && o.NgayChi <= denNgay);

            var phieuThus = phieuThuDataQuery.Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
            {
                PhieuThuId = o.Id,
                NgayThu = o.NgayThu,
                LaPhieuHuy = false,
                MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                DataPhieuChis = o.TaiKhoanBenhNhanChis
                                    .Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                                    .Select(chi => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
                                    {
                                        Id = chi.Id,
                                        NgayChi = chi.NgayChi,
                                        TienChiPhi = chi.TienChiPhi,
                                        SoTienBaoHiemTuNhanChiTra = chi.SoTienBaoHiemTuNhanChiTra,
                                        TienMat = 0,
                                        ChuyenKhoan = 0,
                                        YeuCauKhamBenhId = chi.YeuCauKhamBenhId,
                                        YeuCauDichVuKyThuatId = chi.YeuCauDichVuKyThuatId,
                                        YeuCauDuocPhamBenhVienId = chi.YeuCauDuocPhamBenhVienId,
                                        YeuCauVatTuBenhVienId = chi.YeuCauVatTuBenhVienId,
                                        DonThuocThanhToanChiTietId = chi.DonThuocThanhToanChiTietId,
                                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = chi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                                        YeuCauTruyenMauId = chi.YeuCauTruyenMauId,
                                        YeuCauGoiDichVuId = chi.YeuCauGoiDichVuId,
                                    }).ToList(),
                //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                //SoBLHD = o.SoPhieuHienThi,
                //TongChiPhiBNTT = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                //CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                //CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                //SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                //SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                //SoTienThuPos = o.POS.GetValueOrDefault(),
                //SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                //NguoiThu = o.NhanVienThucHien.User.HoTen,
                //ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList(),
            }).ToList();
            var phieuHuys = phieuHuyDataQuery.Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
            {
                PhieuThuId = o.Id,
                NgayThu = o.NgayHuy ?? o.NgayThu,
                LaPhieuHuy = true,
                MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                DataPhieuChis = o.TaiKhoanBenhNhanChis
                                    .Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                                    .Select(chi => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
                                    {
                                        Id = chi.Id,
                                        NgayChi = chi.NgayChi,
                                        TienChiPhi = chi.TienChiPhi,
                                        SoTienBaoHiemTuNhanChiTra = chi.SoTienBaoHiemTuNhanChiTra,
                                        TienMat = 0,
                                        ChuyenKhoan = 0,
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
            var phieuChis = phieuChiDataQuery.Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
            {
                PhieuChiId = o.Id,
                NgayThu = o.NgayChi,
                LaPhieuHuy = false,
                MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                GoiDichVu = o.TaiKhoanBenhNhanThu != null && o.TaiKhoanBenhNhanThu.ThuTienGoiDichVu != null && o.TaiKhoanBenhNhanThu.ThuTienGoiDichVu == true,
                DataPhieuChis = new List<BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi>(){ new BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
                                    {
                                        Id = o.Id,
                                        NgayChi = o.NgayChi,
                                        TienChiPhi = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.TienChiPhi : 0,
                                        SoTienBaoHiemTuNhanChiTra = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.SoTienBaoHiemTuNhanChiTra : 0,
                                        TienMat = 0,
                                        ChuyenKhoan = 0,
                                        YeuCauKhamBenhId = o.YeuCauKhamBenhId,
                                        YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId,
                                        YeuCauDuocPhamBenhVienId = o.YeuCauDuocPhamBenhVienId,
                                        YeuCauVatTuBenhVienId = o.YeuCauVatTuBenhVienId,
                                        DonThuocThanhToanChiTietId = o.DonThuocThanhToanChiTietId,
                                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                                        YeuCauTruyenMauId = o.YeuCauTruyenMauId,
                                    } },
            }).ToList();

            var allDataThuChi = phieuThus.Concat(phieuHuys).Concat(phieuChis).ToList();

            var dichVuKhamBenhIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauKhamBenhId != null).Select(o => o.YeuCauKhamBenhId).Distinct().ToList();
            var dichVuKyThuatIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuKyThuatId != null).Select(o => o.YeuCauDichVuKyThuatId).Distinct().ToList();
            var dichVuGiuongIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null).Select(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId).Distinct().ToList();
            var truyenMauIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauTruyenMauId != null).Select(o => o.YeuCauTruyenMauId).Distinct().ToList();
            var yeuCauDuocPhamIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(o => o.YeuCauDuocPhamBenhVienId).Distinct().ToList();
            var yeuCauVatTuIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauVatTuBenhVienId != null).Select(o => o.YeuCauVatTuBenhVienId).Distinct().ToList();
            var donThuocIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTietId).Distinct().ToList();
            var thongTinQuyetToanGoiDichVuIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauGoiDichVuId != null).Select(o => o.Id).Distinct().ToList();

            var maxTake = 18000;                                    

            //var dataDichVuKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking.Where(o => dichVuKhamBenhIds.Contains(o.Id))
            //    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
            //    {
            //        Id = o.Id,
            //        TenDichVu = o.TenDichVu,                    
            //        YeuCauKhamBenh = true,
            //        NoiThucHienId = o.NoiThucHienId ?? o.NoiDangKyId,
            //        NoiChiDinhId = o.NoiChiDinhId
            //    }).ToList();
            var dataDichVuKhamBenh = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
            for (int i = 0; i < dichVuKhamBenhIds.Count; i = i + maxTake)
            {
                var takeIds = dichVuKhamBenhIds.Skip(i).Take(maxTake).ToList();

                var info = _yeuCauKhamBenhRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    YeuCauKhamBenh = true,
                    NoiThucHienId = o.NoiThucHienId ?? o.NoiDangKyId,
                    NoiChiDinhId = o.NoiChiDinhId
                }).ToList();

                dataDichVuKhamBenh.AddRange(info);
            }

            //var dataDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => dichVuKyThuatIds.Contains(o.Id))
            //    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
            //    {
            //        Id = o.Id,
            //        TenDichVu = o.TenDichVu,                    
            //        YeuCauDichVuKyThuat = true,
            //        LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
            //        NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
            //        NoiThucHienId = o.NoiThucHienId,
            //        NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
            //        NoiChiDinhId = o.NoiChiDinhId
            //    }).ToList();
            var dataDichVuKyThuat = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
            for (int i = 0; i < dichVuKyThuatIds.Count; i = i + maxTake)
            {
                var takeIds = dichVuKyThuatIds.Skip(i).Take(maxTake).ToList();

                var info = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    YeuCauDichVuKyThuat = true,
                    LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    NoiThucHienId = o.NoiThucHienId,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId
                }).ToList();

                dataDichVuKyThuat.AddRange(info);
            }            

            var dataDichVuGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking.Where(o => dichVuGiuongIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,                    
                    YeuCauGiuong = true,
                    NoiThucHienId = o.PhongBenhVienId,
                    NoiChiDinhId = o.PhongBenhVienId
                }).ToList();

            var dataTruyenMau = _yeuCauTruyenMauRepository.TableNoTracking.Where(o => truyenMauIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,                    
                    YeuCauTruyenMau = true,
                    NoiThucHienId = o.NoiThucHienId,
                    NoiChiDinhId = o.NoiChiDinhId
                }).ToList();

            //var dataYeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(o => yeuCauDuocPhamIds.Contains(o.Id))
            //    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
            //    {
            //        Id = o.Id,
            //        TenDichVu = o.Ten,                    
            //        YeuCauDuocPham = true,
            //        NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
            //        NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
            //        NoiChiDinhId = o.NoiChiDinhId
            //    }).ToList();
            var dataYeuCauDuocPham = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
            for (int i = 0; i < yeuCauDuocPhamIds.Count; i = i + maxTake)
            {
                var takeIds = yeuCauDuocPhamIds.Skip(i).Take(maxTake).ToList();

                var info = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    YeuCauDuocPham = true,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId
                }).ToList();

                dataYeuCauDuocPham.AddRange(info);
            }

            //var dataYeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(o => yeuCauVatTuIds.Contains(o.Id))
            //    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
            //    {
            //        Id = o.Id,
            //        TenDichVu = o.Ten,                    
            //        YeuCauVatTu = true,
            //        NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
            //        NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
            //        NoiChiDinhId = o.NoiChiDinhId
            //    }).ToList();
            var dataYeuCauVatTu = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
            for (int i = 0; i < yeuCauVatTuIds.Count; i = i + maxTake)
            {
                var takeIds = yeuCauVatTuIds.Skip(i).Take(maxTake).ToList();

                var info = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    YeuCauVatTu = true,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId
                }).ToList();

                dataYeuCauVatTu.AddRange(info);
            }

            //var dataDonThuoc = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => donThuocIds.Contains(o.Id))
            //    .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
            //    {
            //        Id = o.Id,
            //        TenDichVu = o.Ten,                    
            //        DonThuocBHYT = true,
            //        NoiThucHienKhamBenhId = o.DonThuocThanhToan.YeuCauKhamBenhId != null ? o.DonThuocThanhToan.YeuCauKhamBenh.NoiThucHienId : null,
            //        NoiChiDinhId = o.DonThuocThanhToan.NoiTruDonThuocId != null ? o.DonThuocThanhToan.NoiTruDonThuoc.NoiKeDonId : (long?)null,
            //    }).ToList();
            var dataDonThuoc = new List<BaoCaoDoanhThuTheoNhomDichVuDataDichVu>();
            for (int i = 0; i < donThuocIds.Count; i = i + maxTake)
            {
                var takeIds = donThuocIds.Skip(i).Take(maxTake).ToList();

                var info = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    DonThuocBHYT = true,
                    NoiThucHienKhamBenhId = o.DonThuocThanhToan.YeuCauKhamBenhId != null ? o.DonThuocThanhToan.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiChiDinhId = o.DonThuocThanhToan.NoiTruDonThuocId != null ? o.DonThuocThanhToan.NoiTruDonThuoc.NoiKeDonId : (long?)null,
                }).ToList();

                dataDonThuoc.AddRange(info);
            }

            var taiKhoanBenhNhanChiThongTins = _taiKhoanBenhNhanChiThongTinRepository.TableNoTracking.Where(o => thongTinQuyetToanGoiDichVuIds.Contains(o.Id)).ToList();

            var dsMaYCTNChuaCoSoBenhAns = allDataThuChi.Where(o => string.IsNullOrEmpty(o.SoBenhAn)).Select(o => o.MaYeuCauTiepNhan).Distinct().ToList();

            var dsSoBenhAn = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null && dsMaYCTNChuaCoSoBenhAns.Contains(o.MaYeuCauTiepNhan))
                .Select(o => new { o.MaYeuCauTiepNhan, o.NoiTruBenhAn.SoBenhAn }).ToList();

            var baoCaoDoanhThuTheoNhomDichVuGridVos = new List<BaoCaoDoanhThuTheoNhomDichVuGridVo>();
            foreach (var dataPhieuThu in allDataThuChi)
            {
                foreach (var dataPhieuChi in dataPhieuThu.DataPhieuChis)
                {
                    if (dataPhieuChi.YeuCauGoiDichVuId != null)
                    {
                        var taiKhoanBenhNhanChiThongTin = taiKhoanBenhNhanChiThongTins.FirstOrDefault(o => o.Id == dataPhieuChi.Id);
                        if (taiKhoanBenhNhanChiThongTin != null)
                        {
                            List<NoiDungQuyetToanGoiMarketing> noiDungQuyetToanGoiMarketings = null;
                            try
                            {
                                noiDungQuyetToanGoiMarketings = JsonConvert.DeserializeObject<List<NoiDungQuyetToanGoiMarketing>>(taiKhoanBenhNhanChiThongTin.NoiDung);
                            }
                            catch (Exception ex)
                            {
                                
                            }

                            if (noiDungQuyetToanGoiMarketings != null && noiDungQuyetToanGoiMarketings.Count > 0)
                            {
                                foreach (var noiDungQuyetToanGoiMarketing in noiDungQuyetToanGoiMarketings)
                                {
                                    var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoDoanhThuTheoNhomDichVuGridVo
                                    {
                                        Id = dataPhieuChi.Id,
                                        MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                                        MaNB = dataPhieuThu.MaNB,
                                        HoVaTen = dataPhieuThu.HoTen,
                                        NamSinh = dataPhieuThu.NamSinh?.ToString(),
                                        GioiTinh = dataPhieuThu.GioiTinh?.GetDescription(),
                                        SoBenhAn = dataPhieuThu.SoBenhAn,
                                        NgayThu = dataPhieuThu.NgayThu,
                                        NguoiGioiThieu = dataPhieuThu.NguoiGioiThieu
                                    };
                                    if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                                    {
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                                    }
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = noiDungQuyetToanGoiMarketing.NoiDung;
                                    var thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * (noiDungQuyetToanGoiMarketing.ThanhTien);
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.TongCong = thanhTien;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauKhamBenh ? thanhTien : 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuGiuong ? thanhTien : 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = 0;

                                    baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                                    if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat)
                                    {
                                        if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = thanhTien;
                                        }
                                        else if (NhomDichVuNoiSoi.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = thanhTien;
                                        }
                                        else if (NhomDichVuNoiSoiTMH.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = thanhTien;
                                        }
                                        else if (NhomDichVuSieuAm.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = thanhTien;
                                        }
                                        else if (NhomDichVuXQuang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = thanhTien;
                                        }
                                        else if (NhomDichVuCTScan.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = thanhTien;
                                        }
                                        else if (NhomDichVuMRI.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = thanhTien;
                                        }
                                        else if (NhomDichVuDienTimDienNao.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = thanhTien;
                                        }
                                        else if (NhomDichVuTDCNDoLoang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = thanhTien;
                                        }
                                        else if (NhomDichVuPhauThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                        }
                                        else if (NhomDichVuThuThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                        }
                                        else if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                        {
                                            if (noiDungQuyetToanGoiMarketing.NoiDung != null &&
                                                noiDungQuyetToanGoiMarketing.NoiDung.ToLower().Contains("phẫu thuật"))
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                            }
                                            else
                                            {
                                                baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                            }

                                        }
                                        else
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thanhTien;
                                        }
                                    }

                                    baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";

                                    baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                                }
                            }
                        }
                    }
                    else
                    {
                        var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoDoanhThuTheoNhomDichVuGridVo
                        {
                            Id = dataPhieuChi.Id,
                            MaNB = dataPhieuThu.MaNB,
                            MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                            HoVaTen = dataPhieuThu.HoTen,
                            NamSinh = dataPhieuThu.NamSinh?.ToString(),
                            GioiTinh = dataPhieuThu.GioiTinh?.GetDescription(),
                            SoBenhAn = dataPhieuThu.SoBenhAn,
                            NgayThu = dataPhieuThu.NgayThu,
                            NguoiGioiThieu = dataPhieuThu.NguoiGioiThieu
                        };
                        if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                        }

                        BaoCaoDoanhThuTheoNhomDichVuDataDichVu thongTinDichVu = null;
                        if (dataPhieuChi.YeuCauKhamBenhId != null)
                        {
                            thongTinDichVu = dataDichVuKhamBenh.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauKhamBenhId);
                        }
                        else if (dataPhieuChi.YeuCauDichVuKyThuatId != null)
                        {
                            thongTinDichVu = dataDichVuKyThuat.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDichVuKyThuatId);
                        }
                        else if (dataPhieuChi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null)
                        {
                            thongTinDichVu = dataDichVuGiuong.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId);
                        }
                        else if (dataPhieuChi.YeuCauTruyenMauId != null)
                        {
                            thongTinDichVu = dataTruyenMau.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauTruyenMauId);
                        }
                        else if (dataPhieuChi.YeuCauDuocPhamBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauDuocPham.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDuocPhamBenhVienId);
                        }
                        else if (dataPhieuChi.YeuCauVatTuBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauVatTu.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauVatTuBenhVienId);
                        }
                        else if (dataPhieuChi.DonThuocThanhToanChiTietId != null)
                        {
                            thongTinDichVu = dataDonThuoc.FirstOrDefault(o => o.Id == dataPhieuChi.DonThuocThanhToanChiTietId);
                        }
                        if (thongTinDichVu == null)
                        {
                            thongTinDichVu = new BaoCaoDoanhThuTheoNhomDichVuDataDichVu()
                            {
                                TenDichVu = "Dịch vụ giường",
                                YeuCauGiuong = true
                            };
                        }
                        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = thongTinDichVu.TenDichVu;
                        var thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * (dataPhieuChi.TienChiPhi.GetValueOrDefault() + dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault());
                        baoCaoDoanhThuTheoNhomDichVuGridVo.TongCong = thanhTien;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = thongTinDichVu.YeuCauKhamBenh ? thanhTien : 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = thongTinDichVu.YeuCauGiuong ? thanhTien : 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thongTinDichVu.YeuCauTruyenMau ? thanhTien : 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = thongTinDichVu.YeuCauDuocPham || thongTinDichVu.DonThuocBHYT ? thanhTien : 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = thongTinDichVu.YeuCauVatTu ? thanhTien : 0;

                        baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                        if (thongTinDichVu.YeuCauDichVuKyThuat)
                        {
                            if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = thanhTien;
                            }
                            else if (NhomDichVuNoiSoi.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = thanhTien;
                            }
                            else if (NhomDichVuNoiSoiTMH.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = thanhTien;
                            }
                            else if (NhomDichVuSieuAm.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = thanhTien;
                            }
                            else if (NhomDichVuXQuang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = thanhTien;
                            }
                            else if (NhomDichVuCTScan.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = thanhTien;
                            }
                            else if (NhomDichVuMRI.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = thanhTien;
                            }
                            else if (NhomDichVuDienTimDienNao.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = thanhTien;
                            }
                            else if (NhomDichVuTDCNDoLoang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = thanhTien;
                            }
                            else if (NhomDichVuPhauThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                            }
                            else if (NhomDichVuThuThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                            }
                            else if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                            {
                                if (thongTinDichVu.TenDichVu != null &&
                                    thongTinDichVu.TenDichVu.ToLower().Contains("phẫu thuật"))
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                }
                                else
                                {
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                }

                            }
                            else
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thanhTien;
                            }
                        }

                        if (dataPhieuThu.GoiDichVu)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                        }
                        else
                        {
                            long? noiThucHienDuocTinhId = null;
                            if (thongTinDichVu.YeuCauKhamBenh)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId;
                            }
                            if (thongTinDichVu.YeuCauDichVuKyThuat)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId ?? thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauGiuong)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauTruyenMau)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                            }
                            if (thongTinDichVu.YeuCauDuocPham)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                                //if (thongTinDichVu.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                                //{
                                //    noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                                //}
                                //else
                                //{
                                //    noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                                //}
                            }
                            if (thongTinDichVu.YeuCauVatTu)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                                //if (thongTinDichVu.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                                //{
                                //    noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                                //}
                                //else
                                //{
                                //    noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                                //}
                            }
                            if (thongTinDichVu.DonThuocBHYT)
                            {
                                noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                            }
                            var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == noiThucHienDuocTinhId)?.KhoaPhong.Ten;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = tenKhoaPhong ?? "Không xác định";
                        }

                        baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                    }
                }
            }
            return new GridDataSource { Data = baoCaoDoanhThuTheoNhomDichVuGridVos.OrderBy(o => o.Nhom.Equals("Gói dịch vụ") ? 1 : 2).ThenBy(o => o.Nhom).ThenBy(o => o.NgayThu).ToArray(), TotalRowCount = baoCaoDoanhThuTheoNhomDichVuGridVos.Count };
        }
        public async Task<GridDataSource> OldGetDataBaoCaoDoanhThuTheoNhomDichVuAsync(BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay;
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToList();
                        
            var dataDichVuKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking.Where(o => o.KhongTinhPhi != true && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh < denNgay && o.TaiKhoanBenhNhanChis.Any(c=>c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "",
                    YeuCauGoiDichVuId = o.YeuCauGoiDichVuId,
                    HopDongKhamSucKhoeId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c=>new BaoCaoDoanhThuTheoNhomDichVuDataChiVo{Id = c.Id,DaHuy = c.DaHuy, NgayChi = c.NgayChi}).ToList(),
                    Soluong = 1,
                    Gia = o.Gia,
                    DonGiaSauChietKhau = o.DonGiaSauChietKhau,
                    SoTienMienGiam = o.SoTienMienGiam,
                    NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieuId != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    YeuCauKhamBenh = true,
                    NoiThucHienId = o.NoiThucHienId ?? o.NoiDangKyId,
                    NoiChiDinhId = o.NoiChiDinhId,
                    ThoiDiemHoanThanh = o.ThoiDiemHoanThanh
                }).ToList();
            var dataDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => o.KhongTinhPhi != true && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.TaiKhoanBenhNhanChis.Any(c => c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "",
                    YeuCauGoiDichVuId = o.YeuCauGoiDichVuId,
                    HopDongKhamSucKhoeId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    Soluong = o.SoLan,
                    Gia = o.Gia,
                    DonGiaSauChietKhau = o.DonGiaSauChietKhau,
                    SoTienMienGiam = o.SoTienMienGiam,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoDoanhThuTheoNhomDichVuDataChiVo { Id = c.Id, DaHuy = c.DaHuy, NgayChi = c.NgayChi }).ToList(),
                    NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieuId != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    YeuCauDichVuKyThuat = true,
                    LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    NoiThucHienId = o.NoiThucHienId,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId,
                    ThoiDiemHoanThanh = o.ThoiDiemHoanThanh
                }).ToList();

            var dataDichVuGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.TaiKhoanBenhNhanChis.Any(c => c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "",
                    YeuCauGoiDichVuId = o.YeuCauGoiDichVuId,
                    HopDongKhamSucKhoeId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    Soluong = o.SoLuong,
                    Gia = o.Gia,
                    DonGiaSauChietKhau = o.DonGiaSauChietKhau,
                    SoTienMienGiam = o.SoTienMienGiam,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoDoanhThuTheoNhomDichVuDataChiVo { Id = c.Id, DaHuy = c.DaHuy, NgayChi = c.NgayChi }).ToList(),
                    NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieuId != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    YeuCauGiuong = true,
                    NoiThucHienId = o.PhongBenhVienId,
                    NoiChiDinhId = o.PhongBenhVienId,
                    ThoiDiemHoanThanh = o.NgayPhatSinh
                }).ToList();

            var dataTruyenMau = _yeuCauTruyenMauRepository.TableNoTracking.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.TaiKhoanBenhNhanChis.Any(c => c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "",
                    HopDongKhamSucKhoeId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    Soluong = 1,
                    Gia = o.DonGiaBan.GetValueOrDefault(),
                    DonGiaSauChietKhau = 0,
                    SoTienMienGiam = o.SoTienMienGiam,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoDoanhThuTheoNhomDichVuDataChiVo { Id = c.Id, DaHuy = c.DaHuy, NgayChi = c.NgayChi }).ToList(),
                    NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieuId != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    YeuCauTruyenMau = true,
                    NoiThucHienId = o.NoiThucHienId,
                    NoiChiDinhId = o.NoiChiDinhId,
                    ThoiDiemHoanThanh = o.ThoiDiemHoanThanh
                }).ToList();

            var dataYeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(o => o.KhongTinhPhi != true && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.TaiKhoanBenhNhanChis.Any(c => c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "",
                    HopDongKhamSucKhoeId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    Soluong = o.SoLuong,
                    Gia = o.DonGiaBan,
                    DonGiaSauChietKhau = 0,
                    SoTienMienGiam = o.SoTienMienGiam,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoDoanhThuTheoNhomDichVuDataChiVo { Id = c.Id, DaHuy = c.DaHuy, NgayChi = c.NgayChi }).ToList(),
                    NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieuId != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    YeuCauDuocPham = true,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ?  o.YeuCauDichVuKyThuat.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId,
                    ThoiDiemHoanThanh = o.XuatKhoDuocPhamChiTietId != null ? o.XuatKhoDuocPhamChiTiet.NgayXuat : null
                }).ToList();
            var dataYeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(o => o.KhongTinhPhi != true && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.TaiKhoanBenhNhanChis.Any(c => c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : "",
                    HopDongKhamSucKhoeId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    Soluong = o.SoLuong,
                    Gia = o.DonGiaBan,
                    DonGiaSauChietKhau = 0,
                    SoTienMienGiam = o.SoTienMienGiam,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoDoanhThuTheoNhomDichVuDataChiVo { Id = c.Id, DaHuy = c.DaHuy, NgayChi = c.NgayChi }).ToList(),
                    NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieuId != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    YeuCauVatTu = true,
                    NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
                    NoiChiDinhId = o.NoiChiDinhId,
                    ThoiDiemHoanThanh = o.XuatKhoVatTuChiTietId != null ? o.XuatKhoVatTuChiTiet.NgayXuat : null
                }).ToList();

            var dataDonThuoc = _donThuocThanhToanRepository.TableNoTracking.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.LoaiDonThuoc ==Enums.EnumLoaiDonThuoc.ThuocBHYT)
                .SelectMany(o=>o.DonThuocThanhToanChiTiets).Where(o=>o.TaiKhoanBenhNhanChis.Any(c => c.NgayChi >= tuNgay && c.NgayChi < denNgay))
                .Select(o => new BaoCaoDoanhThuTheoNhomDichVuDataVo
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    LoaiYeuCauTiepNhan = o.DonThuocThanhToan.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaYeuCauTiepNhan = o.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                    NamSinh = o.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.DonThuocThanhToan.YeuCauTiepNhan.GioiTinh,
                    HopDongKhamSucKhoeId = o.DonThuocThanhToan.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? o.DonThuocThanhToan.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : (long?)null,
                    Soluong = o.SoLuong,
                    Gia = o.DonGiaBan,
                    DonGiaSauChietKhau = 0,
                    SoTienMienGiam = o.SoTienMienGiam,
                    DataChis = o.TaiKhoanBenhNhanChis.Select(c => new BaoCaoDoanhThuTheoNhomDichVuDataChiVo { Id = c.Id, DaHuy = c.DaHuy, NgayChi = c.NgayChi }).ToList(),
                    NguoiGioiThieu = o.DonThuocThanhToan.YeuCauTiepNhan.NoiGioiThieuId != null ? o.DonThuocThanhToan.YeuCauTiepNhan.NoiGioiThieu.Ten : "",
                    DonThuocBHYT = true,
                    NoiThucHienKhamBenhId = o.DonThuocThanhToan.YeuCauKhamBenhId != null ? o.DonThuocThanhToan.YeuCauKhamBenh.NoiThucHienId : null,
                    NoiChiDinhId = o.DonThuocThanhToan.NoiTruDonThuocId != null ? o.DonThuocThanhToan.NoiTruDonThuoc.NoiKeDonId : (long?)null,
                    ThoiDiemHoanThanh = o.XuatKhoDuocPhamChiTietViTri.NgayXuat
                }).ToList();

            var allData = dataDichVuKhamBenh.Concat(dataDichVuKyThuat).Concat(dataDichVuGiuong).Concat(dataTruyenMau)
                .Concat(dataYeuCauDuocPham).Concat(dataYeuCauVatTu).Concat(dataDonThuoc);

            var baoCaoDoanhThuTheoNhomDichVuGridVos = new List<BaoCaoDoanhThuTheoNhomDichVuGridVo>();
            foreach (var baoCaoDoanhThuTheoNhomDichVuDataVo in allData.Where(o=>o.HopDongKhamSucKhoeId == null))
            {
                var ngayThu = baoCaoDoanhThuTheoNhomDichVuDataVo.DataChis.OrderBy(o=>o.NgayChi).LastOrDefault(o => o.DaHuy != true)?.NgayChi;
                if (ngayThu != null && ngayThu >= tuNgay && ngayThu < denNgay && baoCaoDoanhThuTheoNhomDichVuDataVo.ThoiDiemTinhHoanThanh != null && baoCaoDoanhThuTheoNhomDichVuDataVo.ThoiDiemTinhHoanThanh < denNgay)
                {
                    var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoDoanhThuTheoNhomDichVuGridVo
                    {
                        Id = baoCaoDoanhThuTheoNhomDichVuDataVo.Id,
                        MaTN = baoCaoDoanhThuTheoNhomDichVuDataVo.MaYeuCauTiepNhan,
                        HoVaTen = baoCaoDoanhThuTheoNhomDichVuDataVo.HoTen,
                        NamSinh = baoCaoDoanhThuTheoNhomDichVuDataVo.NamSinh?.ToString(),
                        GioiTinh = baoCaoDoanhThuTheoNhomDichVuDataVo.GioiTinh?.GetDescription(),
                        SoBenhAn = baoCaoDoanhThuTheoNhomDichVuDataVo.SoBenhAn,
                        NoiDung = baoCaoDoanhThuTheoNhomDichVuDataVo.TenDichVu,
                        NgayThu = ngayThu.Value,
                        NguoiGioiThieu = baoCaoDoanhThuTheoNhomDichVuDataVo.NguoiGioiThieu
                    };

                    baoCaoDoanhThuTheoNhomDichVuGridVo.TongCong = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauKhamBenh ? baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien : 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauGiuong ? baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien : 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauTruyenMau ? baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien : 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauDuocPham || baoCaoDoanhThuTheoNhomDichVuDataVo.DonThuocBHYT ? baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien : 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauVatTu ? baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien : 0;

                    baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                    baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                    if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauDichVuKyThuat)
                    {
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if(NhomDichVuNoiSoi.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuNoiSoiTMH.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuSieuAm.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuXQuang.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuCTScan.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuMRI.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuDienTimDienNao.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuTDCNDoLoang.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuPhauThuat.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (NhomDichVuThuThuat.Contains(baoCaoDoanhThuTheoNhomDichVuDataVo.NhomDichVuBenhVienId.GetValueOrDefault()))
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                        else if (baoCaoDoanhThuTheoNhomDichVuDataVo.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                        {
                            if (baoCaoDoanhThuTheoNhomDichVuDataVo.TenDichVu != null &&
                                baoCaoDoanhThuTheoNhomDichVuDataVo.TenDichVu.ToLower().Contains("phẫu thuật"))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                            }
                            else
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                            }
                            
                        }
                        else
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = baoCaoDoanhThuTheoNhomDichVuDataVo.ThanhTien;
                        }
                    }

                    if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauGoiDichVuId != null)
                    {
                        baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                    }
                    else
                    {
                        long? noiThucHienDuocTinhId = null;
                        if(baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauKhamBenh)
                        {
                            noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienId;
                        }
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauDichVuKyThuat)
                        {
                            noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienId ?? baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                        }
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauGiuong)
                        {
                            noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                        }
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauTruyenMau)
                        {
                            noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                        }
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauDuocPham)
                        {
                            if (baoCaoDoanhThuTheoNhomDichVuDataVo.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                            {
                                noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                            }
                            else
                            {
                                noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienPTTTId ?? baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienKhamBenhId ?? baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                            }
                        }
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.YeuCauVatTu)
                        {
                            if (baoCaoDoanhThuTheoNhomDichVuDataVo.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                            {
                                noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                            }
                            else
                            {
                                noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienPTTTId ?? baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienKhamBenhId ?? baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                            }
                        }
                        if (baoCaoDoanhThuTheoNhomDichVuDataVo.DonThuocBHYT)
                        {
                            noiThucHienDuocTinhId = baoCaoDoanhThuTheoNhomDichVuDataVo.NoiThucHienKhamBenhId ?? baoCaoDoanhThuTheoNhomDichVuDataVo.NoiChiDinhId;
                        }
                        var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == noiThucHienDuocTinhId)?.KhoaPhong.Ten;
                        baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = tenKhoaPhong;
                    }

                    baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                }
            }
            return new GridDataSource { Data = baoCaoDoanhThuTheoNhomDichVuGridVos.OrderBy(o=>o.Nhom.Equals("Gói dịch vụ")?1:2).ThenBy(o => o.Nhom).ThenBy(o => o.NgayThu).ToArray(), TotalRowCount = baoCaoDoanhThuTheoNhomDichVuGridVos.Count };
        }
    }
}
