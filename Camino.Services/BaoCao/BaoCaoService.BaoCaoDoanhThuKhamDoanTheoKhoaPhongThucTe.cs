using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
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
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoKetQuaKhamChuaBenh;
using Camino.Core.Domain.ValueObject.CauHinh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private GridDataSource GetDataDoanhThuKhamDoanThucTe(BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeQueryInfo queryInfo)
        {
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (queryInfo.FromDate != null && !string.IsNullOrEmpty(queryInfo.FromDate))
            {
                DateTime.TryParseExact(queryInfo.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (queryInfo.ToDate != null && !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                DateTime.TryParseExact(queryInfo.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var results = new List<BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo>();
            var countPage = 0;

            if (tuNgay != null && denNgay != null)
            {
                var thongTinKhams = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                && (x.YeuCauKhamBenhs.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                                               //&& a.ThoiDiemHoanThanh != null 
                                                               //&& a.ThoiDiemHoanThanh.Value >= tuNgay
                                                               //&& a.ThoiDiemHoanThanh.Value <= denNgay)
                                                               && (a.ThoiDiemHoanThanh != null || a.ThoiDiemThucHien != null)
                                                               && (a.ThoiDiemHoanThanh ?? a.ThoiDiemThucHien) >= tuNgay
                                                               && (a.ThoiDiemHoanThanh ?? a.ThoiDiemThucHien) <= denNgay)
                                    || x.YeuCauDichVuKyThuats.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                       //&& a.ThoiDiemHoanThanh != null
                                                                       //&& a.ThoiDiemHoanThanh.Value >= tuNgay
                                                                       //&& a.ThoiDiemHoanThanh.Value <= denNgay)))
                                                                       && (a.ThoiDiemHoanThanh != null || a.ThoiDiemKetLuan != null || a.ThoiDiemThucHien != null)
                                                                       && (a.ThoiDiemHoanThanh ?? a.ThoiDiemKetLuan ?? a.ThoiDiemThucHien) >= tuNgay
                                                                       && (a.ThoiDiemHoanThanh ?? a.ThoiDiemKetLuan ?? a.ThoiDiemThucHien) <= denNgay)))
                    .Select(item => new BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo()
                    {
                        Id = item.Id,
                        ThoiDiemTiepNhan = item.ThoiDiemTiepNhan,
                        MaTN = item.MaYeuCauTiepNhan,
                        HoTen = item.HoTen,
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        NamSinh = item.NamSinh,
                        GioiTinh = item.GioiTinh.GetDescription(),
                        CongTyId = item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                        TenCongTy = item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                    })
                    .OrderBy(x => x.TenCongTy)
                    .ThenBy(x => x.ThoiDiemTiepNhan);

                countPage = thongTinKhams.Count();
                results = thongTinKhams.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                
                if (results.Any())
                {
                    var lstYeuCauTiepNhanId = results.Select(x => x.Id).ToList();
                    var lstDoanhThuDichVuKham = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.GoiKhamSucKhoeId != null
                                    && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                    && lstYeuCauTiepNhanId.Contains(x.YeuCauTiepNhanId)
                                    //&& x.ThoiDiemHoanThanh != null 
                                    //&& x.ThoiDiemHoanThanh.Value >= tuNgay 
                                    //&& x.ThoiDiemHoanThanh.Value <= denNgay)

                                    && (x.ThoiDiemHoanThanh != null || x.ThoiDiemThucHien != null)
                                    && (x.ThoiDiemHoanThanh ?? x.ThoiDiemThucHien) >= tuNgay
                                    && (x.ThoiDiemHoanThanh ?? x.ThoiDiemThucHien) <= denNgay)
                        .Select(item => new DoanhThuTheoDichVuThucTeVo()
                        {
                            YeucauTiepNhanId = item.YeuCauTiepNhanId,
                            NhomDichVuBenhVienId = 0,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            TenDichVu = item.TenDichVu,
                            SoLan = 1,
                            GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                            DichVuKhamBenhBenhVienId = item.DichVuKhamBenhBenhVienId,
                            NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                            KhoaId = item.NoiDangKy.KhoaPhongId,
                            TenKhoa = item.NoiDangKy.KhoaPhong.Ten
                        })
                        //.GroupBy(x => new {x.YeucauTiepNhanId, x.KhoaId})
                        //.Select(item => new DoanhThuTheoDichVuVo()
                        //{
                        //    YeucauTiepNhanId = item.Key.YeucauTiepNhanId,
                        //    NhomDichVuBenhVienId = 0,
                        //    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                        //    DoanhThu = item.Any(a => a.DoanhThu != null && a.DoanhThu != 0) ? item.Sum(a => a.DoanhThu ?? 0) : (decimal?)null,
                        //    KhoaId = item.Key.KhoaId,
                        //    TenKhoa = item.First().TenKhoa
                        //})
                        .ToList();

                    var lstDoanhThuDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.GoiKhamSucKhoeId != null
                                    && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    && lstYeuCauTiepNhanId.Contains(x.YeuCauTiepNhanId)
                                    //&& x.ThoiDiemHoanThanh != null
                                    //&& x.ThoiDiemHoanThanh.Value >= tuNgay
                                    //&& x.ThoiDiemHoanThanh.Value <= denNgay)

                                    && (x.ThoiDiemHoanThanh != null || x.ThoiDiemKetLuan != null || x.ThoiDiemThucHien != null)
                                    && (x.ThoiDiemHoanThanh ?? x.ThoiDiemKetLuan ?? x.ThoiDiemThucHien) >= tuNgay
                                    && (x.ThoiDiemHoanThanh ?? x.ThoiDiemKetLuan ?? x.ThoiDiemThucHien) <= denNgay)
                        .Select(item => new DoanhThuTheoDichVuThucTeVo()
                        {
                            YeucauTiepNhanId = item.YeuCauTiepNhanId,
                            NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                            NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                            TenDichVu = item.TenDichVu,
                            LaDichVuXetNghiem = item.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem,
                            SoLan = item.SoLan,
                            GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                            DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
                            NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                            KhoaId = item.NoiThucHien.KhoaPhongId,
                            TenKhoa = item.NoiThucHien.KhoaPhong.Ten
                        })
                        //.GroupBy(x => new { x.YeucauTiepNhanId, x.KhoaId })
                        //.Select(item => new DoanhThuTheoDichVuVo()
                        //{
                        //    YeucauTiepNhanId = item.Key.YeucauTiepNhanId,
                        //    NhomDichVuBenhVienId = item.First().NhomDichVuBenhVienId,
                        //    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                        //    LaDichVuXetNghiem = item.First().LaDichVuXetNghiem,
                        //    DoanhThu = item.Any(a => a.DoanhThu != null && a.DoanhThu != 0) ? item.Sum(a => a.DoanhThu ?? 0) : (decimal?)null,
                        //    KhoaId = item.Key.KhoaId,
                        //    TenKhoa = item.First().TenKhoa
                        //})
                        .ToList();

                    var goiKhamSucKhoeDichVuKhamBenhIds = lstDoanhThuDichVuKham.Select(o => o.GoiKhamSucKhoeId.GetValueOrDefault()).Distinct().ToList();
                    var goiKhamSucKhoeDichVuKhamBenhs = _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                        .Where(o => goiKhamSucKhoeDichVuKhamBenhIds.Contains(o.GoiKhamSucKhoeId))
                        .Select(o => new
                        {
                            o.GoiKhamSucKhoeId,
                            o.DichVuKhamBenhBenhVienId,
                            o.NhomGiaDichVuKhamBenhBenhVienId,
                            o.DonGiaThucTe
                        }).ToList();
                    var goiKhamSucKhoeDichVuKyThuatIds = lstDoanhThuDichVuKyThuat.Select(o => o.GoiKhamSucKhoeId.GetValueOrDefault()).Distinct().ToList();
                    var goiKhamSucKhoeDichVuKyThuats = _goiKhamSucKhoeDichVuKyThuatRepository.TableNoTracking
                        .Where(o => goiKhamSucKhoeDichVuKyThuatIds.Contains(o.GoiKhamSucKhoeId))
                        .Select(o => new
                        {
                            o.GoiKhamSucKhoeId,
                            o.DichVuKyThuatBenhVienId,
                            o.NhomGiaDichVuKyThuatBenhVienId,
                            o.DonGiaThucTe
                        }).ToList();

                    foreach(var doanhThuDichVuKham in lstDoanhThuDichVuKham)
                    {
                        var goiKhamSucKhoeDichVuKhamBenh = goiKhamSucKhoeDichVuKhamBenhs
                            .FirstOrDefault(o => o.GoiKhamSucKhoeId == doanhThuDichVuKham.GoiKhamSucKhoeId && o.DichVuKhamBenhBenhVienId == doanhThuDichVuKham.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == doanhThuDichVuKham.NhomGiaDichVuKhamBenhBenhVienId);
                        doanhThuDichVuKham.DoanhThu = (goiKhamSucKhoeDichVuKhamBenh?.DonGiaThucTe ?? 0) * doanhThuDichVuKham.SoLan;
                    }
                    foreach (var doanhThuDichKyThuat in lstDoanhThuDichVuKyThuat)
                    {
                        var goiKhamSucKhoeDichVuKyThuat = goiKhamSucKhoeDichVuKyThuats
                            .FirstOrDefault(o => o.GoiKhamSucKhoeId == doanhThuDichKyThuat.GoiKhamSucKhoeId && o.DichVuKyThuatBenhVienId == doanhThuDichKyThuat.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == doanhThuDichKyThuat.NhomGiaDichVuKyThuatBenhVienId);
                        doanhThuDichKyThuat.DoanhThu = (goiKhamSucKhoeDichVuKyThuat?.DonGiaThucTe ?? 0) * doanhThuDichKyThuat.SoLan;
                    }

                    var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
                    foreach (var yeuCauTiepNhan in results)
                    {
                        yeuCauTiepNhan.CauHinhBaoCao = cauHinhBaoCao;
                        yeuCauTiepNhan.DoanhThuTheoDichVus.AddRange(
                            lstDoanhThuDichVuKham.Where(x => x.YeucauTiepNhanId == yeuCauTiepNhan.Id)
                                .Union(lstDoanhThuDichVuKyThuat.Where(x =>
                                    x.YeucauTiepNhanId == yeuCauTiepNhan.Id)));
                    }
                }
            }
            return new GridDataSource
            {
                Data = results.ToArray(),
                TotalRowCount = countPage
            };
        }

        public GridDataSource GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeForGrid(QueryInfo queryInfo)
        {
            var thongTinKhams = new List<BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo>();
            var timKiemNangCaoObj = new BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeQueryInfo>(queryInfo.AdditionalSearchString);
            }

            timKiemNangCaoObj.Skip = queryInfo.Skip;
            timKiemNangCaoObj.Take = queryInfo.Take;
            return GetDataDoanhThuKhamDoanThucTe(timKiemNangCaoObj);
        }

        public virtual byte[] ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeQueryInfo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeQueryInfo>(query.AdditionalSearchString);
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

            var datas = (ICollection<BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo>("STT", p => ind++)
            };

            #region khởi tạo list key column
            var keyColTheoKhoas = new List<ColumnExcelInfoVo>();
            string[] arrColumnDefault = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            var colGioiTinh = new ColumnExcelInfoVo()
            {
                ColumnKey = "GioiTinh",
                ColumnName = "F"
            };
            keyColTheoKhoas.Add(colGioiTinh); //col giới tính là mặc định
            #endregion

            #region get list khoa có doanh thu
            var lstKhoa = datas
                .SelectMany(x => x.DoanhThuTheoKhoas)
                .Select(item => new DoanhThuTheoKhoaThucTeVo
                {
                    KhoaId = item.KhoaId,
                    TenKhoa = item.TenKhoa
                })
                .GroupBy(x => new { x.KhoaId })
                .Select(item => new DoanhThuTheoKhoaThucTeVo
                {
                    KhoaId = item.Key.KhoaId,
                    TenKhoa = item.First().TenKhoa
                }).ToList();

            lstKhoa.Add(new DoanhThuTheoKhoaThucTeVo
            {
                KhoaId = -1,
                TenKhoa = "Tổng Cộng"
            });
            #endregion

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[KT] BC THDT KHÁM ĐOÀN THEO KHOA/PHÒNG GIA THUC TẾ");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 10;
                    worksheet.DefaultColWidth = 20;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:N3"])
                    {
                        range.Worksheet.Cells["A3:N3"].Merge = true;
                        range.Worksheet.Cells["A3:N3"].Value = "BÁO CÁO TỔNG HỢP DOANH THU KHÁM ĐOÀN THEO KHOA/ PHÒNG";
                        range.Worksheet.Cells["A3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:N3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:N4"])
                    {
                        range.Worksheet.Cells["A4:N4"].Merge = true;
                        range.Worksheet.Cells["A4:N4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                       + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:N4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:N4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:N6"])
                    {
                        range.Worksheet.Cells["A6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:F6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:F6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:F6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //del ?
                        range.Worksheet.Cells["B6"].Value = "Tên công ty";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã TN";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Họ và tên";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Năm sinh";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Giới tính";

                        // xử lý tạo colimn động theo khoa phòng
                        int lanLapTenCot = 0;
                        //for (DateTime ngayKetQua = query.TuNgay.Value; ngayKetQua <= query.DenNgay.Value; ngayKetQua = ngayKetQua.AddDays(1))
                        foreach (var khoa in lstKhoa)
                        {
                            string keyByKhoaId = khoa.KhoaId.ToString();
                            string columnNameNew = "";

                            if (lanLapTenCot > 0)
                            {
                                //columnNameNew = "A";
                                for (int k = 0; k < lanLapTenCot; k++)
                                {
                                    columnNameNew += "A";
                                }
                            }

                            var lastColumnName = keyColTheoKhoas.Last().ColumnName;
                            if (lastColumnName.EndsWith('Z'))
                            {
                                columnNameNew += (lanLapTenCot == 0 ? "AA" : (columnNameNew + "A"));
                                lanLapTenCot++;
                            }
                            else
                            {
                                if (lanLapTenCot > 0)
                                {
                                    lastColumnName = lastColumnName.Substring(lastColumnName.Length - 1, 1);
                                }
                                var indexLastColumnName = arrColumnDefault.IndexOf(lastColumnName);
                                var columnNameNext = arrColumnDefault[indexLastColumnName + 1];
                                columnNameNew += columnNameNext;
                            }

                            var newColumn = new ColumnExcelInfoVo()
                            {
                                ColumnKey = keyByKhoaId,
                                ColumnName = columnNameNew
                            };
                            keyColTheoKhoas.Add(newColumn);

                            // xử lý add cột mới vào file excel
                            range.Worksheet.Cells[columnNameNew + "6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[columnNameNew + "6"].Value = khoa.TenKhoa;
                            range.Worksheet.Cells[columnNameNew + "6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[columnNameNew + "6"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells[columnNameNew + "6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[columnNameNew + "6"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[columnNameNew + "6"].Style.Font.Bold = true;
                        }
                    }

                    //write data from line 7
                    int index = 7;
                    int stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.TenCongTy;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.MaTN;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index].Value = item.HoTen;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["E" + index].Value = item.NamSinh;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["F" + index].Value = item.GioiTinh;

                            #region đổ data
                            foreach (var col in keyColTheoKhoas)
                            {
                                if (col.ColumnKey == colGioiTinh.ColumnKey)
                                {
                                    continue;
                                }

                                worksheet.Cells[col.ColumnName + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells[col.ColumnName + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells[col.ColumnName + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[col.ColumnName + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[col.ColumnName + index].Style.Numberformat.Format = "#,##0.00";
                                if (long.Parse(col.ColumnKey) == -1)
                                {
                                    worksheet.Cells[col.ColumnName + index].Style.Font.Color.SetColor(Color.Red);
                                    worksheet.Cells[col.ColumnName + index].Value = item.TongDoanhThuTheoKhoa;
                                }
                                else
                                {
                                    var sumDoanhThu = item.DoanhThuTheoKhoas.Where(a => a.KhoaId == long.Parse(col.ColumnKey) && a.DoanhThu != null && a.DoanhThu != 0).Sum(a => a.DoanhThu);                                   
                                    worksheet.Cells[col.ColumnName + index].Value = sumDoanhThu == 0 ? (decimal?)null : sumDoanhThu;
                                }
                            }

                            #endregion

                            stt++;
                            index++;
                        }

                        //total
                        worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;


                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["D" + index].Style.Font.Bold = true;
                        worksheet.Cells["D" + index].Value = "Tổng cộng";

                        #region đổ data tổng cộng
                        foreach (var col in keyColTheoKhoas)
                        {
                            if (col.ColumnKey == colGioiTinh.ColumnKey)
                            {
                                continue;
                            }

                            worksheet.Cells[col.ColumnName + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells[col.ColumnName + index].Style.Font.Color.SetColor(Color.Red);
                            worksheet.Cells[col.ColumnName + index].Style.Font.Bold = true;
                            worksheet.Cells[col.ColumnName + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[col.ColumnName + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[col.ColumnName + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells[col.ColumnName + index].Style.Numberformat.Format = "#,##0.00";
                            if (long.Parse(col.ColumnKey) == -1)
                            {
                                worksheet.Cells[col.ColumnName + index].Value = datas.SelectMany(a => a.DoanhThuTheoKhoas).Sum(a => a.DoanhThu);
                            }
                            else
                            {
                                worksheet.Cells[col.ColumnName + index].Value = datas.SelectMany(a => a.DoanhThuTheoKhoas).Where(a => a.KhoaId == long.Parse(col.ColumnKey)).Sum(a => a.DoanhThu);
                            }
                        }

                        #endregion
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}