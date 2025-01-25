using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using Camino.Data;
using Newtonsoft.Json;
using Camino.Services.BaoCaoVatTus;
using Camino.Core.Domain.ValueObject.ThongKeDanhSachKhamDoan;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Camino.Services.ThongKeDanhSachKhamDoan
{
    [ScopedDependency(ServiceType = typeof(IThongKeDanhSachKhamDoan))]
    public class ThongKeDanhSachKhamDoanService : MasterFileService<HopDongKhamSucKhoeNhanVien>, IThongKeDanhSachKhamDoan
    {
        public IRepository<Core.Domain.Entities.KhamDoans.CongTyKhamSucKhoe> _congTyKhamSucKhoeRepository;
        public IRepository<Core.Domain.Entities.KhamDoans.HopDongKhamSucKhoe> _hopDongKhamSucKhoeRepository;

        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;


        public ThongKeDanhSachKhamDoanService(IRepository<HopDongKhamSucKhoeNhanVien> repository,
            IRepository<Core.Domain.Entities.KhamDoans.CongTyKhamSucKhoe> congTyKhamSucKhoe,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<Core.Domain.Entities.KhamDoans.HopDongKhamSucKhoe> hopDongKhamSucKhoeRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository
            ) : base(repository)
        {
            _congTyKhamSucKhoeRepository = congTyKhamSucKhoe;
            _hopDongKhamSucKhoeRepository = hopDongKhamSucKhoeRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;

        }

        public List<LookupItemVo> GetCongTyKhamSucKhoe(LookupQueryInfo queryInfo)
        {
            var getCongTyKSKs = _congTyKhamSucKhoeRepository.TableNoTracking
                                    .Select(item => new LookupItemVo
                                    {
                                        DisplayName = item.Ten,
                                        KeyId = Convert.ToInt32(item.Id),
                                    }).ApplyLike(queryInfo.Query, g => g.DisplayName)
                                    .Take(queryInfo.Take)
                                    .ToList();
            getCongTyKSKs.Insert(0, new LookupItemVo { KeyId = 0, DisplayName = "Tất cả" });
            return getCongTyKSKs;
        }

        public List<LookupItemVo> GetSoHopDongTheoCongTy(LookupQueryInfo queryInfo, long? congTyKhamSucKhoeId)
        {
            if (congTyKhamSucKhoeId == null)
                return new List<LookupItemVo>();
            var getSoHopDongKhams = _hopDongKhamSucKhoeRepository.TableNoTracking
                                    .Where(x => (congTyKhamSucKhoeId != 0 && x.CongTyKhamSucKhoeId == congTyKhamSucKhoeId) ||
                                                (congTyKhamSucKhoeId == 0 && x.NgayHieuLuc.Date <= DateTime.Now.Date && (x.NgayKetThuc == null || DateTime.Now.Date <= x.NgayKetThuc.Value.Date)))
                                    .OrderByDescending(x => x.NgayHieuLuc)
                                    .Select(item => new LookupItemVo
                                    {
                                        DisplayName = item.SoHopDong,
                                        KeyId = Convert.ToInt32(item.Id),
                                    }).ApplyLike(queryInfo.Query, g => g.DisplayName)
                                    .Take(queryInfo.Take)
                                    .ToList();
            return getSoHopDongKhams;
        }

        public async Task<GridDataSource> GetDataThongKeDichVuKhamSucKhoe(QueryInfo queryInfo, bool exportExcel)
        {
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<ThongKeDanhSachKhamDoanVo>().ToArray(), TotalRowCount = 0 };
            }


            var thongKeDichVuKhamSucKhoeQueryInfo = JsonConvert.DeserializeObject<ThongKeDichVuKhamSucKhoeQueryInfo>(queryInfo.AdditionalSearchString);
            if (thongKeDichVuKhamSucKhoeQueryInfo.CongTyKhamSucKhoeId == null)
                return new GridDataSource { Data = new List<ThongKeDanhSachKhamDoanVo>().ToArray(), TotalRowCount = 0 };

            var thongKeDanhSachKhamDoanVo = (
                         _yeuCauKhamBenhRepository
                         .TableNoTracking.Where(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe.HopDongKhamSucKhoeId == thongKeDichVuKhamSucKhoeQueryInfo.HopDongKhamSucKhoeId
                         && x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && (x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham || x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham) &&
                         ((x.GoiKhamSucKhoeId != null) || (x.GoiKhamSucKhoeDichVuPhatSinhId != null)))
                         .Select(x => new ThongKeDanhSachKhamDoanVo
                         {
                             Id = x.Id,
                             YeuCauTiepNhanId = x.YeuCauTiepNhan.Id,
                             TenCongTy = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                             TenHopDong = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong,
                             HoTen = x.YeuCauTiepNhan.HoTen,
                             NgayThangNamSinh = DateHelper.DOBFormat(x.YeuCauTiepNhan.NgaySinh, x.YeuCauTiepNhan.ThangSinh, x.YeuCauTiepNhan.NamSinh),
                             GioiTinh = x.YeuCauTiepNhan.GioiTinh,
                             MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                             MaNguoiBenh = x.YeuCauTiepNhan.BenhNhan.MaBN,

                             DichVuTrongGoiDaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham ? x.TenDichVu : string.Empty,
                             ThoiGianThucHienDisplay = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                             DichVuTrongGoiChuaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham ? x.TenDichVu : string.Empty,

                             DichVuPhatSinhDaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham ? x.TenDichVu : string.Empty,
                             ThoiGianThucHienDichVuPhatSinhDisplay = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                             DichVuPhatSinhChuaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham ? x.TenDichVu : string.Empty
                         }).Union(
                         _yeuCauDichVuKyThuatRepository
                         .TableNoTracking.Where(x =>
                          x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe.HopDongKhamSucKhoeId == thongKeDichVuKhamSucKhoeQueryInfo.HopDongKhamSucKhoeId
                          && x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                          ((x.GoiKhamSucKhoeId != null) || (x.GoiKhamSucKhoeDichVuPhatSinhId != null)))
                         .Select(x => new ThongKeDanhSachKhamDoanVo
                         {
                             Id = x.Id,
                             YeuCauTiepNhanId = x.YeuCauTiepNhan.Id,
                             TenCongTy = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                             TenHopDong = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong,
                             HoTen = x.YeuCauTiepNhan.HoTen,
                             NgayThangNamSinh = DateHelper.DOBFormat(x.YeuCauTiepNhan.NgaySinh, x.YeuCauTiepNhan.ThangSinh, x.YeuCauTiepNhan.NamSinh),
                             GioiTinh = x.YeuCauTiepNhan.GioiTinh,
                             MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                             MaNguoiBenh = x.YeuCauTiepNhan.BenhNhan.MaBN,

                             DichVuTrongGoiDaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ? x.TenDichVu : string.Empty,
                             ThoiGianThucHienDisplay = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                             DichVuTrongGoiChuaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? x.TenDichVu : string.Empty,

                             DichVuPhatSinhDaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ? x.TenDichVu : string.Empty,
                             ThoiGianThucHienDichVuPhatSinhDisplay = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                             DichVuPhatSinhChuaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? x.TenDichVu : string.Empty
                         })));

            if (!string.IsNullOrEmpty(thongKeDichVuKhamSucKhoeQueryInfo.TimKiem))
            {
                thongKeDanhSachKhamDoanVo = thongKeDanhSachKhamDoanVo
                    .ApplyLike(thongKeDichVuKhamSucKhoeQueryInfo.TimKiem.Trim(), x => x.MaNguoiBenh,
                                                                                      x => x.MaYeuCauTiepNhan,
                                                                                      x => x.HoTen,
                                                                                      x => x.TenHopDong);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : thongKeDanhSachKhamDoanVo.CountAsync();
            var queryTask = thongKeDanhSachKhamDoanVo.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataTotalThongKeDichVuKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<ThongKeDanhSachKhamDoanVo>().ToArray(), TotalRowCount = 0 };
            }

            var thongKeDichVuKhamSucKhoeQueryInfo = JsonConvert.DeserializeObject<ThongKeDichVuKhamSucKhoeQueryInfo>(queryInfo.AdditionalSearchString);

            if (thongKeDichVuKhamSucKhoeQueryInfo.CongTyKhamSucKhoeId == null)
                return new GridDataSource { Data = new List<ThongKeDanhSachKhamDoanVo>().ToArray(), TotalRowCount = 0 };

            var thongKeDanhSachKhamDoanVo = (
                        _yeuCauKhamBenhRepository
                        .TableNoTracking.Where(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe.HopDongKhamSucKhoeId == thongKeDichVuKhamSucKhoeQueryInfo.HopDongKhamSucKhoeId
                        && x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && (x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham || x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham) &&
                        ((x.GoiKhamSucKhoeId != null) || (x.GoiKhamSucKhoeDichVuPhatSinhId != null)))
                        .Select(x => new ThongKeDanhSachKhamDoanVo
                        {
                            Id = x.Id,
                            YeuCauTiepNhanId = x.YeuCauTiepNhan.Id,
                            TenCongTy = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                            TenHopDong = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            NgayThangNamSinh = DateHelper.DOBFormat(x.YeuCauTiepNhan.NgaySinh, x.YeuCauTiepNhan.ThangSinh, x.YeuCauTiepNhan.NamSinh),
                            GioiTinh = x.YeuCauTiepNhan.GioiTinh,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaNguoiBenh = x.YeuCauTiepNhan.BenhNhan.MaBN,

                            DichVuTrongGoiDaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham ? x.TenDichVu : string.Empty,
                            ThoiGianThucHienDisplay = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                            DichVuTrongGoiChuaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham ? x.TenDichVu : string.Empty,

                            DichVuPhatSinhDaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham ? x.TenDichVu : string.Empty,
                            ThoiGianThucHienDichVuPhatSinhDisplay = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                            DichVuPhatSinhChuaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham ? x.TenDichVu : string.Empty
                        }).Union(
                        _yeuCauDichVuKyThuatRepository
                        .TableNoTracking.Where(x =>
                         x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe.HopDongKhamSucKhoeId == thongKeDichVuKhamSucKhoeQueryInfo.HopDongKhamSucKhoeId
                         && x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || x.TrangThai != Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                         ((x.GoiKhamSucKhoeId != null) || (x.GoiKhamSucKhoeDichVuPhatSinhId != null)))
                        .Select(x => new ThongKeDanhSachKhamDoanVo
                        {
                            Id = x.Id,
                            YeuCauTiepNhanId = x.YeuCauTiepNhan.Id,
                            TenCongTy = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                            TenHopDong = x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            NgayThangNamSinh = DateHelper.DOBFormat(x.YeuCauTiepNhan.NgaySinh, x.YeuCauTiepNhan.ThangSinh, x.YeuCauTiepNhan.NamSinh),
                            GioiTinh = x.YeuCauTiepNhan.GioiTinh,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            MaNguoiBenh = x.YeuCauTiepNhan.BenhNhan.MaBN,

                            DichVuTrongGoiDaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ? x.TenDichVu : string.Empty,
                            ThoiGianThucHienDisplay = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                            DichVuTrongGoiChuaThucHien = x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? x.TenDichVu : string.Empty,

                            DichVuPhatSinhDaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien ? x.TenDichVu : string.Empty,
                            ThoiGianThucHienDichVuPhatSinhDisplay = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && x.ThoiDiemThucHien != null ? x.ThoiDiemThucHien.Value.ApplyFormatDateTime() : string.Empty,
                            DichVuPhatSinhChuaThucHien = x.GoiKhamSucKhoeDichVuPhatSinhId != null && x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ? x.TenDichVu : string.Empty
                        })));

            if (!string.IsNullOrEmpty(thongKeDichVuKhamSucKhoeQueryInfo.TimKiem))
            {
                thongKeDanhSachKhamDoanVo = thongKeDanhSachKhamDoanVo
                    .ApplyLike(thongKeDichVuKhamSucKhoeQueryInfo.TimKiem.Trim(), x => x.MaNguoiBenh,
                                                                                      x => x.MaYeuCauTiepNhan,
                                                                                      x => x.HoTen,
                                                                                      x => x.TenHopDong);
            }

            var countTask = thongKeDanhSachKhamDoanVo.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public virtual byte[] ExportBaoCaoThongKeDichVuKhamSucKhoe(QueryInfo queryInfo, List<ThongKeDanhSachKhamDoanVo> datas)
        {
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KSK] THỐNG KÊ DỊCH VỤ KHÁM SỨC KHỎE");

                    // set row
                    //worksheet.DefaultRowHeight = 16;                    

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 38;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 35;

                    worksheet.DefaultColWidth = 7;

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "DANH SÁCH DỊCH VỤ KHÁM SỨC KHỎE";
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4"])
                    {
                        range.Worksheet.Cells["A4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A4"].Value = "STT";
                        range.Worksheet.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["B4"])
                    {
                        range.Worksheet.Cells["B4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B4"].Value = "Tên Công Ty";
                        range.Worksheet.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["C4"])
                    {
                        range.Worksheet.Cells["C4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C4"].Value = "Hợp Đồng";
                        range.Worksheet.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["D4"])
                    {
                        range.Worksheet.Cells["D4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D4"].Value = "Mã TN";
                        range.Worksheet.Cells["D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["E4"])
                    {
                        range.Worksheet.Cells["E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E4"].Value = "Mã NB";
                        range.Worksheet.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["E4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["F4"])
                    {
                        range.Worksheet.Cells["F4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F4"].Value = "Họ Tên";
                        range.Worksheet.Cells["F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["F4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["G4"])
                    {
                        range.Worksheet.Cells["G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G4"].Value = "Ngày/Tháng/Năm Sinh";
                        range.Worksheet.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["H4"])
                    {
                        range.Worksheet.Cells["H4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H4"].Value = "Giới tính";
                        range.Worksheet.Cells["H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["H4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["I4"])
                    {
                        range.Worksheet.Cells["I4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I4"].Value = "DV TRONG GÓI ĐÃ THỰC HIỆN";
                        range.Worksheet.Cells["I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["I4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["I4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["J4"])
                    {
                        range.Worksheet.Cells["J4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J4"].Value = " THỜI GIAN THỰC HIỆN";
                        range.Worksheet.Cells["J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["J4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["J4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["J4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["K4"])
                    {
                        range.Worksheet.Cells["K4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K4"].Value = "DV TRONG GÓI CHƯA THỰC HIỆN";
                        range.Worksheet.Cells["K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["K4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["K4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["L4"])
                    {
                        range.Worksheet.Cells["L4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L4"].Value = "DV PHÁT SINH ĐÃ THỰC HIỆN";
                        range.Worksheet.Cells["L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["M4"])
                    {
                        range.Worksheet.Cells["M4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M4"].Value = "THỜI GIAN THỰC HIỆN";
                        range.Worksheet.Cells["M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["M4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["M4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["M4"])
                    {
                        range.Worksheet.Cells["N4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N4"].Value = "DV PHÁT SINH CHƯA THỰC HIỆN";
                        range.Worksheet.Cells["N4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["N4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["N4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["N4"].Style.Font.Bold = true;
                    }


                    var index = 5; // bắt đầu đổ data từ dòng 5
                    var indexMerge = 5;
                    var indexMergeEnd = 5;
                    var STT = 1;

                    var listYeuCauTiepNhanId = datas.Select(z => z.YeuCauTiepNhanId).Distinct().ToList();

                    foreach (var yeuCauTiepNhanId in listYeuCauTiepNhanId)
                    {
                        foreach (var chitiet in datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId))
                        {
                            using (var range = worksheet.Cells["A" + index + ":N" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);

                                range.Worksheet.Cells["I" + index].Value = chitiet.DichVuTrongGoiDaThucHien;
                                range.Worksheet.Cells["J" + index].Value = chitiet.ThoiGianThucHienDisplay;
                                range.Worksheet.Cells["K" + index].Value = chitiet.DichVuTrongGoiChuaThucHien;
                                range.Worksheet.Cells["L" + index].Value = chitiet.DichVuPhatSinhDaThucHien;
                                range.Worksheet.Cells["M" + index].Value = chitiet.ThoiGianThucHienDichVuPhatSinhDisplay;
                                range.Worksheet.Cells["N" + index].Value = chitiet.DichVuPhatSinhChuaThucHien;
                            }

                            index++;
                        }

                        indexMergeEnd = index - 1;

                        using (var range = worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd].Value = STT;
                            range.Worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["A" + indexMerge + ":A" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.TenCongTy).FirstOrDefault();
                            range.Worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["B" + indexMerge + ":B" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.TenHopDong).FirstOrDefault();
                            range.Worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["C" + indexMerge + ":C" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.MaYeuCauTiepNhan).FirstOrDefault();
                            range.Worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["D" + indexMerge + ":D" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.MaNguoiBenh).FirstOrDefault();
                            range.Worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["E" + indexMerge + ":E" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["E" + indexMerge + ":F" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["F" + indexMerge + ":F" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.HoTen).FirstOrDefault();
                            range.Worksheet.Cells["F" + indexMerge + ":F" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["F" + indexMerge + ":F" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["E" + indexMerge + ":F" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["F" + indexMerge + ":F" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["F" + indexMerge + ":F" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.NgayThangNamSinh).FirstOrDefault();
                            range.Worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["G" + indexMerge + ":G" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(z => z.NgayThangNamSinh).FirstOrDefault();
                            range.Worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["H" + indexMerge + ":H" + indexMergeEnd].Merge = true;
                        }

                        STT++;
                        indexMerge = index;
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
