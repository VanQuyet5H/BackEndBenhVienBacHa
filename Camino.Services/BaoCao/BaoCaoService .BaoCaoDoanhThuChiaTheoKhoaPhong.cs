using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        #region  GetDataDoanhThuChiaTheoKhoaPhongForGridAsync
        /// <summary>
        /// GetDataDoanhThuChiaTheoKhoaPhongForGridAsync 
        /// </summary>
        /// <param name="BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo"></param>
        /// <returns></returns>
        public async Task<List<BaoCaoDoanhThuChiaTheoKhoaPhongGridVo>> GetDataDoanhThuChiaTheoKhoaPhongForGridAsync(BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay;
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            string[] noiGioiThieuTachDoanhThuDPVTIds = cauHinhBaoCao.NoiGioiThieuTachDoanhThuDPVTIds != null ? cauHinhBaoCao.NoiGioiThieuTachDoanhThuDPVTIds?.Split(";") : new string[0];
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToList();
            var khoaPhongs = _KhoaPhongRepository.TableNoTracking.ToList();

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
                //HoTen = o.YeuCauTiepNhan.HoTen,
                //MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                //NamSinh = o.YeuCauTiepNhan.NamSinh,
                //GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
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
                //HoTen = o.YeuCauTiepNhan.HoTen,
                //MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                //NamSinh = o.YeuCauTiepNhan.NamSinh,
                //GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
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
                //HoTen = o.YeuCauTiepNhan.HoTen,
                //MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                //NamSinh = o.YeuCauTiepNhan.NamSinh,
                //GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
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
            var khoaPhongTinhDoanhThuDVKTs = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Where(o => o.KhoaPhongTinhDoanhThuId != null)
                .Select(o => new { o.Id, o.KhoaPhongTinhDoanhThuId })
                .ToList();
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

            //var dsMaYCTNChuaCoSoBenhAns = allDataThuChi.Where(o => string.IsNullOrEmpty(o.SoBenhAn)).Select(o => o.MaYeuCauTiepNhan).Distinct().ToList();

            //var dsSoBenhAn = _yeuCauTiepNhanRepository.TableNoTracking
            //    .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null && dsMaYCTNChuaCoSoBenhAns.Contains(o.MaYeuCauTiepNhan))
            //    .Select(o => new { o.MaYeuCauTiepNhan, o.NoiTruBenhAn.SoBenhAn }).ToList();

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
                                    //if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                                    //{
                                    //    baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                                    //}
                                    //baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                                    //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = noiDungQuyetToanGoiMarketing.NoiDung;
                                    baoCaoDoanhThuTheoNhomDichVuGridVo.KhoaPhongId = (long)EnumKhoaPhong.KhoaPhuSan;
                                    if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat && noiDungQuyetToanGoiMarketing.DichVuBenhVienId != null)
                                    {
                                        var khoaPhongTinhDoanhThuDVKT = khoaPhongTinhDoanhThuDVKTs.FirstOrDefault(o => o.Id == noiDungQuyetToanGoiMarketing.DichVuBenhVienId);
                                        if(khoaPhongTinhDoanhThuDVKT != null)
                                        {
                                            baoCaoDoanhThuTheoNhomDichVuGridVo.KhoaPhongId = khoaPhongTinhDoanhThuDVKT.KhoaPhongTinhDoanhThuId;
                                        }
                                    }

                                    if (dataPhieuThu.NoiGioiThieuId != null)
                                    {
                                        var hopDong = thongTinHopDongs
                                            .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                        && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                            .OrderBy(o => o.NgayBatDau)
                                            .LastOrDefault();
                                        if (hopDong != null)
                                        {
                                            if (noiDungQuyetToanGoiMarketing.DichVuBenhVienId != null && noiDungQuyetToanGoiMarketing.NhomGiaDichVuId != null && noiDungQuyetToanGoiMarketing.SoLuong != null && noiDungQuyetToanGoiMarketing.DonGia != null)
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
                        //if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                        //{
                        //    baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                        //}

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
                                        if (chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan2 == null)
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
                                            if (lanYeuCau == 1)
                                            {
                                                heSo = chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan1;
                                            }
                                            else if (lanYeuCau == 2)
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

                                            foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
                                            {
                                                if (yeuCauDichVuKyThuat.Id != dataPhieuChi.YeuCauDichVuKyThuatId)
                                                {
                                                    solanDaYeuCau += yeuCauDichVuKyThuat.SoLan;
                                                }
                                                else
                                                {
                                                    if (dataPhieuChi.SoLuong.GetValueOrDefault() == 1)
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
                                                        if (solanDaYeuCau > 1)
                                                        {
                                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                                        }
                                                        else
                                                        {
                                                            for (int i = solanDaYeuCau; i < (solanDaYeuCau + dataPhieuChi.SoLuong.GetValueOrDefault()); i++)
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
                                                            if (heSoDichVuKyThuats.Distinct().Count() == 1)
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
                                                .Where(o => yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Contains(o.Id) && o.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaDichVuGiuongBenhVienId)
                                                .OrderBy(o => o.ThoiDiemChiDinh)
                                                .Select(o => o.Id).ToList();

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
                                if (hopDong != null)
                                {
                                    var chiTietHeSoDuocPham = hopDong.ChiTietHeSoDuocPhams.Where(o => o.DuocPhamBenhVienId == thongTinDichVu.DuocPhamBenhVienId).FirstOrDefault();
                                    if (chiTietHeSoDuocPham != null)
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
                        var khoaPhongTinhDoanhThuId = (long)EnumKhoaPhong.KhoaPhuSan;
                        if (thongTinDichVu == null)
                        {
                            thongTinDichVu = new BaoCaoDoanhThuTheoNhomDichVuDataDichVu()
                            {
                                TenDichVu = "Dịch vụ giường",
                                YeuCauGiuong = true                                
                            };
                        }
                        long? noiThucHienDuocTinhId = null;
                        if (thongTinDichVu.YeuCauKhamBenh)
                        {
                            noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId;
                        }
                        if (thongTinDichVu.YeuCauDichVuKyThuat)
                        {
                            noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId ?? thongTinDichVu.NoiChiDinhId;                                
                            if (thongTinDichVu.DichVuKyThuatBenhVienId != null)
                            {
                                var khoaPhongTinhDoanhThuDVKT = khoaPhongTinhDoanhThuDVKTs.FirstOrDefault(o => o.Id == thongTinDichVu.DichVuKyThuatBenhVienId);
                                if (khoaPhongTinhDoanhThuDVKT != null)
                                {
                                    khoaPhongTinhDoanhThuId = khoaPhongTinhDoanhThuDVKT.KhoaPhongTinhDoanhThuId.Value;
                                    noiThucHienDuocTinhId = null;
                                }
                            }
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
                            //Tổng doanh thu thuốc + VTYT trong báo cáo doanh thu theo nhóm dịch vụ đối với NB có hình thức đến là BS. Ngô Anh Tú
                            if (dataPhieuThu.NoiGioiThieuId != null && noiGioiThieuTachDoanhThuDPVTIds.Contains(dataPhieuThu.NoiGioiThieuId.ToString()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.ThuocVTYTThamMy = true;
                                khoaPhongTinhDoanhThuId = (long)EnumKhoaPhong.KhoaThamMy;
                                noiThucHienDuocTinhId = null;
                            }

                        }
                        if (thongTinDichVu.YeuCauVatTu)
                        {
                            noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                            //Tổng doanh thu thuốc + VTYT trong báo cáo doanh thu theo nhóm dịch vụ đối với NB có hình thức đến là BS. Ngô Anh Tú
                            if (dataPhieuThu.NoiGioiThieuId != null && noiGioiThieuTachDoanhThuDPVTIds.Contains(dataPhieuThu.NoiGioiThieuId.ToString()))
                            {
                                baoCaoDoanhThuTheoNhomDichVuGridVo.ThuocVTYTThamMy = true;
                                khoaPhongTinhDoanhThuId = (long)EnumKhoaPhong.KhoaThamMy;
                                noiThucHienDuocTinhId = null;
                            }
                        }
                        if (thongTinDichVu.DonThuocBHYT)
                        {
                            noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                        }
                        if(noiThucHienDuocTinhId != null)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.KhoaPhongId = phongBenhViens.First(o => o.Id == noiThucHienDuocTinhId).KhoaPhongId;
                        }
                        else
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.KhoaPhongId = khoaPhongTinhDoanhThuId;
                        }


                        if (thongTinDichVu.DonGiaNhapDuocPham != null || thongTinDichVu.DonGiaNhapVatTu != null)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho = Math.Round((thongTinDichVu.DonGiaNhapDuocPham ?? thongTinDichVu.DonGiaNhapVatTu).GetValueOrDefault() * (decimal)dataPhieuChi.SoLuong.GetValueOrDefault(), 2);
                        }

                        if (!heSoDichVuKyThuats.Any())
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam = dataPhieuChi.SoTienMienGiam.GetValueOrDefault();
                            baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra = Math.Round(((decimal)dataPhieuChi.SoLuong.GetValueOrDefault() * dataPhieuChi.DonGiaBaoHiem.GetValueOrDefault() * dataPhieuChi.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * dataPhieuChi.MucHuongBaoHiem.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet = dataPhieuChi.TienChiPhi.GetValueOrDefault() + dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() + dataPhieuChi.SoTienMienGiam.GetValueOrDefault() + baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra.GetValueOrDefault();
                            //if(!baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault().AlmostEqual(Math.Round(dataPhieuChi.Gia.GetValueOrDefault() * (decimal)dataPhieuChi.SoLuong.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)))
                            //{

                            //}    
                            baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo = heSo;
                            decimal thanhTien = 0;
                            if (baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho != null && loaiGia == LoaiGiaNoiGioiThieuHopDong.GiaNhap)
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

                            baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra;

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
                            foreach (var heSoDichVuKyThuat in heSoDichVuKyThuats)
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
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra;
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
            List<BaoCaoDoanhThuChiaTheoKhoaPhongGridVo> returnData = new List<BaoCaoDoanhThuChiaTheoKhoaPhongGridVo>();
            foreach (var groupDoanhThuKhachLe in baoCaoDoanhThuTheoNhomDichVuGridVos.GroupBy(o => o.KhoaPhongId))
            {
                returnData.Add(new BaoCaoDoanhThuChiaTheoKhoaPhongGridVo
                {
                    KhoaPhongId = groupDoanhThuKhachLe.Key.Value,
                    KhoaPhong = khoaPhongs.First(o=>o.Id == groupDoanhThuKhachLe.Key.Value).Ten,
                    DoanhThuKhachLe = groupDoanhThuKhachLe.Where(o=> !o.ThuocVTYTThamMy).Select(o=>o.TongCong.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    DoanhThuThuocVaVTYT = groupDoanhThuKhachLe.Where(o => o.ThuocVTYTThamMy).Select(o => o.TongCong.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    DoanhThuBaoHiemYTe = groupDoanhThuKhachLe.Select(o => o.BHYTChiTra.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                });
            }

            //KSK

            var lstDoanhThuDichVuKhamQuery = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && x.GoiKhamSucKhoeId != null
                                        && ((x.ThoiDiemHoanThanh != null && x.ThoiDiemHoanThanh >= tuNgay && x.ThoiDiemHoanThanh <= denNgay) || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && x.ThoiDiemThucHien >= tuNgay && x.ThoiDiemThucHien <= denNgay))
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null);
            var lstDoanhThuDichVuKyThuatQuery = _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && x.GoiKhamSucKhoeId != null
                                        && ((x.ThoiDiemHoanThanh != null && x.ThoiDiemHoanThanh >= tuNgay && x.ThoiDiemHoanThanh <= denNgay) || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && x.ThoiDiemThucHien >= tuNgay && x.ThoiDiemThucHien <= denNgay))
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null);
            
            var lstDoanhThuDichVuKham = lstDoanhThuDichVuKhamQuery
                            .Select(item => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeDataVo()
                            {
                                YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                YeucauKhamBenhId = item.Id,
                                TenDichVu = item.TenDichVu,
                                GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                                DichVuKhamBenhBenhVienId = item.DichVuKhamBenhBenhVienId,
                                NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                                Gia = item.Gia,
                                SoLan = 1,
                                SoTienMienGiam = item.SoTienMienGiam,
                                NoiThucHienId = item.NoiThucHienId,
                                NoiChiDinhId = item.NoiChiDinhId
                            })
                            .ToList();
            var lstDoanhThuDichVuKyThuat = lstDoanhThuDichVuKyThuatQuery
                            .Select(item => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeDataVo()
                            {
                                YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                YeucauDichVuKyThuatId = item.Id,
                                NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                                LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                                TenDichVu = item.TenDichVu,
                                GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                                DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
                                NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                                Gia = item.Gia,
                                SoLan = item.SoLan,
                                SoTienMienGiam = item.SoTienMienGiam,
                                NoiThucHienId = item.NoiThucHienId,
                                NoiChiDinhId = item.NoiChiDinhId
                            })
                            .ToList();
            var allDichVu = lstDoanhThuDichVuKham.Concat(lstDoanhThuDichVuKyThuat).ToList();

            var goiKhamSucKhoeIds = allDichVu.Select(o => o.GoiKhamSucKhoeId.GetValueOrDefault()).Distinct().ToList();
            var goiKhamSucKhoeDichVuKhamBenhs = _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                .Where(o => goiKhamSucKhoeIds.Contains(o.GoiKhamSucKhoeId))
                .Select(o => new
                {
                    o.GoiKhamSucKhoeId,
                    o.DichVuKhamBenhBenhVienId,
                    o.NhomGiaDichVuKhamBenhBenhVienId,
                    o.DonGiaThucTe
                }).ToList();
            var goiKhamSucKhoeDichVuKyThuats = _goiKhamSucKhoeDichVuKyThuatRepository.TableNoTracking
                .Where(o => goiKhamSucKhoeIds.Contains(o.GoiKhamSucKhoeId))
                .Select(o => new
                {
                    o.GoiKhamSucKhoeId,
                    o.DichVuKyThuatBenhVienId,
                    o.NhomGiaDichVuKyThuatBenhVienId,
                    o.DonGiaThucTe
                }).ToList();


            var doanhThuKSKs = new List<BaoCaoDoanhThuKSKChiaTheoKhoaPhongGridVo>();
            foreach (var dv in allDichVu)
            {                
                var doanhThuKSK = new BaoCaoDoanhThuKSKChiaTheoKhoaPhongGridVo();

                if (dv.YeucauKhamBenhId != null)
                {
                    var goiKhamSucKhoeDichVuKhamBenh = goiKhamSucKhoeDichVuKhamBenhs
                        .FirstOrDefault(o => o.GoiKhamSucKhoeId == dv.GoiKhamSucKhoeId && o.DichVuKhamBenhBenhVienId == dv.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == dv.NhomGiaDichVuKhamBenhBenhVienId);
                    var doanhThu = (goiKhamSucKhoeDichVuKhamBenh?.DonGiaThucTe ?? 0) * dv.SoLan;
                    doanhThuKSK.DoanhThuThucTe = doanhThu;
                    var noiThucHienDuocTinhId = dv.NoiThucHienId ?? dv.NoiChiDinhId;
                    if (noiThucHienDuocTinhId != null)
                    {
                        doanhThuKSK.KhoaPhongId = phongBenhViens.First(o => o.Id == noiThucHienDuocTinhId).KhoaPhongId;
                    }
                }
                else
                {
                    var goiKhamSucKhoeDichVuKyThuat = goiKhamSucKhoeDichVuKyThuats
                        .FirstOrDefault(o => o.GoiKhamSucKhoeId == dv.GoiKhamSucKhoeId && o.DichVuKyThuatBenhVienId == dv.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == dv.NhomGiaDichVuKyThuatBenhVienId);
                    var doanhThu = (goiKhamSucKhoeDichVuKyThuat?.DonGiaThucTe ?? 0) * dv.SoLan;
                    doanhThuKSK.DoanhThuThucTe = doanhThu;

                    var noiThucHienDuocTinhId = dv.NoiThucHienId ?? dv.NoiChiDinhId;
                    if (dv.DichVuKyThuatBenhVienId != null)
                    {
                        var khoaPhongTinhDoanhThuDVKT = khoaPhongTinhDoanhThuDVKTs.FirstOrDefault(o => o.Id == dv.DichVuKyThuatBenhVienId);
                        if (khoaPhongTinhDoanhThuDVKT != null)
                        {
                            doanhThuKSK.KhoaPhongId = khoaPhongTinhDoanhThuDVKT.KhoaPhongTinhDoanhThuId.Value;
                            noiThucHienDuocTinhId = null;
                        }
                    }

                    if (noiThucHienDuocTinhId != null)
                    {
                        doanhThuKSK.KhoaPhongId = phongBenhViens.First(o => o.Id == noiThucHienDuocTinhId).KhoaPhongId;
                    }
                }
                doanhThuKSKs.Add(doanhThuKSK);
            }
            foreach (var groupDoanhThuKSK in doanhThuKSKs.GroupBy(o => o.KhoaPhongId))
            {
                var returnItem = returnData.FirstOrDefault(o => o.KhoaPhongId == groupDoanhThuKSK.Key);
                if (returnItem != null)
                {
                    returnItem.DoanhThuDoan = groupDoanhThuKSK.Select(o => o.DoanhThuThucTe).DefaultIfEmpty().Sum();
                }
                else
                {
                    returnData.Add(new BaoCaoDoanhThuChiaTheoKhoaPhongGridVo
                    {
                        KhoaPhongId = groupDoanhThuKSK.Key,
                        KhoaPhong = khoaPhongs.First(o => o.Id == groupDoanhThuKSK.Key).Ten,
                        DoanhThuDoan = groupDoanhThuKSK.Select(o => o.DoanhThuThucTe).DefaultIfEmpty().Sum()
                    });
                }
            }

            //doanh thu nha thuoc
            var phieuThuNhaThuocs = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc &&
                             (tuNgay <= o.NgayThu || tuNgay <= o.NgayHuy) &&
                             (o.NgayThu <= denNgay || o.NgayHuy <= denNgay))
                .Select(o => new
                {
                    Id = o.NhanVienThucHienId,
                    NgayThu = o.NgayThu,
                    DaHuy = o.DaHuy,
                    NgayHuy = o.NgayHuy,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    PhieuChis = o.TaiKhoanBenhNhanChis
                                    .Select(chi => new 
                                    { 
                                        chi.TienChiPhi, 
                                        chi.TienMat, 
                                        chi.SoTienMienGiam,
                                        chi.SoTienBaoHiemTuNhanChiTra,
                                        chi.Gia, 
                                        chi.SoLuong, 
                                        chi.DonThuocThanhToanChiTietTheoPhieuThuId, 
                                        chi.DonVTYTThanhToanChiTietTheoPhieuThuId 
                                    }).ToList(),

                    TienMat = o.TienMat,
                    ChuyenKhoan = o.ChuyenKhoan,
                    Pos = o.POS,
                    CongNo = o.CongNo,
                    BaoCaoDoanhThuNhaThuocDaTaCongNoVos = o.CongTyBaoHiemTuNhanCongNos.Select(cn => new { SoTien = cn.SoTien }).ToList(),
                }).ToList();

            var donThuocThanhToanChiTietTheoPhieuThuIds = phieuThuNhaThuocs.SelectMany(o => o.PhieuChis)
                .Select(o => o.DonThuocThanhToanChiTietTheoPhieuThuId.GetValueOrDefault()).Distinct().ToList();
            var donVTYTThanhToanChiTietTheoPhieuThuIds = phieuThuNhaThuocs.SelectMany(o => o.PhieuChis)
                .Select(o => o.DonVTYTThanhToanChiTietTheoPhieuThuId.GetValueOrDefault()).Distinct().ToList();

            var chiTietVATDonThuocs = new List<BaoCaoDoanhThuChiTietVATDonThuocDataVo>();
            for (int i = 0; i < donThuocThanhToanChiTietTheoPhieuThuIds.Count; i = i + maxTake)
            {
                var takeIds = donThuocThanhToanChiTietTheoPhieuThuIds.Skip(i).Take(maxTake).ToList();

                var info = _donThuocThanhToanChiTietTheoPhieuThuRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuChiTietVATDonThuocDataVo
                {
                    Id = o.Id,
                    VAT = o.VAT
                }).ToList();

                chiTietVATDonThuocs.AddRange(info);
            }

            var chiTietVATDonVTYTs = new List<BaoCaoDoanhThuChiTietVATDonThuocDataVo>();
            for (int i = 0; i < donVTYTThanhToanChiTietTheoPhieuThuIds.Count; i = i + maxTake)
            {
                var takeIds = donVTYTThanhToanChiTietTheoPhieuThuIds.Skip(i).Take(maxTake).ToList();

                var info = _donVTYTThanhToanChiTietTheoPhieuThuRepository.TableNoTracking.Where(o => takeIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuChiTietVATDonThuocDataVo
                {
                    Id = o.Id,
                    VAT = o.VAT
                }).ToList();

                chiTietVATDonVTYTs.AddRange(info);
            }

            decimal doanhThuNhaThuoc = 0;
            decimal doanhThuChiPhi = 0;
            decimal doanhThuNhaThuocChuaVAT = 0;
            foreach (var phieuThuNhaThuoc in phieuThuNhaThuocs)
            {
                if(phieuThuNhaThuoc.DaHuy == true && tuNgay <= phieuThuNhaThuoc.NgayThu && phieuThuNhaThuoc.NgayThu <= denNgay 
                    && tuNgay <= phieuThuNhaThuoc.NgayHuy && phieuThuNhaThuoc.NgayHuy <= denNgay)
                {
                    continue;
                }

                if(tuNgay <= phieuThuNhaThuoc.NgayThu && phieuThuNhaThuoc.NgayThu <= denNgay)
                {
                    //var doanhThu = phieuThuNhaThuoc.TienMat.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.ChuyenKhoan.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.Pos.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.CongNo.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.BaoCaoDoanhThuNhaThuocDaTaCongNoVos.Select(o => o.SoTien).DefaultIfEmpty().Sum();                                        
                    //doanhThuNhaThuoc += doanhThu;

                    decimal giaDonThuocChuaVAT = 0;

                    foreach(var phieuChi in phieuThuNhaThuoc.PhieuChis)
                    {
                        int vat = 0;
                        if(phieuChi.DonThuocThanhToanChiTietTheoPhieuThuId != null)
                        {
                            var chiTietVATDonThuoc = chiTietVATDonThuocs.FirstOrDefault(o => o.Id == phieuChi.DonThuocThanhToanChiTietTheoPhieuThuId);
                            if(chiTietVATDonThuoc != null)
                            {
                                vat = chiTietVATDonThuoc.VAT;
                            }
                            else
                            {

                            }
                        }
                        else if(phieuChi.DonVTYTThanhToanChiTietTheoPhieuThuId != null)
                        {
                            var chiTietVATDonThuoc = chiTietVATDonVTYTs.FirstOrDefault(o => o.Id == phieuChi.DonVTYTThanhToanChiTietTheoPhieuThuId);
                            if (chiTietVATDonThuoc != null)
                            {
                                vat = chiTietVATDonThuoc.VAT;
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                        var giaChuaVat =Math.Round(phieuChi.Gia.GetValueOrDefault() / (1 + ((decimal)vat / 100)) * (decimal)phieuChi.SoLuong.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero) - phieuChi.SoTienMienGiam.GetValueOrDefault();
                        if (giaChuaVat < 0)
                        {
                            giaChuaVat = 0;
                        }
                        giaDonThuocChuaVAT += giaChuaVat;
                    }
                    doanhThuNhaThuocChuaVAT += giaDonThuocChuaVAT;

                    //if (!Math.Round(doanhThu).AlmostEqual(Math.Round(theoGia)))
                    //{

                    //}
                }
                if (phieuThuNhaThuoc.DaHuy == true && tuNgay <= phieuThuNhaThuoc.NgayHuy && phieuThuNhaThuoc.NgayHuy <= denNgay)
                {
                    //var doanhThu = phieuThuNhaThuoc.TienMat.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.ChuyenKhoan.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.Pos.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.CongNo.GetValueOrDefault()
                    //    + phieuThuNhaThuoc.BaoCaoDoanhThuNhaThuocDaTaCongNoVos.Select(o => o.SoTien).DefaultIfEmpty().Sum();                    
                    //doanhThuNhaThuoc += doanhThu * (-1);

                    decimal giaDonThuocChuaVAT = 0;

                    foreach (var phieuChi in phieuThuNhaThuoc.PhieuChis)
                    {
                        int vat = 0;
                        if (phieuChi.DonThuocThanhToanChiTietTheoPhieuThuId != null)
                        {
                            var chiTietVATDonThuoc = chiTietVATDonThuocs.FirstOrDefault(o => o.Id == phieuChi.DonThuocThanhToanChiTietTheoPhieuThuId);
                            if (chiTietVATDonThuoc != null)
                            {
                                vat = chiTietVATDonThuoc.VAT;
                            }
                            else
                            {

                            }
                        }
                        else if (phieuChi.DonVTYTThanhToanChiTietTheoPhieuThuId != null)
                        {
                            var chiTietVATDonThuoc = chiTietVATDonVTYTs.FirstOrDefault(o => o.Id == phieuChi.DonVTYTThanhToanChiTietTheoPhieuThuId);
                            if (chiTietVATDonThuoc != null)
                            {
                                vat = chiTietVATDonThuoc.VAT;
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                        var giaChuaVat = Math.Round(phieuChi.Gia.GetValueOrDefault() / (1 + ((decimal)vat / 100)) * (decimal)phieuChi.SoLuong.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero) - phieuChi.SoTienMienGiam.GetValueOrDefault();
                        if (giaChuaVat < 0)
                        {
                            giaChuaVat = 0;
                        }
                        giaDonThuocChuaVAT += giaChuaVat;
                    }
                    doanhThuNhaThuocChuaVAT += giaDonThuocChuaVAT * (-1);
                    //if (!Math.Round(doanhThu).AlmostEqual(Math.Round(theoGia)))
                    //{

                    //}
                }
            }
            if (doanhThuNhaThuocChuaVAT != 0)
            {
                returnData.Add(new BaoCaoDoanhThuChiaTheoKhoaPhongGridVo
                {
                    KhoaPhongId = -1,
                    KhoaPhong = "Nhà thuốc",
                    DoanhThuKhachLe = doanhThuNhaThuocChuaVAT
                });
            }
            
            if(queryInfo.KhoaPhongThucHienDichVuId.GetValueOrDefault() != 0)
            {
                return returnData.Where(o => o.KhoaPhongId == queryInfo.KhoaPhongThucHienDichVuId).ToList();
            }

            return returnData;
        }
        public async Task<SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo> GetTotalBaoCaoDoanhThuChiaTheoKhoaPhongForGridAsync(BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo)
        {
            var allData = await GetDataDoanhThuChiaTheoKhoaPhongForGridAsync(queryInfo);

            return new SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo
            {
                TotalDoanhThuKhachLe = allData.Select(o => o.DoanhThuKhachLe.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuDoan = allData.Select(o => o.DoanhThuDoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuBaoHiemYTe = allData.Select(o => o.DoanhThuBaoHiemYTe.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuCoDinh = allData.Select(o => o.DoanhThuCoDinh.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuThuocVaVTYT = allData.Select(o => o.DoanhThuThuocVaVTYT.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuSuDungPhongMo = allData.Select(o => o.DoanhThuSuDungPhongMo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuLuongBacSiPartime = allData.Select(o => o.DoanhThuLuongBacSiPartime.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTienDien = allData.Select(o => o.DoanhThuTienDien.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuSuatAn = allData.Select(o => o.DoanhThuSuatAn.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTongCong = allData.Select(o => o.DoanhThuTongCong.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
            };
        }
        public virtual byte[] ExportDoanhThuChiaTheoKhoaPhong(List<BaoCaoDoanhThuChiaTheoKhoaPhongGridVo> dataSource, BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo query, SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo total)
        {
            var datas = dataSource;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DOANH THU CHIA THEO KHOA PHÒNG");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                   
                    worksheet.Row(6).Height = 30;
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K" };

                    using (var range = worksheet.Cells["A1:K1"])
                    {
                        range.Worksheet.Cells["A1:K1"].Merge = true;
                        range.Worksheet.Cells["A1:K1"].Value = "DOANH THU CHIA THEO KHOA PHÒNG";
                        range.Worksheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:K1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A2:K2"])
                    {
                        range.Worksheet.Cells["A2:K2"].Merge = true;
                        range.Worksheet.Cells["A2:K2"].Value = "Khoa phòng:" + "...........................";
                        range.Worksheet.Cells["A2:K2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A2:K2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:K2"].Style.Font.Bold = true;
                    }




                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = $"Từ ngày: " + query.TuNgay.ApplyFormatDateTimeSACH()
                                                                  + " - đến ngày: " + query.DenNgay.ApplyFormatDateTimeSACH();

                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    



                    using (var range = worksheet.Cells["A5:K6"])
                    {
                        range.Worksheet.Cells["A6:K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:K6"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:K6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:K6"].Style.WrapText = true;

                        range.Worksheet.Cells["A5:K5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:K5"].Merge = true;
                        range.Worksheet.Cells["A5:K5"].Value = "DOANH THU";


                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "KHOA PHÒNG";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "KHÁCH LẺ";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "ĐOÀN";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "BẢO HIỂM Y TẾ";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "CỐ ĐỊNH";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "THUỐC + VTYT";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "SỬ DỤNG PHÒNG MỔ";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "LƯƠNG BÁC SĨ PARTIME";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "TIỀN ĐIỆN";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "SUẤT ĂN";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "TỔNG CỘNG";

                        int index = 7;

                        foreach (var item in datas.ToList())
                        {

                          


                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            }
                            


                            worksheet.Cells["A" + index].Value = item.KhoaPhong;
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                            worksheet.Cells["B" + index].Value = item.DoanhThuKhachLe;
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["C" + index].Value = item.DoanhThuDoan;

                            worksheet.Cells["D" + index].Value = item.DoanhThuBaoHiemYTe;
                            worksheet.Cells["E" + index].Value = item.DoanhThuCoDinh;

                            worksheet.Cells["F" + index].Value = item.DoanhThuThuocVaVTYT;
                            worksheet.Cells["G" + index].Value = item.DoanhThuSuDungPhongMo;
                            worksheet.Cells["H" + index].Value = item.DoanhThuLuongBacSiPartime;

                            worksheet.Cells["I" + index].Value = item.DoanhThuTienDien;
                            worksheet.Cells["J" + index].Value = item.DoanhThuSuatAn;
                            worksheet.Cells["K" + index].Value = item.DoanhThuTongCong;



                            // FOR MAT b -> k
                            worksheet.Cells["B" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                            index++;
                        }
                        if(total != null)
                        {
                        
                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }


                        worksheet.Cells["A" + index].Style.Font.Bold = true;

                        worksheet.Cells["A" + index].Value = "Tổng cộng:";

                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["B" + index].Style.Font.Bold = true;
                        worksheet.Cells["B" + index].Value = total.TotalDoanhThuKhachLe;

                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["C" + index].Style.Font.Bold = true;
                            worksheet.Cells["C" + index].Value = total.TotalDoanhThuDoan;

                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["D" + index].Style.Font.Bold = true;
                            worksheet.Cells["D" + index].Value = total.TotalDoanhThuBaoHiemYTe;

                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["E" + index].Style.Font.Bold = true;
                            worksheet.Cells["E" + index].Value = total.TotalDoanhThuCoDinh;

                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["F" + index].Style.Font.Bold = true;
                            worksheet.Cells["F" + index].Value = total.TotalDoanhThuThuocVaVTYT;

                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["G" + index].Style.Font.Bold = true;
                            worksheet.Cells["G" + index].Value = total.TotalDoanhThuSuDungPhongMo;

                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index].Value = total.TotalDoanhThuLuongBacSiPartime;

                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Style.Font.Bold = true;
                            worksheet.Cells["I" + index].Value = total.TotalDoanhThuTienDien;

                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Font.Bold = true;
                            worksheet.Cells["J" + index].Value = total.TotalDoanhThuSuatAn;

                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Font.Bold = true;
                            worksheet.Cells["K" + index].Value = total.TotalDoanhThuTongCong;



                        // FOR MAT b -> k
                        worksheet.Cells["B" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                        }
                    }





                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        #endregion

        #region  GetTotalBaoCaoDoanhThuChiaTheoKhoaPhongForGridAsync
        /// <summary>
        /// GetTotalBaoCaoDoanhThuChiaTheoKhoaPhongForGridAsync 
        /// </summary>
        /// <param name="BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo"></param>
        /// <returns></returns>
        public async Task<GridDataSource> GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync(BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo)
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
                BenhNhanId = o.YeuCauTiepNhan.BenhNhanId,
                HopDongKhamSucKhoeNhanVienId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                //HoTen = o.YeuCauTiepNhan.HoTen,
                //MaNB = o.YeuCauTiepNhan.MaBN,
                //NamSinh = o.YeuCauTiepNhan.NamSinh,
                //GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                HinhThucDenId = o.YeuCauTiepNhan.HinhThucDenId,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
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
                BenhNhanId = o.YeuCauTiepNhan.BenhNhanId,
                HopDongKhamSucKhoeNhanVienId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                //HoTen = o.YeuCauTiepNhan.HoTen,
                //MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                //NamSinh = o.YeuCauTiepNhan.NamSinh,
                //GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                HinhThucDenId = o.YeuCauTiepNhan.HinhThucDenId,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
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
                BenhNhanId = o.YeuCauTiepNhan.BenhNhanId,
                HopDongKhamSucKhoeNhanVienId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                //HoTen = o.YeuCauTiepNhan.HoTen,
                //MaNB = o.YeuCauTiepNhan.BenhNhan.MaBN,
                //NamSinh = o.YeuCauTiepNhan.NamSinh,
                //GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                HinhThucDenId = o.YeuCauTiepNhan.HinhThucDenId,
                NoiGioiThieuId = o.YeuCauTiepNhan.NoiGioiThieuId,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
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

            var hopDongKhamSucKhoeNhanVienIds = allDataThuChi.Where(o => o.HopDongKhamSucKhoeNhanVienId != null).Select(o => o.HopDongKhamSucKhoeNhanVienId.Value).ToList();
            var dichVuKhamBenhIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauKhamBenhId != null).Select(o => o.YeuCauKhamBenhId).Distinct().ToList();
            var dichVuKyThuatIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuKyThuatId != null).Select(o => o.YeuCauDichVuKyThuatId).Distinct().ToList();
            var dichVuGiuongIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null).Select(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId).Distinct().ToList();
            var truyenMauIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauTruyenMauId != null).Select(o => o.YeuCauTruyenMauId).Distinct().ToList();
            var yeuCauDuocPhamIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(o => o.YeuCauDuocPhamBenhVienId).Distinct().ToList();
            var yeuCauVatTuIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauVatTuBenhVienId != null).Select(o => o.YeuCauVatTuBenhVienId).Distinct().ToList();
            var donThuocIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTietId).Distinct().ToList();
            var thongTinQuyetToanGoiDichVuIds = allDataThuChi.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauGoiDichVuId != null).Select(o => o.Id).Distinct().ToList();

            var maxTake = 18000;

            var dataHopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(o => hopDongKhamSucKhoeNhanVienIds.Contains(o.Id))
                .Select(o=> new {o.Id, o.HopDongKhamSucKhoe.CongTyKhamSucKhoeId, TenCongTy = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten})
                .ToList();

            var dataHinhThucDen = _hinhThucDenRepository.TableNoTracking
                .Select(o => new { o.Id, o.Ten })
                .ToList();

            var dataNoiGioiThieu = _noiGioiThieuRepository.TableNoTracking
                .Select(o => new { o.Id, o.Ten })
                .ToList();

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
                    //NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
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
                    //NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    //NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
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
                    //NoiThucHienKhamBenhId = o.YeuCauKhamBenhId != null ? o.YeuCauKhamBenh.NoiThucHienId : null,
                    //NoiThucHienPTTTId = o.YeuCauDichVuKyThuat != null && (o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) ? o.YeuCauDichVuKyThuat.NoiThucHienId : null,
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
                    //NoiThucHienKhamBenhId = o.DonThuocThanhToan.YeuCauKhamBenhId != null ? o.DonThuocThanhToan.YeuCauKhamBenh.NoiThucHienId : null,
                    //NoiChiDinhId = o.DonThuocThanhToan.NoiTruDonThuocId != null ? o.DonThuocThanhToan.NoiTruDonThuoc.NoiKeDonId : (long?)null,
                }).ToList();

                dataDonThuoc.AddRange(info);
            }

            var taiKhoanBenhNhanChiThongTins = _taiKhoanBenhNhanChiThongTinRepository.TableNoTracking.Where(o => thongTinQuyetToanGoiDichVuIds.Contains(o.Id)).ToList();

            //var dsMaYCTNChuaCoSoBenhAns = allDataThuChi.Where(o => string.IsNullOrEmpty(o.SoBenhAn)).Select(o => o.MaYeuCauTiepNhan).Distinct().ToList();

            //var dsSoBenhAn = _yeuCauTiepNhanRepository.TableNoTracking
            //    .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null && dsMaYCTNChuaCoSoBenhAns.Contains(o.MaYeuCauTiepNhan))
            //    .Select(o => new { o.MaYeuCauTiepNhan, o.NoiTruBenhAn.SoBenhAn }).ToList();

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

            var baoCaoDoanhThuTheoNhomDichVuGridVos = new List<BaoCaoTongHopDoanhThuTheoNguonBenhNhanChiTietGridVo>();
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
                                    var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoTongHopDoanhThuTheoNguonBenhNhanChiTietGridVo
                                    {
                                        Id = dataPhieuChi.Id,
                                        BenhNhanId = dataPhieuThu.BenhNhanId,
                                        HinhThucDenId = dataPhieuThu.HinhThucDenId,
                                        NoiGioiThieuId = dataPhieuThu.NoiGioiThieuId,
                                        HopDongKhamSucKhoeNhanVienId = dataPhieuThu.HopDongKhamSucKhoeNhanVienId,
                                        //MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                                        //MaNB = dataPhieuThu.MaNB,
                                        //HoVaTen = dataPhieuThu.HoTen,
                                        //NamSinh = dataPhieuThu.NamSinh?.ToString(),
                                        //GioiTinh = dataPhieuThu.GioiTinh?.GetDescription(),
                                        //SoBenhAn = dataPhieuThu.SoBenhAn,
                                        NgayThu = dataPhieuThu.NgayThu
                                        //NguoiGioiThieu = dataPhieuThu.NguoiGioiThieu
                                    };
                                    //if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                                    //{
                                    //    baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                                    //}
                                    //baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                                    //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = noiDungQuyetToanGoiMarketing.NoiDung;

                                    if (dataPhieuThu.NoiGioiThieuId != null)
                                    {
                                        var hopDong = thongTinHopDongs
                                            .Where(o => o.NoiGioiThieuId == dataPhieuThu.NoiGioiThieuId
                                                        && o.NgayBatDau.Date <= dataPhieuThu.NgayThu.Date && (o.NgayKetThuc == null || dataPhieuThu.NgayThu.Date <= o.NgayKetThuc.Value.Date))
                                            .OrderBy(o => o.NgayBatDau)
                                            .LastOrDefault();
                                        if (hopDong != null)
                                        {
                                            if (noiDungQuyetToanGoiMarketing.DichVuBenhVienId != null && noiDungQuyetToanGoiMarketing.NhomGiaDichVuId != null && noiDungQuyetToanGoiMarketing.SoLuong != null && noiDungQuyetToanGoiMarketing.DonGia != null)
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

                                        baoCaoDoanhThuTheoNhomDichVuGridVo.DoanhThuThucTe = thanhTien;
                                        baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault();

                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauKhamBenh ? thanhTien : 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuGiuong ? thanhTien : 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = 0;

                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                                        //baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                                        //if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat)
                                        //{
                                        //    if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuNoiSoi.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuNoiSoiTMH.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuSieuAm.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuXQuang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuCTScan.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuMRI.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuDienTimDienNao.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuTDCNDoLoang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuPhauThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                        //    }
                                        //    else if (NhomDichVuThuThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                        //    }
                                        //    else if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                        //    {
                                        //        if (noiDungQuyetToanGoiMarketing.NoiDung != null &&
                                        //            noiDungQuyetToanGoiMarketing.NoiDung.ToLower().Contains("phẫu thuật"))
                                        //        {
                                        //            baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                                        //        }
                                        //        else
                                        //        {
                                        //            baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thanhTien;
                                        //    }
                                        //}

                                        baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < heSos.Count; i++)
                                        {
                                            var heSoTheoLan = heSos[i];
                                            var soLuong = (i == (heSos.Count - 1)) ? (noiDungQuyetToanGoiMarketing.SoLuong.GetValueOrDefault() - i) : 1;

                                            var baoCaoDoanhThuTheoNhomDichVuGridVoClone = baoCaoDoanhThuTheoNhomDichVuGridVo.CopyObject<BaoCaoTongHopDoanhThuTheoNguonBenhNhanChiTietGridVo>();

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra = 0;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet = Math.Round(noiDungQuyetToanGoiMarketing.DonGia.GetValueOrDefault() * (decimal)soLuong, 2);

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo = heSoTheoLan;

                                            var thanhTien = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet.GetValueOrDefault();

                                            thanhTien = Math.Round(thanhTien * baoCaoDoanhThuTheoNhomDichVuGridVoClone.HeSo, 2);

                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.DoanhThuThucTe = thanhTien;
                                            baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet.GetValueOrDefault();
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.KhamBenh = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauKhamBenh ? thanhTien : 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.NgayGiuong = noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuGiuong ? thanhTien : 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.Thuoc = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.VTYT = 0;

                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = 0;
                                            //baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = 0;
                                            //if (noiDungQuyetToanGoiMarketing.LoaiDichVuYeuCau == LoaiDichVuYeuCau.YeuCauDichVuKyThuat)
                                            //{
                                            //    if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuNoiSoi.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuNoiSoiTMH.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuSieuAm.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuXQuang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuCTScan.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuMRI.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuDienTimDienNao.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuTDCNDoLoang.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuPhauThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                            //    }
                                            //    else if (NhomDichVuThuThuat.Contains(noiDungQuyetToanGoiMarketing.NhomDichVuBenhVienId.GetValueOrDefault()))
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                            //    }
                                            //    else if (noiDungQuyetToanGoiMarketing.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                            //    {
                                            //        if (noiDungQuyetToanGoiMarketing.NoiDung != null &&
                                            //            noiDungQuyetToanGoiMarketing.NoiDung.ToLower().Contains("phẫu thuật"))
                                            //        {
                                            //            baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                            //        }
                                            //        else
                                            //        {
                                            //            baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                            //        }

                                            //    }
                                            //    else
                                            //    {
                                            //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = thanhTien;
                                            //    }
                                            //}

                                            baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVoClone);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var baoCaoDoanhThuTheoNhomDichVuGridVo = new BaoCaoTongHopDoanhThuTheoNguonBenhNhanChiTietGridVo
                        {
                            Id = dataPhieuChi.Id,
                            BenhNhanId = dataPhieuThu.BenhNhanId,
                            HinhThucDenId = dataPhieuThu.HinhThucDenId,
                            NoiGioiThieuId = dataPhieuThu.NoiGioiThieuId,
                            HopDongKhamSucKhoeNhanVienId = dataPhieuThu.HopDongKhamSucKhoeNhanVienId,
                            //MaNB = dataPhieuThu.MaNB,
                            //MaTN = dataPhieuThu.MaYeuCauTiepNhan,
                            //HoVaTen = dataPhieuThu.HoTen,
                            //NamSinh = dataPhieuThu.NamSinh?.ToString(),
                            //GioiTinh = dataPhieuThu.GioiTinh?.GetDescription(),
                            //SoBenhAn = dataPhieuThu.SoBenhAn,
                            NgayThu = dataPhieuThu.NgayThu
                        };
                        //if (string.IsNullOrEmpty(baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn))
                        //{
                        //    baoCaoDoanhThuTheoNhomDichVuGridVo.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == dataPhieuThu.MaYeuCauTiepNhan)?.SoBenhAn ?? string.Empty;
                        //}

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
                                        if (chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan3 == null && chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan2 == null)
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
                                            if (lanYeuCau == 1)
                                            {
                                                heSo = chiTietHeSoDichVuKhamBenh.HeSoGioiThieuTuLan1;
                                            }
                                            else if (lanYeuCau == 2)
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

                                            foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
                                            {
                                                if (yeuCauDichVuKyThuat.Id != dataPhieuChi.YeuCauDichVuKyThuatId)
                                                {
                                                    solanDaYeuCau += yeuCauDichVuKyThuat.SoLan;
                                                }
                                                else
                                                {
                                                    if (dataPhieuChi.SoLuong.GetValueOrDefault() == 1)
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
                                                        if (solanDaYeuCau > 1)
                                                        {
                                                            heSo = chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan3 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan2 ?? chiTietHeSoDichVuKyThuat.HeSoGioiThieuTuLan1;
                                                        }
                                                        else
                                                        {
                                                            for (int i = solanDaYeuCau; i < (solanDaYeuCau + dataPhieuChi.SoLuong.GetValueOrDefault()); i++)
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
                                                            if (heSoDichVuKyThuats.Distinct().Count() == 1)
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
                                                .Where(o => yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Contains(o.Id) && o.DichVuGiuongBenhVienId == thongTinDichVu.DichVuGiuongBenhVienId && o.NhomGiaDichVuGiuongBenhVienId == thongTinDichVu.NhomGiaDichVuGiuongBenhVienId)
                                                .OrderBy(o => o.ThoiDiemChiDinh)
                                                .Select(o => o.Id).ToList();

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
                                if (hopDong != null)
                                {
                                    var chiTietHeSoDuocPham = hopDong.ChiTietHeSoDuocPhams.Where(o => o.DuocPhamBenhVienId == thongTinDichVu.DuocPhamBenhVienId).FirstOrDefault();
                                    if (chiTietHeSoDuocPham != null)
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
                        //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiDung = thongTinDichVu.TenDichVu;

                        //if (dataPhieuThu.GoiDichVu)
                        //{
                        //    baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = "Gói dịch vụ";
                        //}
                        //else
                        //{
                        //    long? noiThucHienDuocTinhId = null;
                        //    if (thongTinDichVu.YeuCauKhamBenh)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId;
                        //    }
                        //    if (thongTinDichVu.YeuCauDichVuKyThuat)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienId ?? thongTinDichVu.NoiChiDinhId;
                        //    }
                        //    if (thongTinDichVu.YeuCauGiuong)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                        //    }
                        //    if (thongTinDichVu.YeuCauTruyenMau)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiChiDinhId;
                        //    }
                        //    if (thongTinDichVu.YeuCauDuocPham)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                        //    }
                        //    if (thongTinDichVu.YeuCauVatTu)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienPTTTId ?? thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                        //    }
                        //    if (thongTinDichVu.DonThuocBHYT)
                        //    {
                        //        noiThucHienDuocTinhId = thongTinDichVu.NoiThucHienKhamBenhId ?? thongTinDichVu.NoiChiDinhId;
                        //    }
                        //    var tenKhoaPhong = phongBenhViens.FirstOrDefault(o => o.Id == noiThucHienDuocTinhId)?.KhoaPhong.Ten;
                        //    baoCaoDoanhThuTheoNhomDichVuGridVo.Nhom = tenKhoaPhong ?? "Không xác định";
                        //}


                        if (thongTinDichVu.DonGiaNhapDuocPham != null || thongTinDichVu.DonGiaNhapVatTu != null)
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho = Math.Round((thongTinDichVu.DonGiaNhapDuocPham ?? thongTinDichVu.DonGiaNhapVatTu).GetValueOrDefault() * (decimal)dataPhieuChi.SoLuong.GetValueOrDefault(), 2);
                        }

                        if (!heSoDichVuKyThuats.Any())
                        {
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam = dataPhieuChi.SoTienMienGiam.GetValueOrDefault();
                            baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra = Math.Round(((decimal)dataPhieuChi.SoLuong.GetValueOrDefault() * dataPhieuChi.DonGiaBaoHiem.GetValueOrDefault() * dataPhieuChi.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * dataPhieuChi.MucHuongBaoHiem.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet = dataPhieuChi.TienChiPhi.GetValueOrDefault() + dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() + dataPhieuChi.SoTienMienGiam.GetValueOrDefault() + baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra.GetValueOrDefault();
                            baoCaoDoanhThuTheoNhomDichVuGridVo.HeSo = heSo;
                            decimal thanhTien = 0;
                            if (baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNhapKho != null && loaiGia == LoaiGiaNoiGioiThieuHopDong.GiaNhap)
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
                            baoCaoDoanhThuTheoNhomDichVuGridVo.DoanhThuThucTe = thanhTien;
                            baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVo.GiaNiemYet.GetValueOrDefault();
                            baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVo.SoTienMienGiam.GetValueOrDefault();
                            baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVo.BHYTChiTra.GetValueOrDefault();

                            //baoCaoDoanhThuTheoNhomDichVuGridVo.KhamBenh = thongTinDichVu.YeuCauKhamBenh ? thanhTien : 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.NgayGiuong = thongTinDichVu.YeuCauGiuong ? thanhTien : 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thongTinDichVu.YeuCauTruyenMau ? thanhTien : 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.Thuoc = thongTinDichVu.YeuCauDuocPham || thongTinDichVu.DonThuocBHYT ? thanhTien : 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.VTYT = thongTinDichVu.YeuCauVatTu ? thanhTien : 0;

                            //baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = 0;
                            //baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = 0;
                            //if (thongTinDichVu.YeuCauDichVuKyThuat)
                            //{
                            //    if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.XetNghiem = thanhTien;
                            //    }
                            //    else if (NhomDichVuNoiSoi.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoi = thanhTien;
                            //    }
                            //    else if (NhomDichVuNoiSoiTMH.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.NoiSoiTMH = thanhTien;
                            //    }
                            //    else if (NhomDichVuSieuAm.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.SieuAm = thanhTien;
                            //    }
                            //    else if (NhomDichVuXQuang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.XQuang = thanhTien;
                            //    }
                            //    else if (NhomDichVuCTScan.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.CTScan = thanhTien;
                            //    }
                            //    else if (NhomDichVuMRI.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.MRI = thanhTien;
                            //    }
                            //    else if (NhomDichVuDienTimDienNao.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.DienTimDienNao = thanhTien;
                            //    }
                            //    else if (NhomDichVuTDCNDoLoang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.TDCNDoLoangXuong = thanhTien;
                            //    }
                            //    else if (NhomDichVuPhauThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                            //    }
                            //    else if (NhomDichVuThuThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                            //    }
                            //    else if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                            //    {
                            //        if (thongTinDichVu.TenDichVu != null &&
                            //            thongTinDichVu.TenDichVu.ToLower().Contains("phẫu thuật"))
                            //        {
                            //            baoCaoDoanhThuTheoNhomDichVuGridVo.PhauThuat = thanhTien;
                            //        }
                            //        else
                            //        {
                            //            baoCaoDoanhThuTheoNhomDichVuGridVo.ThuThuat = thanhTien;
                            //        }

                            //    }
                            //    else
                            //    {
                            //        baoCaoDoanhThuTheoNhomDichVuGridVo.DVKhac = thanhTien;
                            //    }
                            //}

                            baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVo);
                        }
                        else
                        {
                            foreach (var heSoDichVuKyThuat in heSoDichVuKyThuats)
                            {
                                var baoCaoDoanhThuTheoNhomDichVuGridVoClone = baoCaoDoanhThuTheoNhomDichVuGridVo.CopyObject<BaoCaoTongHopDoanhThuTheoNguonBenhNhanChiTietGridVo>();

                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam = Math.Round(dataPhieuChi.SoTienMienGiam.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2);
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra = Math.Round((dataPhieuChi.DonGiaBaoHiem.GetValueOrDefault() * dataPhieuChi.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * dataPhieuChi.MucHuongBaoHiem.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet = Math.Round(dataPhieuChi.TienChiPhi.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2)
                                                                                + Math.Round(dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() / heSoDichVuKyThuats.Count, 2)
                                                                                + baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam.GetValueOrDefault()
                                                                                + baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra.GetValueOrDefault();
                                
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

                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.DoanhThuThucTe = thanhTien;
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.GiaNiemYet.GetValueOrDefault();
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.SoTienMienGiam.GetValueOrDefault();
                                baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra = ((dataPhieuThu.PhieuChiId != 0 || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * baoCaoDoanhThuTheoNhomDichVuGridVoClone.BHYTChiTra.GetValueOrDefault();

                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.KhamBenh = thongTinDichVu.YeuCauKhamBenh ? thanhTien : 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.NgayGiuong = thongTinDichVu.YeuCauGiuong ? thanhTien : 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = thongTinDichVu.YeuCauTruyenMau ? thanhTien : 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.Thuoc = thongTinDichVu.YeuCauDuocPham || thongTinDichVu.DonThuocBHYT ? thanhTien : 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.VTYT = thongTinDichVu.YeuCauVatTu ? thanhTien : 0;

                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = 0;
                                //baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = 0;
                                //if (thongTinDichVu.YeuCauDichVuKyThuat)
                                //{
                                //    if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.XetNghiem = thanhTien;
                                //    }
                                //    else if (NhomDichVuNoiSoi.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoi = thanhTien;
                                //    }
                                //    else if (NhomDichVuNoiSoiTMH.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.NoiSoiTMH = thanhTien;
                                //    }
                                //    else if (NhomDichVuSieuAm.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.SieuAm = thanhTien;
                                //    }
                                //    else if (NhomDichVuXQuang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.XQuang = thanhTien;
                                //    }
                                //    else if (NhomDichVuCTScan.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.CTScan = thanhTien;
                                //    }
                                //    else if (NhomDichVuMRI.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.MRI = thanhTien;
                                //    }
                                //    else if (NhomDichVuDienTimDienNao.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.DienTimDienNao = thanhTien;
                                //    }
                                //    else if (NhomDichVuTDCNDoLoang.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.TDCNDoLoangXuong = thanhTien;
                                //    }
                                //    else if (NhomDichVuPhauThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                //    }
                                //    else if (NhomDichVuThuThuat.Contains(thongTinDichVu.NhomDichVuBenhVienId.GetValueOrDefault()))
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                //    }
                                //    else if (thongTinDichVu.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                //    {
                                //        if (thongTinDichVu.TenDichVu != null &&
                                //            thongTinDichVu.TenDichVu.ToLower().Contains("phẫu thuật"))
                                //        {
                                //            baoCaoDoanhThuTheoNhomDichVuGridVoClone.PhauThuat = thanhTien;
                                //        }
                                //        else
                                //        {
                                //            baoCaoDoanhThuTheoNhomDichVuGridVoClone.ThuThuat = thanhTien;
                                //        }

                                //    }
                                //    else
                                //    {
                                //        baoCaoDoanhThuTheoNhomDichVuGridVoClone.DVKhac = thanhTien;
                                //    }
                                //}

                                baoCaoDoanhThuTheoNhomDichVuGridVos.Add(baoCaoDoanhThuTheoNhomDichVuGridVoClone);
                            }
                        }
                    }
                }
            }

            foreach( var item in baoCaoDoanhThuTheoNhomDichVuGridVos.Where(o=>o.HopDongKhamSucKhoeNhanVienId != null))
            {
                item.CongTyKhamSucKhoeId = dataHopDongKhamSucKhoeNhanVien.FirstOrDefault(o => o.Id == item.HopDongKhamSucKhoeNhanVienId)?.CongTyKhamSucKhoeId;
            }

            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();

            foreach (var item in baoCaoDoanhThuTheoNhomDichVuGridVos.Where(o => o.HinhThucDenId == null))
            {
                item.HinhThucDenId = cauHinhBaoCao.HinhThucDenTuDenId;
            }

            var groupDoanhThuTheoNguonBenhNhanChiTiets = baoCaoDoanhThuTheoNhomDichVuGridVos.GroupBy(o => new { o.CongTyKhamSucKhoeId, o.HinhThucDenId, o.NoiGioiThieuId })
                .OrderBy(o=>o.Key.CongTyKhamSucKhoeId.GetValueOrDefault())
                .ThenBy(o => o.Key.HinhThucDenId != cauHinhBaoCao.HinhThucDenGioiThieuId ? o.Key.HinhThucDenId : int.MaxValue)
                .ThenBy(o => o.Key.NoiGioiThieuId)
                .ToList();

            
            var dataReturn = new List<BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo>();
            foreach(var group in groupDoanhThuTheoNguonBenhNhanChiTiets)
            {
                var nguonKhachHang = "";
                if (group.Key.CongTyKhamSucKhoeId != null)
                {
                    nguonKhachHang = dataHopDongKhamSucKhoeNhanVien.FirstOrDefault(o => o.CongTyKhamSucKhoeId == group.Key.CongTyKhamSucKhoeId)?.TenCongTy;
                }
                else
                {
                    if (group.Key.HinhThucDenId != null)
                    {
                        if (group.Key.HinhThucDenId == cauHinhBaoCao.HinhThucDenGioiThieuId)
                        {
                            nguonKhachHang = dataNoiGioiThieu.FirstOrDefault(o => o.Id == group.Key.NoiGioiThieuId)?.Ten;
                        }
                        else
                        {
                            nguonKhachHang = dataHinhThucDen.FirstOrDefault(o => o.Id == group.Key.HinhThucDenId)?.Ten;
                        }
                    }
                }
                if(string.IsNullOrEmpty(queryInfo.TimKiemTheoTuKhoa) || nguonKhachHang.ToLower().Contains(queryInfo.TimKiemTheoTuKhoa.ToLower()))
                {
                    dataReturn.Add(new BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo
                    {
                        NguonKhachHang = nguonKhachHang,
                        SoLuongKhachHang = group.GroupBy(o => o.BenhNhanId).Count(),
                        DoanhThuTheoGiaNiemYet = group.Sum(o => o.GiaNiemYet.GetValueOrDefault()),
                        MienGiam = group.Sum(o => o.SoTienMienGiam.GetValueOrDefault()),
                        BaoHiemChiTra = group.Sum(o => o.BHYTChiTra.GetValueOrDefault()),
                        DoanhThuThucTeDuocThuTuKhachHang = group.Sum(o => o.DoanhThuThucTe.GetValueOrDefault()),
                    });
                }    
                
            }
            return new GridDataSource { Data = dataReturn.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count };
            //Data test
            //var arrTen = new string[]{"Nguyễn Thị Bông", "Nguyễn Thị Kiều Sơn", "Nguyễn Hoàng Thế Phong", "Văn Dương Lâm", "Võ Tùng Dương", "Lý Kiều Sương", "Ngô Gia Tư", "Nguyễn Hoàng Công Sơn", "Lý Minh Sương","Vương Quốc Anh",
            //               "Nguyễn Thị Sương", "Nguyễn Thị Hoa", "Nguyễn Lâm", "Văn Thế", "Võ Tùng Đào", "Lý Kiều Mai", "Ngô Gia Phả", "Nguyễn Hoàng Công Triết", "Lý Minh Vũ", "Thế Quốc Anh",
            //               "Nguyễn Thị Thu", "Nguyễn Thị Sang", "Nguyễn Lâm Hoàng", "Văn Thế Sơn", "Võ Bá Đào", "Lý Thị Mai", "Phá Gia Phả", "Lẩu Hoàng Công Triết", "Phước Minh Vũ", "Đào Quốc Anh",

            //         "Nguyễn Thị Bông 1", "Nguyễn Thị Kiều Sơn 1", "Nguyễn Hoàng Thế Phong 1", "Văn Dương Lâm 1", "Võ Tùng Dương 1", "Lý Kiều Sương 1", "Ngô Gia Tư 1", "Nguyễn Hoàng Công Sơn 1", "Lý Minh Sương 1","Vương Quốc Anh 1",
            //               "Nguyễn Thị Sương 1", "Nguyễn Thị Hoa 1", "Nguyễn Lâm 1", "Văn Thế 1", "Võ Tùng Đào 1", "Lý Kiều Mai 1", "Ngô Gia Phả 1", "Nguyễn Hoàng Công Triết 1", "Lý Minh Vũ 1", "Thế Quốc Anh 1",
            //               "Nguyễn Thị Thu 1", "Nguyễn Thị Sang 1", "Nguyễn Lâm Hoàng 1", "Văn Thế Sơn 1", "Võ Bá Đào 1", "Lý Thị Mai 1", "Phá Gia Phả 1", "Lẩu Hoàng Công Triết 1", "Phước Minh Vũ 1", "Đào Quốc Anh 1",

            //                "Nguyễn Thị Bông 2", "Nguyễn Thị Kiều Sơn 2", "Nguyễn Hoàng Thế Phong 2", "Văn Dương Lâm 2", "Võ Tùng Dương 2", "Lý Kiều Sương 2", "Ngô Gia Tư 2", "Nguyễn Hoàng Công Sơn 2", "Lý Minh Sương 2","Vương Quốc Anh 2",
            //               "Nguyễn Thị Sương 2", "Nguyễn Thị Hoa 2", "Nguyễn Lâm 2", "Văn Thế 2", "Võ Tùng Đào 2", "Lý Kiều Mai 2", "Ngô Gia Phả 2", "Nguyễn Hoàng Công Triết 2", "Lý Minh Vũ 2", "Thế Quốc Anh 2",
            //               "Nguyễn Thị Thu 2", "Nguyễn Thị Sang 2", "Nguyễn Lâm Hoàng 2", "Văn Thế Sơn 2", "Võ Bá Đào 2", "Lý Thị Mai 2", "Phá Gia Phả 2", "Lẩu Hoàng Công Triết 2", "Phước Minh Vũ 2", "Đào Quốc Anh 2",
                    
            //    "Nguyễn Thị Bông 3", "Nguyễn Thị Kiều Sơn 3", "Nguyễn Hoàng Thế Phong 3", "Văn Dương Lâm 3", "Võ Tùng Dương 3", "Lý Kiều Sương 3", "Ngô Gia Tư 3", "Nguyễn Hoàng Công Sơn 3", "Lý Minh Sương 3","Vương Quốc Anh 3",
            //               "Nguyễn Thị Sương 3", "Nguyễn Thị Hoa 3", "Nguyễn Lâm 3", "Văn Thế 3", "Võ Tùng Đào 3", "Lý Kiều Mai 3", "Ngô Gia Phả 3", "Nguyễn Hoàng Công Triết 3", "Lý Minh Vũ 3", "Thế Quốc Anh 3",
            //               "Nguyễn Thị Thu 3", "Nguyễn Thị Sang 3", "Nguyễn Lâm Hoàng 3", "Văn Thế Sơn 3", "Võ Bá Đào 3", "Lý Thị Mai 3", "Phá Gia Phả 3", "Lẩu Hoàng Công Triết 3", "Phước Minh Vũ 3", "Đào Quốc Anh 3",
                     
            //    "Nguyễn Thị Bông 4", "Nguyễn Thị Kiều Sơn 4", "Nguyễn Hoàng Thế Phong 4", "Văn Dương Lâm 4", "Võ Tùng Dương 4", "Lý Kiều Sương 4", "Ngô Gia Tư 4", "Nguyễn Hoàng Công Sơn 4", "Lý Minh Sương 4","Vương Quốc Anh 4",
            //               "Nguyễn Thị Sương 4", "Nguyễn Thị Hoa 4", "Nguyễn Lâm 4", "Văn Thế 4", "Võ Tùng Đào 4", "Lý Kiều Mai 4", "Ngô Gia Phả 4", "Nguyễn Hoàng Công Triết 4", "Lý Minh Vũ 4", "Thế Quốc Anh 4",
            //               "Nguyễn Thị Thu 4", "Nguyễn Thị Sang 4", "Nguyễn Lâm Hoàng 4", "Văn Thế Sơn 4", "Võ Bá Đào 4", "Lý Thị Mai 4", "Phá Gia Phả 4", "Lẩu Hoàng Công Triết 4", "Phước Minh Vũ 4", "Đào Quốc Anh 4",
            //};

            
            //for(int i = 0; i < 150; i++)
            //{
            //    var vo = new BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo();
            //    vo.NguonKhachHang = arrTen[i];
            //    vo.SoLuongKhachHang = i == 0 ?  10 * 1 : 10*i;
            //    vo.DoanhThuTheoGiaNiemYet = i == 0 ? 10 * 1 * 2 : 10 * i * 2;
            //    vo.MienGiam = i == 0 ? 10 * 1 * 3  : 10 * i * 3;
            //    vo.BaoHiemChiTra = i == 0 ? 10 * 1 * 4 : 10 * i * 4;
            //    vo.DoanhThuThucTeDuocThuTuKhachHang = i == 0 ? 10 * 1 * 5 : 10 * i * 5;
            //    query.Add(vo);
            //}



            //return new GridDataSource { Data = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = query.Count };
           
        }
        public async Task<SumTongHopDoanhThuTheoNguonBenhNhanGridVo> GetTotalBaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhanForGridAsync(BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo)
        {
            var allData = await GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync(queryInfo);
            var allDatas = allData.Data.Cast<BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo>().ToList();

            return new SumTongHopDoanhThuTheoNguonBenhNhanGridVo
            {
                TotalSoLuongKhachHang = allDatas.Select(o => o.SoLuongKhachHang).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTheoGiaNiemYet = allDatas.Select(o => o.DoanhThuTheoGiaNiemYet.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalMienGiam = allDatas.Select(o => o.MienGiam.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalBaoHiemChiTra = allDatas.Select(o => o.BaoHiemChiTra.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuThucTeDuocThuTuKhachHang = allDatas.Select(o => o.DoanhThuThucTeDuocThuTuKhachHang.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
            };
        }
        public virtual byte[] ExportDoanhThuongHopDoanhThuTheoNguonBenhNhan(List<BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo> dataSource, BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo query)
        {
            var datas = dataSource;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỔNG HỢP DOANH THU THEO NGUỒN BỆNH NHÂN");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 50;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;

                    //worksheet.Row(6).Height = 30;
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G"};

                    using (var range = worksheet.Cells["A1:G1"])
                    {
                        range.Worksheet.Cells["A1:G1"].Merge = true;
                        range.Worksheet.Cells["A1:G1"].Value = "BÁO CÁO TỔNG HỢP DOANH THU THEO NGUỒN BỆNH NHÂN";
                        range.Worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                    }



                    using (var range = worksheet.Cells["A2:G2"])
                    {
                        range.Worksheet.Cells["A2:G2"].Merge = true;
                        range.Worksheet.Cells["A2:G2"].Value = $"Từ ngày: " + query.TuNgay.ApplyFormatDateTimeSACH()
                                                                  + " - đến ngày: " + query.DenNgay.ApplyFormatDateTimeSACH();

                        range.Worksheet.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:G2"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A2:G2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:G2"].Style.Font.Bold = true;
                    }





                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:G4"].Style.WrapText = true;

                        range.Worksheet.Cells["A4:G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.Font.Bold = true;


                        range.Worksheet.Cells["A4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A4"].Value = "STT";

                        range.Worksheet.Cells["B4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B4"].Value = $"Nguồn khách hàng{Environment.NewLine}(1)";// "Nguồn khách hàng";

                        range.Worksheet.Cells["C4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C4"].Value = $"Số lượng khách hàng{Environment.NewLine}(2)";// "Số lượng khách hàng";

                        range.Worksheet.Cells["D4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D4"].Value = $"Doanh Thu theo giá niêm yết{Environment.NewLine}(3)";// "Doanh Thu theo giá niêm yết";

                        range.Worksheet.Cells["E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E4"].Value = $"Miễn giảm{Environment.NewLine}(4)";// "Miễn giảm ";

                        range.Worksheet.Cells["F4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F4"].Value = $"BHYT chi trả{Environment.NewLine}(5)";// "BHYT chi trả";

                        range.Worksheet.Cells["G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G4"].Value = $"Doanh thu thực tế thu được từ khách hàng{Environment.NewLine}(6)";// "Doanh thu thực tế thu được từ khách hàng";


                        int index = 5;
                        int stt = 1;
                        foreach (var item in datas.ToList())
                        {
                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            }



                            worksheet.Cells["A" + index].Value = stt++;
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            worksheet.Cells["B" + index].Value = item.NguonKhachHang;
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["C" + index].Value = item.SoLuongKhachHang;

                            worksheet.Cells["D" + index].Value = item.DoanhThuTheoGiaNiemYet;
                            worksheet.Cells["E" + index].Value = item.MienGiam;

                            worksheet.Cells["F" + index].Value = item.BaoHiemChiTra;
                            worksheet.Cells["G" + index].Value = item.DoanhThuThucTeDuocThuTuKhachHang;
                            worksheet.Cells["G" + index].Style.Font.Bold = true;




                            // FOR MAT D -> k
                            //worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                            

                            index++;
                        }
                        if (datas.Any())
                        {
                            var total = new SumTongHopDoanhThuTheoNguonBenhNhanGridVo
                            {
                                TotalSoLuongKhachHang = datas.Sum(o => o.SoLuongKhachHang),
                                TotalDoanhThuTheoGiaNiemYet = datas.Sum(o => o.DoanhThuTheoGiaNiemYet),
                                TotalMienGiam = datas.Sum(o => o.MienGiam),
                                TotalBaoHiemChiTra = datas.Sum(o => o.BaoHiemChiTra),
                                TotalDoanhThuThucTeDuocThuTuKhachHang = datas.Sum(o => o.DoanhThuThucTeDuocThuTuKhachHang)
                            };
                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            }

                            worksheet.Cells["A" + index].Style.Font.Bold = true;

                            worksheet.Cells["A" + index].Value = "";

                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["B" + index].Style.Font.Bold = true;
                            worksheet.Cells["B" + index].Value = "Cộng";

                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["C" + index].Style.Font.Bold = true;
                            worksheet.Cells["C" + index].Value = total.TotalSoLuongKhachHang;

                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["D" + index].Style.Font.Bold = true;
                            worksheet.Cells["D" + index].Value = total.TotalDoanhThuTheoGiaNiemYet;

                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["E" + index].Style.Font.Bold = true;
                            worksheet.Cells["E" + index].Value = total.TotalMienGiam;

                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["F" + index].Style.Font.Bold = true;
                            worksheet.Cells["F" + index].Value = total.TotalBaoHiemChiTra;

                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["G" + index].Style.Font.Bold = true;
                            worksheet.Cells["G" + index].Value = total.TotalDoanhThuThucTeDuocThuTuKhachHang;


                            // FOR MAT D -> k
                            //worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                        }
                    }





                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        #endregion
    }
}
