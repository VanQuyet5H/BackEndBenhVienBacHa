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
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        //BVHD-3849: update 03/03/2022
        public async Task<GridDataSource> GetDataBaoCaoTongHopDoanhThuThaiSanDaSinhForGridAsync(BaoCaoTongHopDoanhThuThaiSanDaSinhQueryInfo queryInfo)
        {
            var dataTongHopDoanhThuThaiSanDaSinhQuery = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.NoiTruBenhAn != null && o.NoiTruBenhAn.ThoiDiemRaVien != null &&
                            o.NoiTruBenhAn.ThoiDiemRaVien >= queryInfo.FromDate &&
                            o.NoiTruBenhAn.ThoiDiemRaVien < queryInfo.ToDate &&
                            o.YeuCauNhapVienCons.Any());
            if (!string.IsNullOrEmpty(queryInfo.StrQuery))
            {
                dataTongHopDoanhThuThaiSanDaSinhQuery = dataTongHopDoanhThuThaiSanDaSinhQuery.ApplyLike(queryInfo.StrQuery.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }

            var dataTongHopDoanhThuThaiSanDaSinh = dataTongHopDoanhThuThaiSanDaSinhQuery
                .Select(o => new DataTongHopDoanhThuThaiSanDaSinh
                {
                    YeuCauTiepNhanId = o.Id,
                    YeuCauTiepNhanNgoaiTruId = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    MaTN = o.MaYeuCauTiepNhan,
                    BenhNhanId = o.BenhNhanId.GetValueOrDefault(),
                    MaBN = o.BenhNhan.MaBN,
                    TenBN = o.HoTen,
                    NgaySinh = o.NgaySinh,
                    ThangSinh = o.ThangSinh,
                    NamSinh = o.NamSinh,
                    DiaChi = o.DiaChiDayDu,
                    NgayVaoVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                    NgayRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
                    LoaiBenhAn = o.NoiTruBenhAn.LoaiBenhAn,
                    DaQuyetToan = o.NoiTruBenhAn.DaQuyetToan,
                    YeuCauTiepNhanConIds = o.YeuCauNhapVienCons.SelectMany(c => c.YeuCauTiepNhans).Select(d => d.Id).ToList()
                }).ToList();

            var yeuCauTiepNhanIds = new List<long>();
            var yeuCauTiepNhanChuaQuyetToanIds = new List<long>();
            var yeuCauTiepNhanConIds = new List<long>();
            foreach (var tongHopDoanhThuThaiSanDaSinh in dataTongHopDoanhThuThaiSanDaSinh)
            {
                yeuCauTiepNhanIds.Add(tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanId);
                if (tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId != null)
                {
                    yeuCauTiepNhanIds.Add(tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId.Value);
                }
                if (tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanConIds.Count > 0)
                {
                    yeuCauTiepNhanIds.AddRange(tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanConIds);
                    yeuCauTiepNhanConIds.AddRange(tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanConIds);
                }

                if (tongHopDoanhThuThaiSanDaSinh.DaQuyetToan != true)
                {
                    yeuCauTiepNhanChuaQuyetToanIds.Add(tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanId);
                    if (tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId != null)
                    {
                        yeuCauTiepNhanChuaQuyetToanIds.Add(tongHopDoanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId.Value);
                    }
                }
            }

            var yeuCauTiepNhanConChuaQuyetToanIds = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.DaQuyetToan != true && yeuCauTiepNhanConIds.Contains(o.Id)).Select(o => o.Id).ToList();
            yeuCauTiepNhanChuaQuyetToanIds.AddRange(yeuCauTiepNhanConChuaQuyetToanIds);

            var yeuCauKhamBenhGoiDichVuIds = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVuId != null && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.YeuCauTiepNhanId, o.YeuCauGoiDichVuId }).Distinct().ToList();

            var yeuCauDichVuKyThuatGoiDichVuIds = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVuId != null && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.YeuCauTiepNhanId, o.YeuCauGoiDichVuId }).Distinct().ToList();

            var yeuCauDichVuGiuongGoiDichVuIds = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                .Where(o => o.YeuCauGoiDichVuId != null && yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.YeuCauTiepNhanId, o.YeuCauGoiDichVuId }).Distinct().ToList();

            var yeuCauTiepNhanGoiDichVuIds = yeuCauKhamBenhGoiDichVuIds.Concat(yeuCauDichVuKyThuatGoiDichVuIds).Concat(yeuCauDichVuGiuongGoiDichVuIds).ToList();

            var yeuCauGoiDichVuIds = yeuCauTiepNhanGoiDichVuIds.Select(o => o.YeuCauGoiDichVuId.GetValueOrDefault()).ToList();
            var dataGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking.Where(o => yeuCauGoiDichVuIds.Contains(o.Id))
                .Select(o => new DataGoiDvTongHopDoanhThuThaiSanDaSinh
                {
                    YeuCauGoiDichVuId = o.Id,
                    BenhNhanId = o.BenhNhanId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    SoTienSauChietKhau = o.GiaSauChietKhau,
                    SoTienDaTamUng = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    BenhNhanSoSinhId = o.BenhNhanSoSinhId,
                    TrangThai = o.TrangThai
                }).ToList();
            var benhNhanIds = dataTongHopDoanhThuThaiSanDaSinh.Select(o => o.BenhNhanId).ToList();
            var dataGoiDichVuTheoBN = _yeuCauGoiDichVuRepository.TableNoTracking.Where(o => benhNhanIds.Contains(o.BenhNhanId))
                .Select(o => new DataGoiDvTongHopDoanhThuThaiSanDaSinh
                {
                    YeuCauGoiDichVuId = o.Id,
                    BenhNhanId = o.BenhNhanId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    SoTienSauChietKhau = o.GiaSauChietKhau,
                    SoTienDaTamUng = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    BenhNhanSoSinhId = o.BenhNhanSoSinhId,
                    TrangThai = o.TrangThai
                }).ToList();

            var dataPhieuThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.LoaiThuTienBenhNhan == Core.Domain.Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.DaHuy != true)
                .Select(o => new DataPhieuThuTongHopDoanhThuThaiSanDaSinh
                {
                    TaiKhoanBenhNhanThuId = o.Id,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    BenhNhanId = o.TaiKhoanBenhNhanId,
                    NgayThu = o.NgayThu,
                    ThuTienGoiDichVu = o.ThuTienGoiDichVu,
                    ChiPhis = o.TaiKhoanBenhNhanChis.Select(c => new DataPhieuThuCHiPhiTongHopDoanhThuThaiSanDaSinh
                    {
                        LoaiChiTienBenhNhan = c.LoaiChiTienBenhNhan,
                        DaHuy = c.DaHuy,
                        TienChiPhi = c.TienChiPhi,
                        SoTienBaoHiemTuNhanChiTra = c.SoTienBaoHiemTuNhanChiTra,
                        SoLuong = c.SoLuong,
                        DonGiaBaoHiem = c.DonGiaBaoHiem,
                        TiLeBaoHiemThanhToan = c.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = c.MucHuongBaoHiem
                    }
                    ).ToList()
                }).ToList();

            //get data dv chua thanh toan

            var chiPhiDichVuKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Where(yc => yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                    Soluong = 1,
                    DonGia = s.Gia,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();

            var chiPhiDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Where(yc => yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                    Soluong = s.SoLan,
                    DonGia = s.Gia,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();

            var chiPhiDichVuTruyenMaus = _yeuCauTruyenMauRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Where(yc => yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    Soluong = 1,
                    DonGia = s.DonGiaBan.GetValueOrDefault(),
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();

            var chiPhiDichVuGiuongChiPhiBenhViens = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Where(yc => (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();
            var chiPhiDichVuGiuongChiPhiBHYTs = _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = s.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    Soluong = s.SoLuong,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            
            foreach (var chiPhi in chiPhiDichVuGiuongChiPhiBenhViens)
            {
                var chiPhiGiuongBHYT = chiPhiDichVuGiuongChiPhiBHYTs.FirstOrDefault(o => o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId == chiPhi.Id);
                if (chiPhiGiuongBHYT != null)
                {
                    chiPhi.DuocHuongBHYT = true;
                    chiPhi.BaoHiemChiTra = chiPhiGiuongBHYT.BaoHiemChiTra;
                    chiPhi.DonGiaBHYT = chiPhiGiuongBHYT.DonGiaBHYT;
                    chiPhi.TiLeBaoHiemThanhToan = chiPhiGiuongBHYT.TiLeBaoHiemThanhToan;
                    chiPhi.MucHuongBaoHiem = chiPhiGiuongBHYT.MucHuongBaoHiem;
                }
            }

            var chiPhiDuocPhams = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Where(yc => yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();

            var chiPhiVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId))
                .Where(yc => yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();

            var chiPhiToaThuocs = _donThuocThanhToanRepository.TableNoTracking.Where(o => yeuCauTiepNhanChuaQuyetToanIds.Contains(o.YeuCauTiepNhanId.GetValueOrDefault()))
                .Where(dt =>
                    dt.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && dt.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy &&
                    (dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                     dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Select(s => new ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault(),
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    BaoHiemChiTra = s.BaoHiemChiTra,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                }).ToList();


            var chiPhiChuaThanhToans = new List<ChiPhiChuaThanhToanDoanhThuThaiSanDaSinh>();
            chiPhiChuaThanhToans.AddRange(chiPhiDichVuKhamBenhs);
            chiPhiChuaThanhToans.AddRange(chiPhiDichVuKyThuats);
            chiPhiChuaThanhToans.AddRange(chiPhiDichVuTruyenMaus);
            chiPhiChuaThanhToans.AddRange(chiPhiDichVuGiuongChiPhiBenhViens);
            chiPhiChuaThanhToans.AddRange(chiPhiDuocPhams);
            chiPhiChuaThanhToans.AddRange(chiPhiVatTus);
            chiPhiChuaThanhToans.AddRange(chiPhiToaThuocs);

            var dataReturn = new List<BaoCaoTongHopDoanhThuThaiSanDaSinhGridVo>();
            foreach (var doanhThuThaiSanDaSinh in dataTongHopDoanhThuThaiSanDaSinh)
            {
                var yeuCauTiepNhanGoiDichVuMeVaConIds = yeuCauTiepNhanGoiDichVuIds
                    .Where(o => o.YeuCauTiepNhanId == doanhThuThaiSanDaSinh.YeuCauTiepNhanId ||
                                o.YeuCauTiepNhanId == doanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId.GetValueOrDefault() ||
                                doanhThuThaiSanDaSinh.YeuCauTiepNhanConIds.Contains(o.YeuCauTiepNhanId));
                var goiDichVuMeVaConIds = yeuCauTiepNhanGoiDichVuMeVaConIds.Where(o => o.YeuCauGoiDichVuId != null).Select(o => o.YeuCauGoiDichVuId.Value);
                var goiMeVaCons = dataGoiDichVu.Where(o => goiDichVuMeVaConIds.Contains(o.YeuCauGoiDichVuId)).ToList();

                var goiDvTheoBNs = dataGoiDichVuTheoBN.Where(o => o.BenhNhanId == doanhThuThaiSanDaSinh.BenhNhanId).ToList();
                foreach (var dataGoiDvTongHopDoanhThuThaiSanDaSinh in goiDvTheoBNs)
                {
                    var ngayRaVien = doanhThuThaiSanDaSinh.NgayRaVien ?? DateTime.Now;
                    if (!goiMeVaCons.Select(o => o.YeuCauGoiDichVuId).Contains(dataGoiDvTongHopDoanhThuThaiSanDaSinh.YeuCauGoiDichVuId) && 
                        dataGoiDvTongHopDoanhThuThaiSanDaSinh.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy &&
                        dataGoiDvTongHopDoanhThuThaiSanDaSinh.ThoiDiemChiDinh < ngayRaVien &&
                        dataGoiDvTongHopDoanhThuThaiSanDaSinh.ThoiDiemChiDinh > ngayRaVien.AddMonths(-10))
                    {
                        goiMeVaCons.Add(dataGoiDvTongHopDoanhThuThaiSanDaSinh);
                    }
                }

                var soTienDangKyGoi = goiMeVaCons.Select(o => o.SoTienSauChietKhau).DefaultIfEmpty().Sum();
                var soTienTamUngGoi = goiMeVaCons.Select(o => o.SoTienDaTamUng).DefaultIfEmpty().Sum();

                var phieuThuMeVaCons = dataPhieuThu
                    .Where(o => o.YeuCauTiepNhanId == doanhThuThaiSanDaSinh.YeuCauTiepNhanId || o.YeuCauTiepNhanId == doanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId.GetValueOrDefault() ||
                                                        doanhThuThaiSanDaSinh.YeuCauTiepNhanConIds.Contains(o.YeuCauTiepNhanId))
                    .ToList();

                var tongTienDichVuDaThuNgoaiGoi = phieuThuMeVaCons.Where(o => o.ThuTienGoiDichVu != true).Select(o => o.TongSoTienChiPhi + o.TongSoTienBHYT).DefaultIfEmpty().Sum();
                var tongTienBHYTChiTraDaThu = phieuThuMeVaCons.Select(o => o.TongSoTienBHYT).DefaultIfEmpty().Sum();

                var yeuCauTiepNhanMeVaConChuaThanhToanIds = new List<long>();
                if (doanhThuThaiSanDaSinh.DaQuyetToan != true)
                {
                    yeuCauTiepNhanMeVaConChuaThanhToanIds.Add(doanhThuThaiSanDaSinh.YeuCauTiepNhanId);
                    if (doanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId != null)
                    {
                        yeuCauTiepNhanMeVaConChuaThanhToanIds.Add(doanhThuThaiSanDaSinh.YeuCauTiepNhanNgoaiTruId.Value);
                    }
                }
                foreach (var yeuCauTiepNhanConId in doanhThuThaiSanDaSinh.YeuCauTiepNhanConIds)
                {
                    if (yeuCauTiepNhanConChuaQuyetToanIds.Contains(yeuCauTiepNhanConId))
                    {
                        yeuCauTiepNhanMeVaConChuaThanhToanIds.Add(yeuCauTiepNhanConId);
                    }
                }
                decimal tongTienDichVuChuaThuNgoaiGoi = 0;
                decimal tongTienBHYTChiTraChuaThu = 0;
                if (yeuCauTiepNhanMeVaConChuaThanhToanIds.Any())
                {
                    var chiPhiMeVaConChuaThanhToans = chiPhiChuaThanhToans.Where(o => yeuCauTiepNhanMeVaConChuaThanhToanIds.Contains(o.YeuCauTiepNhanId)).ToList();
                    tongTienDichVuChuaThuNgoaiGoi = chiPhiMeVaConChuaThanhToans.Where(o=>o.YeuCauGoiDichVuId == null).Select(o=>o.ThanhTien - o.SoTienMG).DefaultIfEmpty().Sum();
                    tongTienBHYTChiTraChuaThu = chiPhiMeVaConChuaThanhToans.Select(o => o.BHYTThanhToan).DefaultIfEmpty().Sum();

                }

                var baoCaoTongHopDoanhThuThaiSanDaSinhGridVo = new BaoCaoTongHopDoanhThuThaiSanDaSinhGridVo
                {
                    Id = doanhThuThaiSanDaSinh.YeuCauTiepNhanId,
                    MaTN = doanhThuThaiSanDaSinh.MaTN,
                    MaBN = doanhThuThaiSanDaSinh.MaBN,
                    TenBN = doanhThuThaiSanDaSinh.TenBN,
                    NgaySinhStr = $"{(doanhThuThaiSanDaSinh.NgaySinh != null ? (doanhThuThaiSanDaSinh.NgaySinh.Value.ToString("00") + "/") : "")}" +
                                  $"{(doanhThuThaiSanDaSinh.ThangSinh != null ? (doanhThuThaiSanDaSinh.ThangSinh.Value.ToString("00") + "/") : "")}" +
                                  $"{(doanhThuThaiSanDaSinh.NamSinh != null ? (doanhThuThaiSanDaSinh.NamSinh.Value.ToString()) : "")}",
                    DiaChi = doanhThuThaiSanDaSinh.DiaChi,
                    NgayVaoVien = doanhThuThaiSanDaSinh.NgayVaoVien,
                    NgayRaVien = doanhThuThaiSanDaSinh.NgayRaVien,
                    CachThucDe = doanhThuThaiSanDaSinh.LoaiBenhAn == Core.Domain.Enums.LoaiBenhAn.SanKhoaMo ? "Đẻ mổ" : "Đẻ thường",
                    TongTienSauChietKhau = soTienDangKyGoi,
                    TongTienDichVuNgoaiGoi = tongTienDichVuDaThuNgoaiGoi + tongTienDichVuChuaThuNgoaiGoi,
                    TongTienBHYTChiTra = tongTienBHYTChiTraDaThu + tongTienBHYTChiTraChuaThu,
                    SoTienDaThanhToan = soTienTamUngGoi + tongTienDichVuDaThuNgoaiGoi - tongTienBHYTChiTraDaThu,
                };
                dataReturn.Add(baoCaoTongHopDoanhThuThaiSanDaSinhGridVo);
            }
            return new GridDataSource { Data = dataReturn.ToArray(), TotalRowCount = dataReturn.Count() };
        }

        //public async Task<GridDataSource> GetDataBaoCaoTongHopDoanhThuThaiSanDaSinhForGridAsyncOld(BaoCaoTongHopDoanhThuThaiSanDaSinhQueryInfo queryInfo)
        //{
        //    var dataTongHopDoanhThuThaiSanDaSinh = _yeuCauTiepNhanRepository.TableNoTracking
        //        .Where(o => o.NoiTruBenhAn != null && o.NoiTruBenhAn.DaQuyetToan == true &&
        //                    o.TaiKhoanBenhNhanThus.Any(t => t.LoaiThuTienBenhNhan == Core.Domain.Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && t.DaHuy != true && t.NgayThu >= queryInfo.FromDate && t.NgayThu < queryInfo.ToDate) &&
        //                    o.YeuCauNhapVienCons.Any())
        //        .Select(o => new DataTongHopDoanhThuThaiSanDaSinh
        //        {
        //            YeuCauTiepNhanId = o.Id,
        //            MaTN = o.MaYeuCauTiepNhan,
        //            TenBN = o.HoTen,
        //            NgaySinh = o.NgaySinh,
        //            ThangSinh = o.ThangSinh,
        //            NamSinh = o.NamSinh,
        //            DiaChi = o.DiaChiDayDu,
        //            NgayVaoVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
        //            NgayRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
        //            LoaiBenhAn = o.NoiTruBenhAn.LoaiBenhAn,
        //            DvktGoiDichVuIds = o.YeuCauDichVuKyThuats.Select(d => d.YeuCauGoiDichVuId).ToList(),
        //            GiuongGoiDichVuIds = o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Select(g => g.YeuCauGoiDichVuId).ToList(),
        //            BenhNhanConIds = o.YeuCauNhapVienCons.Select(c => c.BenhNhanId).ToList(),
        //            DataPhieuThus = o.TaiKhoanBenhNhanThus.Where(t => t.LoaiThuTienBenhNhan == Core.Domain.Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && t.DaHuy != true)
        //                                        .Select(t => new DataPhieuThuTongHopDoanhThuThaiSanDaSinh
        //                                        {
        //                                            TaiKhoanBenhNhanThuId = t.Id,
        //                                            BenhNhanId = t.TaiKhoanBenhNhanId,
        //                                            NgayThu = t.NgayThu,
        //                                        }).ToList()
        //        }).ToList();

        //    var goiDichVuMeIds = dataTongHopDoanhThuThaiSanDaSinh.SelectMany(o => o.DvktGoiDichVuIds).Where(o => o != null).Select(o => o.Value).Distinct()
        //                    .Concat(dataTongHopDoanhThuThaiSanDaSinh.SelectMany(o => o.GiuongGoiDichVuIds).Where(o => o != null).Select(o => o.Value).Distinct()).ToList();

        //    var dataGoiDichVuMe = _yeuCauGoiDichVuRepository.TableNoTracking.Where(o => goiDichVuMeIds.Contains(o.Id))
        //        .Select(o => new DataGoiDvTongHopDoanhThuThaiSanDaSinh
        //        {
        //            YeuCauGoiDichVuId = o.Id,
        //            ThoiDiemChiDinh = o.ThoiDiemChiDinh,
        //            SoTienSauChietKhau = o.GiaSauChietKhau,
        //            SoTienDaTamUng = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
        //            BenhNhanSoSinhId = o.BenhNhanSoSinhId
        //        }).ToList();

        //    var benhNhanConIds = dataTongHopDoanhThuThaiSanDaSinh.SelectMany(o => o.BenhNhanConIds).ToList();

        //    var dataGoiDichVuCon = _yeuCauGoiDichVuRepository.TableNoTracking.Where(o => o.BenhNhanSoSinhId != null && benhNhanConIds.Contains(o.BenhNhanSoSinhId.Value))
        //        .Select(o => new DataGoiDvTongHopDoanhThuThaiSanDaSinh
        //        {
        //            YeuCauGoiDichVuId = o.Id,
        //            ThoiDiemChiDinh = o.ThoiDiemChiDinh,
        //            SoTienSauChietKhau = o.GiaSauChietKhau,
        //            SoTienDaTamUng = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
        //            BenhNhanSoSinhId = o.BenhNhanSoSinhId
        //        }).ToList();
        //    var benhNhanIds = benhNhanConIds.Concat(dataTongHopDoanhThuThaiSanDaSinh.Select(o => o.BenhNhanId)).ToList();
        //    var dataPhieuThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
        //        .Where(o => benhNhanIds.Contains(o.TaiKhoanBenhNhanId) && o.LoaiThuTienBenhNhan == Core.Domain.Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.DaHuy != true)
        //        .Select(o => new DataPhieuThuTongHopDoanhThuThaiSanDaSinh
        //        {
        //            TaiKhoanBenhNhanThuId = o.Id,
        //            BenhNhanId = o.TaiKhoanBenhNhanId,
        //            NgayThu = o.NgayThu,
        //            ThuTienGoiDichVu = o.ThuTienGoiDichVu,
        //            ChiPhis = o.TaiKhoanBenhNhanChis.Select(c => new DataPhieuThuCHiPhiTongHopDoanhThuThaiSanDaSinh
        //            {
        //                LoaiChiTienBenhNhan = c.LoaiChiTienBenhNhan,
        //                DaHuy = c.DaHuy,
        //                TienChiPhi = c.TienChiPhi,
        //                SoLuong = c.SoLuong,
        //                DonGiaBaoHiem = c.DonGiaBaoHiem,
        //                TiLeBaoHiemThanhToan = c.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = c.MucHuongBaoHiem
        //            }
        //            ).ToList()
        //        }).ToList();

        //    var dataReturn = new List<BaoCaoTongHopDoanhThuThaiSanDaSinhGridVo>();
        //    foreach (var doanhThuThaiSanDaSinh in dataTongHopDoanhThuThaiSanDaSinh)
        //    {
        //        var phieuThuLast = doanhThuThaiSanDaSinh.DataPhieuThus.OrderBy(o => o.NgayThu).Last();
        //        if (phieuThuLast.NgayThu >= queryInfo.FromDate && phieuThuLast.NgayThu < queryInfo.ToDate)
        //        {
        //            var goiMe = dataGoiDichVuMe.Where(o => doanhThuThaiSanDaSinh.DvktGoiDichVuIds.Contains(o.YeuCauGoiDichVuId) || doanhThuThaiSanDaSinh.GiuongGoiDichVuIds.Contains(o.YeuCauGoiDichVuId));
        //            var goiCon = dataGoiDichVuCon.Where(o => doanhThuThaiSanDaSinh.BenhNhanConIds.Contains(o.BenhNhanSoSinhId.GetValueOrDefault()));

        //            var soTienDangKyGoi = goiMe.Select(o => o.SoTienSauChietKhau).DefaultIfEmpty().Sum() + goiCon.Select(o => o.SoTienSauChietKhau).DefaultIfEmpty().Sum();
        //            var soTienTamUngGoi = goiMe.Select(o => o.SoTienDaTamUng).DefaultIfEmpty().Sum() + goiCon.Select(o => o.SoTienDaTamUng).DefaultIfEmpty().Sum();

        //            var dataPhieuThuNgoaiGoiCon = dataPhieuThu.Where(o => doanhThuThaiSanDaSinh.BenhNhanConIds.Contains(o.BenhNhanId) && o.ThuTienGoiDichVu != true).ToList();
        //            var dataPhieuThuNgoaiGoiMe = dataPhieuThu.Where(o => o.ThuTienGoiDichVu != true &&
        //                                                (doanhThuThaiSanDaSinh.DataPhieuThus.Select(t => t.TaiKhoanBenhNhanThuId).Contains(o.TaiKhoanBenhNhanThuId)
        //                                                    || (o.BenhNhanId == doanhThuThaiSanDaSinh.BenhNhanId && goiMe.Any() && goiMe.OrderBy(g => g.ThoiDiemChiDinh).First().ThoiDiemChiDinh <= o.NgayThu && o.NgayThu < phieuThuLast.NgayThu))).ToList();

        //            var dataPhieuThuTrongGoiCon = dataPhieuThu.Where(o => doanhThuThaiSanDaSinh.BenhNhanConIds.Contains(o.BenhNhanId) && o.ThuTienGoiDichVu == true).ToList();
        //            var dataPhieuThuTrongGoiMe = dataPhieuThu.Where(o => o.ThuTienGoiDichVu == true &&
        //                                                (doanhThuThaiSanDaSinh.DataPhieuThus.Select(t => t.TaiKhoanBenhNhanThuId).Contains(o.TaiKhoanBenhNhanThuId)
        //                                                    || (o.BenhNhanId == doanhThuThaiSanDaSinh.BenhNhanId && goiMe.Any() && goiMe.OrderBy(g => g.ThoiDiemChiDinh).First().ThoiDiemChiDinh <= o.NgayThu && o.NgayThu < phieuThuLast.NgayThu))).ToList();

        //            var tongTienDichVuNgoaiGoi = dataPhieuThuNgoaiGoiMe.Select(o => o.TongSoTienChiPhi).DefaultIfEmpty().Sum() + dataPhieuThuNgoaiGoiCon.Select(o => o.TongSoTienChiPhi).DefaultIfEmpty().Sum();
        //            var tongTienBHYTChiTra = dataPhieuThuNgoaiGoiMe.Select(o => o.TongSoTienBHYT).DefaultIfEmpty().Sum() + dataPhieuThuNgoaiGoiCon.Select(o => o.TongSoTienBHYT).DefaultIfEmpty().Sum()
        //                + dataPhieuThuTrongGoiCon.Select(o => o.TongSoTienBHYT).DefaultIfEmpty().Sum() + dataPhieuThuTrongGoiMe.Select(o => o.TongSoTienBHYT).DefaultIfEmpty().Sum();
        //            var baoCaoTongHopDoanhThuThaiSanDaSinhGridVo = new BaoCaoTongHopDoanhThuThaiSanDaSinhGridVo
        //            {
        //                Id = doanhThuThaiSanDaSinh.YeuCauTiepNhanId,
        //                MaTN = doanhThuThaiSanDaSinh.MaTN,
        //                TenBN = doanhThuThaiSanDaSinh.TenBN,
        //                NgaySinhStr = $"{(doanhThuThaiSanDaSinh.NgaySinh != null ? (doanhThuThaiSanDaSinh.NgaySinh.Value.ToString("00") + "/") : "")}" +
        //                                $"{(doanhThuThaiSanDaSinh.ThangSinh != null ? (doanhThuThaiSanDaSinh.ThangSinh.Value.ToString("00") + "/") : "")}" +
        //                                $"{(doanhThuThaiSanDaSinh.NamSinh != null ? (doanhThuThaiSanDaSinh.NamSinh.Value.ToString()) : "")}",
        //                DiaChi = doanhThuThaiSanDaSinh.DiaChi,
        //                NgayVaoVien = doanhThuThaiSanDaSinh.NgayVaoVien,
        //                NgayRaVien = doanhThuThaiSanDaSinh.NgayRaVien,
        //                CachThucDe = doanhThuThaiSanDaSinh.LoaiBenhAn == Core.Domain.Enums.LoaiBenhAn.SanKhoaMo ? "Đẻ mổ" : "Đẻ thường",
        //                TongTienSauChietKhau = soTienDangKyGoi,
        //                TongTienDichVuNgoaiGoi = tongTienDichVuNgoaiGoi,
        //                TongTienBHYTChiTra = tongTienBHYTChiTra,
        //                SoTienDaThanhToan = soTienTamUngGoi + tongTienDichVuNgoaiGoi - tongTienBHYTChiTra,
        //            };
        //            dataReturn.Add(baoCaoTongHopDoanhThuThaiSanDaSinhGridVo);
        //        }
        //    }
        //    return new GridDataSource { Data = dataReturn.ToArray(), TotalRowCount = dataReturn.Count() };
        //}

        public virtual byte[] ExportBaoCaoTongHopDoanhThuThaiSanDaSinh(GridDataSource gridDataSource, BaoCaoTongHopDoanhThuThaiSanDaSinhQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTongHopDoanhThuThaiSanDaSinhGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỔNG HỢP DOANH THU THAI SẢN ĐÃ SINH");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 40;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 40;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    worksheet.Row(3).Height = 21;
                    worksheet.Row(6).Height = 51;
                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:P3"])
                    {
                        range.Worksheet.Cells["A3:P3"].Merge = true;
                        range.Worksheet.Cells["A3:P3"].Value = "BÁO CÁO TỔNG HỢP DOANH THU THAI SẢN ĐÃ SINH";
                        range.Worksheet.Cells["A3:P3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:P3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:P3"].Style.Font.SetFromFont(new Font("Tahoma", 15));
                        range.Worksheet.Cells["A3:P3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:P3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:P4"])
                    {
                        range.Worksheet.Cells["A4:P4"].Merge = true;
                        range.Worksheet.Cells["A4:P4"].Value = "Thời gian từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:P4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:P4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:P4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:P4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:P4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6:P6"])
                    {
                        range.Worksheet.Cells["A6:P6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:P6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:P6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:P6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:P6"].Style.WrapText = true;
                        range.Worksheet.Cells["A6:P6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value ="STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Mã NB";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã TN";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Tên BN";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Ngày sinh";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Địa chỉ";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Ngày vào viện";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Ngày ra viện";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "Cách thức đẻ";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Số tiền gói đăng ký";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "Số tiền phát sinh ngoài gói (chưa trừ BHYT)";

                        range.Worksheet.Cells["L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L6"].Value = "Tổng số tiền phải thanh toán(chưa trừ BHYT)";

                        range.Worksheet.Cells["M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M6"].Value = "Thành tiền BHYT chi trả";

                        range.Worksheet.Cells["N6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N6"].Value = "Thành tiền BN phải trả";
                    
                        range.Worksheet.Cells["O6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O6"].Value = "Số tiền BN đã thanh toán";

                        range.Worksheet.Cells["P6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P6"].Value = "Số tiền BN còn phải thanh toán";

                    }

                    int index = 7;
                    int stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":P" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                range.Worksheet.Cells["A" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value = stt;

                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Value = item.MaBN;

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = item.MaTN;

                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Value = item.TenBN;
                                range.Worksheet.Cells["D" + index ].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Value = item.NgaySinhStr;

                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Value = item.DiaChi;
                                range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Value = item.NgayVaoVienStr;

                                range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["H" + index].Value = item.NgayRaVienStr;

                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["I" + index].Value = item.CachThucDe;
                                range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["J" + index].Value = item.TongTienSauChietKhau;
                                range.Worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["K" + index].Value = item.TongTienDichVuNgoaiGoi;
                                range.Worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["L" + index].Value = item.TongSoTienChuaTruBHYT;
                                range.Worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["M" + index].Value = item.TongTienBHYTChiTra;
                                range.Worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["N" + index].Value = item.ThanhTien;
                                range.Worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["O" + index].Value = item.SoTienDaThanhToan;
                                range.Worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["P" + index].Value = item.SoTienConThieu;
                                range.Worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                                stt++;
                                index++;

                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["A" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":P" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["A" + index + ":P" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":I" + index].Style.WrapText = true;
                    worksheet.Cells["A" + index + ":I" + index].Merge = true;
                    worksheet.Cells["A" + index + ":I" + index].Value = "Tổng cộng";

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Value = datas.Sum(s => s.TongTienSauChietKhau);

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Value = datas.Sum(s => s.TongTienDichVuNgoaiGoi);

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Value = datas.Sum(s => s.TongSoTienChuaTruBHYT);

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Value = datas.Sum(s => s.TongTienBHYTChiTra);

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index].Value = datas.Sum(s => s.ThanhTien);

                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["O" + index].Value = datas.Sum(s => s.SoTienDaThanhToan);

                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["P" + index].Value = datas.Sum(s => s.SoTienConThieu);
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
