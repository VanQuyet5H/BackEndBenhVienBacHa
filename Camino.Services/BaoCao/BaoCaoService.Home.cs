using Camino.Core.Domain.ValueObject.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<ThongKeBenhVien> GetThongKeBenhVienAsync(ThongKeKhamBenhSearch queryInfo)
        {
            var fromDate = queryInfo.TuNgay ?? DateTime.Now.Date;
            var toDate = queryInfo.DenNgay ?? DateTime.Now;
            var dataTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.ThoiDiemTiepNhan >= fromDate && o.ThoiDiemTiepNhan < toDate &&
                            (o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru || o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe) && o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .Select(o => new DataTiepNhan { 
                    Id = o.Id, 
                    CoBHYT = o.CoBHYT != null && o.CoBHYT == true,
                    TrangThaiYeuCauKhamBenhs= o.YeuCauKhamBenhs.Select(k=>k.TrangThai).ToList(),
                    CoKSK = o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                }).ToList();

            var dataKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.ThoiDiemDangKy >= fromDate && o.ThoiDiemDangKy < toDate &&
                            o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(o => new DataKhamBenh
                {
                    Id = o.Id,
                    TrangThai = o.TrangThai,
                    DichVuKhamBenhBenhVienId = o.DichVuKhamBenhBenhVienId,
                    TenDichVu = o.TenDichVu
                }).ToList();

            var dataGiuongBenh = _giuongBenhRepository.TableNoTracking
                .Where(o => o.IsDisabled != true)
                .Select(o => new DataGiuongBenh
                {
                    Id = o.Id,
                    CoSuDung = o.HoatDongGiuongBenhs.Any(h=>(h.ThoiDiemBatDau>=fromDate && h.ThoiDiemBatDau<toDate) || (h.ThoiDiemBatDau < fromDate && (h.ThoiDiemKetThuc == null || h.ThoiDiemKetThuc >= fromDate))),
                    KhoaPhongId = o.PhongBenhVien.KhoaPhongId,
                    TenKhoaPhong = o.PhongBenhVien.KhoaPhong.Ten,
                }).ToList();

            var dataNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            (o.YeuCauNhapVien != null && o.YeuCauNhapVien.YeuCauTiepNhanMeId == null) &&
                            o.NoiTruBenhAn != null && ((o.NoiTruBenhAn.ThoiDiemNhapVien >= fromDate && o.NoiTruBenhAn.ThoiDiemNhapVien < toDate) || (o.NoiTruBenhAn.ThoiDiemNhapVien < fromDate && (o.NoiTruBenhAn.ThoiDiemRaVien == null || o.NoiTruBenhAn.ThoiDiemRaVien >= fromDate))))
                .Select(o => new DataNoiTru
                {
                    Id = o.Id,
                    ThoiDiemNhapVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                    ThoiDiemRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
                    HinhThucRaVien = o.NoiTruBenhAn.HinhThucRaVien,
                    KhoaPhongDieuTris = o.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Select(k=>new DataNoiTruKhoaPhongDieuTri
                    {
                        KhoaPhongId = k.KhoaPhongChuyenDenId,
                        ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa,
                        TenKhoaPhong = k.KhoaPhongChuyenDen.Ten
                    }).ToList()
                }).ToList();

            var dataSoSinh = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            (o.YeuCauNhapVien != null && o.YeuCauNhapVien.YeuCauTiepNhanMeId != null) &&
                            o.NoiTruBenhAn != null && ((o.NoiTruBenhAn.ThoiDiemNhapVien >= fromDate && o.NoiTruBenhAn.ThoiDiemNhapVien < toDate) || (o.NoiTruBenhAn.ThoiDiemNhapVien < fromDate && (o.NoiTruBenhAn.ThoiDiemRaVien == null || o.NoiTruBenhAn.ThoiDiemRaVien >= fromDate))))
                .Select(o => new DataSoSinh
                {
                    Id = o.Id,
                    ThoiDiemNhapVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                    ThoiDiemRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
                    HinhThucRaVien = o.NoiTruBenhAn.HinhThucRaVien,
                    KhoaPhongDieuTris = o.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Select(k => new DataNoiTruKhoaPhongDieuTri
                    {
                        KhoaPhongId = k.KhoaPhongChuyenDenId,
                        ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa,
                        TenKhoaPhong = k.KhoaPhongChuyenDen.Ten
                    }).ToList(),
                    ThoiGianDieuTriBenhAnSoSinhs = o.NoiTruBenhAn.NoiTruThoiGianDieuTriBenhAnSoSinhs.Select(k => new DataThoiGianDieuTriBenhAnSoSinh
                    {
                        NgayDieuTri = k.NgayDieuTri,
                        GioBatDau = k.GioBatDau,
                        GioKetThuc = k.GioKetThuc
                    }).ToList()
                }).ToList();

            var thongKeBenhVien = new ThongKeBenhVien();
            thongKeBenhVien.ThongKeTiepNhan = new ThongKeTiepNhan
            {
                TongSoTiepNhan = dataTiepNhan.Count,
                TongSoKSK = dataTiepNhan.Count(o=>o.CoKSK),
                TongSoDichVu = dataTiepNhan.Count(o => !o.CoKSK && !o.CoKhamBenh),
                TongSoBHYT = dataTiepNhan.Count(o => !o.CoKSK && o.CoKhamBenh && o.CoBHYT),
                TongSoVienPhi = dataTiepNhan.Count(o => !o.CoKSK && o.CoKhamBenh && !o.CoBHYT),
            };

            thongKeBenhVien.ThongKeKhamBenh = new ThongKeKhamBenh
            {
                TongSoNguoiKhamBenh = dataKhamBenh.Count,
                TongSoChoKham = dataKhamBenh.Count(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham),
                TongSoHoanThanh = dataKhamBenh.Count(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
            };
            thongKeBenhVien.ThongKeKhamBenh.TongSoDangKham = thongKeBenhVien.ThongKeKhamBenh.TongSoNguoiKhamBenh -
                                                             thongKeBenhVien.ThongKeKhamBenh.TongSoChoKham -
                                                             thongKeBenhVien.ThongKeKhamBenh.TongSoHoanThanh;

            thongKeBenhVien.ThongKeGiuongBenh = new ThongKeGiuongBenh
            {
                TongSoGiuongBenh = dataGiuongBenh.Count,
                TongSoDaDung = dataGiuongBenh.Count(o=> o.CoSuDung),
            };
            thongKeBenhVien.ThongKeGiuongBenh.TongSoTrong = thongKeBenhVien.ThongKeGiuongBenh.TongSoGiuongBenh -
                                                            thongKeBenhVien.ThongKeGiuongBenh.TongSoDaDung;

            thongKeBenhVien.ThongKeNoiTru = new ThongKeNoiTru
            {
                TongSoNhapVien = dataNoiTru.Count(o => o.ThoiDiemNhapVien >= fromDate && o.ThoiDiemNhapVien < toDate),
                TongSoDieuTri = dataNoiTru.Count(o => o.ThoiDiemRaVien > toDate || o.ThoiDiemRaVien == null),
                TongSoTuVong = dataNoiTru.Count(o => o.ThoiDiemRaVien != null &&
                                                     o.ThoiDiemRaVien <= toDate && (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong ||
                                                                                    o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H)),
                TongChuyenVien = dataNoiTru.Count(o => o.ThoiDiemRaVien != null &&
                                                       o.ThoiDiemRaVien <= toDate && o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien),
                TongSoRaVien = dataNoiTru.Count(o => o.ThoiDiemRaVien != null &&
                                                     o.ThoiDiemRaVien <= toDate && o.HinhThucRaVien != EnumHinhThucRaVien.ChuyenVien &&
                                                     o.HinhThucRaVien != EnumHinhThucRaVien.TuVong &&
                                                     o.HinhThucRaVien != EnumHinhThucRaVien.TuVongTruoc24H),
            };

            thongKeBenhVien.ThongKeSoSinh = new ThongKeSoSinh
            {
                TongSoSSThuong = dataSoSinh.Count(o => !o.ThoiGianDieuTriBenhAnSoSinhs.Any() && (o.ThoiDiemRaVien > toDate || o.ThoiDiemRaVien == null)),
                TongSoSSBenh = dataSoSinh.Count(o => o.ThoiGianDieuTriBenhAnSoSinhs.Any() && (o.ThoiDiemRaVien > toDate || o.ThoiDiemRaVien == null)),
                TongSoTuVong = dataSoSinh.Count(o => o.ThoiDiemRaVien != null &&
                                                     o.ThoiDiemRaVien <= toDate && (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong ||
                                                                                    o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H)),
                TongSoChuyenVien = dataSoSinh.Count(o => o.ThoiDiemRaVien != null &&
                                                       o.ThoiDiemRaVien <= toDate && o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien),
                TongSoRaVien = dataSoSinh.Count(o => o.ThoiDiemRaVien != null &&
                                                     o.ThoiDiemRaVien <= toDate && o.HinhThucRaVien != EnumHinhThucRaVien.ChuyenVien &&
                                                     o.HinhThucRaVien != EnumHinhThucRaVien.TuVong &&
                                                     o.HinhThucRaVien != EnumHinhThucRaVien.TuVongTruoc24H),
            };

            foreach (var groupKhamBenh in dataKhamBenh.GroupBy(o=>o.DichVuKhamBenhBenhVienId).OrderBy(o=>o.Key))
            {
                var chartTinhTrangKham = new ChartTinhTrangKham
                {
                    KhoaKham = groupKhamBenh.First().TenDichVu,
                    TongSoNguoiChuaKham = groupKhamBenh.Count(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham),
                    TongSoNguoiHoanThanh = groupKhamBenh.Count(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham),
                    TongSoNguoiDangKham = groupKhamBenh.Count(o => o.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
                };
                thongKeBenhVien.ChartTinhTrangKhams.Add(chartTinhTrangKham);
            }

            foreach (var groupGiuongBenh in dataGiuongBenh.GroupBy(o => o.KhoaPhongId).OrderBy(o => o.Key))
            {
                var chartTinhTrangSuDungGiuong = new ChartTinhTrangSuDungGiuong
                {
                    KhoaKham = groupGiuongBenh.First().TenKhoaPhong,
                    TongSoGiuongDaSuDung = groupGiuongBenh.Count(o => o.CoSuDung),
                    TongSoGiuongTrong = groupGiuongBenh.Count(o => !o.CoSuDung),
                };
                thongKeBenhVien.ChartTinhTrangSuDungGiuongs.Add(chartTinhTrangSuDungGiuong);
            }

            foreach (var itemNoiTru in dataNoiTru)
            {
                var khoaPhongDangDieuTri = itemNoiTru.KhoaPhongDieuTris.Where(o => o.ThoiDiemVaoKhoa < toDate).OrderBy(o => o.ThoiDiemVaoKhoa).LastOrDefault();
                itemNoiTru.DataNoiTruKhoaPhongDangDieuTri = khoaPhongDangDieuTri ?? itemNoiTru.KhoaPhongDieuTris.First();
            }

            foreach (var groupNoiTru in dataNoiTru.GroupBy(o => o.DataNoiTruKhoaPhongDangDieuTri.KhoaPhongId).OrderBy(o => o.Key))
            {
                var chartTinhTrangDieuTriNoiTru = new ChartTinhTrangDieuTriNoiTru
                {
                    KhoaKham = groupNoiTru.First().DataNoiTruKhoaPhongDangDieuTri.TenKhoaPhong,
                    TongSoNhapVien = groupNoiTru.Count(o => o.ThoiDiemNhapVien >= fromDate && o.ThoiDiemNhapVien < toDate),
                    TongSoDangDieuTri = groupNoiTru.Count(o => o.ThoiDiemRaVien > toDate || o.ThoiDiemRaVien == null),
                    TongSoTuVong = groupNoiTru.Count(o => o.ThoiDiemRaVien != null &&
                                                          o.ThoiDiemRaVien <= toDate && (o.HinhThucRaVien == EnumHinhThucRaVien.TuVong ||
                                                                                         o.HinhThucRaVien == EnumHinhThucRaVien.TuVongTruoc24H)),
                    TongSoChuyenVien = groupNoiTru.Count(o => o.ThoiDiemRaVien != null &&
                                                              o.ThoiDiemRaVien <= toDate && o.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien),
                    TongSoDaRaVien = groupNoiTru.Count(o => o.ThoiDiemRaVien != null &&
                                                            o.ThoiDiemRaVien <= toDate && o.HinhThucRaVien != EnumHinhThucRaVien.ChuyenVien &&
                                                            o.HinhThucRaVien != EnumHinhThucRaVien.TuVong &&
                                                            o.HinhThucRaVien != EnumHinhThucRaVien.TuVongTruoc24H),
                };
                thongKeBenhVien.ChartTinhTrangDieuTriNoiTrus.Add(chartTinhTrangDieuTriNoiTru);
            }

            return thongKeBenhVien;
        }
    }
}
