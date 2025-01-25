using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoBangKeXuatThuocTheoBenhNhanForGridAsync(BaoCaoBangKeXuatThuocTheoBenhNhanQueryInfo queryInfo, bool exportExcel = false)
        {
            var allDataXuatChiTietViTri = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat >= queryInfo.FromDate && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat < queryInfo.ToDate)
                    .Select(o => new
                    {
                        Id = o.Id,
                        PhieuXuatId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.Id,
                        NgayXuat = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        SoChungTu = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        SoLuong = o.SoLuongXuat,
                        VAT = o.NhapKhoDuocPhamChiTiet.VAT,
                        TiLeTheoThapGia = o.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                        DonGiaNhap = o.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                        DonGiaBan = o.NhapKhoDuocPhamChiTiet.DonGiaBan,
                        YeucauDuocPhamBenhViens = o.XuatKhoDuocPhamChiTiet.YeuCauDuocPhamBenhViens.Select(z => new { z.Id, z.TrangThai }).ToList(),
                        DonThuocThanhToanChiTietIds = o.DonThuocThanhToanChiTiets.Select(z => z.Id).ToList()
                    })
                    .ToList();
            var allGroupDataXuats = allDataXuatChiTietViTri
                .GroupBy(o => new { o.PhieuXuatId, o.SoChungTu, o.NgayXuat }).OrderBy(o => o.Key.NgayXuat)
                .ToList();
            var groupDataXuats = allGroupDataXuats
                .Where(g=>g.SelectMany(o=>o.YeucauDuocPhamBenhViens).Any(yc => yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy) || g.SelectMany(o => o.DonThuocThanhToanChiTietIds).Any())
                .ToList();
            if (!exportExcel)
                groupDataXuats = groupDataXuats.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var yeuCauDuocPhamBenhVienIds = new List<long>();
            var donThuocThanhToanChiTietIds = new List<long>();
            List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien> chiTietThuTienYeuCauDuocPhamBenhViens = new List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien>();
            List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien> chiTietThuTienDonThuocThanhToanChiTiets = new List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien>();
            foreach(var groupDataXuat in groupDataXuats)
            {
                yeuCauDuocPhamBenhVienIds.AddRange(groupDataXuat.SelectMany(o => o.YeucauDuocPhamBenhViens).Where(yc => yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Select(yc => yc.Id));
                donThuocThanhToanChiTietIds.AddRange(groupDataXuat.SelectMany(o => o.DonThuocThanhToanChiTietIds));
            }

            if (yeuCauDuocPhamBenhVienIds.Any())
            {
                chiTietThuTienYeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && yeuCauDuocPhamBenhVienIds.Contains(o.Id))
                    .Select(o => new BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien
                    {
                        Id = o.Id,
                        HoTen = o.YeuCauTiepNhan.HoTen,
                        CongNos = o.CongTyBaoHiemTuNhanCongNos.Where(x => x.TaiKhoanBenhNhanThuId != null && x.DaHuy != true).Select(x => x.CongTyBaoHiemTuNhan.Ten).ToList()
                    }).ToList();
            }
            if (donThuocThanhToanChiTietIds.Any())
            {
                chiTietThuTienDonThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                    .Where(o => donThuocThanhToanChiTietIds.Contains(o.Id))
                    .Select(o => new BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien
                    {
                        Id = o.Id,
                        HoTen = o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                        CongNos = o.CongTyBaoHiemTuNhanCongNos.Where(x => x.TaiKhoanBenhNhanThuId != null && x.DaHuy != true).Select(x => x.CongTyBaoHiemTuNhan.Ten).ToList()
                    }).ToList();
            }
            var thongTinDuocPham = _duocPhamBenhVienRepository.TableNoTracking.Select(o =>
                new
                {
                    o.Id,
                    DVT = o.DuocPham.DonViTinh.Ten,
                    o.DuocPham.Ten,
                    o.Ma,
                    o.DuocPham.HamLuong
                }).ToList();

            var totalRowCount = allGroupDataXuats.Count();
            var data = new List<BaoCaoBangKeXuatThuocTheoBenhNhanGridVo>();

            foreach (var groupDataXuat in groupDataXuats)
            {
                var chiTietDuocPhams = new List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham>();
                foreach (var baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham in groupDataXuat)
                {
                    BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien chiTietThuTien = null;
                    if (baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.YeucauDuocPhamBenhViens.Any())
                    {
                        var yeucauDuocPhamBenhVienIds = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.YeucauDuocPhamBenhViens.Select(o => o.Id).ToList();
                        chiTietThuTien = chiTietThuTienYeuCauDuocPhamBenhViens.FirstOrDefault(o => yeucauDuocPhamBenhVienIds.Contains(o.Id));
                    }
                    if (baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DonThuocThanhToanChiTietIds.Any())
                    {
                        chiTietThuTien = chiTietThuTienDonThuocThanhToanChiTiets.FirstOrDefault(o => baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DonThuocThanhToanChiTietIds.Contains(o.Id));
                    }

                    //baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.ThongTinKhachHang =
                    //    $"{xuatKhoDuocPham.NgayXuat.ToString("HH:mm:ss")}: {chiTietThuTien?.HoTen}-{xuatKhoDuocPham.SoChungTu}";
                    //baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.CongNos = chiTietThuTien?.CongNos ?? new List<string>();
                    var dp = thongTinDuocPham.FirstOrDefault(o => o.Id == baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DuocPhamBenhVienId);
                    var chiTietDuocPham = new BaoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham
                    {
                        Id = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.Id,
                        DuocPhamBenhVienId = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DuocPhamBenhVienId,
                        MaDuoc = dp?.Ma,
                        Ten = dp?.Ten,
                        HamLuong = dp?.HamLuong,
                        DVT = dp?.DVT,
                        SoLuong = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.SoLuong,
                        VAT = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.VAT,
                        TiLeTheoThapGia = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.TiLeTheoThapGia,
                        DonGiaNhap = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DonGiaNhap,
                        DonGiaBan = baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DonGiaBan,
                        ThongTinKhachHang = $"{baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.NgayXuat.ToString("HH:mm:ss")}: {chiTietThuTien?.HoTen}-{baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.SoChungTu}",
                        CongNos = chiTietThuTien?.CongNos ?? new List<string>()
                    };
                    chiTietDuocPhams.Add(chiTietDuocPham);
                }
                data.AddRange(chiTietDuocPhams.GroupBy(o =>
                        new
                        {
                            o.ThongTinKhachHang,
                            o.DuocPhamBenhVienId,
                            o.DonGiaNhap,
                            o.DonGiaBan,
                            o.VAT,
                            o.HamLuong,
                            o.Ten,
                            o.MaDuoc,
                            o.DVT
                        }, o => o,
                    (k, v) => new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
                    {
                        Id = v.First().Id,
                        MaDuoc = k.MaDuoc,
                        Ten = k.Ten,
                        HamLuong = k.HamLuong,
                        DVT = k.DVT,
                        SoLuong = v.Sum(o => o.SoLuong),
                        DonGiaDaCoVat = Math.Round(k.DonGiaNhap + k.DonGiaNhap * k.VAT / 100, 2),
                        DonGiaBan = k.DonGiaBan,
                        ChiTietCongNo = string.Join(", ", v.SelectMany(o => o.CongNos).Distinct()),
                        ThueSuat = k.VAT,
                        ThongTinKhachHang = k.ThongTinKhachHang,
                        NgayXuat = groupDataXuat.Key.NgayXuat
                    }).Where(o => !o.SoLuong.GetValueOrDefault().AlmostEqual(0)));
            }

            return new GridDataSource { Data = data.ToArray(), TotalRowCount = totalRowCount };
        }
        public async Task<GridDataSource> GetDataBaoCaoBangKeXuatThuocTheoBenhNhanForGridAsyncOld(BaoCaoBangKeXuatThuocTheoBenhNhanQueryInfo queryInfo, bool exportExcel = false)
        {
            var xuatKhoDuocPhamQuery = _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.KhoXuatId == queryInfo.KhoId && o.NgayXuat >= queryInfo.FromDate && o.NgayXuat < queryInfo.ToDate
                && (o.XuatKhoDuocPhamChiTiets.SelectMany(x=>x.XuatKhoDuocPhamChiTietViTris).SelectMany(y=>y.DonThuocThanhToanChiTiets).Any() || o.XuatKhoDuocPhamChiTiets.Any(x=>x.YeuCauDuocPhamBenhViens.Any(y=>y.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))));
            
            IQueryable<XuatKhoDuocPham> xuatKhoDuocPhamOrderQuery;
            if (!exportExcel)
            {
                xuatKhoDuocPhamOrderQuery = xuatKhoDuocPhamQuery.OrderBy(o => o.NgayXuat).Skip(queryInfo.Skip).Take(queryInfo.Take);
            }
            else
            {
                xuatKhoDuocPhamOrderQuery = xuatKhoDuocPhamQuery.OrderBy(o => o.NgayXuat);
            }

            var xuatKhoDuocPhamData = xuatKhoDuocPhamOrderQuery.Select(o => new BaoCaoBangKeXuatThuocTheoBenhNhanQueryData
            {
                NgayXuat = o.NgayXuat,
                SoChungTu = o.SoPhieu,
                ChiTietDuocPhams = o.XuatKhoDuocPhamChiTiets.SelectMany(x => x.XuatKhoDuocPhamChiTietViTris)
                    .Select(y => new BaoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham
                    {
                        Id = y.Id,
                        DuocPhamBenhVienId = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        MaDuoc = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                        Ten = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                        DVT = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLuong = y.SoLuongXuat,
                        VAT = y.NhapKhoDuocPhamChiTiet.VAT,
                        TiLeTheoThapGia = y.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                        DonGiaNhap = y.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                        DonGiaBan = y.NhapKhoDuocPhamChiTiet.DonGiaBan,
                        YeucauDuocPhamBenhViens = y.XuatKhoDuocPhamChiTiet.YeuCauDuocPhamBenhViens.Where(z=>z.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Select(z=>z.Id).ToList(),
                        DonThuocThanhToanChiTiets = y.DonThuocThanhToanChiTiets.Select(z => z.Id).ToList()
                    }).ToList()
            }).ToList();

            var yeuCauDuocPhamBenhVienIds = xuatKhoDuocPhamData.SelectMany(o => o.ChiTietDuocPhams).SelectMany(x => x.YeucauDuocPhamBenhViens).Distinct().ToList();
            var donThuocThanhToanChiTietIds = xuatKhoDuocPhamData.SelectMany(o => o.ChiTietDuocPhams).SelectMany(x => x.DonThuocThanhToanChiTiets).Distinct().ToList();
            List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien> chiTietThuTienYeuCauDuocPhamBenhViens = new List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien>();
            List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien> chiTietThuTienDonThuocThanhToanChiTiets = new List<BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien>();
            if (yeuCauDuocPhamBenhVienIds.Any())
            {
                chiTietThuTienYeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && yeuCauDuocPhamBenhVienIds.Contains(o.Id))
                    .Select(o => new BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien
                    {
                        Id = o.Id,
                        HoTen = o.YeuCauTiepNhan.HoTen,
                        CongNos = o.CongTyBaoHiemTuNhanCongNos.Where(x=>x.TaiKhoanBenhNhanThuId != null && x.DaHuy != true).Select(x=>x.CongTyBaoHiemTuNhan.Ten).ToList()
                    }).ToList();
            }
            if (donThuocThanhToanChiTietIds.Any())
            {
                chiTietThuTienDonThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                    .Where(o => donThuocThanhToanChiTietIds.Contains(o.Id))
                    .Select(o => new BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien
                    {
                        Id = o.Id,
                        HoTen = o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                        CongNos = o.CongTyBaoHiemTuNhanCongNos.Where(x => x.TaiKhoanBenhNhanThuId != null && x.DaHuy != true).Select(x => x.CongTyBaoHiemTuNhan.Ten).ToList()
                    }).ToList();
            }

            foreach (var xuatKhoDuocPham in xuatKhoDuocPhamData)
            {
                foreach (var baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham in xuatKhoDuocPham.ChiTietDuocPhams)
                {
                    BaoCaoBangKeXuatThuocTheoBenhNhanChiTietThuTien chiTietThuTien = null;
                    if (baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.YeucauDuocPhamBenhViens.Any())
                    {
                        chiTietThuTien = chiTietThuTienYeuCauDuocPhamBenhViens.FirstOrDefault(o => baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.YeucauDuocPhamBenhViens.Contains(o.Id));
                    }
                    if (baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DonThuocThanhToanChiTiets.Any())
                    {
                        chiTietThuTien = chiTietThuTienDonThuocThanhToanChiTiets.FirstOrDefault(o => baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.DonThuocThanhToanChiTiets.Contains(o.Id));
                    }

                    baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.ThongTinKhachHang =
                        $"{xuatKhoDuocPham.NgayXuat.ToString("HH:mm:ss")}: {chiTietThuTien?.HoTen}-{xuatKhoDuocPham.SoChungTu}";
                    baoCaoBangKeXuatThuocTheoBenhNhanChiTietDuocPham.CongNos = chiTietThuTien?.CongNos ?? new List<string>();
                }
            }

            var totalRowCount = exportExcel ? xuatKhoDuocPhamData.Count : xuatKhoDuocPhamQuery.Count();
            var data = new List<BaoCaoBangKeXuatThuocTheoBenhNhanGridVo>();
            foreach (var xuatKhoDuocPham in xuatKhoDuocPhamData)
            {
                data.AddRange(xuatKhoDuocPham.ChiTietDuocPhams.GroupBy(o =>
                        new
                        {
                            o.ThongTinKhachHang,
                            o.DuocPhamBenhVienId,
                            o.DonGiaNhap,
                            o.DonGiaBan,
                            o.VAT,
                            o.HamLuong,
                            o.Ten,
                            o.MaDuoc,
                            o.DVT
                        }, o => o,
                    (k, v) => new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
                    {
                        Id = v.First().Id,
                        MaDuoc = k.MaDuoc,
                        Ten = k.Ten,
                        HamLuong = k.HamLuong,
                        DVT = k.DVT,
                        SoLuong = v.Sum(o=>o.SoLuong),
                        DonGiaDaCoVat = Math.Round(k.DonGiaNhap + k.DonGiaNhap*k.VAT/100, 2),
                        DonGiaBan = k.DonGiaBan,
                        ChiTietCongNo = string.Join(", ",v.SelectMany(o=>o.CongNos).Distinct()),
                        ThueSuat = k.VAT,
                        ThongTinKhachHang = k.ThongTinKhachHang,
                        NgayXuat = xuatKhoDuocPham.NgayXuat
                    }).Where(o=>!o.SoLuong.GetValueOrDefault().AlmostEqual(0)));
            }
            //var data = new List<BaoCaoBangKeXuatThuocTheoBenhNhanGridVo>()
            //{
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 1,
            //        MaDuoc = "MEDT203",
            //        Ten = "Medrol",
            //        HamLuong = "4mg",
            //        DVT = "Viên",
            //        SoLuong = 4,
            //        DonGiaDaCoVat = 983,
            //        DonGiaBan = 1130,
            //        ChiTietCongNo = "Kế toán chị Hòa",
            //        ThueSuat = 5,
            //        ThongTinKhachHang = "07:57:38: Khách Lẻ-KD-NTBVX08210300001",
            //        NgayXuat = new DateTime(2021,03,01,07,57,38),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 2,
            //        MaDuoc = "ALAT404",
            //        Ten = "Alaxan",
            //        DVT = "Viên",
            //        SoLuong = 4,
            //        DonGiaDaCoVat = 1190,
            //        DonGiaBan = 1309,
            //        ThueSuat = 5,
            //        ThongTinKhachHang = "08:11:50: Khách Lẻ-KD-NTBVX08210300002",
            //        NgayXuat = new DateTime(2021,03,01,07,57,38),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 3,
            //        MaDuoc = "SILT204",
            //        Ten = "Silygamma",
            //        HamLuong = "150mg",
            //        DVT = "Viên",
            //        SoLuong = 60,
            //        DonGiaDaCoVat = 4935,
            //        DonGiaBan = 5429,
            //        ChiTietCongNo = "Công ty hapulico",
            //        ThueSuat = 5,
            //        ThongTinKhachHang = "08:29:13: Lê Quang Hồng-KD-NTBVX08210300003",
            //        NgayXuat = new DateTime(2021,03,01,08,29,13),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 4,
            //        MaDuoc = "LIVT200",
            //        Ten = "Livetin-Ep",
            //        DVT = "Viên",
            //        SoLuong = 60,
            //        DonGiaDaCoVat = 4400,
            //        DonGiaBan = 4840,
            //        ChiTietCongNo = "Công ty hapulico",
            //        ThueSuat = 10,
            //        ThongTinKhachHang = "08:29:13: Lê Quang Hồng-KD-NTBVX08210300003",
            //        NgayXuat = new DateTime(2021,03,01,08,29,13),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 5,
            //        MaDuoc = "AMMT432",
            //        Ten = "Amoxicillin",
            //        HamLuong = "500mg",
            //        DVT = "Viên",
            //        SoLuong = 60,
            //        DonGiaDaCoVat = (decimal)595.45,
            //        DonGiaBan = 685,
            //        ChiTietCongNo = "BIC",
            //        ThueSuat = 5,
            //        ThongTinKhachHang = "09:02:42: Nguyễn Gia Khánh-KD-NTBVX08210300004 - Số 70 Tổ 21, Phường Ngọc Thụy, Quận Long Biên, Thành phố Hà Nội",
            //        NgayXuat = new DateTime(2021,03,01,09,02,42),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 6,
            //        MaDuoc = "MEMT424",
            //        Ten = "Mepoly",
            //        HamLuong = "10ml",
            //        DVT = "Lọ",
            //        SoLuong = 1,
            //        DonGiaDaCoVat = 37000,
            //        DonGiaBan = 39590,
            //        ChiTietCongNo = "BIC",
            //        ThueSuat = 5,
            //        ThongTinKhachHang = "09:02:42: Nguyễn Gia Khánh-KD-NTBVX08210300004 - Số 70 Tổ 21, Phường Ngọc Thụy, Quận Long Biên, Thành phố Hà Nội",
            //        NgayXuat = new DateTime(2021,03,01,09,02,42),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 7,
            //        MaDuoc = "MOMT209",
            //        Ten = "Motilium M",
            //        HamLuong = "10mg",
            //        DVT = "Viên",
            //        SoLuong = 10,
            //        DonGiaDaCoVat = (decimal)2145.03,
            //        DonGiaBan = 2360,
            //        ChiTietCongNo = "BIC",
            //        ThongTinKhachHang = "09:02:42: Nguyễn Gia Khánh-KD-NTBVX08210300004 - Số 70 Tổ 21, Phường Ngọc Thụy, Quận Long Biên, Thành phố Hà Nội",
            //        ThueSuat = 5,
            //        NgayXuat = new DateTime(2021,03,01,09,02,42),

            //    },
            //    new BaoCaoBangKeXuatThuocTheoBenhNhanGridVo
            //    {
            //        Id = 8,
            //        MaDuoc = "CLMT203",
            //        Ten = "Clarithromycin 250mg Stada",
            //        DVT = "Viên",
            //        SoLuong = 30,
            //        DonGiaDaCoVat = 3300,
            //        DonGiaBan = 3630,
            //        ChiTietCongNo = "BIC",
            //        ThongTinKhachHang = "09:02:42: Nguyễn Gia Khánh-KD-NTBVX08210300004 - Số 70 Tổ 21, Phường Ngọc Thụy, Quận Long Biên, Thành phố Hà Nội",
            //        ThueSuat = 5,
            //        NgayXuat = new DateTime(2021,03,01,09,02,42),
            //    },

            //};
            return new GridDataSource { Data = data.ToArray(), TotalRowCount = totalRowCount };
        }
        public virtual byte[] ExportBaoCaoBangKeXuatThuocTheoBenhNhan(GridDataSource gridDataSource, BaoCaoBangKeXuatThuocTheoBenhNhanQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBangKeXuatThuocTheoBenhNhanGridVo>)gridDataSource.Data;
            var listNgay = datas.GroupBy(s => new { s.NgayXuatStr }).Select(s => new NgayGroupVo
            {
                NgayXuatStr = s.First().NgayXuatStr
            }).OrderBy(s=>s.NgayXuatStr).ToList();
            var listKhachHang = datas.GroupBy(s => new { s.ThongTinKhachHang }).Select(s => new ThongTinKhachHangGroupVo
            {
                NgayXuatStr = s.First().NgayXuatStr,
                ThongTinKhachHang = s.First().ThongTinKhachHang,
                ChiTietCongNo = s.First().ChiTietCongNo,
                NgayXuat = s.First().NgayXuat
            }).OrderBy(s => s.NgayXuat).ToList();

            using(var stream =  new MemoryStream())
            {
                using(var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ XUẤT THUỐC THEO BỆNH NHÂN");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(7).Height = 21;
                    using(var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:K7"])
                    {
                        range.Worksheet.Cells["A7:K7"].Merge = true;
                        range.Worksheet.Cells["A7:K7"].Value = "BẢNG KÊ XUẤT THUỐC THEO BỆNH NHÂN";
                        range.Worksheet.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:K7"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A7:K7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:K7"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A8:K8"])
                    {
                        range.Worksheet.Cells["A8:K8"].Merge = true;
                        range.Worksheet.Cells["A8:K8"].Value = "Từ ngày: " + query.FromDate.ApplyFormatDate()
                                                          + " - đến ngày: " + query.ToDate.ApplyFormatDate();
                        range.Worksheet.Cells["A8:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A8:K8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:K8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:K8"].Style.Font.Bold = true;
                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();

                    using (var range = worksheet.Cells["A9:K9"])
                    {
                        range.Worksheet.Cells["A9:K9"].Merge = true;
                        range.Worksheet.Cells["A9:K9"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A9:K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A9:K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:K9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A9:K9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A9:K9"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A11:N11"])
                    {
                        range.Worksheet.Cells["A11:N11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A11:N11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A11:N11"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A11:N11"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A11:N11"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A11:N11"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A11:N11"].Style.WrapText = true;

                        range.Worksheet.Cells["A11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A11"].Value = "STT";

                        range.Worksheet.Cells["B11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B11"].Value = "Mã dược";

                        range.Worksheet.Cells["C11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C11"].Value = "Tên dược, hàm lượng";

                        range.Worksheet.Cells["D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D11"].Value = "ĐVT";

                        range.Worksheet.Cells["E11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E11"].Value = "SL";

                        range.Worksheet.Cells["F11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F11"].Value = "Đơn giá (VAT)";

                        range.Worksheet.Cells["G11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G11"].Value = "Thành tiền (VAT)";

                        range.Worksheet.Cells["H11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H11"].Value = "Đơn giá bán";

                        range.Worksheet.Cells["I11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I11"].Value = "Thành tiền bán";

                        range.Worksheet.Cells["J11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J11"].Value = "Đơn giá hoàn trả";

                        range.Worksheet.Cells["K11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K11"].Value = "Thành tiền hoàn trả";

                        range.Worksheet.Cells["L11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L11"].Value = "Chi tiết Công nợ";

                        range.Worksheet.Cells["M11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M11"].Value = "Số Hóa đơn";

                        range.Worksheet.Cells["N11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N11"].Value = "Thuế suất";

                    }

                    var stt = 1;
                    var index = 12;

                    if (listNgay.Any())
                    {
                        foreach (var ngay in listNgay)
                        {
                            var listKhachHangTheoNgay = listKhachHang.Where(s => s.NgayXuatStr == ngay.NgayXuatStr).ToList();
                            if (listKhachHangTheoNgay.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":N" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":K" + index].Value = ngay.NgayXuatStr;
                                    range.Worksheet.Cells["A" + index + ":K" + index].Merge = true;

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    index++;
                                }

                                foreach(var khachHang in listKhachHangTheoNgay)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":N" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index + ":E" + index].Value = khachHang.ThongTinKhachHang;
                                        range.Worksheet.Cells["A" + index + ":E" + index].Merge = true;
                                        range.Worksheet.Cells["A" + index + ":E" + index].Style.WrapText = true;
                                        range.Worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;

                                        range.Worksheet.Cells["F" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["F" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["F" + index + ":G" + index].Value = datas.Where(s=>s.ThongTinKhachHang == khachHang.ThongTinKhachHang && s.NgayXuat == khachHang.NgayXuat).Sum(s=>s.ThanhTien);
                                        range.Worksheet.Cells["F" + index + ":G" + index].Merge = true;
                                        range.Worksheet.Cells["F" + index + ":G" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["F" + index + ":G" + index].Style.Font.Bold = true;

                                        range.Worksheet.Cells["H" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["H" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["H" + index + ":I" + index].Value = datas.Where(s => s.ThongTinKhachHang == khachHang.ThongTinKhachHang && s.NgayXuat == khachHang.NgayXuat).Sum(s => s.ThanhTienBan);
                                        range.Worksheet.Cells["H" + index + ":I" + index].Merge = true;
                                        range.Worksheet.Cells["H" + index + ":I" + index].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells["H" + index + ":I" + index].Style.Font.Bold = true;

                                        var thanhTienHoanTra = datas.Where(s => s.ThongTinKhachHang == khachHang.ThongTinKhachHang && s.NgayXuat == khachHang.NgayXuat).Sum(s => s.ThanhTienHoanTra);
                                        range.Worksheet.Cells["J" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["J" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["J" + index + ":K" + index].Value = thanhTienHoanTra;
                                        range.Worksheet.Cells["J" + index + ":K" + index].Style.Font.Bold = true;
                                        range.Worksheet.Cells["J" + index + ":K" + index].Merge = true;
                                        if (thanhTienHoanTra != 0)
                                        {
                                            range.Worksheet.Cells["J" + index + ":K" + index].Style.Numberformat.Format = "#,##0.00";
                                        }

                                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["L" + index].Value = khachHang.ChiTietCongNo;
                                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        index++;
                                    }

                                    var listTheoKhachHang = datas.Where(s => s.NgayXuat == khachHang.NgayXuat && s.ThongTinKhachHang == khachHang.ThongTinKhachHang).ToList();
                                    if (listTheoKhachHang.Any())
                                    {
                                        foreach (var item in listTheoKhachHang)
                                        {
                                            worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                            worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                            worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);

                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["B" + index].Value = item.MaDuoc;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["C" + index].Style.WrapText = true;
                                            worksheet.Cells["C" + index].Value = $"{item.Ten} {item.HamLuong}";

                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["D" + index].Value = item.DVT;

                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells["E" + index].Value = item.SoLuong;

                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["F" + index].Value = item.DonGiaDaCoVat;

                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["G" + index].Value = item.ThanhTien;

                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["H" + index].Value = item.DonGiaBan;

                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["I" + index].Value = item.ThanhTienBan;

                                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["J" + index].Value = item.DonGiaHoanTra;

                                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                            worksheet.Cells["K" + index].Value = item.ThanhTienHoanTra != 0 ? item.ThanhTienHoanTra : (decimal?)null;

                                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            worksheet.Cells["N" + index].Value = item.ThueSuat;

                                            index++;
                                            stt++;
                                        }
                                    }

                                }

                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":E" + index].Merge = true;
                    worksheet.Cells["A" + index + ":E" + index].Value = "Tổng";
                    worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["G" + index].Value = datas.Sum(s => s.ThanhTien);
                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["I" + index].Value = datas.Sum(s => s.ThanhTienBan);
                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    var sumHoanTra = datas.Sum(s => s.ThanhTienHoanTra);
                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Value = sumHoanTra;
                    if (sumHoanTra != 0)
                    {
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    }

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

    }
}
