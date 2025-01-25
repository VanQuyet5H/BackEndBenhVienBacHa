using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Services.Helpers;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoNguoiBenhDenKhamForGridAsync(QueryInfo queryInfo)
        {
            var thongTinKhams = new List<BaoCaoNguoiBenhDenKhamGridVo>();
            var timKiemNangCaoObj = new BaoCaoNguoiBenhDenKhamQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoNguoiBenhDenKhamQueryInfoVo>(queryInfo.AdditionalSearchString);
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
                thongTinKhams = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                && x.ThoiDiemThucHien != null
                                && x.ThoiDiemThucHien >= tuNgay
                                && x.ThoiDiemThucHien <= denNgay)
                    .Select(item => new BaoCaoNguoiBenhDenKhamGridVo
                    {
                        Id = item.Id,
                        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        ThoiGianTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan,
                        ThoiGianBatDauKham = item.ThoiDiemThucHien,
                        ThoiGianKetThucKham = item.ThoiDiemHoanThanh,
                        TenNguoiBenh = item.YeuCauTiepNhan.HoTen,
                        SoDienThoai = item.YeuCauTiepNhan.SoDienThoaiDisplay ?? item.YeuCauTiepNhan.NguoiLienHeSoDienThoai.ApplyFormatPhone(),
                        LoaiYeuCauTiepNhan = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                        MucHuong = item.YeuCauTiepNhan.BHYTMucHuong,
                        CoBHYT = item.YeuCauTiepNhan.CoBHYT,
                        GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                        BacSiKham = item.BacSiThucHien.User.HoTen,
                        ChuanDoanIcd = item.Icdchinh != null ? $"{item.Icdchinh.Ma} - {item.Icdchinh.TenTiengViet}" : "",
                        DichVuKhamThucHien = item.TenDichVu,
                        DichVuKyThuatThucHien = "",
                        KetQua = "",
                        HuongGiaiQuyet = item.CachGiaiQuyet,
                        NgayHenTaiKham = item.NgayTaiKham,
                        NguoiGioiThieu = item.YeuCauTiepNhan.NoiGioiThieu.Ten
                    })
                    .OrderBy(x => x.ThoiGianBatDauKham)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();

                var lstYeuCauKhamId = thongTinKhams.Select(x => x.Id).Distinct().ToList();
                if (lstYeuCauKhamId.Any())
                {
                    var lstICDKemTheo = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => lstYeuCauKhamId.Contains(x.Id))
                        .SelectMany(x => x.YeuCauKhamBenhICDKhacs)
                        .Select(item => new BaoCaoNguoiBenhDenKhamICDVo()
                        {
                            YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                            MaICD = item.ICD.Ma,
                            TenICD = item.ICD.TenTiengViet
                        })
                        .ToList();

                    var lstDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && x.YeuCauKhamBenhId != null
                                    && lstYeuCauKhamId.Contains(x.YeuCauKhamBenhId.Value))
                        .Select(item => new BaoCaoNguoiBenhDenKhamDichVuKyThuatVo()
                        {
                            YeuCauKhamBenhId = item.YeuCauKhamBenhId.Value,
                            TenDichVu = item.TenDichVu,
                            KetQuaPhienXetNghiemChiTietVos = item.PhienXetNghiemChiTiets.Any(a => a.ThoiDiemKetLuan != null)
                                ? item.PhienXetNghiemChiTiets.Select(ct =>
                                new KetQuaPhienXetNghiemChiTietVo
                                {
                                    Id = ct.Id,
                                    MaBarcode = ct.PhienXetNghiem.BarCodeId,
                                    ThoiDiemKetLuan = ct.ThoiDiemKetLuan,
                                    ThoiDiemLayMau = ct.ThoiDiemLayMau,
                                    KetQuaChiSoXetNghiemChiTietVos = ct.KetQuaXetNghiemChiTiets.Select(kq => new KetQuaChiSoXetNghiemChiTietVo
                                    {
                                        DichVuXetNghiemId = kq.DichVuXetNghiemId,
                                        DichVuXetNghiemMa = kq.DichVuXetNghiemMa,
                                        DichVuXetNghiemTen = kq.DichVuXetNghiemTen,
                                        ThoiDiemDuyetKetQua = kq.ThoiDiemDuyetKetQua,
                                        SoThuTu = kq.SoThuTu,
                                        CapDichVu = kq.CapDichVu,
                                        MaChiSo = kq.MaChiSo,
                                        MauMayXetNghiemId = kq.MauMayXetNghiemId,
                                        MayXetNghiemId = kq.MayXetNghiemId,
                                        GiaTriTuMay = kq.GiaTriTuMay,
                                        GiaTriNhapTay = kq.GiaTriNhapTay,
                                        GiaTriDuyet = kq.GiaTriDuyet,
                                        GiaTriCu = kq.GiaTriCu,
                                    }).ToList()
                                }).ToList() : new List<KetQuaPhienXetNghiemChiTietVo>()
                        }).ToList();
                    var ketnoichiso = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Where(o => o.HieuLuc).ToList();

                    foreach (var yeuCauKham in thongTinKhams)
                    {
                        var lstICD = lstICDKemTheo.Where(x => x.YeuCauKhamBenhId == yeuCauKham.Id)
                            .Select(x => x.TenHienThi).ToList();
                        if (!string.IsNullOrEmpty(yeuCauKham.ChuanDoanIcd))
                        {
                            lstICD.Insert(0, yeuCauKham.ChuanDoanIcd);
                        }

                        var lstDichVu = lstDichVuKyThuat.Where(x => x.YeuCauKhamBenhId == yeuCauKham.Id).ToList();
                        var lstTenDichVu = lstDichVu.Select(x => x.TenDichVu).ToList();
                        var lstDichVuXetNghiem = lstDichVu.Where(x => x.KetQuaPhienXetNghiemChiTietVos.Any()).ToList();
                        var lstKetQua = new List<string>();
                        foreach (var dichVuXetNghiem in lstDichVuXetNghiem)
                        {
                            var ketQuaPhienXetNghiemChiTietLast = dichVuXetNghiem.KetQuaPhienXetNghiemChiTietVos.OrderBy(o => o.Id).Last();
                            if (ketQuaPhienXetNghiemChiTietLast.ThoiDiemKetLuan != null)
                            {
                                var ketQua = LISHelper.GetKetQuaDichVuXetNghiem(dichVuXetNghiem.TenDichVu, ketQuaPhienXetNghiemChiTietLast.KetQuaChiSoXetNghiemChiTietVos.ToList(), ketnoichiso);
                                lstKetQua.Add(ketQua);
                            }
                        }

                        yeuCauKham.ChuanDoanIcd = string.Join(", ", lstICD);
                        yeuCauKham.DichVuKyThuatThucHien = string.Join(", ", lstTenDichVu);
                        yeuCauKham.KetQua = string.Join(". ", lstKetQua);
                    }
                }
            }

            return new GridDataSource
            {
                Data = thongTinKhams.ToArray(),
                TotalRowCount = thongTinKhams.Count()
            };
        }

        public async Task<GridDataSource> GetTotalBaoCaoNguoiBenhDenKhamForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoNguoiBenhDenKhamQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoNguoiBenhDenKhamQueryInfoVo>(queryInfo.AdditionalSearchString);
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
                var thongTinKhams = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                && x.ThoiDiemThucHien != null
                                && x.ThoiDiemThucHien >= tuNgay
                                && x.ThoiDiemThucHien <= denNgay)
                    .Select(item => new BaoCaoNguoiBenhDenKhamGridVo
                    {
                        Id = item.Id
                    });
                var countTask = thongTinKhams.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource { TotalRowCount = countTask.Result };
            }

            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoNguoiBenhDenKhamAsync(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoNguoiBenhDenKhamQueryInfoVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoNguoiBenhDenKhamQueryInfoVo>(query.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var datas = (ICollection<BaoCaoNguoiBenhDenKhamGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoNguoiBenhDenKhamGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO NGƯỜI BỆNH ĐẾN KHÁM");
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
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;


                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:Q1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A8:Q8"])
                    {
                        range.Worksheet.Cells["D8:J8"].Merge = true;
                        range.Worksheet.Cells["D8:J8"].Value = "DANH SÁCH CHI TIẾT NGƯỜI BỆNH KHÁM BỆNH";
                        range.Worksheet.Cells["D8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["D8:J8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D8:J8"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A9:Q9"])
                    {
                        range.Worksheet.Cells["D9:J9"].Merge = true;
                        range.Worksheet.Cells["D9:J9"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["D9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["D9:J9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D9:J9"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A11:S12"])
                    {
                        range.Worksheet.Cells["A11:S11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A11:S11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:S11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:S11"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A11:S11"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A11:S11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A12:S12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A12:S12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:S12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:S12"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A12:S12"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A12:S12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A11:A12"].Merge = true;
                        range.Worksheet.Cells["A11:A12"].Value = "STT";
                        range.Worksheet.Cells["A11:A12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A11:A12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B11:B12"].Merge = true;
                        range.Worksheet.Cells["B11:B12"].Value = "Mã Y Tế";
                        range.Worksheet.Cells["B11:B12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C11:F11"].Merge = true;
                        range.Worksheet.Cells["C11:F11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C11:F11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C11:F11"].Value = "Thời Gian Khám";
                        range.Worksheet.Cells["C11:F11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["C12"].Value = "Thời gian tiếp nhận";
                        range.Worksheet.Cells["C12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D12"].Value = "Thời gian bắt đầu khám";
                        range.Worksheet.Cells["D12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E12"].Value = "Giờ kết thúc khám";
                        range.Worksheet.Cells["E12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["F12"].Value = "Ngày";
                        range.Worksheet.Cells["F12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G11:G12"].Merge = true;
                        range.Worksheet.Cells["G11:G12"].Value = "Tên Người Bệnh";
                        range.Worksheet.Cells["G11:G12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G11:G12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H11:H12"].Merge = true;
                        range.Worksheet.Cells["H11:H12"].Value = "Số Điện Thoại";
                        range.Worksheet.Cells["H11:H12"].Style.WrapText = true;
                        range.Worksheet.Cells["H11:H12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H11:H12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I11:I12"].Merge = true;
                        range.Worksheet.Cells["I11:I12"].Value = "Đối Tượng";
                        range.Worksheet.Cells["I11:I12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I11:I12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J11:J12"].Merge = true;
                        range.Worksheet.Cells["J11:J12"].Value = "Giới Tính";
                        range.Worksheet.Cells["J11:J12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J11:J12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J11:J12"].Style.WrapText = true;

                        range.Worksheet.Cells["K11:K12"].Merge = true;
                        range.Worksheet.Cells["K11:K12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K11:K12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K11:K12"].Style.WrapText = true;
                        range.Worksheet.Cells["K11:K12"].Value = "Ngày Sinh";
                        range.Worksheet.Cells["K11:K12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L11:L12"].Merge = true;
                        range.Worksheet.Cells["L11:L12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L11:L12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L11:L12"].Style.WrapText = true;
                        range.Worksheet.Cells["L11:L12"].Value = "Địa Chỉ";
                        range.Worksheet.Cells["L11:L12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M11:N11"].Merge = true;
                        range.Worksheet.Cells["M11:N11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M11:N11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M11:N11"].Value = "Bác Sĩ, Chuẩn Đoán, ICD";
                        range.Worksheet.Cells["M11:N11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["M11:M12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M11:M12"].Value = "Bác Sĩ Khám";
                        range.Worksheet.Cells["M11:M12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                        range.Worksheet.Cells["N11:N12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N11:N12"].Value = "Chuẩn Đoán, ICD";
                        range.Worksheet.Cells["N11:N12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O11:O12"].Merge = true;
                        range.Worksheet.Cells["O11:O12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O11:O12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O11:O12"].Value = "Dịch Vụ Thực Hiện";
                        range.Worksheet.Cells["O11:O12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P11:P12"].Merge = true;
                        range.Worksheet.Cells["P11:P12"].Value = "Kết Quả";
                        range.Worksheet.Cells["P11:P12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P11:P12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P11:P12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q11:Q12"].Merge = true;
                        range.Worksheet.Cells["Q11:Q12"].Value = "Hướng Giải Quyết";
                        range.Worksheet.Cells["Q11:Q12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q11:Q12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q11:Q12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R11:R12"].Merge = true;
                        range.Worksheet.Cells["R11:R12"].Value = "Ngày Hẹn Tái Khám";
                        range.Worksheet.Cells["R11:R12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R11:R12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R11:R12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S11:S12"].Merge = true;
                        range.Worksheet.Cells["S11:S12"].Value = "Người Giới Thiệu";
                        range.Worksheet.Cells["S11:S12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S11:S12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S11:S12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        
                    }

                    var manager = new PropertyManager<BaoCaoNguoiBenhDenKhamGridVo>(requestProperties);
                    int index = 14;

                    ////Đổ dât vào bảng excel
                    ///
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":S" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaYeuCauTiepNhan;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.ThoiGianTiepNhanDisplay;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.ThoiGianBatDauKhamDisplay;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.ThoiGianKetThucKhamDisplay;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NgayKhamDisplay;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.TenNguoiBenh;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.SoDienThoai;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.DoiTuong;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.GioiTinh;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.NgaySinhDisplay;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.DiaChi;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.BacSiKham;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.ChuanDoanIcd;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.DichVuKhamThucHien + (!string.IsNullOrEmpty(item.DichVuKyThuatThucHien) ?  ("\r\n" + item.DichVuKyThuatThucHien) : "");
                                range.Worksheet.Cells["O" + index].Style.WrapText = true;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.KetQua;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.HuongGiaiQuyet;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.NgayHenTaiKhamDisplay;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.NguoiGioiThieu;

                                index++;
                                stt++;
                            }
                        }
                    }

                    index++;
                    worksheet.Cells["A" + index + ":S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":S" + index].Style.Font.Bold = true;

                    worksheet.Cells["K" + index + ":N" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["K" + index + ":N" + index].Style.Font.Italic = true;
                    worksheet.Cells["K" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":N" + index].Merge = true;
                    index++;

                    worksheet.Cells["K" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["K" + index + ":N" + index].Value = "Người Lập";
                    worksheet.Cells["K" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":N" + index].Merge = true;
                    worksheet.Cells["K" + index + ":N" + index].Style.Font.Bold = true;

                    index = index + 4;

                    var currentUserId = _userAgentHelper.GetCurrentUserId();
                    var nhanVien = _nhanVienRepository.TableNoTracking.Where(x => x.Id == currentUserId).Select(x => x.User.HoTen).FirstOrDefault();
                    worksheet.Cells["K" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["K" + index + ":N" + index].Value = nhanVien;
                    worksheet.Cells["K" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":N" + index].Merge = true;
                    worksheet.Cells["K" + index + ":N" + index].Style.Font.Bold = true;

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
