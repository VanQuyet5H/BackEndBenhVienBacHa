using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial class YeuCauTiepNhanService
    {
        public GridDataSource TimKiemTiepNhan(TimKiemQueryInfo queryInfo)
        {
            var nhanVienLogin = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaNhanNhanVien = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == nhanVienLogin).Select(p => p.KhoaPhongId).FirstOrDefault();


            if (queryInfo == null || string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                return new GridDataSource();
            }
            var today = DateTime.Now;

            var yeuCauTiepNhanNoiTrus = BaseRepository.TableNoTracking
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
            x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru &&
            (x.MaYeuCauTiepNhan == queryInfo.SearchTerms || x.BenhNhan.MaBN == queryInfo.SearchTerms || x.HoTen.ToUpper() == queryInfo.SearchTerms.ToUpper()))
                .Select(x => new TimKiemTiepNhanGirdVo()
                {
                    Id = x.Id,
                    MaTN = x.MaYeuCauTiepNhan,
                    MaBN = x.BenhNhan.MaBN,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    ThangSinh = x.ThangSinh,
                    NamSinh = x.NamSinh,
                    GioiTinh = x.GioiTinh,
                    SoDienThoai = x.SoDienThoai,
                    DiaChi = x.DiaChiDayDu,
                    NgayTiepNhan = x.ThoiDiemTiepNhan.ApplyFormatDateTime(),
                    NgayTiepNhanDT = x.ThoiDiemTiepNhan,
                    LoaiYeuCauTiepNhan = "Nội trú",
                    TinhTrang = x.NoiTruBenhAn != null ? (x.NoiTruBenhAn.DaQuyetToan == null || x.NoiTruBenhAn.DaQuyetToan == false) ? Enums.EnumTrangThaiDieuTriNoiTru.DangDieuTri.GetDescription()
                    : (x.NoiTruBenhAn.HinhThucRaVien == Enums.EnumHinhThucRaVien.ChuyenVien ? Enums.EnumTrangThaiDieuTriNoiTru.ChuyenVien.GetDescription() : Enums.EnumTrangThaiDieuTriNoiTru.DaRaVien.GetDescription()) : "Đang chờ tạo BA",
                    ChuyenKhoa =x.NoiTruBenhAn != null ? !(x.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.
                                       Any(co => co.ThoiDiemRaKhoa == null && co.KhoaPhongChuyenDenId == khoaNhanNhanVien)) : false,
                    Url = x.NoiTruBenhAn != null ? ("/dieu-tri-noi-tru/chi-tiet-dieu-tri/" + x.Id) : "/noi-tru/tiep-nhan"

                })
                .ToList();

            var yeuCauTiepNhanNgoaiTrus = BaseRepository.TableNoTracking
                .Where(x =>
            x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
            yeuCauTiepNhanNoiTrus.All(o => o.MaTN != x.MaYeuCauTiepNhan) &&
            (x.MaYeuCauTiepNhan == queryInfo.SearchTerms || x.BenhNhan.MaBN == queryInfo.SearchTerms || x.HoTen.ToUpper() == queryInfo.SearchTerms.ToUpper()))
                .Select(x => new TimKiemTiepNhanGirdVo()
                {
                    Id = x.Id,
                    MaTN = x.MaYeuCauTiepNhan,
                    MaBN = x.BenhNhan.MaBN,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    ThangSinh = x.ThangSinh,
                    NamSinh = x.NamSinh,
                    GioiTinh = x.GioiTinh,
                    SoDienThoai = x.SoDienThoai,
                    DiaChi = x.DiaChiDayDu,
                    NgayTiepNhan = x.ThoiDiemTiepNhan.ApplyFormatDateTime(),
                    NgayTiepNhanDT = x.ThoiDiemTiepNhan,
                    LoaiYeuCauTiepNhan = "Ngoại trú",
                    TinhTrang = x.TrangThaiYeuCauTiepNhan.GetDescription(),
                    ChuyenKhoa = false,
                    Url = x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat || x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ? "/lich-su-tiep-nhan/chi-tiet/" + x.Id : "/danh-sach-tiep-nhan/chinh-sua/" + x.Id
                })
                .ToList();
            var yeuCauTiepNhans = new List<TimKiemTiepNhanGirdVo>();
            if (yeuCauTiepNhanNoiTrus.Any())
            {
                foreach (var item in yeuCauTiepNhanNoiTrus)
                {
                    var result = BaseRepository.GetById(item.Id, s =>
                   s.Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                   .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
                   .Include(x => x.YeuCauTiepNhanTheBHYTs)
                   );
                    if (result != null)
                    {
                        item.MucHuong = result.YeuCauTiepNhanTheBHYTs.Any() ? result.YeuCauTiepNhanTheBHYTs.OrderByDescending(a => a.MucHuong).Select(a => a.MucHuong).FirstOrDefault() : 0;
                        item.KhoaDieuTri = result.NoiTruBenhAn != null && result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any() ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.LastOrDefault(p => DateTime.Now >= p.ThoiDiemVaoKhoa)?.KhoaPhongChuyenDen?.Ten : "";
                        item.PhongDieuTri = (result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan && p.ThoiDiemBatDauSuDung <= today && (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today)) ? result.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
                                                                                                                                                                               p.ThoiDiemBatDauSuDung <= today &&
                                                                                                                                                                               (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))?
                                                                                                                                                           .GiuongBenh?.PhongBenhVien?.Ten : "");
                    }
                }
                yeuCauTiepNhans.AddRange(yeuCauTiepNhanNoiTrus);
            }
            if (yeuCauTiepNhanNgoaiTrus.Any())
            {
                foreach (var item in yeuCauTiepNhanNgoaiTrus)
                {
                    var result = BaseRepository.GetById(item.Id, s =>
                   s.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiThucHien).ThenInclude(x => x.KhoaPhong)
                   .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiDangKy).ThenInclude(x => x.KhoaPhong)
                   );
                    if (result != null)
                    {
                        item.MucHuong = result.BHYTMucHuong ?? 0;
                        item.KhoaDieuTri = (result.YeuCauKhamBenhs.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ? (result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiThucHien != null && result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiThucHien.KhoaPhong != null ? result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiThucHien.KhoaPhong.Ten : (result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiDangKy != null && result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiDangKy.KhoaPhong != null ? result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiDangKy.KhoaPhong.Ten : "")) : "");
                        item.PhongDieuTri = (result.YeuCauKhamBenhs.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) ? (result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiThucHien != null ? result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiThucHien.Ten : (result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiDangKy != null ? result.YeuCauKhamBenhs.First(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).NoiDangKy.Ten : "")) : "");
                    }
                }
                yeuCauTiepNhans.AddRange(yeuCauTiepNhanNgoaiTrus);
            }

            var query = yeuCauTiepNhans.AsQueryable();           
            var queryResult = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryResult, TotalRowCount = queryResult.Length };

        }
    }
}
