using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoTongHopDangKyGoiDichVuForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauGoiDichVus = new List<BaoCaoTongHopDangKyGoiDichVuGridVo>();
            var timKiemNangCaoObj = new BaoCaoTongHopDangKyGoiDichVuQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTongHopDangKyGoiDichVuQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var yeuCauGoiDichVuQuery = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay);

                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    yeuCauGoiDichVuQuery = yeuCauGoiDichVuQuery.ApplyLike(timKiemNangCaoObj.SearchString.Trim(), x => x.BenhNhan.HoTen, x => x.BenhNhan.MaBN);
                }

                yeuCauGoiDichVus = yeuCauGoiDichVuQuery
                    .Select(item => new BaoCaoTongHopDangKyGoiDichVuGridVo()
                    {
                        Id = item.Id,
                        NgayDangKy = item.ThoiDiemChiDinh,
                        MaNB = item.BenhNhan.MaBN,
                        TenBN = item.BenhNhan.HoTen,
                        NgaySinh = item.BenhNhan.NgaySinh,
                        ThangSinh = item.BenhNhan.ThangSinh,
                        NamSinh = item.BenhNhan.NamSinh,
                        DiaChi = item.BenhNhan.DiaChiDayDu,
                        TenGoi = item.TenChuongTrinh,
                        GiaTriGoi = item.GiaSauChietKhau,
                        DaThu = item.SoTienBenhNhanDaChi,
                        GiaTriDichVuDaThucHien = null,
                        SoTienHoanTra = item.SoTienTraLai,
                        HuyGoi = item.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy
                    })
                    .OrderBy(x => x.NgayDangKy)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();

                var lstYeuCauGoiDichVuId = yeuCauGoiDichVus.Select(x => x.Id).ToList();
                if (lstYeuCauGoiDichVuId.Any())
                {
                    var lstChiPhiDaDungTheoDichVu = new List<ChiPhiDichVuDaDungTrongGoiVo>();
                    lstChiPhiDaDungTheoDichVu = 
                        _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.YeuCauGoiDichVuId != null
                                    && lstYeuCauGoiDichVuId.Contains(x.YeuCauGoiDichVuId.Value))
                        .Select(item => new ChiPhiDichVuDaDungTrongGoiVo()
                        {
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                            DonGia = item.DonGiaSauChietKhau.Value,
                            SoLuong = 1,
                            DuocBHYTChiTra = item.BaoHiemChiTra == true && item.DuocHuongBaoHiem,
                            DonGiaBaoHiem = item.DonGiaBaoHiem,
                            MucHuongBaoHiem = item.MucHuongBaoHiem,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan
                        })
                        .Union(
                            _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && x.YeuCauGoiDichVuId != null
                                            && lstYeuCauGoiDichVuId.Contains(x.YeuCauGoiDichVuId.Value))
                                .Select(item => new ChiPhiDichVuDaDungTrongGoiVo()
                                {
                                    YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                                    DonGia = item.DonGiaSauChietKhau.Value,
                                    SoLuong = item.SoLan,
                                    DuocBHYTChiTra = item.BaoHiemChiTra == true && item.DuocHuongBaoHiem,
                                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                                    MucHuongBaoHiem = item.MucHuongBaoHiem,
                                    TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan
                                })
                            )
                        .ToList();

                    var lstChiPhiGiuong = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauGoiDichVuId != null
                                    && lstYeuCauGoiDichVuId.Contains(x.YeuCauGoiDichVuId.Value))
                        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                        .ToList();

                    lstChiPhiDaDungTheoDichVu.AddRange(
                        lstChiPhiGiuong.Select(item => new ChiPhiDichVuDaDungTrongGoiVo()
                        {
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                            DonGia = item.DonGiaSauChietKhau.Value,
                            SoLuong = item.SoLuong,
                            DuocBHYTChiTra = item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem),
                            DonGiaBaoHiem = item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem)
                                ? item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.First(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem).DonGiaBaoHiem : (decimal?)null,
                            MucHuongBaoHiem = item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem)
                                ? item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.First(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem).MucHuongBaoHiem : (int?)null,
                            TiLeBaoHiemThanhToan = item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem)
                                ? item.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.First(a => a.BaoHiemChiTra == true && a.DuocHuongBaoHiem).TiLeBaoHiemThanhToan : (int?)null,
                        }));

                    foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
                    {
                        yeuCauGoiDichVu.GiaTriDichVuDaThucHien = lstChiPhiDaDungTheoDichVu
                            .Where(x => x.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id).Sum(x => x.ThanhTien);
                    }
                }
            }

            return new GridDataSource
            {
                Data = yeuCauGoiDichVus.ToArray(),
                TotalRowCount = yeuCauGoiDichVus.Count()
            };
        }

        public async Task<GridDataSource> GetTotalBaoCaoTongHopDangKyGoiDichVuForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoTongHopDangKyGoiDichVuQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTongHopDangKyGoiDichVuQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var yeuCauGoiDichVuQuery = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.ThoiDiemChiDinh >= tuNgay && x.ThoiDiemChiDinh <= denNgay);

                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    yeuCauGoiDichVuQuery = yeuCauGoiDichVuQuery.ApplyLike(timKiemNangCaoObj.SearchString.Trim(), x => x.BenhNhan.HoTen, x => x.BenhNhan.MaBN);
                }

                var yeuCauGoiDichVus = yeuCauGoiDichVuQuery
                    .Select(item => new BaoCaoTongHopDangKyGoiDichVuGridVo()
                    {
                        Id = item.Id
                    });
                var countTask = yeuCauGoiDichVus.Count();
                return new GridDataSource { TotalRowCount = countTask };

            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoTongHopDangKyGoiDichVu(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoTongHopDangKyGoiDichVuQueryInfo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTongHopDangKyGoiDichVuQueryInfo>(query.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var data = (ICollection<BaoCaoTongHopDangKyGoiDichVuGridVo>)gridDataSource.Data;

            using(var stream = new MemoryStream())
            {
                using(var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỔNG HỢP ĐĂNG KÝ GÓI DỊCH VỤ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 35;
                    worksheet.Column(7).Width = 35;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    worksheet.Row(3).Height = 21;

                    using(var range = worksheet.Cells["A1:G1"])
                    {
                        range.Worksheet.Cells["A1:G1"].Merge = true;
                        range.Worksheet.Cells["A1:G1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:O3"])
                    {
                        range.Worksheet.Cells["A3:O3"].Merge = true;
                        range.Worksheet.Cells["A3:O3"].Value = "BÁO CÁO TỔNG HỢP ĐĂNG KÝ GÓI DỊCH VỤ";
                        range.Worksheet.Cells["A3:O3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:O3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:O3"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A3:O3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:O3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:O4"])
                    {
                        range.Worksheet.Cells["A4:O4"].Merge = true;
                        range.Worksheet.Cells["A4:O4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:O4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:O4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:O4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:O4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:O4"].Style.Font.Italic = true;
                    }

                    using(var range = worksheet.Cells["A6:O6"])
                    {
                        range.Worksheet.Cells["A6:O6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:O6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:O6"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A6:O6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:O6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:O6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:O6"].Style.WrapText = true;

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Ngày";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã NB";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Tên Người Bệnh";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Năm sinh";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Địa chỉ";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Tên gói ";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Giá trị gói ";

                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "Đã thu";

                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Còn phải thu";

                        range.Worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6"].Value = "Giá trị DV đã thực hiện trong gói";

                        range.Worksheet.Cells["L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L6"].Value = "Giá trị DV chưa thực hiện trong gói";

                        range.Worksheet.Cells["M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M6"].Value = "Phí phạt hủy gói";

                        range.Worksheet.Cells["N6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N6"].Value = "Hoàn trả";

                        range.Worksheet.Cells["O6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O6"].Value = "Hủy";
                    }

                    var stt = 1;
                    var index = 7;
                    ///đô data
                    ///
                    if (data.Any())
                    {
                        foreach(var item in data)
                        {
                            worksheet.Cells["A" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells["A" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = stt;
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.NgayDangKyStr;
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = item.MaNB;
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = item.TenBN;
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = item.NamSinh;
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = item.DiaChi;
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = item.TenGoi;
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = item.GiaTriGoi;
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = item.DaThu;
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = item.ConThieu;
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Value = item.GiaTriDichVuDaThucHien;
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Value = item.GiaTriDichVuChuaThucHien;
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Value = item.PhiPhatHuyGoi != 0 ? item.PhiPhatHuyGoi : (decimal?) null;
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Value = item.SoTienHoanTra;
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["O" + index].Value = item.HuyGoi == true ? "x" : "";
                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            stt++;
                            index++;
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

    }
}
