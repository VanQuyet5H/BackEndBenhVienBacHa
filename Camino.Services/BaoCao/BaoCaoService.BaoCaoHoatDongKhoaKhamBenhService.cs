using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoHoatDongKhoaKhamBenhForGridAsync(BaoCaoHoaDongKhoaKhamBenhQueryInfo queryInfo)
        {
            //var cauHinhKhoaKhamBenh = _cauHinhService.GetByName("CauHinhBaoCao.KhoaKhamBenh");
            //long.TryParse(cauHinhKhoaKhamBenh?.Value, out long khoaKhamBenhId);

            //var phongBenhViens = _phongBenhVienRepository.TableNoTracking
            //    .Include(x => x.YeuCauKhamBenhNoiDangKys).ThenInclude(x => x.YeuCauTiepNhan)
            //    .Where(x => x.YeuCauKhamBenhNoiDangKys.Any(a => a.ThoiDiemDangKy >= queryInfo.FromDate
            //                                                      && a.ThoiDiemDangKy <= queryInfo.ToDate
            //                                                      && a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
            //    .ToList();
            var now = DateTime.Now;
            //BVHD-3635
            var cauHinhICDKhamSucKhoe = _cauHinhService.GetSetting("CauHinhKhamSucKhoe.IcdKhamSucKhoe");
            long.TryParse(cauHinhICDKhamSucKhoe?.Value, out long icdKhamSucKhoeId);
            var result = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x =>
                        //a.ThoiDiemDangKy >= queryInfo.FromDate
                        //&& a.ThoiDiemDangKy <= queryInfo.ToDate
                        ((queryInfo.FromDate != null && queryInfo.ToDate != null 
                                                     && x.ThoiDiemThucHien != null 
                                                     && x.ThoiDiemThucHien >= queryInfo.FromDate 
                                                     && x.ThoiDiemThucHien <= queryInfo.ToDate)
                                || (queryInfo.FromDateHoanThanh != null && queryInfo.ToDateHoanThanh != null 
                                                                        && x.ThoiDiemHoanThanh != null 
                                                                        && x.ThoiDiemHoanThanh >= queryInfo.FromDateHoanThanh 
                                                                        && x.ThoiDiemHoanThanh <= queryInfo.ToDateHoanThanh))
                            //&& x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                            && (x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
                .Select(item => new BaoCaoHoatDongKhoaKhamBenhGridVo()
                {
                    Id = item.NoiDangKyId ?? 0,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    PhongBenhVien = item.NoiDangKy.Ten + (!string.IsNullOrEmpty(item.NoiDangKy.Tang) ? $" ({item.NoiDangKy.Tang})" : ""),
                    Bhyt = (item.DuocHuongBaoHiem && item.GoiKhamSucKhoeId == null && item.IcdchinhId != icdKhamSucKhoeId && item.ChanDoanSoBoICDId != icdKhamSucKhoeId) ? 1 : (int?)null,
                    VienPhi = (!item.DuocHuongBaoHiem  && item.GoiKhamSucKhoeId == null && item.IcdchinhId != icdKhamSucKhoeId && item.ChanDoanSoBoICDId != icdKhamSucKhoeId) ? 1 : (int?)null,
                    //KskDoan = null, // hiện tại chưa xử lý trường hợp này
                    KskDoanCongTy = (item.GoiKhamSucKhoeId != null
                                     && item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.LoaiHopDong == (int)Enums.LoaiHopDong.KhamSucKhoeCongTy) ? 1 : (int?)null,
                    //BVHD-3635
                    KskBHTN = (item.GoiKhamSucKhoeId == null && (item.IcdchinhId == icdKhamSucKhoeId || item.ChanDoanSoBoICDId == icdKhamSucKhoeId)) ? 1 : (int?)null,
                    GiayKsk = null, // hiện tại chưa xử lý trường hợp này
                    Goi = item.YeuCauGoiDichVuId != null ? 1 : (int?)null,
                    TreEm = (item.YeuCauTiepNhan.NamSinh != null 
                             && (((now.Year - item.YeuCauTiepNhan.NamSinh) < 6)
                                || (((now.Year - item.YeuCauTiepNhan.NamSinh) == 6)
                                    && (item.YeuCauTiepNhan.ThangSinh == null || ((now.Month - item.YeuCauTiepNhan.ThangSinh) >= 0))
                                    && (item.YeuCauTiepNhan.NgaySinh == null || ((now.Day - item.YeuCauTiepNhan.NgaySinh) >= 0)))))
                            ? 1 : (int?)null,
                    SoLanCapCuu = null, // cần cập nhật
                    SoNguoiBenhVaoVien = (item.CoNhapVien == true && item.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(b => b.NoiTruBenhAn != null))) ? 1 : (int?)null,
                    SoNguoiBenhChuyenVien = (item.CoChuyenVien == true 
                                             || item.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(b => b.NoiTruBenhAn.HinhThucRaVien == Enums.EnumHinhThucRaVien.ChuyenVien))) ? 1 : (int?)null,
                    SoNguoiBenhTuVong = (item.CoTuVong == true
                                         || item.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(b => b.NoiTruBenhAn.KetQuaDieuTri == Enums.EnumKetQuaDieuTri.TuVong))) ? 1 : (int?)null,
                    SoNguoiBenhDieuTriNgoaiTru = null, // cần cập nhật
                    SoNgayDieuTriNgoaiTru = null // cần cập nhật
                })
                .OrderBy(x => x.PhongBenhVien)
                .ToList();
            var datas = result
                .GroupBy(x => x.PhongBenhVien)
                .Select(item => new BaoCaoHoatDongKhoaKhamBenhGridVo()
                {
                    Id = item.First().Id,
                    PhongBenhVien = item.Key,
                    BhytCoGoi = item.Any(a => a.Bhyt != null && a.Goi != null) ? item.Count(a => a.Bhyt != null && a.Goi != null) : (int?)null,
                    BhytKhongGoi = item.Any(a => a.Bhyt != null && a.Goi == null) ? item.Count(a => a.Bhyt != null && a.Goi == null) : (int?)null,
                    VienPhiCoGoi = item.Any(a => a.VienPhi != null && a.Goi != null) ? item.Count(a => a.VienPhi != null && a.Goi != null) : (int?)null,
                    VienPhiKhongGoi = item.Any(a => a.VienPhi != null && a.Goi == null) ? item.Count(a => a.VienPhi != null && a.Goi == null) : (int?)null,
                    KskDoanCongTy = item.Any(a => a.KskDoanCongTy != null) ? item.Sum(a => a.KskDoanCongTy) : (int?)null,
                    KskBHTN = item.Any(a => a.KskBHTN != null) ? item.Sum(a => a.KskBHTN) : (int?)null,
                    GiayKsk = item.Any(a => a.GiayKsk != null) ? item.Sum(a => a.GiayKsk) : (int?)null,
                    TreEm = item.Any(a => a.TreEm != null) ? item.Sum(a => a.TreEm) : (int?)null,

                    // phần này sẽ tính theo lần tiếp nhận chứ ko phải theo lần khám
                    SoLanCapCuu = null,
                    SoNguoiBenhVaoVien = null,
                    SoNguoiBenhChuyenVien = null,
                    SoNguoiBenhDieuTriNgoaiTru = null,
                    SoNgayDieuTriNgoaiTru = null,

                    //Bhyt = item.Any(a => a.Bhyt != null) ? item.Sum(a => a.Bhyt) : (int?)null,
                    //VienPhi = item.Any(a => a.VienPhi != null) ? item.Sum(a => a.VienPhi) : (int?)null,
                    //GiayKsk = item.Any(a => a.GiayKsk != null) ? item.Sum(a => a.GiayKsk) : (int?)null,
                    //KskDoanCongTy = item.Any(a => a.KskDoanCongTy != null) ? item.Sum(a => a.KskDoanCongTy) : (int?)null,
                    //Goi = item.Any(a => a.Goi != null) ? item.Sum(a => a.Goi) : (int?)null,
                    //TreEm = item.Any(a => a.TreEm != null) ? item.Sum(a => a.TreEm) : (int?)null,
                    //SoLanCapCuu = item.Any(a => a.SoLanCapCuu != null) ? item.Sum(a => a.SoLanCapCuu) : (int?)null,
                    //SoNguoiBenhVaoVien = item.Any(a => a.SoNguoiBenhVaoVien != null) ? item.Sum(a => a.SoNguoiBenhVaoVien) : (int?)null,
                    //SoNguoiBenhChuyenVien = item.Any(a => a.SoNguoiBenhChuyenVien != null) ? item.Sum(a => a.SoNguoiBenhChuyenVien) : (int?)null,
                    //SoNguoiBenhDieuTriNgoaiTru = item.Any(a => a.SoNguoiBenhDieuTriNgoaiTru != null) ? item.Sum(a => a.SoNguoiBenhDieuTriNgoaiTru) : (int?)null,
                    //SoNgayDieuTriNgoaiTru = item.Any(a => a.SoNgayDieuTriNgoaiTru != null) ? item.Sum(a => a.SoNgayDieuTriNgoaiTru) : (int?)null,
                    TongSo = item.Count()
                })
                .ToList();

            foreach (var data in datas)
            {
                var dataTheoTiepNhan = result.Where(x => x.Id == data.Id).GroupBy(x => new {x.YeuCauTiepNhanId}).Select(item => new BaoCaoHoatDongKhoaKhamBenhGridVo()
                {
                    Id = item.First().Id,
                    SoLanCapCuu = null,
                    SoNguoiBenhVaoVien = item.Any(a => a.SoNguoiBenhVaoVien != null) ? item.Sum(a => a.SoNguoiBenhVaoVien) : (int?)null,
                    SoNguoiBenhChuyenVien = item.Any(a => a.SoNguoiBenhChuyenVien != null) ? item.Sum(a => a.SoNguoiBenhChuyenVien) : (int?)null,
                    SoNguoiBenhTuVong = item.Any(a => a.SoNguoiBenhTuVong != null) ? item.Sum(a => a.SoNguoiBenhTuVong) : (int?)null,
                    SoNguoiBenhDieuTriNgoaiTru = null,
                    SoNgayDieuTriNgoaiTru = null
                })
                .ToList();

                data.SoLanCapCuu = null;
                data.SoNguoiBenhVaoVien = dataTheoTiepNhan.Any(a => a.SoNguoiBenhVaoVien != null) ? dataTheoTiepNhan.Sum(a => a.SoNguoiBenhVaoVien) : (int?)null;
                data.SoNguoiBenhChuyenVien = dataTheoTiepNhan.Any(a => a.SoNguoiBenhChuyenVien != null) ? dataTheoTiepNhan.Sum(a => a.SoNguoiBenhChuyenVien) : (int?)null;
                data.SoNguoiBenhTuVong = dataTheoTiepNhan.Any(a => a.SoNguoiBenhTuVong != null) ? dataTheoTiepNhan.Sum(a => a.SoNguoiBenhTuVong) : (int?)null;
                data.SoNguoiBenhDieuTriNgoaiTru = null;
                data.SoNgayDieuTriNgoaiTru = null;
            }

            return new GridDataSource
            {
                Data = datas.ToArray(),
                TotalRowCount = datas.Count()
            };
        }

        public virtual byte[] ExportBaoCaoHoatDongKhoaKhamBenh(GridDataSource gridDataSource, BaoCaoHoaDongKhoaKhamBenhQueryInfo query)
        {
            var datas = (ICollection<BaoCaoHoatDongKhoaKhamBenhGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoHoatDongKhoaKhamBenhGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG KHOA KHÁM BỆNH");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;
                    worksheet.Column(17).Width = 15;
                    worksheet.Column(17).Width = 15;
                    worksheet.Column(17).Width = 15;


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
                        range.Worksheet.Cells["D8:J8"].Value = "HOẠT ĐỘNG KHOA KHÁM BỆNH";
                        range.Worksheet.Cells["D8:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D8:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D8:J8"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["D8:J8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D8:J8"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A9:Q9"])
                    {
                        range.Worksheet.Cells["D9:J9"].Merge = true;
                        range.Worksheet.Cells["D9:J9"].Value = "Từ ngày: " + (query.FromDate ?? query.FromDateHoanThanh)?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + (query.ToDate ?? query.ToDateHoanThanh)?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["D9:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D9:J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D9:J9"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["D9:J9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D9:J9"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A11:P13"])
                    {
                        range.Worksheet.Cells["A11:Q11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A11:Q11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:Q11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:Q11"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A11:Q11"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A11:Q11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A12:Q12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A12:Q12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:Q12"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:Q12"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A12:Q12"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A12:Q12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A12:Q13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A12:Q13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A12:Q13"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A12:Q13"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A12:Q13"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A12:Q13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A11:A13"].Merge = true;
                        range.Worksheet.Cells["A11:A13"].Value = "STT";
                        range.Worksheet.Cells["A11:A13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A11:A13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B11:B13"].Merge = true;
                        range.Worksheet.Cells["B11:B13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C11:C13"].Merge = true;
                        range.Worksheet.Cells["C11:C13"].Value = "Tổng Số";
                        range.Worksheet.Cells["C11:C13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D11:K11"].Merge = true;
                        range.Worksheet.Cells["D11:K11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D11:K11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D11:K11"].Value = "Số Lần Khám";
                        range.Worksheet.Cells["D11:K11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D12:E12"].Merge = true;
                        range.Worksheet.Cells["D12:E12"].Value = "BHYT";
                        range.Worksheet.Cells["D12:E12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D12:E12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D13"].Value = "Có gói";
                        range.Worksheet.Cells["D13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E13"].Value = "Không gói";
                        range.Worksheet.Cells["E13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F12:G12"].Merge = true;
                        range.Worksheet.Cells["F12:G12"].Value = "Viện Phí";
                        range.Worksheet.Cells["F12:G12"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F12:G12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F13"].Value = "Có gói";
                        range.Worksheet.Cells["F13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G13"].Value = "Không gói";
                        range.Worksheet.Cells["G13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H12:H13"].Merge = true;
                        range.Worksheet.Cells["H12:H13"].Value = "KSK BHTN";
                        range.Worksheet.Cells["H12:H13"].Style.WrapText = true;
                        range.Worksheet.Cells["H12:H13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H12:H13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I12:I13"].Merge = true;
                        range.Worksheet.Cells["I12:I13"].Value = "KSK Đoàn, Công ty";
                        range.Worksheet.Cells["I12:I13"].Style.WrapText = true;
                        range.Worksheet.Cells["I12:I13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I12:I13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J12:J13"].Merge = true;
                        range.Worksheet.Cells["J12:J13"].Value = "Giấy KSK";
                        range.Worksheet.Cells["J12:J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J12:J13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K12:K13"].Merge = true;
                        range.Worksheet.Cells["K12:K13"].Value = "Tr.đó: Trẻ em <=6";
                        range.Worksheet.Cells["K12:K13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K12:K13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K12:K13"].Style.WrapText = true;


                        range.Worksheet.Cells["L11:L13"].Merge = true;
                        range.Worksheet.Cells["L11:L13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L11:L13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L11:L13"].Style.WrapText = true;
                        range.Worksheet.Cells["L11:L13"].Value = "Số Lần Cấp Cứu";
                        range.Worksheet.Cells["L11:L13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M11:M13"].Merge = true;
                        range.Worksheet.Cells["M11:M13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M11:M13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M11:M13"].Style.WrapText = true;
                        range.Worksheet.Cells["M11:M13"].Value = "Số Người Bệnh Vào Viện";
                        range.Worksheet.Cells["M11:M13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N11:N13"].Merge = true;
                        range.Worksheet.Cells["N11:N13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N11:N13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N11:N13"].Style.WrapText = true;
                        range.Worksheet.Cells["N11:N13"].Value = "Số Người Bệnh Chuyển Viện";
                        range.Worksheet.Cells["N11:N13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O11:O13"].Merge = true;
                        range.Worksheet.Cells["O11:O13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O11:O13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O11:O13"].Style.WrapText = true;
                        range.Worksheet.Cells["O11:O13"].Value = "Số Người Bệnh Tử Vong";
                        range.Worksheet.Cells["O11:O13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P11:Q11"].Merge = true;
                        range.Worksheet.Cells["P11:Q11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["P11:Q11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P11:Q11"].Value = "Đ.Trị Ngoại Trú";
                        range.Worksheet.Cells["P11:Q11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P12:P13"].Merge = true;
                        range.Worksheet.Cells["P12:P13"].Value = "Số người bệnh";
                        range.Worksheet.Cells["P12:P13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P12:P13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P12:P13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q12:Q13"].Merge = true;
                        range.Worksheet.Cells["Q12:Q13"].Value = "Số ngày";
                        range.Worksheet.Cells["Q12:Q13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q12:Q13"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q12:Q13"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A14:Q14"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A14:B14"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C14:Q14"].Style.Font.SetFromFont(new Font("Times New Roman", 10));



                        //range.Worksheet.Cells["A12"].Value = "STT";
                        //range.Worksheet.Cells["A12"].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //range.Worksheet.Cells["A12"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B14"].Value = "Tổng Số";
                        range.Worksheet.Cells["B14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C14"].Value = datas.Sum(s => s.TongSo);
                        range.Worksheet.Cells["C14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C14"].Style.Numberformat.Format = "#,##";
                        range.Worksheet.Cells["C14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D14"].Value = datas.Sum(s => s.BhytCoGoi);
                        range.Worksheet.Cells["D14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["D14"].Style.Numberformat.Format = "#,##";
                        range.Worksheet.Cells["D14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E14"].Value = datas.Sum(s=>s.BhytKhongGoi);
                        range.Worksheet.Cells["E14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["E14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["F14"].Value = datas.Sum(s=>s.VienPhiCoGoi);
                        range.Worksheet.Cells["F14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["G14"].Value = datas.Sum(s => s.VienPhiKhongGoi);
                        range.Worksheet.Cells["G14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["G14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["H14"].Value = datas.Sum(s => s.KskBHTN);
                        range.Worksheet.Cells["H14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["H14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["I14"].Value = datas.Sum(s => s.KskDoanCongTy);
                        range.Worksheet.Cells["I14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["I14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["J14"].Value = datas.Sum(s => s.GiayKsk);
                        range.Worksheet.Cells["J14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["J14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["K14"].Value = datas.Sum(s => s.TreEm);
                        range.Worksheet.Cells["K14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["K14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["L14"].Value = datas.Sum(s => s.SoLanCapCuu);
                        range.Worksheet.Cells["L14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["L14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["M14"].Value = datas.Sum(s => s.SoNguoiBenhVaoVien);
                        range.Worksheet.Cells["M14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["M14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["N14"].Value = datas.Sum(s => s.SoNguoiBenhChuyenVien);
                        range.Worksheet.Cells["N14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["N14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["O14"].Value = datas.Sum(s => s.SoNguoiBenhTuVong);
                        range.Worksheet.Cells["O14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["O14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["P14"].Value = datas.Sum(s => s.SoNguoiBenhDieuTriNgoaiTru);
                        range.Worksheet.Cells["P14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["P14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["P14"].Style.Numberformat.Format = "#,##";

                        range.Worksheet.Cells["Q14"].Value = datas.Sum(s => s.SoNgayDieuTriNgoaiTru);
                        range.Worksheet.Cells["Q14"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q14"].Style.Font.Bold = true;
                        range.Worksheet.Cells["Q14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q14"].Style.Numberformat.Format = "#,##";
                    }

                    var manager = new PropertyManager<BaoCaoHoatDongKhoaKhamBenhGridVo>(requestProperties);
                    int index = 15;

                    ////Đổ dât vào bảng excel
                    ///
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach(var item in datas)
                        {
                            using (var range = worksheet.Cells["A"+ index + ":Q" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.PhongBenhVien;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["C" + index].Value = item.TongSo;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.BhytCoGoi;
                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.BhytKhongGoi;
                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.VienPhiCoGoi;
                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.VienPhiKhongGoi;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.KskBHTN;
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.KskDoanCongTy;
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.GiayKsk;
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.TreEm;
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.SoLanCapCuu;
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.SoNguoiBenhVaoVien;
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.SoNguoiBenhChuyenVien;
                                worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.SoNguoiBenhTuVong;
                                worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.SoNguoiBenhDieuTriNgoaiTru;
                                worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.SoNgayDieuTriNgoaiTru;
                                worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                index++;
                                stt++;
                            }
                        }
                    }

                    index++;
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    worksheet.Cells["K" + index + ":N" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["K" + index + ":N" + index].Style.Font.Italic = true;
                    worksheet.Cells["K" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":N" + index].Merge = true;

                    index = index + 2;

                    worksheet.Cells["A" + index + ":N" + (index + 1)].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":B" + (index + 1)].Value = $"NGƯỜI LẬP BIỂU\r\n (Ký, ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":B" + (index + 1)].Style.WrapText = true;
                    worksheet.Cells["A" + index + ":B" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":B" + (index + 1)].Merge = true;
                    worksheet.Cells["A" + index + ":B" + (index + 1)].Style.Font.Bold = true;


                    worksheet.Cells["E" + index + ":H" + (index + 1)].Value = $"TRƯỞNG PHÒNG KHTH\r\n (Ký, ghi rõ họ tên)";
                    worksheet.Cells["E" + index + ":H" + (index + 1)].Style.WrapText = true;
                    worksheet.Cells["E" + index + ":H" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["E" + index + ":H" + (index + 1)].Merge = true;
                    worksheet.Cells["E" + index + ":H" + (index + 1)].Style.Font.Bold = true;


                    worksheet.Cells["K" + index + ":N" + (index + 1)].Value = $"GIÁM ĐỐC\r\n (Ký, ghi rõ họ tên)";
                    worksheet.Cells["K" + index + ":N" + (index + 1)].Style.WrapText = true;
                    worksheet.Cells["K" + index + ":N" + (index + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":N" + (index + 1)].Merge = true;
                    worksheet.Cells["K" + index + ":N" + (index + 1)].Style.Font.Bold = true;



                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
