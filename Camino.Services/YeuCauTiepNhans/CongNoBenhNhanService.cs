using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CongNoBenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauTiepNhans
{
    [ScopedDependency(ServiceType = typeof(ICongNoBenhNhanService))]
    public class CongNoBenhNhanService : YeuCauTiepNhanBaseService, ICongNoBenhNhanService
    {
        private IRepository<BenhNhan> _benhNhanRepository;
        private IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;
        private IRepository<User> _userRepository;
        private readonly IRepository<Template> _templateRepository;

        public CongNoBenhNhanService(
            IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuNhanRepository,
            IUserAgentHelper userAgentHelper,
            ICauHinhService cauHinhService,
            ILocalizationService localizationService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IRepository<BenhNhan> benhNhanRepository,
            IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository,
            IRepository<User> userRepository,
            IRepository<Template> templateRepository
        ) : base(
            yeuCauTiepNhanRepository,
            userAgentHelper,
            cauHinhService,
            localizationService,
            taiKhoanBenhNhanService
        )
        {
            _benhNhanRepository = benhNhanRepository;
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _userRepository = userRepository;
            _templateRepository = templateRepository;
        }

        //BVHD-3956 Xuất Excel

        private List<CongNoBenhNhanXuatExcel> CongNoBenhNhanXuatExcels(CongNoBenhNhanGridSearch queryInfo)
        {
            var taiKhoanBenhNhanThuQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => o.CongNo != null && o.CongNo > 0 && o.DaThuNo != true && o.DaHuy != true &&
                            (queryInfo.DTFromDate == null || o.NgayThu >= queryInfo.DTFromDate) &&
                            (queryInfo.DTToDate == null || o.NgayThu < queryInfo.DTToDate));

            if (queryInfo.CongNoId == CongNoBenhNhan.NhaThuoc)
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc);
            }
            else if (queryInfo.CongNoId == CongNoBenhNhan.NoiTru)
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru);
            }
            else if (queryInfo.CongNoId == CongNoBenhNhan.NgoaiTru)
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru);
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.ApplyLike(queryInfo.SearchString, o => o.YeuCauTiepNhan.MaYeuCauTiepNhan, o => o.TaiKhoanBenhNhan.BenhNhan.HoTen, o => o.TaiKhoanBenhNhan.BenhNhan.MaBN);
            }

            var taiKhoanBenhNhanThuData = taiKhoanBenhNhanThuQuery
                .Select(o => new
                {
                    o.TaiKhoanBenhNhanId,
                    o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    o.NgayThu,
                    o.NoiDungThu,
                    SoTienCongNo = o.CongNo.GetValueOrDefault(),
                    ThuNos = o.TaiKhoanBenhNhanThus.Select(thu => new { thu.NgayThu, thu.TienMat, thu.ChuyenKhoan, thu.CongNo }).ToList()
                }).ToList();

            var taiKhoanBenhNhanIds = taiKhoanBenhNhanThuData.Select(o => o.TaiKhoanBenhNhanId).ToList();
            var thongTinBenhNhans = _benhNhanRepository.TableNoTracking
                .Where(o => taiKhoanBenhNhanIds.Contains(o.Id))
                .Select(o => new
                {
                    Id = o.Id,
                    MaBenhNhan = o.MaBN,
                    HoTen = o.HoTen,
                    GioiTinh = o.GioiTinh,
                    NamSinh = o.NamSinh,
                    ThangSinh = o.ThangSinh,
                    NgaySinh = o.NgaySinh,
                    SoDienThoai = o.SoDienThoaiDisplay,
                    DiaChi = o.DiaChiDayDu,
                })
                .ToList();

            var returnData = new List<CongNoBenhNhanXuatExcel>();
            foreach (var groupByBenhNhan in taiKhoanBenhNhanThuData.GroupBy(o => o.TaiKhoanBenhNhanId))
            {
                var thongTinBenhNhan = thongTinBenhNhans.First(o => o.Id == groupByBenhNhan.Key);
                returnData.Add(new CongNoBenhNhanXuatExcel
                {
                    Id = groupByBenhNhan.Key,
                    MaBenhNhan = thongTinBenhNhan.MaBenhNhan,
                    HoTen = thongTinBenhNhan.HoTen,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NamSinh = DateHelper.DOBFormat(thongTinBenhNhan.NgaySinh, thongTinBenhNhan.ThangSinh, thongTinBenhNhan.NamSinh),
                    SoDienThoai = thongTinBenhNhan.SoDienThoai,
                    DiaChi = thongTinBenhNhan.DiaChi,
                    NgayGhiNoDisplay = String.Join(';', groupByBenhNhan.Select(o => o.NgayThu.ApplyFormatDateTime())),
                    MaTN = String.Join(';', groupByBenhNhan.Select(o => o.MaYeuCauTiepNhan).Distinct()),
                    LyDo = String.Join(';', groupByBenhNhan.Select(o => o.NoiDungThu).Distinct()),
                    SoTienPhaiThu = groupByBenhNhan.Sum(o=>o.SoTienCongNo),
                    ThanhToanCongNos = groupByBenhNhan.SelectMany(o=>o.ThuNos)
                                        .Select(t=>new ThannhToanCongNo {NgayThanhToan = t.NgayThu, SoTienPhaiThu = t.TienMat.GetValueOrDefault() + t.ChuyenKhoan.GetValueOrDefault() + t.CongNo.GetValueOrDefault() })
                                        .OrderBy(o=>o.NgayThanhToan).ToList()
                });
            }

            return returnData.OrderBy(o=>o.MaTN).ToList();
        }

        public byte[] ExportDanhSachConNoBenhNhan(QueryInfo queryInfo)
        {
            var queryString = new CongNoBenhNhanGridSearch();
            queryString.SearchString = queryInfo.SearchTerms;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<CongNoBenhNhanGridSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.ToDate.TryParseExactCustom(out var toDate);
                    queryString.DTToDate = toDate;
                }
                if (!string.IsNullOrEmpty(queryString.FromDate))
                {
                    queryString.FromDate.TryParseExactCustom(out var fromDate);
                    queryString.DTFromDate = fromDate;
                }
            }

            var datas = CongNoBenhNhanXuatExcels(queryString);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<CongNoBenhNhanXuatExcel>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("CÔNG NỢ NGƯỜI BỆNH - CÒN NỢ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A2:M2"])
                    {
                        range.Worksheet.Cells["A2:M2"].Merge = true;
                        range.Worksheet.Cells["A2:M2"].Value = "CÔNG NỢ NGƯỜI BỆNH - CÒN NỢ";
                        range.Worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:M2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:M2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "Từ ngày: " + queryString.DTFromDate?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + queryString.DTToDate?.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A4:M4"])
                    {
                        range.Worksheet.Cells["A4:M4"].Merge = true;
                        range.Worksheet.Cells["A4:M4"].Value = $"Công nợ: {queryString.CongNoId.GetDescription()}";
                        range.Worksheet.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:M4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:M7"])
                    {
                        range.Worksheet.Cells["A6:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:M7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:M7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:M7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6:A7"].Merge = true;
                        range.Worksheet.Cells["A6:A7"].Value = "STT";
                        range.Worksheet.Cells["A6:A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B6:B7"].Merge = true;
                        range.Worksheet.Cells["B6:B7"].Value = "Ngày ghi nợ";
                        range.Worksheet.Cells["B6:B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B6:B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C6:C7"].Merge = true;
                        range.Worksheet.Cells["C6:C7"].Value = "Mã TN";
                        range.Worksheet.Cells["C6:C7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C6:C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D6:D7"].Merge = true;
                        range.Worksheet.Cells["D6:D7"].Value = "Mã NB";
                        range.Worksheet.Cells["D6:D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D6:D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E6:E7"].Merge = true;
                        range.Worksheet.Cells["E6:E7"].Value = "Họ tên";
                        range.Worksheet.Cells["E6:E7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E6:E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F6:F7"].Merge = true;
                        range.Worksheet.Cells["F6:F7"].Value = "Giới tính";
                        range.Worksheet.Cells["F6:F7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F6:F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G6:G7"].Merge = true;
                        range.Worksheet.Cells["G6:G7"].Value = "Năm sinh";
                        range.Worksheet.Cells["G6:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G6:G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H6:H7"].Merge = true;
                        range.Worksheet.Cells["H6:H7"].Value = "Số điện thoại";
                        range.Worksheet.Cells["H6:H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H6:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I6:I7"].Merge = true;
                        range.Worksheet.Cells["I6:I7"].Value = "Địa chỉ";
                        range.Worksheet.Cells["I6:I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I6:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J6:J7"].Merge = true;
                        range.Worksheet.Cells["J6:J7"].Value = "Số tiền còn phải thu";
                        range.Worksheet.Cells["J6:J7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J6:J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K6:L6"].Merge = true;
                        range.Worksheet.Cells["K6:L6"].Value = "Thanh toán";
                        range.Worksheet.Cells["K6:L6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K6:L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K7:K7"].Merge = true;
                        range.Worksheet.Cells["K7:K7"].Value = "Ngày tháng";
                        range.Worksheet.Cells["K7:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K7:K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L7:L7"].Merge = true;
                        range.Worksheet.Cells["L7:L7"].Value = "Số tiền thanh toán";
                        range.Worksheet.Cells["L7:L7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L7:L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M6:M7"].Merge = true;
                        range.Worksheet.Cells["M6:M7"].Value = "Lý do";
                        range.Worksheet.Cells["M6:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M6:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<CongNoBenhNhanXuatExcel>(requestProperties);
                    int index = 8;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            if (item.ThanhToanCongNos != null)
                            {
                                //var thanhToanCongNos = item.ThanhToanCongNos.Count - 1;
                                //var dongMerge = index + thanhToanCongNos;
                                var dongMerge = index;
                                if(item.ThanhToanCongNos.Count > 1)
                                {
                                    dongMerge = index + item.ThanhToanCongNos.Count - 1;
                                }

                                worksheet.Cells["A" + index + ":A" + dongMerge].Merge = true;
                                worksheet.Cells["A" + index + ":A" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":A" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index + ":A" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["A" + index + ":A" + dongMerge].Value = stt;

                                worksheet.Cells["B" + index + ":B" + dongMerge].Merge = true;
                                worksheet.Cells["B" + index + ":B" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index + ":B" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["B" + index + ":B" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["B" + index + ":B" + dongMerge].Value = item.NgayGhiNoDisplay;

                                worksheet.Cells["C" + index + ":C" + dongMerge].Merge = true;
                                worksheet.Cells["C" + index + ":C" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index + ":C" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["C" + index + ":C" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["C" + index + ":C" + dongMerge].Value = item.MaTN;

                                worksheet.Cells["D" + index + ":D" + dongMerge].Merge = true;
                                worksheet.Cells["D" + index + ":D" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index + ":D" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["D" + index + ":D" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["D" + index + ":D" + dongMerge].Value = item.MaBenhNhan;

                                worksheet.Cells["E" + index + ":E" + dongMerge].Merge = true;
                                worksheet.Cells["E" + index + ":E" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index + ":E" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["E" + index + ":E" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["E" + index + ":E" + dongMerge].Value = item.HoTen;

                                worksheet.Cells["F" + index + ":F" + dongMerge].Merge = true;
                                worksheet.Cells["F" + index + ":F" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index + ":F" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["F" + index + ":F" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["F" + index + ":F" + dongMerge].Value = item.GioiTinhDisplay;

                                worksheet.Cells["G" + index + ":G" + dongMerge].Merge = true;
                                worksheet.Cells["G" + index + ":G" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index + ":G" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["G" + index + ":G" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["G" + index + ":G" + dongMerge].Value = item.NamSinh;

                                worksheet.Cells["H" + index + ":H" + dongMerge].Merge = true;
                                worksheet.Cells["H" + index + ":H" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index + ":H" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H" + index + ":H" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["H" + index + ":H" + dongMerge].Value = item.SoDienThoai;

                                worksheet.Cells["I" + index + ":I" + dongMerge].Merge = true;
                                worksheet.Cells["I" + index + ":I" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index + ":I" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["I" + index + ":I" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["I" + index + ":I" + dongMerge].Value = item.DiaChi;

                                worksheet.Cells["J" + index + ":J" + dongMerge].Merge = true;
                                worksheet.Cells["J" + index + ":J" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index + ":J" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["J" + index + ":J" + index].Style.Numberformat.Format = "#,##0";
                                worksheet.Cells["J" + index + ":J" + dongMerge].Value = item.SoTienPhaiThu;

                                worksheet.Cells["M" + index + ":M" + dongMerge].Merge = true;
                                worksheet.Cells["M" + index + ":M" + dongMerge].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index + ":M" + dongMerge].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["M" + index + ":M" + dongMerge].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["M" + index + ":M" + dongMerge].Value = item.LyDo;

                                if(item.ThanhToanCongNos.Any())
                                {
                                    foreach (var thannhToanCongNo in item.ThanhToanCongNos)
                                    {
                                        worksheet.Cells["K" + index + ":K" + index].Merge = true;
                                        worksheet.Cells["K" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["K" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells["K" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        worksheet.Cells["K" + index + ":K" + index].Value = thannhToanCongNo.NgayThanhToanDisplay;

                                        worksheet.Cells["L" + index + ":L" + index].Merge = true;
                                        worksheet.Cells["L" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["L" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        worksheet.Cells["L" + index + ":L" + index].Style.Numberformat.Format = "#,##0";
                                        worksheet.Cells["L" + index + ":L" + index].Value = thannhToanCongNo.SoTienPhaiThu;

                                        index++;
                                    }
                                }
                                else
                                {
                                    worksheet.Cells["K" + index + ":K" + index].Merge = true;
                                    worksheet.Cells["K" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["K" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    worksheet.Cells["L" + index + ":L" + index].Merge = true;
                                    worksheet.Cells["L" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["L" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    worksheet.Cells["L" + index + ":L" + index].Style.Numberformat.Format = "#,##0";

                                    index++;
                                }

                                stt++;
                            }
                        }
                    }

                    worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                    worksheet.Cells["A" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":I" + index].Merge = true;
                    worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":I" + index].Value = "Tổng cộng";
                    worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["J" + index].Value = datas.Sum(s => s.SoTienPhaiThu);
                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells["L" + index].Value = datas.SelectMany(o=>o.ThanhToanCongNos).Select(o=>o.SoTienPhaiThu.GetValueOrDefault()).DefaultIfEmpty().Sum();
                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0";

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        //BVHD-3956 Xuất Excel

        public async Task<GridDataSource> GetDanhSachBenhNhanConNoAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new CongNoBenhNhanGridSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<CongNoBenhNhanGridSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.ToDate.TryParseExactCustom(out var toDate);
                    queryString.DTToDate = toDate;
                }
                if (!string.IsNullOrEmpty(queryString.FromDate))
                {
                    queryString.FromDate.TryParseExactCustom(out var fromDate);
                    queryString.DTFromDate = fromDate;
                }
            }

            var taiKhoanBenhNhanThuQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => o.CongNo != null && o.CongNo > 0 && o.DaThuNo != true && o.DaHuy != true &&
                            (queryString.DTFromDate == null || o.NgayThu >= queryString.DTFromDate) &&
                            (queryString.DTToDate == null || o.NgayThu < queryString.DTToDate));

            if(queryString.CongNoId == CongNoBenhNhan.NhaThuoc)
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc);
            }
            else if (queryString.CongNoId == CongNoBenhNhan.NoiTru)
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru);
            }
            else if (queryString.CongNoId == CongNoBenhNhan.NgoaiTru)
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru);
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                taiKhoanBenhNhanThuQuery = taiKhoanBenhNhanThuQuery.ApplyLike(queryInfo.SearchTerms, o => o.YeuCauTiepNhan.MaYeuCauTiepNhan, o => o.TaiKhoanBenhNhan.BenhNhan.HoTen, o => o.TaiKhoanBenhNhan.BenhNhan.MaBN);
            }

            var taiKhoanBenhNhanThuData = taiKhoanBenhNhanThuQuery
                .Select(o => new
                {
                    o.TaiKhoanBenhNhanId,
                    o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    o.NgayThu,
                }).ToList();

            var taiKhoanBenhNhanIds = taiKhoanBenhNhanThuData.Select(o=>o.TaiKhoanBenhNhanId).ToList();
            var thongTinBenhNhans = _benhNhanRepository.TableNoTracking
                .Where(o=>taiKhoanBenhNhanIds.Contains(o.Id))
                .Select(o => new
                {
                    Id = o.Id,
                    MaBenhNhan = o.MaBN,
                    HoTen = o.HoTen,
                    GioiTinh = o.GioiTinh,
                    NamSinh = o.NamSinh,
                    ThangSinh = o.ThangSinh,
                    NgaySinh = o.NgaySinh,
                    SoDienThoai = o.SoDienThoaiDisplay,
                    DiaChi = o.DiaChiDayDu,
                })
                .ToList();

            var returnData = new List<CongNoBenhNhanGridVo>();
            foreach(var groupByBenhNhan in taiKhoanBenhNhanThuData.GroupBy(o=>o.TaiKhoanBenhNhanId))
            {
                var thongTinBenhNhan = thongTinBenhNhans.First(o => o.Id == groupByBenhNhan.Key);
                returnData.Add(new CongNoBenhNhanGridVo
                {
                    Id = groupByBenhNhan.Key,
                    MaBenhNhan = thongTinBenhNhan.MaBenhNhan,
                    HoTen = thongTinBenhNhan.HoTen,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NamSinh = DateHelper.DOBFormat(thongTinBenhNhan.NgaySinh, thongTinBenhNhan.ThangSinh, thongTinBenhNhan.NamSinh),
                    SoDienThoai = thongTinBenhNhan.SoDienThoai,
                    DiaChi = thongTinBenhNhan.DiaChi,
                    NgayGhiNo = String.Join(';',groupByBenhNhan.Select(o=>o.NgayThu.ApplyFormatDateTime())),
                    MaTN = String.Join(';', groupByBenhNhan.Select(o => o.MaYeuCauTiepNhan).Distinct())
                });
            }
            return new GridDataSource
            {
                Data = returnData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(),
                TotalRowCount = returnData.Count
            };

            //var query = _benhNhanRepository.TableNoTracking.Where(p => p.TaiKhoanBenhNhan != null &&
            //                                                           p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus != null &&
            //                                                           p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Count > 0 &&
            //                                                           p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Any(o => o.CongNo != null && o.CongNo > 0 && o.DaThuNo != true && o.DaHuy != true &&
            //                                                                                                            (queryString.DTFromDate == null || o.NgayThu >= queryString.DTFromDate) &&
            //                                                                                                            (queryString.DTToDate == null || o.NgayThu < queryString.DTToDate)))
            //                                               .Select(p => new CongNoBenhNhanGridVo()
            //                                               {
            //                                                   Id = p.Id,
            //                                                   MaBenhNhan = p.MaBN,
            //                                                   HoTen = p.HoTen,
            //                                                   GioiTinh = p.GioiTinh,
            //                                                   NamSinh = p.NamSinh,
            //                                                   SoDienThoai = p.SoDienThoaiDisplay,
            //                                                   DiaChi = p.DiaChiDayDu,
            //                                               });

            //if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            //{
            //    query = query.ApplyLike(queryInfo.SearchTerms, p => p.MaBenhNhan, p => p.HoTen, p => p.GioiTinhDisplay, p => p.NamSinh.GetValueOrDefault().ToString(), p => p.SoDienThoai, p => p.DiaChi);
            //}

            //var queryTask = query.OrderBy(queryInfo.SortString)
            //                     .Skip(queryInfo.Skip)
            //                     .Take(queryInfo.Take)
            //                     .ToArrayAsync();

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            //await Task.WhenAll(countTask, queryTask);

            //return new GridDataSource
            //{
            //    Data = queryTask.Result,
            //    TotalRowCount = countTask.Result
            //};
        }

        //public async Task<GridDataSource> GetTotalPagesBenhNhanConNoAsync(QueryInfo queryInfo)
        //{

        //    var queryString = new CongNoBenhNhanGridSearch();
        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        queryString = JsonConvert.DeserializeObject<CongNoBenhNhanGridSearch>(queryInfo.AdditionalSearchString);
        //        if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
        //        {
        //            DateTime denNgay;
        //            queryString.FromDate.TryParseExactCustom(out var tuNgay);
        //            if (string.IsNullOrEmpty(queryString.ToDate))
        //            {
        //                denNgay = DateTime.Now;
        //            }
        //            else
        //            {
        //                queryString.ToDate.TryParseExactCustom(out denNgay);
        //            }

        //            queryString.DTToDate = tuNgay;
        //            queryString.DTFromDate = denNgay;
        //        }
        //    }

        //    var query = _benhNhanRepository.TableNoTracking.Where(p => p.TaiKhoanBenhNhan != null &&
        //                                                               p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus != null &&
        //                                                               p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Count > 0 &&
        //                                                               p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Any(o => o.CongNo != null && o.CongNo > 0 && o.DaThuNo != true && o.DaHuy != true))
        //                                                   .Select(p => new CongNoBenhNhanGridVo()
        //                                                   {
        //                                                       Id = p.Id,
        //                                                       MaBenhNhan = p.MaBN,
        //                                                       HoTen = p.HoTen,
        //                                                       GioiTinh = p.GioiTinh,
        //                                                       NamSinh = p.NamSinh,
        //                                                       SoDienThoai = p.SoDienThoaiDisplay,
        //                                                       DiaChi = p.DiaChiDayDu,
        //                                                   });

        //    if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
        //    {
        //        query = query.ApplyLike(queryInfo.SearchTerms, p => p.MaBenhNhan, p => p.HoTen, p => p.GioiTinhDisplay, p => p.NamSinh.GetValueOrDefault().ToString(), p => p.SoDienThoai, p => p.DiaChi);
        //    }

        //    var countTask = query.CountAsync();

        //    await Task.WhenAll(countTask);

        //    return new GridDataSource
        //    {
        //        TotalRowCount = countTask.Result
        //    };
        //}

        public async Task<GridDataSource> GetDanhSachBenhNhanHetNoAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new CongNoBenhNhanGridSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<CongNoBenhNhanGridSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }

                    queryString.DTToDate = tuNgay;
                    queryString.DTFromDate = denNgay;
                }
            }

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.TaiKhoanBenhNhan != null &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus != null &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Count > 0 &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Any(o => o.CongNo != null && o.CongNo > 0 && o.DaHuy != true) &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Where(o => o.CongNo != null && o.CongNo > 0 && o.DaHuy != true).All(o => o.DaThuNo == true))
                                                           .Select(p => new CongNoBenhNhanGridVo()
                                                           {
                                                               Id = p.Id,
                                                               MaBenhNhan = p.MaBN,
                                                               HoTen = p.HoTen,
                                                               GioiTinh = p.GioiTinh,
                                                               NamSinh = p.NamSinh.GetValueOrDefault().ToString(),
                                                               SoDienThoai = p.SoDienThoaiDisplay,
                                                               DiaChi = p.DiaChiDayDu,
                                                           });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, p => p.MaBenhNhan, p => p.HoTen);
            }

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesBenhNhanHetNoAsync(QueryInfo queryInfo)
        {
            var queryString = new CongNoBenhNhanGridSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<CongNoBenhNhanGridSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }

                    queryString.DTToDate = tuNgay;
                    queryString.DTFromDate = denNgay;
                }
            }

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.TaiKhoanBenhNhan != null &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus != null &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Count > 0 &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Any(o => o.CongNo != null && o.CongNo > 0 && o.DaHuy != true) &&
                                                                       p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Where(o => o.CongNo != null && o.CongNo > 0 && o.DaHuy != true).All(o => o.DaThuNo == true))
                                                           .Select(p => new CongNoBenhNhanGridVo()
                                                           {
                                                               Id = p.Id,
                                                               MaBenhNhan = p.MaBN,
                                                               HoTen = p.HoTen,
                                                               GioiTinh = p.GioiTinh,
                                                               NamSinh = p.NamSinh.GetValueOrDefault().ToString(),
                                                               SoDienThoai = p.SoDienThoaiDisplay,
                                                               DiaChi = p.DiaChiDayDu,
                                                           });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms, p => p.MaBenhNhan, p => p.HoTen);
            }

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<CongNoBenhNhanTTHCVo> GetCongNoBenhNhanTTHCAsync(long benhNhanId)
        {
            var thongTinBenhNhan = await _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                                            .Select(p => new CongNoBenhNhanTTHCVo()
                                                                            {
                                                                                Id = p.Id,
                                                                                MaBenhNhan = p.MaBN,
                                                                                HoTen = p.HoTen,
                                                                                GioiTinh = p.GioiTinh,
                                                                                NamSinh = p.NamSinh,
                                                                                SoDienThoai = p.SoDienThoaiDisplay,
                                                                                DiaChi = p.DiaChiDayDu,
                                                                                ConNo = p.TaiKhoanBenhNhan != null &&
                                                                                        p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus != null &&
                                                                                        p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Count > 0 &&
                                                                                        p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus.Any(o => o.CongNo != null && o.CongNo > 0 && o.DaThuNo != true && o.DaHuy != true && o.DaHuy != true)
                                                                            })
                                                                            .FirstOrDefaultAsync();

            return thongTinBenhNhan;
        }

        public async Task<GridDataSource> GetDanhSachThongTinThuNoAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var test = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId).FirstOrDefault();

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                           .SelectMany(p => p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus)
                                                           .Where(p2 => p2.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuNo && p2.DaHuy != true)
                                                           .Select(p2 => new ThongTinThuNoGridVo()
                                                           {
                                                               Id = p2.Id,
                                                               SoPhieuThu = p2.SoPhieuHienThi,
                                                               SoPhieuNoId = p2.ThuNoPhieuThuId.GetValueOrDefault(),
                                                               SoPhieuNo = p2.ThuNoPhieuThu.SoPhieuHienThi,
                                                               MaTiepNhan = p2.ThuNoPhieuThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                               Ngay = p2.NgayThu,
                                                               SoTienThu = p2.TienMat.GetValueOrDefault() + p2.ChuyenKhoan.GetValueOrDefault() + p2.POS.GetValueOrDefault()
                                                           });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesThongTinThuNoAsync(QueryInfo queryInfo)
        {
            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                           .SelectMany(p => p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus)
                                                           .Where(p2 => p2.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuNo && p2.DaHuy != true);

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetDanhSachThongTinChuaThuNoAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                           .SelectMany(p => p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus)
                                                           .Where(p2 => p2.CongNo != null && p2.CongNo > 0 && p2.DaThuNo != true && p2.DaHuy != true)
                                                           .Select(p2 => new ThongTinChuaThuNoGridVo()
                                                           {
                                                               Id = p2.Id,
                                                               SoPhieuNo = p2.SoPhieuHienThi,
                                                               MaTiepNhan = p2.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                               Ngay = p2.NgayThu,
                                                               SoTienCongNo = p2.CongNo.GetValueOrDefault(),
                                                               SoTienDaThu = p2.TaiKhoanBenhNhanThus.Sum(s => s.TienMat.GetValueOrDefault() + s.ChuyenKhoan.GetValueOrDefault() + s.POS.GetValueOrDefault()),
                                                           });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesThongTinChuaThuNoAsync(QueryInfo queryInfo)
        {
            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                           .SelectMany(p => p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus)
                                                           .Where(p2 => p2.CongNo != null && p2.CongNo > 0 && p2.DaThuNo != true && p2.DaHuy != true);

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetDanhSachThongTinDaThuNoAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                           .SelectMany(p => p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus)
                                                           .Where(p2 => p2.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuNo && p2.DaHuy != true)
                                                           .Select(p2 => new ThongTinDaThuNoGridVo()
                                                           {
                                                               Id = p2.Id,
                                                               SoPhieuThu = p2.SoPhieuHienThi,
                                                               SoPhieuNoId = p2.ThuNoPhieuThuId.GetValueOrDefault(),
                                                               SoPhieuNo = p2.ThuNoPhieuThu.SoPhieuHienThi,
                                                               MaTiepNhan = p2.ThuNoPhieuThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                               Ngay = p2.NgayThu,
                                                               SoTienThu = p2.TienMat.GetValueOrDefault() + p2.ChuyenKhoan.GetValueOrDefault() + p2.POS.GetValueOrDefault()
                                                           });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesThongTinDaThuNoAsync(QueryInfo queryInfo)
        {
            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == benhNhanId && p.TaiKhoanBenhNhan != null)
                                                           .SelectMany(p => p.TaiKhoanBenhNhan.TaiKhoanBenhNhanThus)
                                                           .Where(p2 => p2.LoaiThuTienBenhNhan == LoaiThuTienBenhNhan.ThuNo && p2.DaHuy != true);

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public decimal GetSoTienChuaThu(long taiKhoanBenhNhanThuId)
        {
            var soTienChuaThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(p => p.Id == taiKhoanBenhNhanThuId)
                                                                              .Include(p => p.TaiKhoanBenhNhanThus)
                                                                              .Include(p => p.TaiKhoanBenhNhan).ThenInclude(p => p.BenhNhan)
                                                                              .Select(p => p.CongNo.GetValueOrDefault() -
                                                                                           (p.TaiKhoanBenhNhanThus.Any() ? p.TaiKhoanBenhNhanThus.Sum(p2 => p2.TienMat.GetValueOrDefault() + p2.ChuyenKhoan.GetValueOrDefault() + p2.POS.GetValueOrDefault()) : 0))
                                                                              .FirstOrDefault();



            return soTienChuaThu;
        }

        public ThongTinTraNoVo GetThongTinTraNo(long taiKhoanBenhNhanThuId)
        {
            var taiKhoanBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(p => p.Id == taiKhoanBenhNhanThuId)
                                                                                    .Include(p => p.TaiKhoanBenhNhanThus)
                                                                                    .Include(p => p.TaiKhoanBenhNhan).ThenInclude(p => p.BenhNhan)
                                                                                    .FirstOrDefault();

            var thongTinTraNo = new ThongTinTraNoVo()
            {
                Id = taiKhoanBenhNhanThu.TaiKhoanBenhNhan.BenhNhan.Id,
                TenBenhNhan = taiKhoanBenhNhanThu.TaiKhoanBenhNhan.BenhNhan.HoTen,
                SoTienChuaThu = taiKhoanBenhNhanThu.CongNo.GetValueOrDefault() - taiKhoanBenhNhanThu.TaiKhoanBenhNhanThus.Sum(p2 => p2.TienMat.GetValueOrDefault() + p2.ChuyenKhoan.GetValueOrDefault() + p2.POS.GetValueOrDefault())
            };

            return thongTinTraNo;
        }

        public long ThuTienTraNo(ThuTienTraNoVo thuTienTraNoVo)
        {
            var taiKhoanBenhNhanThuCongNo = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(p => p.Id == thuTienTraNoVo.Id)
                                                                                          .Include(p => p.TaiKhoanBenhNhan)
                                                                                          .FirstOrDefault();

            var userId = _userAgentHelper.GetCurrentUserId();
            var userThuNgan = _userRepository.GetById(_userAgentHelper.GetCurrentUserId());
            var noiLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var maTaiKhoan = userId.ToString();
            if (userThuNgan.Email != null && userThuNgan.Email.IndexOf("@") > 0)
            {
                maTaiKhoan = userThuNgan.Email.Substring(0, userThuNgan.Email.IndexOf("@") > 10 ? 10 : userThuNgan.Email.IndexOf("@"));
            }

            var taiKhoanBenhNhanThu = new TaiKhoanBenhNhanThu();
            taiKhoanBenhNhanThu.TaiKhoanBenhNhanId = taiKhoanBenhNhanThuCongNo.TaiKhoanBenhNhanId;
            taiKhoanBenhNhanThu.YeuCauTiepNhanId = taiKhoanBenhNhanThuCongNo.YeuCauTiepNhanId;
            taiKhoanBenhNhanThu.LoaiThuTienBenhNhan = LoaiThuTienBenhNhan.ThuNo;
            taiKhoanBenhNhanThu.TienMat = thuTienTraNoVo.TienMat;
            taiKhoanBenhNhanThu.ChuyenKhoan = thuTienTraNoVo.ChuyenKhoan;
            taiKhoanBenhNhanThu.POS = thuTienTraNoVo.POS;
            taiKhoanBenhNhanThu.NoiDungThu = thuTienTraNoVo.NoiDungThu;
            taiKhoanBenhNhanThu.NgayThu = thuTienTraNoVo.NgayThu;
            taiKhoanBenhNhanThu.SoQuyen = 1;
            taiKhoanBenhNhanThu.NhanVienThucHienId = userId;
            taiKhoanBenhNhanThu.NoiThucHienId = noiLamViecId;
            taiKhoanBenhNhanThu.LoaiNoiThu = LoaiNoiThu.ThuNgan;
            taiKhoanBenhNhanThu.SoPhieuHienThi = ResourceHelper.CreateSoPhieuThu(userId, maTaiKhoan);
            taiKhoanBenhNhanThu.ThuNoPhieuThuId = thuTienTraNoVo.Id;

            _taiKhoanBenhNhanThuRepository.Add(taiKhoanBenhNhanThu);

            if (thuTienTraNoVo.SoTienChuaThu == thuTienTraNoVo.SoTienThu)
            {
                taiKhoanBenhNhanThuCongNo.DaThuNo = true;
            }

            //taiKhoanBenhNhanThuCongNo.TaiKhoanBenhNhan.SoDuTaiKhoan += thuTienTraNoVo.SoTienThu;
            _taiKhoanBenhNhanThuRepository.Update(taiKhoanBenhNhanThuCongNo);

            return taiKhoanBenhNhanThu.Id;
        }

        public List<HtmlToPdfVo> GetHTMLTatCaPhieuThuNoBenhNhan(long taiKhoanBenhNhanThuChinhId, string hostingName)
        {
            var phieuThuNgans = new List<HtmlToPdfVo>();
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _userRepository.GetById(currentUserId).HoTen;

            var result = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals("PhieuThuBenhNhanTraTien"));

            var taiKhoanBenhNhanThuChinh = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(p => p.Id == taiKhoanBenhNhanThuChinhId)
                                                                                         .Include(p => p.TaiKhoanBenhNhanThus)
                                                                                         .ThenInclude(p => p.YeuCauTiepNhan)
                                                                                         .Include(p => p.YeuCauTiepNhan)
                                                                                         .FirstOrDefault();

            var content = string.Empty;

            foreach (var item in taiKhoanBenhNhanThuChinh.TaiKhoanBenhNhanThus)
            {
                var phieuThuNgan = new HtmlToPdfVo();

                if (!string.IsNullOrEmpty(content))
                {
                    content = content + "<div class=\"pagebreak\"> </div>";
                }

                var soTienThu = item.TienMat.GetValueOrDefault() + item.ChuyenKhoan.GetValueOrDefault() + item.POS.GetValueOrDefault();

                var diaChi = taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan.DiaChiDayDu;


                var data = new
                {
                    TitlePheuTraNo = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                                          "PHIẾU THU VIỆN PHÍ NỘI TRÚ" : "PHIẾU THU VIỆN PHÍ NGOẠI TRÚ",

                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    NguoiNopTien = taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan.HoTen,
                    //NamSinh = taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan.NamSinh,
                    NamSinh = DateHelper.DOBFormat(taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan?.NgaySinh, taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan?.ThangSinh, taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan?.NamSinh),
                    GioiTinh = taiKhoanBenhNhanThuChinh?.YeuCauTiepNhan.GioiTinh?.GetDescription(),
                    DiaChi = diaChi,
                    DienDai = item.NoiDungThu,
                    //SoQuyen = item.SoQuyen,
                    SoPhieu = item.SoPhieuHienThi,

                    NgayThuPhieu = $"Ngày {item.NgayThu.Day} tháng {item.NgayThu.Month} năm {item.NgayThu.Year}",
                    ngayThangHientai = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",

                    SoTien = Convert.ToDouble(soTienThu).ApplyFormatMoneyToDouble(),
                    ChungTu = "Chứng từ gốc",
                    NguoiLapPhieu = nguoiLapPhieu,
                    GoiHienTai = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" +
                                 DateTime.Now.Second,
                    SoTienBangChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(soTienThu))
                };
                phieuThuNgan.Html = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);

                phieuThuNgan.PageSize = "A5";
                phieuThuNgan.PageOrientation = "Landscape";
                phieuThuNgans.Add(phieuThuNgan);
            }

            return phieuThuNgans;
        }

        public List<HtmlToPdfVo> GetHTMLPhieuThuNoBenhNhan(long taiKhoanBenhNhanThuId, string hostingName)
        {
            var phieuThuNgans = new List<HtmlToPdfVo>();
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _userRepository.GetById(currentUserId).HoTen;

            var result = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals("PhieuThuBenhNhanTraTien"));

            var taiKhoanBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(p => p.Id == taiKhoanBenhNhanThuId)
                                                                                    .Include(p => p.YeuCauTiepNhan)
                                                                                    .Include(p => p.TaiKhoanBenhNhanThus)
                                                                                    .FirstOrDefault();

            var soTienThu = taiKhoanBenhNhanThu.TienMat.GetValueOrDefault() + taiKhoanBenhNhanThu.ChuyenKhoan.GetValueOrDefault() + taiKhoanBenhNhanThu.POS.GetValueOrDefault();

            var diaChi = taiKhoanBenhNhanThu?.YeuCauTiepNhan.DiaChiDayDu;


            var data = new
            {
                TitlePheuTraNo = taiKhoanBenhNhanThu.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ? "PHIẾU THU VIỆN PHÍ NỘI TRÚ" :
                                        "PHIẾU THU VIỆN PHÍ NGOẠI TRÚ",

                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = taiKhoanBenhNhanThu?.YeuCauTiepNhan.HoTen,

                //NamSinh = taiKhoanBenhNhanThu?.YeuCauTiepNhan.NamSinh,
                NamSinh = DateHelper.DOBFormat(taiKhoanBenhNhanThu?.YeuCauTiepNhan?.NgaySinh, taiKhoanBenhNhanThu?.YeuCauTiepNhan?.ThangSinh, taiKhoanBenhNhanThu?.YeuCauTiepNhan?.NamSinh),

                GioiTinh = taiKhoanBenhNhanThu?.YeuCauTiepNhan.GioiTinh?.GetDescription(),
                DiaChi = diaChi,
                DienDai = taiKhoanBenhNhanThu.NoiDungThu,
                //SoQuyen = taiKhoanBenhNhanThu.SoQuyen,
                SoPhieu = taiKhoanBenhNhanThu.SoPhieuHienThi,

                NgayThuPhieu = $"Ngày {taiKhoanBenhNhanThu.NgayThu.Day} tháng {taiKhoanBenhNhanThu.NgayThu.Month} năm {taiKhoanBenhNhanThu.NgayThu.Year}",
                ngayThangHientai = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",

                SoTien = Convert.ToDouble(soTienThu).ApplyFormatMoneyToDouble(),
                ChungTu = "Chứng từ gốc",
                NguoiLapPhieu = nguoiLapPhieu,
                GoiHienTai = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" +
                             DateTime.Now.Second,
                SoTienBangChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(soTienThu))
            };

            var phieuThuNgan = new HtmlToPdfVo();
            phieuThuNgan.Html = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            phieuThuNgan.PageSize = "A5";
            phieuThuNgan.PageOrientation = "Landscape";

            phieuThuNgans.Add(phieuThuNgan);

            return phieuThuNgans;
        }
    }
}
