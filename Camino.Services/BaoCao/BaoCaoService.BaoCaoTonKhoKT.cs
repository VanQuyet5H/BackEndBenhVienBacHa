using Camino.Core.Domain.ValueObject.BaoCao;
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
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private async Task<List<BaoCaoTonKhoKTGridVo>> GetAllDataForTonKhoKT(BaoCaoTonKhoKTQueryInfo queryInfo)
        {
            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate)
                    .Select(o => new BaoCaoChiTietTonKhoKTGridVo
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        Ma = o.DuocPhamBenhViens.Ma,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        HanSuDungDateTime = o.HanSuDung,
                        NgayNhapXuat = o.NgayNhap,
                        LaVatTuBHYT = o.LaDuocPhamBHYT,
                        Nhom = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuatDuocPham = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)))
                .Select(o => new BaoCaoChiTietTonKhoKTGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanSuDungDateTime = o.NhapKhoDuocPhamChiTiet.HanSuDung,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    LaVatTuBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    Nhom = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoVatTu.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate)
                    .Select(o => new BaoCaoChiTietTonKhoKTGridVo
                    {
                        Id = o.Id,
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        Ma = o.VatTuBenhVien.Ma,
                        Ten = o.VatTuBenhVien.VatTus.Ten,
                        DVT = o.VatTuBenhVien.VatTus.DonViTinh,
                        SoLo = o.Solo,
                        HanSuDungDateTime = o.HanSuDung,
                        NgayNhapXuat = o.NgayNhap,
                        LaVatTuBHYT = o.LaVatTuBHYT,
                        Nhom = o.VatTuBenhVien.VatTus.NhomVatTu.Ten,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= queryInfo.ToDate)))
                .Select(o => new BaoCaoChiTietTonKhoKTGridVo
                {
                    Id = o.Id,
                    VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = o.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    Ten = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    DVT = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    SoLo = o.NhapKhoVatTuChiTiet.Solo,
                    HanSuDungDateTime = o.NhapKhoVatTuChiTiet.HanSuDung,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                    LaVatTuBHYT = o.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    Nhom = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhapDuocPham.Concat(allDataXuatDuocPham).Concat(allDataNhap).Concat(allDataXuat).OrderBy(o => o.Ten).ThenBy(o => o.HanSuDungDateTime).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.LaVatTuBHYT, o.DuocPhamBenhVienId, o.VatTuBenhVienId });
            var dataReturn = new List<BaoCaoTonKhoKTGridVo>();
            foreach (var xuatNhapVatTu in allDataGroup)
            {
                var tonDau = xuatNhapVatTu.Where(o => o.NgayNhapXuat < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = xuatNhapVatTu.Where(o => o.NgayNhapXuat >= queryInfo.FromDate).ToList();
                var baoCaoTonKhoGridVo = new BaoCaoTonKhoKTGridVo
                {
                    MaVTYT = xuatNhapVatTu.First().Ma,
                    TenVTYT = xuatNhapVatTu.First().Ten,
                    DVT = xuatNhapVatTu.First().DVT,
                    //SoLo = xuatNhapVatTu.Key.SoLo,
                    //HanSuDung = xuatNhapVatTu.Key.HanSuDung.ApplyFormatDate(),
                    TonDau = tonDau,
                    Nhap = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Xuat = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Loai = xuatNhapVatTu.Key.LaVatTuBHYT ? (xuatNhapVatTu.Key.DuocPhamBenhVienId != 0 ? "Dược phẩm BHYT" : "Vật tư BHYT") : (xuatNhapVatTu.Key.DuocPhamBenhVienId != 0 ? "Dược phẩm viện phí" : "Vật tư viện phí"),
                    Nhom = xuatNhapVatTu.First().Nhom
                };
                if (baoCaoTonKhoGridVo.TonCuoi != null && !baoCaoTonKhoGridVo.TonCuoi.Value.AlmostEqual(0))
                {
                    dataReturn.Add(baoCaoTonKhoGridVo);
                }
            }
            return dataReturn;
        }

        public async Task<GridDataSource> GetDataBaoCaoTonKhoKTForGridAsync(BaoCaoTonKhoKTQueryInfo queryInfo)
        {
            var allData = await GetAllDataForTonKhoKT(queryInfo);
            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = allData.Count() };
        }

        public async Task<GridDataSource> GetDataTotalPageBaoCaoTonKhoKTForGridAsync(BaoCaoTonKhoKTQueryInfo queryInfo)
        {
            var allData = await GetAllDataForTonKhoKT(queryInfo);
            return new GridDataSource { TotalRowCount = allData.Count() };
        }

        public virtual byte[] ExportBaoCaoTonKhoKTGridVo(GridDataSource gridDataSource, BaoCaoTonKhoKTQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTonKhoKTGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTonKhoKTGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỒN KHO");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.DefaultColWidth = 7;

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
                    using (var range = worksheet.Cells["A3:H3"])
                    {
                        range.Worksheet.Cells["A3:H3"].Merge = true;
                        range.Worksheet.Cells["A3:H3"].Value = "BÁO CÁO TỒN KHO";
                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    var kho = _khoRepository.TableNoTracking.Where(c=>c.Id == query.KhoId).Select(o=> o.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A4:H4"])
                    {
                        range.Worksheet.Cells["A4:H4"].Merge = true;
                        range.Worksheet.Cells["A4:H4"].Value = "Kho: " + kho;
                        range.Worksheet.Cells["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:H4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:H5"])
                    {
                        range.Worksheet.Cells["A5:H5"].Merge = true;
                        range.Worksheet.Cells["A5:H5"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                      + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A5:H5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:H5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:H5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:H5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:H7"])
                    {
                        range.Worksheet.Cells["A7:H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:H7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:H7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:H7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Mã VTYT";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tên thuốc, nồng độ , hàm lượng";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "ĐVT";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Tồn đầu";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Nhập";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Xuất";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Tồn cuối";
                    }


                    //write data from line 9       
                    int index = 8;
                    var stt = 1;
                    var lstLoai = datas.GroupBy(x => new { x.Loai })
                       .Select(item => new LoaiGroupVo
                       {
                           Loai = item.First().Loai,
                           Nhom = item.First().Nhom
                       }).OrderByDescending(p => p.Loai).ToList();
                    if (lstLoai.Any())
                    {
                        foreach (var loai in lstLoai)
                        {
                            //var lstLoaiTheoDatas = datas.Where(o => o.Loai == loai.Loai);
                            //if (lstLoaiTheoDatas.Any())
                            //{
                            using (var range = worksheet.Cells["A" + index + ":L" + index])
                            {
                                worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                worksheet.Cells["A" + index + ":H" + index].Value = loai.Loai;
                                //}
                                index++;

                                var lstNhomTheoLoai = datas.Where(o => o.Loai == loai.Loai)
                               .GroupBy(x => new { x.Loai, x.Nhom })
                                  .Select(item => new NhomGroupVo
                                  {
                                      Loai = item.First().Loai,
                                      Nhom = item.First().Nhom

                                  }).OrderByDescending(p => p.Nhom).ToList(); //demo desc
                                if (lstNhomTheoLoai.Any())
                                {
                                    foreach (var nhom in lstNhomTheoLoai)
                                    {
                                        worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                        worksheet.Cells["A" + index + ":H" + index].Value = nhom.Nhom;
                                        index++;

                                        var listDuocPhamTheoNhom = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();
                                        if (listDuocPhamTheoNhom.Any())
                                        {
                                            foreach (var item in listDuocPhamTheoNhom)
                                            {
                                                // format border, font chữ,....
                                                worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                                worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                                worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Row(index).Height = 20.5;

                                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A" + index].Value = stt;

                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["B" + index].Value = item.MaVTYT;

                                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["C" + index].Value = item.TenVTYT;

                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["D" + index].Value = item.DVT;

                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["E" + index].Value = item.TonDau;

                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["F" + index].Value = item.Nhap;

                                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["G" + index].Value = item.Xuat;

                                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["H" + index].Value = item.TonCuoi;
                                                stt++;
                                                index++;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var date = DateTime.Now;
                        index += 2;
                        using (var range = worksheet.Cells["F" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["F" + index + ":H" + index].Merge = true;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["F" + index + ":H" + index].Value = $"Ngày {date.Day}  tháng {date.Month}  năm {date.Year}";
                        }

                        index++;
                        using (var range = worksheet.Cells["A" + index + ":B" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = "Trưởng khoa";
                        }

                        using (var range = worksheet.Cells["C" + index + ":E" + index])
                        {
                            range.Worksheet.Cells["C" + index + ":E" + index].Merge = true;
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["C" + index + ":E" + index].Value = "Thủ kho";
                        }

                        using (var range = worksheet.Cells["F" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["F" + index + ":H" + index].Merge = true;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["F" + index + ":H" + index].Value = "Người lập";
                        }

                        index++;
                        using (var range = worksheet.Cells["A" + index + ":B" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["C" + index + ":E" + index])
                        {
                            range.Worksheet.Cells["C" + index + ":E" + index].Merge = true;
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["C" + index + ":E" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["C" + index + ":E" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["F" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["F" + index + ":H" + index].Merge = true;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["F" + index + ":H" + index].Value = "(Ký, ghi rõ họ tên)";
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}