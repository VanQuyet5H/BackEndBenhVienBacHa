using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoThuocSapHetHanForGridAsync(BaoCaoThuocSapHetHanQueryInfo queryInfo)
        {
            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.HanSuDung >= queryInfo.FromDate && o.HanSuDung < queryInfo.ToDate)
                    .Select(o => new BaoCaoThuocSapHetHanGridVo
                    {
                        Id = o.Id,
                        MaDuoc = o.DuocPhamBenhViens.Ma,
                        TenThuoc = o.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        HanDung = o.HanSuDung,
                        NhomThuoc = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        DonGia = o.DonGiaTonKho,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                            && o.NhapKhoDuocPhamChiTiet.HanSuDung >= queryInfo.FromDate && o.NhapKhoDuocPhamChiTiet.HanSuDung < queryInfo.ToDate)
                .Select(o => new BaoCaoThuocSapHetHanGridVo
                {
                    Id = o.Id,
                    MaDuoc = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    TenThuoc = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanDung = o.NhapKhoDuocPhamChiTiet.HanSuDung,
                    NhomThuoc = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    DonGia = o.NhapKhoDuocPhamChiTiet.DonGiaTonKho,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.MaDuoc, o.TenThuoc, o.SoLo, o.HanDung, o.DonGia });

            var dataReturn = new List<BaoCaoThuocSapHetHanGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var baoCaoThuocSapHetHanGridVo = new BaoCaoThuocSapHetHanGridVo
                {
                    MaDuoc = xuatNhapDuocPham.Key.MaDuoc,
                    TenThuoc = xuatNhapDuocPham.Key.TenThuoc,
                    HamLuong = xuatNhapDuocPham.First().HamLuong,
                    DVT = xuatNhapDuocPham.First().DVT,
                    SoLo = xuatNhapDuocPham.Key.SoLo,
                    HanDung = xuatNhapDuocPham.Key.HanDung,
                    SoLuong = xuatNhapDuocPham.Select(x => x.SLNhap - x.SLXuat).DefaultIfEmpty().Sum(),
                    DonGia = xuatNhapDuocPham.Key.DonGia,
                    NhomThuoc = xuatNhapDuocPham.First().NhomThuoc
                };
                if (!baoCaoThuocSapHetHanGridVo.SoLuong.AlmostEqual(0) && baoCaoThuocSapHetHanGridVo.SoLuong > 0)
                {
                    dataReturn.Add(baoCaoThuocSapHetHanGridVo);
                }
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.NhomThuoc).ThenBy(s => s.MaDuoc).ToArray(), TotalRowCount = dataReturn.Count };
        }

        public virtual byte[] ExportBaoCaoThuocSapHetHan(GridDataSource gridDataSource, BaoCaoThuocSapHetHanQueryInfo query)
        {
            var datas = (ICollection<BaoCaoThuocSapHetHanGridVo>)gridDataSource.Data;
            var nhomThuocs = datas.GroupBy(s => s.NhomThuoc).Select(s => s.First().NhomThuoc).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO THUỐC SẮP HẾT HẠN DÙNG");

                    //set row
                    worksheet.DefaultRowHeight = 16;

                    //Set chieu rong
                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(2).Height = 21;

                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:H2"])
                    {
                        range.Worksheet.Cells["A2:H2"].Merge = true;
                        range.Worksheet.Cells["A2:H2"].Value = "BÁO CÁO THUỐC SẮP HẾT HẠN DÙNG";
                        range.Worksheet.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A2:H2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:H2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:H2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:H3"])
                    {
                        range.Worksheet.Cells["A3:H3"].Merge = true;
                        range.Worksheet.Cells["A3:H3"].Value = "Ngày: " + DateTime.Now.ApplyFormatDate();

                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:H3"].Style.Font.Italic = true;

                    }
                    var tenKho = string.Empty;
                    if (query.KhoId == 0)
                    {
                        tenKho = "Tất cả";
                    }
                    else
                    {
                        tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == query.KhoId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A4:H4"])
                    {
                        range.Worksheet.Cells["A4:H4"].Merge = true;
                        range.Worksheet.Cells["A4:H4"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A4:H4"].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:H4"].Style.Font.Italic = true;

                    }

                    using (var range = worksheet.Cells["A6:H6"])
                    {
                        range.Worksheet.Cells["A6:H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A6:H6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:H6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:H6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6"].Value = "STT";

                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Mã dược";

                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Tên thuốc, Hàm lượng";

                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Số lô";

                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Hạn dùng";

                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Số lượng";

                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Tổng tiền";

                    }

                    int stt = 1;
                    int index = 7;

                    if (nhomThuocs.Any())
                    {
                        foreach (var nhom in nhomThuocs)
                        {
                            var listTheoNhom = datas.Where(s => s.NhomThuoc == nhom);
                            if (listTheoNhom.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":H" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

                                    range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                                    range.Worksheet.Cells["A" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index + ":C" + index].Value = nhom;
                                    range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                                    index++;

                                }

                                foreach (var item in listTheoNhom)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":H" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["A" + index].Value = stt;

                                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["B" + index].Value = item.MaDuoc;

                                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["C" + index].Value = item.TenThuoc;

                                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["D" + index].Value = item.DVT;

                                        range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["E" + index].Value = item.SoLo;

                                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["F" + index].Value = item.HanDungStr;

                                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["G" + index].Value = item.SoLuong;

                                        range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["H" + index].Value = item.ThanhTien;
                                        range.Worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";

                                        stt++;
                                        index++;
                                    }


                                }
                            }
                        }
                    }

                    using (var range = worksheet.Cells["A" + index + ":H" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        range.Worksheet.Cells["H" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["H" + index].Value = datas.Sum(s => s.ThanhTien);
                        index += 2;
                    }

                    worksheet.Cells["F" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["F" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["F" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["F" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["F" + index + ":G" + index].Merge = true;
                    worksheet.Cells["F" + index + ":G" + index].Style.Font.Bold = true;
                    worksheet.Cells["F" + index + ":G" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    index++;

                    worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":G" + index].Style.Font.Bold = true;

                    worksheet.Cells["B" + index].Value = "Trưởng khoa dược";
                    worksheet.Cells["D" + index].Value = "Phụ trách kho";

                    worksheet.Cells["F" + index + ":G" + index].Value = "Kế toán dược";
                    worksheet.Cells["F" + index + ":G" + index].Merge = true;

                    xlPackage.Save();

                }
                return stream.ToArray();
            }
        }

    }
}


