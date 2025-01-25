using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForExportAsync(BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var dsPhong = await _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToListAsync();
            var khoaPhongData = dsPhong.GroupBy(o => o.KhoaPhong.Id, o => o).Select(g => new
            {
                Id = g.Key,
                KhoaPhong = g.First().KhoaPhong.Ten,
                LoaiDichVuData = new List<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>()
            }).ToList();
            var quayThuocData = new
            {
                Id = 0,
                KhoaPhong = "Quầy thuốc",
                LoaiDichVuData = new List<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>()
            };
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                                KhoaPhong = khoaPhongItem.KhoaPhong,
                                DichVu = yeuCauKhamBenh.TenDichVu,
                                DoanhThuTheoThang = yeuCauKhamBenh.Gia,
                                MienGiamTheoThang = yeuCauKhamBenh.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauKhamBenh.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100
                            };
                            khoaPhongItem.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                                KhoaPhong = khoaPhongItem.KhoaPhong,
                                DichVu = yeuCauDichVuKyThuat.TenDichVu,
                                DoanhThuTheoThang = yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan,
                                MienGiamTheoThang = yeuCauDichVuKyThuat.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauDichVuKyThuat.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan
                            };
                            khoaPhongItem.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                        }
                    }
                }
                foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                                KhoaPhong = khoaPhongItem.KhoaPhong,
                                DichVu = yeuCauDichVuGiuong.Ten,
                                DoanhThuTheoThang = yeuCauDichVuGiuong.Gia,
                                MienGiamTheoThang = yeuCauDichVuGiuong.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauDichVuGiuong.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100
                            };
                            khoaPhongItem.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }
                }

                foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDuocPham.DuocPhamBenhVienId && o.DichVu == yeuCauDuocPham.Ten);
                    if (dichVu == null)
                    {
                        dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                        {
                            Id = yeuCauDuocPham.DuocPhamBenhVienId,
                            KhoaPhong = quayThuocData.KhoaPhong,
                            DichVu = yeuCauDuocPham.Ten,
                            DoanhThuTheoThang = yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong,
                            MienGiamTheoThang = yeuCauDuocPham.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            ChiPhiKhacTheoThang = yeuCauDuocPham.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            BhytTheoThang = yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong
                        };
                        quayThuocData.LoaiDichVuData.Add(dichVu);
                    }
                    else
                    {
                        dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                        dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                    }
                }
                foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauVatTu.VatTuBenhVienId && o.DichVu == yeuCauVatTu.Ten);
                    if (dichVu == null)
                    {
                        dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                        {
                            Id = yeuCauVatTu.VatTuBenhVienId,
                            KhoaPhong = quayThuocData.KhoaPhong,
                            DichVu = yeuCauVatTu.Ten,
                            DoanhThuTheoThang = yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong,
                            MienGiamTheoThang = yeuCauVatTu.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            ChiPhiKhacTheoThang = yeuCauVatTu.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            BhytTheoThang = yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong
                        };
                        quayThuocData.LoaiDichVuData.Add(dichVu);
                    }
                    else
                    {
                        dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                        dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                    }
                }

                foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                {
                    var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == donThuocThanhToanChiTiet.DuocPhamId && o.DichVu == donThuocThanhToanChiTiet.Ten);
                    if (dichVu == null)
                    {
                        dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                        {
                            Id = donThuocThanhToanChiTiet.DuocPhamId,
                            KhoaPhong = quayThuocData.KhoaPhong,
                            DichVu = donThuocThanhToanChiTiet.Ten,
                            DoanhThuTheoThang = donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong,
                            MienGiamTheoThang = donThuocThanhToanChiTiet.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            ChiPhiKhacTheoThang = donThuocThanhToanChiTiet.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            BhytTheoThang = donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong
                        };
                        quayThuocData.LoaiDichVuData.Add(dichVu);
                    }
                    else
                    {
                        dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                        dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    }
                }

                foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                {
                    var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == donVTYTThanhToanChiTiet.VatTuBenhVienId && o.DichVu == donVTYTThanhToanChiTiet.Ten);
                    if (dichVu == null)
                    {
                        dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                        {
                            Id = donVTYTThanhToanChiTiet.VatTuBenhVienId,
                            KhoaPhong = quayThuocData.KhoaPhong,
                            DichVu = donVTYTThanhToanChiTiet.Ten,
                            DoanhThuTheoThang = donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong,
                            MienGiamTheoThang = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            ChiPhiKhacTheoThang = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                            BhytTheoThang = 0
                        };
                        quayThuocData.LoaiDichVuData.Add(dichVu);
                    }
                    else
                    {
                        dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                        dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    }
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs)
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                //            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                //            if (dichVu == null)
                //            {
                //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                //                {
                //                    Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                //                    KhoaPhong = khoaPhongItem.KhoaPhong,
                //                    DichVu = yeuCauKhamBenh.TenDichVu,
                //                    DoanhThuTheoThang = doanhThuDv,
                //                    MienGiamTheoThang = tiLeMienGiam * doanhThuDv,
                //                    ChiPhiKhacTheoThang = tiLeGiamTruKhac * doanhThuDv,
                //                    BhytTheoThang = 0
                //                };
                //                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                //            }
                //            else
                //            {
                //                dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //                dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //                dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //            }
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats)
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;

                //            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                //            if (dichVu == null)
                //            {
                //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                //                {
                //                    Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                //                    KhoaPhong = khoaPhongItem.KhoaPhong,
                //                    DichVu = yeuCauDichVuKyThuat.TenDichVu,
                //                    DoanhThuTheoThang = doanhThuDv,
                //                    MienGiamTheoThang = tiLeMienGiam * doanhThuDv,
                //                    ChiPhiKhacTheoThang = tiLeGiamTruKhac * doanhThuDv,
                //                    BhytTheoThang = 0
                //                };
                //                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                //            }
                //            else
                //            {
                //                dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //                dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //                dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //            }
                //        }
                //    }
                //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens)
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                //            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                //            if (dichVu == null)
                //            {
                //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                //                {
                //                    Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                //                    KhoaPhong = khoaPhongItem.KhoaPhong,
                //                    DichVu = yeuCauDichVuGiuong.Ten,
                //                    DoanhThuTheoThang = doanhThuDv,
                //                    MienGiamTheoThang = tiLeMienGiam * doanhThuDv,
                //                    ChiPhiKhacTheoThang = tiLeGiamTruKhac * doanhThuDv,
                //                    BhytTheoThang = 0
                //                };
                //                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                //            }
                //            else
                //            {
                //                dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //                dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //                dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //            }
                //        }
                //    }
                //}
            }

            if (queryInfo.KySoSanhTuNgay != null && queryInfo.KySoSanhDenNgay != null)
            {
                var ycTiepNhanKySoSanhQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                    o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                    queryInfo.KySoSanhTuNgay.Value <= o.ThoiDiemTiepNhan && o.ThoiDiemTiepNhan < queryInfo.KySoSanhDenNgay.Value);

                var ycTiepNhanKySoSanhData = await ycTiepNhanKySoSanhQuery
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //TODO: need update goi dv
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .ToListAsync();

                foreach (var yeuCauTiepNhan in ycTiepNhanKySoSanhData)
                {
                    foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                                {
                                    Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                                    KhoaPhong = khoaPhongItem.KhoaPhong,
                                    DichVu = yeuCauKhamBenh.TenDichVu,
                                    DoanhThuTheoKySoSanh = yeuCauKhamBenh.Gia,
                                    MienGiamTheoKySoSanh = yeuCauKhamBenh.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = yeuCauKhamBenh.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100
                                };
                                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                            }
                        }
                    }
                    foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                                {
                                    Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                                    KhoaPhong = khoaPhongItem.KhoaPhong,
                                    DichVu = yeuCauDichVuKyThuat.TenDichVu,
                                    DoanhThuTheoKySoSanh = yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan,
                                    MienGiamTheoKySoSanh = yeuCauDichVuKyThuat.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = yeuCauDichVuKyThuat.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan
                                };
                                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                            }
                        }
                    }
                    foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                                {
                                    Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                                    KhoaPhong = khoaPhongItem.KhoaPhong,
                                    DichVu = yeuCauDichVuGiuong.Ten,
                                    DoanhThuTheoKySoSanh = yeuCauDichVuGiuong.Gia,
                                    MienGiamTheoKySoSanh = yeuCauDichVuGiuong.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = yeuCauDichVuGiuong.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100
                                };
                                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                            }
                        }
                    }

                    foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDuocPham.DuocPhamBenhVienId && o.DichVu == yeuCauDuocPham.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = yeuCauDuocPham.DuocPhamBenhVienId,
                                KhoaPhong = quayThuocData.KhoaPhong,
                                DichVu = yeuCauDuocPham.Ten,
                                DoanhThuTheoKySoSanh = yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong,
                                MienGiamTheoKySoSanh = yeuCauDuocPham.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoKySoSanh = yeuCauDuocPham.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoKySoSanh = yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong
                            };
                            quayThuocData.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                            dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                        }
                    }

                    foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauVatTu.VatTuBenhVienId && o.DichVu == yeuCauVatTu.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = yeuCauVatTu.VatTuBenhVienId,
                                KhoaPhong = quayThuocData.KhoaPhong,
                                DichVu = yeuCauVatTu.Ten,
                                DoanhThuTheoKySoSanh = yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong,
                                MienGiamTheoKySoSanh = yeuCauVatTu.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoKySoSanh = yeuCauVatTu.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoKySoSanh = yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong
                            };
                            quayThuocData.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                            dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                        }
                    }

                    foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                    {
                        var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == donThuocThanhToanChiTiet.DuocPhamId && o.DichVu == donThuocThanhToanChiTiet.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = donThuocThanhToanChiTiet.DuocPhamId,
                                KhoaPhong = quayThuocData.KhoaPhong,
                                DichVu = donThuocThanhToanChiTiet.Ten,
                                DoanhThuTheoKySoSanh = donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong,
                                MienGiamTheoKySoSanh = donThuocThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoKySoSanh = donThuocThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoKySoSanh = donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong
                            };
                            quayThuocData.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                            dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                        }
                    }

                    foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                    {
                        var dichVu = quayThuocData.LoaiDichVuData.FirstOrDefault(o => o.Id == donVTYTThanhToanChiTiet.VatTuBenhVienId && o.DichVu == donVTYTThanhToanChiTiet.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                            {
                                Id = donVTYTThanhToanChiTiet.VatTuBenhVienId,
                                KhoaPhong = quayThuocData.KhoaPhong,
                                DichVu = donVTYTThanhToanChiTiet.Ten,
                                DoanhThuTheoKySoSanh = donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong,
                                MienGiamTheoKySoSanh = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoKySoSanh = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoKySoSanh = 0
                            };
                            quayThuocData.LoaiDichVuData.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                            dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        }
                    }
                    //TODO: need update goi dv
                    //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    //{
                    //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                    //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                    //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs)
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                    //            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                    //            if (dichVu == null)
                    //            {
                    //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                    //                {
                    //                    Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                    //                    KhoaPhong = khoaPhongItem.KhoaPhong,
                    //                    DichVu = yeuCauKhamBenh.TenDichVu,
                    //                    DoanhThuTheoKySoSanh = doanhThuDv,
                    //                    MienGiamTheoKySoSanh = tiLeMienGiam * doanhThuDv,
                    //                    ChiPhiKhacTheoKySoSanh = tiLeGiamTruKhac * doanhThuDv,
                    //                    BhytTheoKySoSanh = 0
                    //                };
                    //                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                    //            }
                    //            else
                    //            {
                    //                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //            }
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats)
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;

                    //            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                    //            if (dichVu == null)
                    //            {
                    //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                    //                {
                    //                    Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                    //                    KhoaPhong = khoaPhongItem.KhoaPhong,
                    //                    DichVu = yeuCauDichVuKyThuat.TenDichVu,
                    //                    DoanhThuTheoKySoSanh = doanhThuDv,
                    //                    MienGiamTheoKySoSanh = tiLeMienGiam * doanhThuDv,
                    //                    ChiPhiKhacTheoKySoSanh = tiLeGiamTruKhac * doanhThuDv,
                    //                    BhytTheoKySoSanh = 0
                    //                };
                    //                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                    //            }
                    //            else
                    //            {
                    //                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //            }
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens)
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                    //            var dichVu = khoaPhongItem.LoaiDichVuData.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                    //            if (dichVu == null)
                    //            {
                    //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
                    //                {
                    //                    Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                    //                    KhoaPhong = khoaPhongItem.KhoaPhong,
                    //                    DichVu = yeuCauDichVuGiuong.Ten,
                    //                    DoanhThuTheoKySoSanh = doanhThuDv,
                    //                    MienGiamTheoKySoSanh = tiLeMienGiam * doanhThuDv,
                    //                    ChiPhiKhacTheoKySoSanh = tiLeGiamTruKhac * doanhThuDv,
                    //                    BhytTheoKySoSanh = 0
                    //                };
                    //                khoaPhongItem.LoaiDichVuData.Add(dichVu);
                    //            }
                    //            else
                    //            {
                    //                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            var gridData = new List<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>();
            int num = 0;
            foreach (var khoaPhongItem in khoaPhongData)
            {
                if (khoaPhongItem.LoaiDichVuData.Count > 0)
                {
                    num++;
                    gridData.AddRange(khoaPhongItem.LoaiDichVuData.Select(o =>
                    {
                        o.Stt = num;
                        return o;
                    }));
                }
            }
            if (quayThuocData.LoaiDichVuData.Count > 0)
            {
                num++;
                gridData.AddRange(quayThuocData.LoaiDichVuData.Select(o =>
                {
                    o.Stt = num;
                    return o;
                }));
            }
            return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = gridData.Count };
        }

        public List<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo> GetDataForBaoCaoChiTietDoanhThuTheoKhoaPhong(
            DateTimeFilterVo dateTimeFilter)
        {
            var baoCaoChihTietDoanhThuTheoKhoaPhongGrid = new List<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo>();

            var baoCaoChihTietDoanhThuTheoKhoaPhong = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
            {
                Id = 1,
                KhoaPhong = "Khoa CĐHA",
                DichVu = "MRI",
                DoanhThuTheoThang = 890000,
                MienGiamTheoThang = 100000,
                ChiPhiKhacTheoThang = 100000,
                BhytTheoThang = 100000,
                DoanhThuTheoKySoSanh = 1200000,
                MienGiamTheoKySoSanh = 100000,
                ChiPhiKhacTheoKySoSanh = 100000,
                BhytTheoKySoSanh = 100000
            };

            baoCaoChihTietDoanhThuTheoKhoaPhongGrid.Add(baoCaoChihTietDoanhThuTheoKhoaPhong);

            var baoCaoChihTietDoanhThuTheoKhoaPhong1 = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
            {
                Id = 2,
                KhoaPhong = "Khoa CĐHA",
                DichVu = "XQ",
                DoanhThuTheoThang = 990000,
                MienGiamTheoThang = 120000,
                BhytTheoThang = 102000,
                DoanhThuTheoKySoSanh = 900000,
                MienGiamTheoKySoSanh = 100000,
                BhytTheoKySoSanh = 200000
            };

            baoCaoChihTietDoanhThuTheoKhoaPhongGrid.Add(baoCaoChihTietDoanhThuTheoKhoaPhong1);

            var baoCaoChihTietDoanhThuTheoKhoaPhong2 = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
            {
                Id = 3,
                KhoaPhong = "Khoa xét nghiệm",
                DichVu = "Xét mghiệm máu",
                DoanhThuTheoThang = 980000,
                MienGiamTheoThang = 120000,
                ChiPhiKhacTheoThang = 100000,
                BhytTheoThang = 102000,
                DoanhThuTheoKySoSanh = 900000,
                MienGiamTheoKySoSanh = 100000,
                BhytTheoKySoSanh = 200000
            };

            baoCaoChihTietDoanhThuTheoKhoaPhongGrid.Add(baoCaoChihTietDoanhThuTheoKhoaPhong2);

            var baoCaoChihTietDoanhThuTheoKhoaPhong3 = new BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo
            {
                Id = 4,
                KhoaPhong = "Khoa xét nghiệm",
                DichVu = "Xét nghiệm nước tiểu",
                DoanhThuTheoThang = 1000000,
                MienGiamTheoThang = 120000,
                BhytTheoThang = 102000,
                DoanhThuTheoKySoSanh = 900000,
                BhytTheoKySoSanh = 200000,
                MienGiamTheoKySoSanh = 100000
            };

            baoCaoChihTietDoanhThuTheoKhoaPhongGrid.Add(baoCaoChihTietDoanhThuTheoKhoaPhong3);

            return baoCaoChihTietDoanhThuTheoKhoaPhongGrid;
        }

        // to do nam ho
        // BaoCaoChiTietDoanhThuTheoKhoaPhongMasterGridVo
        public async Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForMasterGridAsync(BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var dsPhong = await _phongBenhVienRepository.TableNoTracking.Include(o => o.KhoaPhong).ToListAsync();

            var khoaPhongData = dsPhong.GroupBy(o => o.KhoaPhong.Id, o => o).Select(g => new BaoCaoChiTietDoanhThuTheoKhoaPhongMasterGridVo
            {
                Id = g.Key,
                KhoaPhong = g.First().KhoaPhong.Ten,
                ToTalDoanhThuTheoThang = 0,
                ToTalMienGiamTheoThang = 0,
                ToTalChiPhiKhacTheoThang = 0,
                ToTalBhytTheoThang = 0,
                ToTalDoanhThuTheoKySoSanh = 0,
                ToTalChiPhiKhacTheoKySoSanh = 0,
                ToTalBhytTheoKySoSanh = 0,
                ToTalMienGiamTheoKySoSanh = 0
            }).ToList();
            var quayThuocData = new BaoCaoChiTietDoanhThuTheoKhoaPhongMasterGridVo
            {
                Id = 0,
                KhoaPhong = "Quầy thuốc",
                ToTalDoanhThuTheoThang = 0,
                ToTalMienGiamTheoThang = 0,
                ToTalChiPhiKhacTheoThang = 0,
                ToTalBhytTheoThang = 0,
                ToTalDoanhThuTheoKySoSanh = 0,
                ToTalChiPhiKhacTheoKySoSanh = 0,
                ToTalBhytTheoKySoSanh = 0,
                ToTalMienGiamTheoKySoSanh = 0
            };

            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        khoaPhongItem.ToTalDoanhThuTheoThang = khoaPhongItem.ToTalDoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                        khoaPhongItem.ToTalMienGiamTheoThang = khoaPhongItem.ToTalMienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ToTalChiPhiKhacTheoThang = khoaPhongItem.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ToTalBhytTheoThang = khoaPhongItem.ToTalBhytTheoThang.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null &&  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        khoaPhongItem.ToTalDoanhThuTheoThang = khoaPhongItem.ToTalDoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                        khoaPhongItem.ToTalMienGiamTheoThang = khoaPhongItem.ToTalMienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ToTalChiPhiKhacTheoThang = khoaPhongItem.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ToTalBhytTheoThang = khoaPhongItem.ToTalBhytTheoThang.GetValueOrDefault() +
                                                           yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                    }
                }
                foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null &&  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                    if (khoaPhongItem != null)
                    {
                        khoaPhongItem.ToTalDoanhThuTheoThang = khoaPhongItem.ToTalDoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                        khoaPhongItem.ToTalMienGiamTheoThang = khoaPhongItem.ToTalMienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ToTalChiPhiKhacTheoThang = khoaPhongItem.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        khoaPhongItem.ToTalBhytTheoThang = khoaPhongItem.ToTalBhytTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                }

                foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    quayThuocData.ToTalDoanhThuTheoThang = quayThuocData.ToTalDoanhThuTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                    quayThuocData.ToTalMienGiamTheoThang = quayThuocData.ToTalMienGiamTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalChiPhiKhacTheoThang = quayThuocData.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalBhytTheoThang = quayThuocData.ToTalBhytTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                }

                foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    quayThuocData.ToTalDoanhThuTheoThang = quayThuocData.ToTalDoanhThuTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                    quayThuocData.ToTalMienGiamTheoThang = quayThuocData.ToTalMienGiamTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalChiPhiKhacTheoThang = quayThuocData.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalBhytTheoThang = quayThuocData.ToTalBhytTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                }

                foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc =>  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                {
                    quayThuocData.ToTalDoanhThuTheoThang = quayThuocData.ToTalDoanhThuTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    quayThuocData.ToTalMienGiamTheoThang = quayThuocData.ToTalMienGiamTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalChiPhiKhacTheoThang = quayThuocData.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalBhytTheoThang = quayThuocData.ToTalBhytTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                }

                foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                {
                    quayThuocData.ToTalDoanhThuTheoThang = quayThuocData.ToTalDoanhThuTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                    quayThuocData.ToTalMienGiamTheoThang = quayThuocData.ToTalMienGiamTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    quayThuocData.ToTalChiPhiKhacTheoThang = quayThuocData.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(x=>x.NoiThucHienId != null))
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //            khoaPhongItem.ToTalDoanhThuTheoThang = khoaPhongItem.ToTalDoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //            khoaPhongItem.ToTalMienGiamTheoThang = khoaPhongItem.ToTalMienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            khoaPhongItem.ToTalChiPhiKhacTheoThang = khoaPhongItem.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(x => x.NoiThucHienId != null))
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                //            khoaPhongItem.ToTalDoanhThuTheoThang = khoaPhongItem.ToTalDoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //            khoaPhongItem.ToTalMienGiamTheoThang = khoaPhongItem.ToTalMienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            khoaPhongItem.ToTalChiPhiKhacTheoThang = khoaPhongItem.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(x => x.NoiThucHienId != null))
                //    {
                //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                //        if (khoaPhongItem != null)
                //        {
                //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //            khoaPhongItem.ToTalDoanhThuTheoThang = khoaPhongItem.ToTalDoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //            khoaPhongItem.ToTalMienGiamTheoThang = khoaPhongItem.ToTalMienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            khoaPhongItem.ToTalChiPhiKhacTheoThang = khoaPhongItem.ToTalChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //}
            }

            if (queryInfo.KySoSanhTuNgay != null && queryInfo.KySoSanhDenNgay != null)
            {
                var ycTiepNhanKySoSanhQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                    o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                    queryInfo.KySoSanhTuNgay.Value <= o.ThoiDiemTiepNhan && o.ThoiDiemTiepNhan < queryInfo.KySoSanhDenNgay.Value);

                var ycTiepNhanKySoSanhData = await ycTiepNhanKySoSanhQuery
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //TODO: need update goi dv
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .ToListAsync();

                foreach (var yeuCauTiepNhan in ycTiepNhanKySoSanhData)
                {
                    foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            khoaPhongItem.ToTalDoanhThuTheoKySoSanh = khoaPhongItem.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                            khoaPhongItem.ToTalMienGiamTheoKySoSanh = khoaPhongItem.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh = khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ToTalBhytTheoKySoSanh = khoaPhongItem.ToTalBhytTheoKySoSanh.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }
                    foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null &&  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            khoaPhongItem.ToTalDoanhThuTheoKySoSanh = khoaPhongItem.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                            khoaPhongItem.ToTalMienGiamTheoKySoSanh = khoaPhongItem.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh = khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ToTalBhytTheoKySoSanh = khoaPhongItem.ToTalBhytTheoKySoSanh.GetValueOrDefault() +
                                                                  yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                        }
                    }
                    foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null &&  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                        if (khoaPhongItem != null)
                        {
                            khoaPhongItem.ToTalDoanhThuTheoKySoSanh = khoaPhongItem.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                            khoaPhongItem.ToTalMienGiamTheoKySoSanh = khoaPhongItem.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh = khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            khoaPhongItem.ToTalBhytTheoKySoSanh = khoaPhongItem.ToTalBhytTheoKySoSanh.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }

                    foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        quayThuocData.ToTalDoanhThuTheoKySoSanh = quayThuocData.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                        quayThuocData.ToTalMienGiamTheoKySoSanh = quayThuocData.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalChiPhiKhacTheoKySoSanh = quayThuocData.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalBhytTheoKySoSanh = quayThuocData.ToTalBhytTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                    }

                    foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        quayThuocData.ToTalDoanhThuTheoKySoSanh = quayThuocData.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                        quayThuocData.ToTalMienGiamTheoKySoSanh = quayThuocData.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalChiPhiKhacTheoKySoSanh = quayThuocData.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalBhytTheoKySoSanh = quayThuocData.ToTalBhytTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                    }

                    foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                    {
                        quayThuocData.ToTalDoanhThuTheoKySoSanh = quayThuocData.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                        quayThuocData.ToTalMienGiamTheoKySoSanh = quayThuocData.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalChiPhiKhacTheoKySoSanh = quayThuocData.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalBhytTheoKySoSanh = quayThuocData.ToTalBhytTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                    }

                    foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                    {
                        quayThuocData.ToTalDoanhThuTheoKySoSanh = quayThuocData.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                        quayThuocData.ToTalMienGiamTheoKySoSanh = quayThuocData.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        quayThuocData.ToTalChiPhiKhacTheoKySoSanh = quayThuocData.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    }
                    //TODO: need update goi dv
                    //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    //{
                    //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                    //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                    //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(x=>x.NoiThucHienId != null))
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauKhamBenh.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                    //            khoaPhongItem.ToTalDoanhThuTheoKySoSanh = khoaPhongItem.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //            khoaPhongItem.ToTalMienGiamTheoKySoSanh = khoaPhongItem.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //            khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh = khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(x => x.NoiThucHienId != null))
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuKyThuat.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                    //            khoaPhongItem.ToTalDoanhThuTheoKySoSanh = khoaPhongItem.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //            khoaPhongItem.ToTalMienGiamTheoKySoSanh = khoaPhongItem.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //            khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh = khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(x => x.NoiThucHienId != null))
                    //    {
                    //        var khoaPhongItem = khoaPhongData.FirstOrDefault(o => dsPhong.First(p => p.Id == yeuCauDichVuGiuong.NoiThucHienId).KhoaPhongId == o.Id);
                    //        if (khoaPhongItem != null)
                    //        {
                    //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                    //            khoaPhongItem.ToTalDoanhThuTheoKySoSanh = khoaPhongItem.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //            khoaPhongItem.ToTalMienGiamTheoKySoSanh = khoaPhongItem.ToTalMienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //            khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh = khoaPhongItem.ToTalChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //        }
                    //    }
                    //}
                }
            }

            khoaPhongData.Add(quayThuocData);
            var gridData = khoaPhongData
                .Where(o => !o.ToTalDoanhThuTheoThang.GetValueOrDefault().AlmostEqual(0) ||
                            !o.ToTalDoanhThuTheoKySoSanh.GetValueOrDefault().AlmostEqual(0))
                .ToList();
            return new GridDataSource { Data = gridData.Skip(queryInfo.LayTatCa ? 0 : queryInfo.Skip).Take(queryInfo.LayTatCa ? int.MaxValue : queryInfo.Take).ToArray(), TotalRowCount = gridData.Count };
        }
        //BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
        public async Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForDetailGridAsync(BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var dsDichVuTheoKhoa = new List<BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo>();
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.NoiThucHien)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.NoiThucHien)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.NoiThucHien)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.NoiThucHien)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.NoiThucHien)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.NoiThucHien)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    if (yeuCauKhamBenh.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                                Stt = dsDichVuTheoKhoa.Count+1,
                                DichVu = yeuCauKhamBenh.TenDichVu,
                                DoanhThuTheoThang = yeuCauKhamBenh.Gia,
                                MienGiamTheoThang = yeuCauKhamBenh.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauKhamBenh.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    if (yeuCauDichVuKyThuat.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                                Stt = dsDichVuTheoKhoa.Count + 1,
                                DichVu = yeuCauDichVuKyThuat.TenDichVu,
                                DoanhThuTheoThang = yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan,
                                MienGiamTheoThang = yeuCauDichVuKyThuat.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauDichVuKyThuat.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                        }
                    }
                }
                foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    if (yeuCauDichVuGiuong.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                                Stt = dsDichVuTheoKhoa.Count + 1,
                                DichVu = yeuCauDichVuGiuong.Ten,
                                DoanhThuTheoThang = yeuCauDichVuGiuong.Gia,
                                MienGiamTheoThang = yeuCauDichVuGiuong.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauDichVuGiuong.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.Gia;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() + yeuCauDichVuGiuong.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuGiuong.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuGiuong.MucHuongBaoHiem.GetValueOrDefault() / 100;
                        }
                    }
                }

                foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc =>  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    if (queryInfo.KhoaPhongId == 0)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                            o.Id == yeuCauDuocPham.DuocPhamBenhVienId && o.DichVu == yeuCauDuocPham.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = yeuCauDuocPham.DuocPhamBenhVienId,
                                Stt = dsDichVuTheoKhoa.Count + 1,
                                DichVu = yeuCauDuocPham.Ten,
                                DoanhThuTheoThang = yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong,
                                MienGiamTheoThang = yeuCauDuocPham.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauDuocPham.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() +
                                                       yeuCauDuocPham.MienGiamChiPhis.Where(mg =>
                                                               mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                           .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang =
                                dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() +
                                                   yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong;
                        }
                    }
                }

                foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    if (queryInfo.KhoaPhongId == 0)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                            o.Id == yeuCauVatTu.VatTuBenhVienId && o.DichVu == yeuCauVatTu.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = yeuCauVatTu.VatTuBenhVienId,
                                Stt = dsDichVuTheoKhoa.Count + 1,
                                DichVu = yeuCauVatTu.Ten,
                                DoanhThuTheoThang = yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong,
                                MienGiamTheoThang = yeuCauVatTu.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = yeuCauVatTu.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() +
                                                       yeuCauVatTu.MienGiamChiPhis.Where(mg =>
                                                               mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                           .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang =
                                dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() +
                                                   yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                        }
                    }
                }

                foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                {
                    if (queryInfo.KhoaPhongId == 0)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                            o.Id == donThuocThanhToanChiTiet.DuocPhamId && o.DichVu == donThuocThanhToanChiTiet.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = donThuocThanhToanChiTiet.DuocPhamId,
                                Stt = dsDichVuTheoKhoa.Count + 1,
                                DichVu = donThuocThanhToanChiTiet.Ten,
                                DoanhThuTheoThang = donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong,
                                MienGiamTheoThang = donThuocThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = donThuocThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang =
                                dichVu.DoanhThuTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() +
                                                       donThuocThanhToanChiTiet.MienGiamChiPhis
                                                           .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                           .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang =
                                dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + donThuocThanhToanChiTiet
                                    .MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher)
                                    .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.BhytTheoThang = dichVu.BhytTheoThang.GetValueOrDefault() +
                                                   donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                        }
                    }
                }

                foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                {
                    if (queryInfo.KhoaPhongId == 0)
                    {
                        var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                            o.Id == donVTYTThanhToanChiTiet.VatTuBenhVienId && o.DichVu == donVTYTThanhToanChiTiet.Ten);
                        if (dichVu == null)
                        {
                            dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                            {
                                Id = donVTYTThanhToanChiTiet.VatTuBenhVienId,
                                Stt = dsDichVuTheoKhoa.Count + 1,
                                DichVu = donVTYTThanhToanChiTiet.Ten,
                                DoanhThuTheoThang = donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong,
                                MienGiamTheoThang = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                ChiPhiKhacTheoThang = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                    .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                BhytTheoThang = 0
                            };
                            dsDichVuTheoKhoa.Add(dichVu);
                        }
                        else
                        {
                            dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                            dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() +
                                                       donVTYTThanhToanChiTiet.MienGiamChiPhis
                                                           .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                           .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            dichVu.ChiPhiKhacTheoThang =
                                dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + donVTYTThanhToanChiTiet
                                    .MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher)
                                    .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        }
                    }
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc =>yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(x=>x.NoiThucHienId != null))
                //    {
                //        if (yeuCauKhamBenh.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                //        {
                //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                //            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                //            if (dichVu == null)
                //            {
                //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                //                {
                //                    Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                //                    Stt = dsDichVuTheoKhoa.Count + 1,
                //                    DichVu = yeuCauKhamBenh.TenDichVu,
                //                    DoanhThuTheoThang = doanhThuDv,
                //                    MienGiamTheoThang = tiLeMienGiam * doanhThuDv,
                //                    ChiPhiKhacTheoThang = tiLeGiamTruKhac * doanhThuDv,
                //                    BhytTheoThang = 0
                //                };
                //                dsDichVuTheoKhoa.Add(dichVu);
                //            }
                //            else
                //            {
                //                dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //                dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //                dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //            }
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(x => x.NoiThucHienId != null))
                //    {
                //        if (yeuCauDichVuKyThuat.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                //        {
                //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;

                //            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                //            if (dichVu == null)
                //            {
                //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                //                {
                //                    Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                //                    Stt = dsDichVuTheoKhoa.Count + 1,
                //                    DichVu = yeuCauDichVuKyThuat.TenDichVu,
                //                    DoanhThuTheoThang = doanhThuDv,
                //                    MienGiamTheoThang = tiLeMienGiam * doanhThuDv,
                //                    ChiPhiKhacTheoThang = tiLeGiamTruKhac * doanhThuDv,
                //                    BhytTheoThang = 0
                //                };
                //                dsDichVuTheoKhoa.Add(dichVu);
                //            }
                //            else
                //            {
                //                dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //                dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //                dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //            }
                //        }
                //    }
                //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(x => x.NoiThucHienId != null))
                //    {
                //        if (yeuCauDichVuGiuong.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                //        {
                //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                //            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                //            if (dichVu == null)
                //            {
                //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                //                {
                //                    Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                //                    Stt = dsDichVuTheoKhoa.Count + 1,
                //                    DichVu = yeuCauDichVuGiuong.Ten,
                //                    DoanhThuTheoThang = doanhThuDv,
                //                    MienGiamTheoThang = tiLeMienGiam * doanhThuDv,
                //                    ChiPhiKhacTheoThang = tiLeGiamTruKhac * doanhThuDv,
                //                    BhytTheoThang = 0
                //                };
                //                dsDichVuTheoKhoa.Add(dichVu);
                //            }
                //            else
                //            {
                //                dichVu.DoanhThuTheoThang = dichVu.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //                dichVu.MienGiamTheoThang = dichVu.MienGiamTheoThang.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //                dichVu.ChiPhiKhacTheoThang = dichVu.ChiPhiKhacTheoThang.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //            }
                //        }
                //    }
                //}
            }

            if (queryInfo.KySoSanhTuNgay != null && queryInfo.KySoSanhDenNgay != null)
            {
                var ycTiepNhanKySoSanhQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                    o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                    queryInfo.KySoSanhTuNgay.Value <= o.ThoiDiemTiepNhan && o.ThoiDiemTiepNhan < queryInfo.KySoSanhDenNgay.Value);

                var ycTiepNhanKySoSanhData = await ycTiepNhanKySoSanhQuery
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.NoiThucHien).ThenInclude(yc=>yc.KhoaPhong)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.NoiThucHien).ThenInclude(yc => yc.KhoaPhong)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.NoiThucHien).ThenInclude(yc => yc.KhoaPhong)
                    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //TODO: need update goi dv
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                    .ToListAsync();

                foreach (var yeuCauTiepNhan in ycTiepNhanKySoSanhData)
                {
                    foreach (var yckb in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        if (yckb.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yckb.DichVuKhamBenhBenhVienId && o.DichVu == yckb.TenDichVu);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = yckb.DichVuKhamBenhBenhVienId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = yckb.TenDichVu,
                                    DoanhThuTheoKySoSanh = yckb.Gia,
                                    MienGiamTheoKySoSanh = yckb.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = yckb.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = yckb.DonGiaBaoHiem.GetValueOrDefault() * yckb.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yckb.MucHuongBaoHiem.GetValueOrDefault() / 100
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yckb.Gia;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + yckb.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yckb.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() +
                                                          yckb.DonGiaBaoHiem.GetValueOrDefault() *
                                                          yckb.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yckb.MucHuongBaoHiem.GetValueOrDefault() /100;
                            }
                        }
                    }
                    foreach (var ycdvkt in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.NoiThucHienId != null && yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                    {
                        if (ycdvkt.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == ycdvkt.DichVuKyThuatBenhVienId && o.DichVu == ycdvkt.TenDichVu);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = ycdvkt.DichVuKyThuatBenhVienId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = ycdvkt.TenDichVu,
                                    DoanhThuTheoKySoSanh = ycdvkt.Gia * ycdvkt.SoLan,
                                    MienGiamTheoKySoSanh = ycdvkt.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = ycdvkt.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = ycdvkt.DonGiaBaoHiem.GetValueOrDefault() * ycdvkt.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * ycdvkt.MucHuongBaoHiem.GetValueOrDefault() / 100 * ycdvkt.SoLan
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + ycdvkt.Gia * ycdvkt.SoLan;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + ycdvkt.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + ycdvkt.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + ycdvkt.DonGiaBaoHiem.GetValueOrDefault() * ycdvkt.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * ycdvkt.MucHuongBaoHiem.GetValueOrDefault() / 100 * ycdvkt.SoLan;
                            }
                        }
                    }
                    foreach (var ycdvg in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(yc => yc.NoiThucHienId != null &&  yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        if (ycdvg.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == ycdvg.DichVuGiuongBenhVienId && o.DichVu == ycdvg.Ten);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = ycdvg.DichVuGiuongBenhVienId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = ycdvg.Ten,
                                    DoanhThuTheoKySoSanh = ycdvg.Gia,
                                    MienGiamTheoKySoSanh = ycdvg.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = ycdvg.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = ycdvg.DonGiaBaoHiem.GetValueOrDefault() * ycdvg.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * ycdvg.MucHuongBaoHiem.GetValueOrDefault() / 100
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + ycdvg.Gia;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + ycdvg.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + ycdvg.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() + ycdvg.DonGiaBaoHiem.GetValueOrDefault() * ycdvg.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * ycdvg.MucHuongBaoHiem.GetValueOrDefault() / 100;
                            }
                        }
                    }

                    foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        if (queryInfo.KhoaPhongId == 0)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                                o.Id == yeuCauDuocPham.DuocPhamBenhVienId && o.DichVu == yeuCauDuocPham.Ten);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = yeuCauDuocPham.DuocPhamBenhVienId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = yeuCauDuocPham.Ten,
                                    DoanhThuTheoKySoSanh = yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong,
                                    MienGiamTheoKySoSanh = yeuCauDuocPham.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = yeuCauDuocPham.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauDuocPham.SoLuong
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.DonGiaBan * (decimal)yeuCauDuocPham.SoLuong;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() +
                                                           yeuCauDuocPham.MienGiamChiPhis.Where(mg =>
                                                                   mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                               .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh =
                                    dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauDuocPham.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() +
                                                          yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault() /100 * (decimal)yeuCauDuocPham.SoLuong;
                            }
                        }
                    }

                    foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                    {
                        if (queryInfo.KhoaPhongId == 0)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                                o.Id == yeuCauVatTu.VatTuBenhVienId && o.DichVu == yeuCauVatTu.Ten);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = yeuCauVatTu.VatTuBenhVienId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = yeuCauVatTu.Ten,
                                    DoanhThuTheoKySoSanh = yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong,
                                    MienGiamTheoKySoSanh = yeuCauVatTu.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = yeuCauVatTu.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.DonGiaBan * (decimal)yeuCauVatTu.SoLuong;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() +
                                                              yeuCauVatTu.MienGiamChiPhis.Where(mg =>
                                                                   mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                               .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh =
                                    dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + yeuCauVatTu.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() +
                                                          yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault() * yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)yeuCauVatTu.SoLuong;
                            }
                        }
                    }

                    foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
                    {
                        if (queryInfo.KhoaPhongId == 0)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                                o.Id == donThuocThanhToanChiTiet.DuocPhamId && o.DichVu == donThuocThanhToanChiTiet.Ten);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = donThuocThanhToanChiTiet.DuocPhamId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = donThuocThanhToanChiTiet.Ten,
                                    DoanhThuTheoKySoSanh = donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong,
                                    MienGiamTheoKySoSanh = donThuocThanhToanChiTiet.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = donThuocThanhToanChiTiet.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh =
                                    dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet.DonGiaBan * (decimal)donThuocThanhToanChiTiet.SoLuong;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() +
                                                           donThuocThanhToanChiTiet.MienGiamChiPhis
                                                               .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                               .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh =
                                    dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donThuocThanhToanChiTiet
                                        .MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher)
                                        .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.BhytTheoKySoSanh = dichVu.BhytTheoKySoSanh.GetValueOrDefault() +
                                                          donThuocThanhToanChiTiet.DonGiaBaoHiem.GetValueOrDefault() * donThuocThanhToanChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * donThuocThanhToanChiTiet.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)donThuocThanhToanChiTiet.SoLuong;
                            }
                        }
                    }

                    foreach (var donVTYTThanhToanChiTiet in yeuCauTiepNhan.DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
                    {
                        if (queryInfo.KhoaPhongId == 0)
                        {
                            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o =>
                                o.Id == donVTYTThanhToanChiTiet.VatTuBenhVienId && o.DichVu == donVTYTThanhToanChiTiet.Ten);
                            if (dichVu == null)
                            {
                                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                                {
                                    Id = donVTYTThanhToanChiTiet.VatTuBenhVienId,
                                    Stt = dsDichVuTheoKhoa.Count + 1,
                                    DichVu = donVTYTThanhToanChiTiet.Ten,
                                    DoanhThuTheoKySoSanh = donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong,
                                    MienGiamTheoKySoSanh = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    ChiPhiKhacTheoKySoSanh = donVTYTThanhToanChiTiet.MienGiamChiPhis
                                        .Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                                    BhytTheoKySoSanh = 0
                                };
                                dsDichVuTheoKhoa.Add(dichVu);
                            }
                            else
                            {
                                dichVu.DoanhThuTheoKySoSanh =
                                    dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet.DonGiaBan * (decimal)donVTYTThanhToanChiTiet.SoLuong;
                                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() +
                                                              donVTYTThanhToanChiTiet.MienGiamChiPhis
                                                               .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                               .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                                dichVu.ChiPhiKhacTheoKySoSanh =
                                    dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + donVTYTThanhToanChiTiet
                                        .MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher)
                                        .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                            }
                        }
                    }
                    //TODO: need update goi dv
                    //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    //{
                    //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                    //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                    //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(x=>x.NoiThucHienId != null))
                    //    {
                    //        if (yeuCauKhamBenh.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                    //        {
                    //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                    //            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId && o.DichVu == yeuCauKhamBenh.TenDichVu);
                    //            if (dichVu == null)
                    //            {
                    //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                    //                {
                    //                    Id = yeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                    //                    Stt = dsDichVuTheoKhoa.Count + 1,
                    //                    DichVu = yeuCauKhamBenh.TenDichVu,
                    //                    DoanhThuTheoKySoSanh = doanhThuDv,
                    //                    MienGiamTheoKySoSanh = tiLeMienGiam * doanhThuDv,
                    //                    ChiPhiKhacTheoKySoSanh = tiLeGiamTruKhac * doanhThuDv,
                    //                    BhytTheoKySoSanh = 0
                    //                };
                    //                dsDichVuTheoKhoa.Add(dichVu);
                    //            }
                    //            else
                    //            {
                    //                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //            }
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(x => x.NoiThucHienId != null))
                    //    {
                    //        if (yeuCauDichVuKyThuat.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                    //        {
                    //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;

                    //            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVu == yeuCauDichVuKyThuat.TenDichVu);
                    //            if (dichVu == null)
                    //            {
                    //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                    //                {
                    //                    Id = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                    //                    Stt = dsDichVuTheoKhoa.Count + 1,
                    //                    DichVu = yeuCauDichVuKyThuat.TenDichVu,
                    //                    DoanhThuTheoKySoSanh = doanhThuDv,
                    //                    MienGiamTheoKySoSanh = tiLeMienGiam * doanhThuDv,
                    //                    ChiPhiKhacTheoKySoSanh = tiLeGiamTruKhac * doanhThuDv,
                    //                    BhytTheoKySoSanh = 0
                    //                };
                    //                dsDichVuTheoKhoa.Add(dichVu);
                    //            }
                    //            else
                    //            {
                    //                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //            }
                    //        }
                    //    }
                    //    foreach (var yeuCauDichVuGiuong in yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(x => x.NoiThucHienId != null))
                    //    {
                    //        if (yeuCauDichVuGiuong.NoiThucHien.KhoaPhongId == queryInfo.KhoaPhongId)
                    //        {
                    //            var doanhThuDv = yeuCauDichVuGiuong.Gia - (yeuCauDichVuGiuong.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);

                    //            var dichVu = dsDichVuTheoKhoa.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.DichVuGiuongBenhVienId && o.DichVu == yeuCauDichVuGiuong.Ten);
                    //            if (dichVu == null)
                    //            {
                    //                dichVu = new BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
                    //                {
                    //                    Id = yeuCauDichVuGiuong.DichVuGiuongBenhVienId,
                    //                    Stt = dsDichVuTheoKhoa.Count + 1,
                    //                    DichVu = yeuCauDichVuGiuong.Ten,
                    //                    DoanhThuTheoKySoSanh = doanhThuDv,
                    //                    MienGiamTheoKySoSanh = tiLeMienGiam * doanhThuDv,
                    //                    ChiPhiKhacTheoKySoSanh = tiLeGiamTruKhac * doanhThuDv,
                    //                    BhytTheoKySoSanh = 0
                    //                };
                    //                dsDichVuTheoKhoa.Add(dichVu);
                    //            }
                    //            else
                    //            {
                    //                dichVu.DoanhThuTheoKySoSanh = dichVu.DoanhThuTheoKySoSanh.GetValueOrDefault() + doanhThuDv;
                    //                dichVu.MienGiamTheoKySoSanh = dichVu.MienGiamTheoKySoSanh.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                    //                dichVu.ChiPhiKhacTheoKySoSanh = dichVu.ChiPhiKhacTheoKySoSanh.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            return new GridDataSource { Data = dsDichVuTheoKhoa.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dsDichVuTheoKhoa.Count };
        }
    }
}
