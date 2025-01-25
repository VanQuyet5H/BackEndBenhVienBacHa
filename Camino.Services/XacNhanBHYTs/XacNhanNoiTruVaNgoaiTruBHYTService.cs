using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IXacNhanNoiTruVaNgoaiTruBHYTService))]
    public class XacNhanNoiTruVaNgoaiTruBHYTService : YeuCauTiepNhanBaseService, IXacNhanNoiTruVaNgoaiTruBHYTService
    {
        IRepository<DuyetBaoHiem> _duyetBaoHiemRepository;
        IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatsRepository;
        IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBHYTsRepository;
        IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViensRepository;
        IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViensRepository;
        IRepository<DonThuocThanhToan> _donThuocThanhToansRepository;
        IRepository<User> _userRepository;
        public XacNhanNoiTruVaNgoaiTruBHYTService(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IUserAgentHelper userAgentHelper, ICauHinhService cauHinhService, 
            ILocalizationService localizationService, ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatsRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> yeuCauDichVuGiuongBenhVienChiPhiBHYTsRepository,
            IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhViensRepository,
            IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhViensRepository,
            IRepository<DonThuocThanhToan> donThuocThanhToansRepository,
            IRepository<User> userRepository,
            IRepository<DuyetBaoHiem> duyetBaoHiemRepository) 
            : base(yeuCauTiepNhanRepository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
            _duyetBaoHiemRepository = duyetBaoHiemRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauDichVuKyThuatsRepository = yeuCauDichVuKyThuatsRepository;
            _yeuCauDichVuGiuongBenhVienChiPhiBHYTsRepository = yeuCauDichVuGiuongBenhVienChiPhiBHYTsRepository;
            _yeuCauDuocPhamBenhViensRepository = yeuCauDuocPhamBenhViensRepository;
            _yeuCauVatTuBenhViensRepository = yeuCauVatTuBenhViensRepository;
            _donThuocThanhToansRepository = donThuocThanhToansRepository;
            _userRepository = userRepository;
        }

        public async Task<GridDataSource> GetDataXacNhanNoiTruVaNgoaiTruHoanThanh(FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo queryInfo, bool forExportExcel)
        {
            var tuNgay = queryInfo.ThoiDiemTiepNhanTu ?? DateTime.Now.Date;
            var denNgay = queryInfo.ThoiDiemTiepNhanDen ?? DateTime.Now;
            var duyetBaoHiemQuery = _duyetBaoHiemRepository.TableNoTracking.Where(o => o.ThoiDiemDuyetBaoHiem >= tuNgay);
            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                duyetBaoHiemQuery = duyetBaoHiemQuery.Where(o => o.YeuCauTiepNhan.MaYeuCauTiepNhan == queryInfo.SearchString || o.YeuCauTiepNhan.HoTen == queryInfo.SearchString || o.YeuCauTiepNhan.BenhNhan.MaBN == queryInfo.SearchString);
            }

            var duyetBaoHiemDataDuyet = duyetBaoHiemQuery
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataDuyet
                {
                    MaTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,                    
                    ThoiDiemDuyet = o.ThoiDiemDuyetBaoHiem,
                    NhanVienDuyetId = o.NhanVienDuyetBaoHiemId,
                }).ToList();
            var duyetBaoHiemGroupByMaTiepNhan = duyetBaoHiemDataDuyet.GroupBy(o => o.MaTiepNhan, o => o,
                (k, v) => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataDuyet
                {
                    MaTiepNhan = k,
                    ThoiDiemDuyet = v.OrderBy(c => c.ThoiDiemDuyet).Last().ThoiDiemDuyet,
                    NhanVienDuyetId = v.OrderBy(c => c.ThoiDiemDuyet).Last().NhanVienDuyetId
                }).Where(o=>o.ThoiDiemDuyet < denNgay).ToList();

            var maTiepNhans = duyetBaoHiemGroupByMaTiepNhan.Select(o => o.MaTiepNhan).ToList();

            var duyetBaoHiemDataTiepNhan = BaseRepository.TableNoTracking
                .Where(o => maTiepNhans.Contains(o.MaYeuCauTiepNhan))
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataTiepNhan
                {
                    YeuCauTiepNhanId = o.Id,
                    MaTiepNhan = o.MaYeuCauTiepNhan,
                    MaBenhNhan = o.BenhNhan.MaBN,
                    LoaiYeuCauTiepNhan = o.LoaiYeuCauTiepNhan,
                    ThoiDiemTiepNhan = o.ThoiDiemTiepNhan,
                    HoTen = o.HoTen,
                    NgaySinh = o.NgaySinh,
                    ThangSinh = o.ThangSinh,
                    NamSinh = o.NamSinh,
                    GioiTinh = o.GioiTinh,
                    DiaChi = o.DiaChiDayDu,
                    SoDienThoai = o.SoDienThoai
                }).ToList();

            foreach(var d in duyetBaoHiemGroupByMaTiepNhan)
            {
                var yctnNgoaiTruData = duyetBaoHiemDataTiepNhan.FirstOrDefault(o => o.MaTiepNhan == d.MaTiepNhan && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru);
                var yctnNoiTruData = duyetBaoHiemDataTiepNhan.FirstOrDefault(o => o.MaTiepNhan == d.MaTiepNhan && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru);

                d.YeuCauTiepNhanNgoaiTruId = yctnNgoaiTruData?.YeuCauTiepNhanId;
                d.YeuCauTiepNhanNoiTruId = yctnNoiTruData?.YeuCauTiepNhanId;
                d.ThoiDiemTiepNhan = yctnNgoaiTruData?.ThoiDiemTiepNhan ?? yctnNoiTruData?.ThoiDiemTiepNhan;
                d.MaBenhNhan = yctnNgoaiTruData?.MaBenhNhan ?? yctnNoiTruData?.MaBenhNhan;
                d.HoTen = yctnNgoaiTruData?.HoTen ?? yctnNoiTruData?.HoTen;
                d.NgaySinh = yctnNgoaiTruData?.NgaySinh ?? yctnNoiTruData?.NgaySinh;
                d.ThangSinh = yctnNgoaiTruData?.ThangSinh ?? yctnNoiTruData?.ThangSinh;
                d.NamSinh = yctnNgoaiTruData?.NamSinh ?? yctnNoiTruData?.NamSinh;
                d.GioiTinh = yctnNgoaiTruData?.GioiTinh ?? yctnNoiTruData?.GioiTinh;
                d.DiaChi = yctnNgoaiTruData?.DiaChi ?? yctnNoiTruData?.DiaChi;
                d.SoDienThoai = yctnNgoaiTruData?.SoDienThoai ?? yctnNoiTruData?.SoDienThoai;
            }
            var duyetBaoHiemData = new List<DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataDuyet>();
            if(queryInfo.NgoaiTru == true && queryInfo.NoiTru == true)
            {
                duyetBaoHiemData = duyetBaoHiemGroupByMaTiepNhan;
            }
            else if(queryInfo.NgoaiTru == true)
            {
                duyetBaoHiemData = duyetBaoHiemGroupByMaTiepNhan.Where(o=>o.YeuCauTiepNhanNoiTruId == null).ToList();
            }
            else if (queryInfo.NoiTru == true)
            {
                duyetBaoHiemData = duyetBaoHiemGroupByMaTiepNhan.Where(o => o.YeuCauTiepNhanNoiTruId != null).ToList();
            }
            var totalRowCount = duyetBaoHiemData.Count();
            duyetBaoHiemData = duyetBaoHiemData.OrderByDescending(o => o.ThoiDiemDuyet).ToList();
            if (!forExportExcel)
            {
                duyetBaoHiemData = duyetBaoHiemData.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
            }

            var yeuCauTiepNhanIds = duyetBaoHiemData.Where(o => o.YeuCauTiepNhanNgoaiTruId != null).Select(o => o.YeuCauTiepNhanNgoaiTruId.GetValueOrDefault())
                .Concat(duyetBaoHiemData.Where(o => o.YeuCauTiepNhanNoiTruId != null).Select(o => o.YeuCauTiepNhanNoiTruId.GetValueOrDefault())).ToList();
            var soTienYeuCauKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.BaoHiemChiTra == true && o.KhongTinhPhi != true)
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
                {
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    Soluong = 1,
                    DonGia = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuong = o.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            var soTienYeuCauDichVuKyThuat = _yeuCauDichVuKyThuatsRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.BaoHiemChiTra == true && o.KhongTinhPhi != true)
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
                {
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    Soluong = o.SoLan,
                    DonGia = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuong = o.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            var soTienYeuCauDichVuGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBHYTsRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.BaoHiemChiTra == true)
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
                {
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    Soluong = o.SoLuong,
                    DonGia = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuong = o.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            var soTienYeuCauDuocPham = _yeuCauDuocPhamBenhViensRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.BaoHiemChiTra == true && o.KhongTinhPhi != true)
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
                {
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    Soluong = o.SoLuong,
                    DonGia = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuong = o.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            var soTienYeuCauVatTu = _yeuCauVatTuBenhViensRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.BaoHiemChiTra == true && o.KhongTinhPhi != true)
                .Select(o => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
                {
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    Soluong = o.SoLuong,
                    DonGia = o.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = o.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuong = o.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            var soTienDonThuoc = _donThuocThanhToansRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId.GetValueOrDefault()) && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(ct => ct.BaoHiemChiTra == true)
                .Select(ct => new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
                {
                    YeuCauTiepNhanId = ct.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault(),
                    Soluong = ct.SoLuong,
                    DonGia = ct.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = ct.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuong = ct.MucHuongBaoHiem.GetValueOrDefault(),
                }).ToList();

            var allSoTienDaXacNhan = soTienYeuCauKhamBenh.Concat(soTienYeuCauDichVuKyThuat).Concat(soTienYeuCauDichVuGiuong).Concat(soTienYeuCauDuocPham).Concat(soTienYeuCauVatTu).Concat(soTienDonThuoc).ToList();
            var dataUser = _userRepository.TableNoTracking.Select(o => new { o.Id, o.HoTen }).ToList();
            var dataRetun = new List<DanhSachXacNhanHoanThanhNoiTruVaNgoaiTru>();
            foreach(var d in duyetBaoHiemData)
            {
                var item = new DanhSachXacNhanHoanThanhNoiTruVaNgoaiTru
                {
                    ThoiDiemTiepNhan = d.ThoiDiemTiepNhan,
                    MaTiepNhan = d.MaTiepNhan,
                    MaBenhNhan = d.MaBenhNhan,
                    HoTen = d.HoTen,
                    NamSinh = DateHelper.DOBFormat(d.NgaySinh, d.ThangSinh, d.NamSinh),
                    GioiTinh = d.GioiTinh?.GetDescription(),
                    DiaChi = d.DiaChi,
                    SoDienThoai = d.SoDienThoai,
                    ThoiDiemDuyet = d.ThoiDiemDuyet,
                    LoaiDieuTri = d.YeuCauTiepNhanNoiTruId != null ? "Nội trú" : "Ngoại trú",
                    NguoiDuyet = dataUser.FirstOrDefault(o=>o.Id == d.NhanVienDuyetId)?.HoTen,
                };
                decimal soTienDaXacNhan = 0;
                if(d.YeuCauTiepNhanNgoaiTruId != null)
                {
                    soTienDaXacNhan += allSoTienDaXacNhan.Where(o => o.YeuCauTiepNhanId == d.YeuCauTiepNhanNgoaiTruId).Select(o => o.SoTienDaXacNhan).DefaultIfEmpty().Sum();
                }
                if (d.YeuCauTiepNhanNoiTruId != null)
                {
                    soTienDaXacNhan += allSoTienDaXacNhan.Where(o => o.YeuCauTiepNhanId == d.YeuCauTiepNhanNoiTruId).Select(o => o.SoTienDaXacNhan).DefaultIfEmpty().Sum();
                }
                item.SoTienDaXacNhan = soTienDaXacNhan;
                dataRetun.Add(item);
            }
            return new GridDataSource
            {
                Data = dataRetun.ToArray(),
                TotalRowCount = totalRowCount,
            };
        }

       
        public virtual byte[] ExportXacNhanNoiTruVaNgoaiTruHoanThanh(GridDataSource gridDataSource, FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo query)
        {
            var datas = (ICollection<DanhSachXacNhanHoanThanhNoiTruVaNgoaiTru>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachXacNhanHoanThanhNoiTruVaNgoaiTru>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[BHYT] DS XÁC NHẬN BHYT ĐÃ HOÀN THÀNH");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;              
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:V1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:M2"])
                    {
                        range.Worksheet.Cells["A2:M2"].Merge = true;
                        range.Worksheet.Cells["A2:M2"].Value = "DANH SÁCH XÁC NHẬN BHYT ĐÃ HOÀN THÀNH";
                        range.Worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:M2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:M2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "Từ ngày: " + query.ThoiDiemTiepNhanTu?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ThoiDiemTiepNhanDen?.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:M5"])
                    {
                        range.Worksheet.Cells["A5:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:M5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:M5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A5:A5"].Merge = true;
                        range.Worksheet.Cells["A5:A5"].Value = "STT";
                        range.Worksheet.Cells["A5:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B5:B5"].Merge = true;
                        range.Worksheet.Cells["B5:B5"].Value = "Thời gian TN";
                        range.Worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B5:B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C5:C5"].Merge = true;
                        range.Worksheet.Cells["C5:C5"].Value = "Mã TN";
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D5:D5"].Merge = true;
                        range.Worksheet.Cells["D5:D5"].Value = "Mã NB";
                        range.Worksheet.Cells["D5:D5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E5:E5"].Merge = true;
                        range.Worksheet.Cells["E5:E5"].Value = "HỌ TÊN";
                        range.Worksheet.Cells["E5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F5:F5"].Merge = true;
                        range.Worksheet.Cells["F5:F5"].Value = "Năm sinh";
                        range.Worksheet.Cells["F5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F5:F5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G5:G5"].Merge = true;
                        range.Worksheet.Cells["G5:G5"].Value = "Giới tính";
                        range.Worksheet.Cells["G5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G5:G5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H5:H5"].Merge = true;
                        range.Worksheet.Cells["H5:H5"].Value = "Địa chỉ";
                        range.Worksheet.Cells["H5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H5:H5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I5:I5"].Merge = true;
                        range.Worksheet.Cells["I5:I5"].Value = "SỐ ĐIỆN THOẠI";
                        range.Worksheet.Cells["I5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I5:I5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J5:J5"].Merge = true;
                        range.Worksheet.Cells["J5:J5"].Value = "SỐ TIỀN ĐÃ XN";
                        range.Worksheet.Cells["J5:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J5:J5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K5:K5"].Merge = true;
                        range.Worksheet.Cells["K5:K5"].Value = "THỜI ĐIỂM DUYỆT";
                        range.Worksheet.Cells["K5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K5:K5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L5:L5"].Merge = true;
                        range.Worksheet.Cells["L5:L5"].Value = "LOẠI ĐIỀU TRỊ";
                        range.Worksheet.Cells["L5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L5:L5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M5:M5"].Merge = true;
                        range.Worksheet.Cells["M5:M5"].Value = "NGƯỜI DUYỆT";
                        range.Worksheet.Cells["M5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M5:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        //range.Worksheet.Cells["N5:N5"].Merge = true;
                        //range.Worksheet.Cells["N5:N5"].Value =
                        //range.Worksheet.Cells["N5:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        //range.Worksheet.Cells["N5:N5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    }

                    var manager = new PropertyManager<DanhSachXacNhanHoanThanhNoiTruVaNgoaiTru>(requestProperties);
                    int index = 6;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":M" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.ThoiDiemTiepNhanDisplay;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaTiepNhan;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.MaBenhNhan;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.HoTen;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NamSinh;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.GioiTinh;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.DiaChi;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.SoDienThoai;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Value = item.SoTienDaXacNhan;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);                              
                                worksheet.Cells["K" + index].Value = item.ThoiDiemDuyetDisplay;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.LoaiDieuTri;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.NguoiDuyet;

                                //worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells["N" + index].Value =

                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }       
    }
}
