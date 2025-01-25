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
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCaos;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoBangKePhieuXuatKhoForGridAsync(BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo, bool exportExcel = false)
        {
            var xuatKhoDuocPhamQuery = _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.KhoXuatId == queryInfo.KhoId && o.NgayXuat >= queryInfo.FromDate && o.NgayXuat < queryInfo.ToDate);

            var xuatKhoDuocPhamData = xuatKhoDuocPhamQuery.Select(o => new BaoCaoBangKePhieuXuatKhoQueryData
            {
                PhieuXuatId = o.Id,
                SoPhieu = o.SoPhieu,
                KhoXuat = o.KhoDuocPhamXuat.Ten,
                KhoNhap = o.KhoNhapId != null ? o.KhoDuocPhamNhap.Ten : "",
                TraNCC = o.TraNCC,
                XuatChoBenhNhan = o.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                NgayXuat = o.NgayXuat,
                BaoCaoBangKePhieuXuatKhoGridVos = o.XuatKhoDuocPhamChiTiets.SelectMany(x => x.XuatKhoDuocPhamChiTietViTris)
                    .Select(y => new BaoCaoBangKePhieuXuatKhoGridVo
                    {
                        Id = y.Id,
                        DuocPhamId = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        MaDuoc = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                        TenDuoc = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = y.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLuong = y.SoLuongXuat,
                        DonGia = y.NhapKhoDuocPhamChiTiet.DonGiaTonKho
                    }).ToList()
            }).ToList();

            var xuatKhoVatTuQuery = _xuatKhoVatTuRepository.TableNoTracking
                .Where(o => o.KhoXuatId == queryInfo.KhoId && o.NgayXuat >= queryInfo.FromDate && o.NgayXuat < queryInfo.ToDate);

            var xuatKhoVatTuData = xuatKhoVatTuQuery.Select(o => new BaoCaoBangKePhieuXuatKhoQueryData
            {
                PhieuXuatId = o.Id,
                SoPhieu = o.SoPhieu,
                KhoXuat = o.KhoVatTuXuat.Ten,
                KhoNhap = o.KhoNhapId != null ? o.KhoVatTuNhap.Ten : "",
                TraNCC = o.TraNCC,
                XuatChoBenhNhan = o.LoaiXuatKho == Enums.EnumLoaiXuatKho.XuatChoBenhNhan,
                NgayXuat = o.NgayXuat,
                BaoCaoBangKePhieuXuatKhoGridVos = o.XuatKhoVatTuChiTiets.SelectMany(x => x.XuatKhoVatTuChiTietViTris)
                    .Select(y => new BaoCaoBangKePhieuXuatKhoGridVo
                    {
                        Id = y.Id,
                        DuocPhamId = y.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                        MaDuoc = y.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                        TenDuoc = y.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                        DVT = y.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                        SoLuong = y.SoLuongXuat,
                        DonGia = y.NhapKhoVatTuChiTiet.DonGiaTonKho
                    }).ToList()
            }).ToList();

            var allData = xuatKhoDuocPhamData.Concat(xuatKhoVatTuData).OrderBy(o => o.NgayXuat).ToList();

            var returnData = new List<BaoCaoBangKePhieuXuatKhoGridVo>();
            //var totalRowCount = allData.Count;
            //var selectData = exportExcel ? allData : allData.Skip(queryInfo.Skip).Take(queryInfo.Take);
            foreach (var queryData in allData)
            {
                var tenPhieu = string.IsNullOrEmpty(queryData.KhoNhap)
                    ? (queryData.TraNCC == true
                        ? $"{queryData.NgayXuat.ApplyFormatDate()} - {queryData.KhoXuat} trả nhà cung cấp - {queryData.SoPhieu}"
                        : (queryData.XuatChoBenhNhan ? $"{queryData.NgayXuat.ApplyFormatDate()} - {queryData.KhoXuat} xuất cho BN - {queryData.SoPhieu}" : $"{queryData.NgayXuat.ApplyFormatDate()} - {queryData.KhoXuat} xuất khác - {queryData.SoPhieu}"))
                    : $"{queryData.NgayXuat.ApplyFormatDate()} - {queryData.KhoXuat} xuất nội bộ {queryData.KhoNhap} - {queryData.SoPhieu}";
                var chiTiets = queryData.BaoCaoBangKePhieuXuatKhoGridVos.Where(o => o.SoLuong > 0)
                    .GroupBy(o => new { o.DuocPhamId, o.DonGia }, o => o, (k, v) => new BaoCaoBangKePhieuXuatKhoGridVo
                    {
                        PhieuXuatId = queryData.PhieuXuatId,
                        TenPhieu = tenPhieu,
                        DuocPhamId = k.DuocPhamId,
                        MaDuoc = v.First().MaDuoc,
                        TenDuoc = v.First().TenDuoc,
                        DVT = v.First().DVT,
                        SoLuong = v.Sum(o => o.SoLuong),
                        DonGia = k.DonGia,
                    });
                returnData.AddRange(chiTiets);
            }
           
            return new GridDataSource { Data = returnData.ToArray(), TotalRowCount = returnData.Count() };
        }

        //public async Task<GridDataSource> GetDataTotalPageBaoCaoBangKePhieuXuatKhoForGridAsync(BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo)
        //{
        //    var allData = await GetAllDataForBangKePhieuXuatKho(queryInfo);
        //    return new GridDataSource { TotalRowCount = allData.Count() };
        //}

        public virtual byte[] ExportBaoCaoBangKePhieuXuatKhoGridVo(GridDataSource gridDataSource, BaoCaoBangKePhieuXuatKhoQueryInfo query)
        {
            var datas = (ICollection<BaoCaoBangKePhieuXuatKhoGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoBangKePhieuXuatKhoGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO BẢNG KÊ PHIẾU XUẤT KHO");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 40;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
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
                    using (var range = worksheet.Cells["A3:G3"])
                    {
                        range.Worksheet.Cells["A3:G3"].Merge = true;
                        range.Worksheet.Cells["A3:G3"].Value = "BẢNG KÊ PHIẾU XUẤT KHO";
                        range.Worksheet.Cells["A3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:G3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:G3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Merge = true;
                        range.Worksheet.Cells["A4:G4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                         + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                    }

                    var tenKho = _khoRepository.TableNoTracking.Where(c => c.Id == query.KhoId).Select(c => c.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A5:G5"])
                    {
                        range.Worksheet.Cells["A5:G5"].Merge = true;
                        range.Worksheet.Cells["A5:G5"].Value = "Phạm vi:" + tenKho;
                        range.Worksheet.Cells["A5:G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:G5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:G5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:G7"])
                    {
                        range.Worksheet.Cells["A7:G7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:G7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:G7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:G7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Mã dược";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tân dược , hàm lượng";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "ĐVT";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Số lượng";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Đơn giá";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Thành tiền";
                    }


                    //write data from line 8
                    int index = 8;
                    var dataTheoKho = datas.GroupBy(x => x.PhieuXuatId).Select(x => x.Key);
                    var stt = 1;
                    var sttKho = 1;
                    var dateNow = DateTime.Now;
                    if (datas.Any())
                    {
                        foreach (var data in dataTheoKho)
                        {
                            stt = 1;
                            var listDataTheoKhoVTYT = datas.Where(x => x.PhieuXuatId == data).ToList();
                            if (listDataTheoKhoVTYT.Any())
                            {
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;
                                worksheet.Cells["A" + index + ":G" + index].Value = $"{sttKho}. {listDataTheoKhoVTYT.FirstOrDefault().TenPhieu}";
                                index++;

                                foreach (var item in listDataTheoKhoVTYT)
                                {
                                    
                                    // format border, font chữ,....
                                    worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Row(index).Height = 20.5;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = item.MaDuoc;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = item.TenDuoc;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = item.DVT;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Value = item.SoLuong;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["F" + index].Value = item.DonGia;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Value = item.ThanhTien;

                                    stt++;
                                    index++;
                                }
                                using (var range = worksheet.Cells["A" + index + ":F" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng:";
                                }

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["G" + index].Merge = true;
                                worksheet.Cells["G" + index].Style.Font.Bold = true;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = listDataTheoKhoVTYT.Sum(x => x.ThanhTien);
                                index++;
                            }
                            sttKho++;
                        }
                        //sumary datas
                        using (var range = worksheet.Cells["A" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng cộng:";
                        }

                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["G" + index].Merge = true;
                        worksheet.Cells["G" + index].Style.Font.Bold = true;
                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index].Value = datas.Sum(x => x.ThanhTien);
                        index += 2;

                        using (var range = worksheet.Cells["E" + index + ":G" + index])
                        {
                            range.Worksheet.Cells["E" + index + ":G" + index].Merge = true;
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["E" + index + ":G" + index].Value = "Ngày " + dateNow.Day + " tháng " + dateNow.Month + " năm " + dateNow.Year;
                            index++;
                        }

                        using (var range = worksheet.Cells["E" + index + ":G" + index])
                        {
                            range.Worksheet.Cells["E" + index + ":G" + index].Merge = true;
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["E" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["E" + index + ":G" + index].Value = "Người lập";
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