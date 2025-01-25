using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;


namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoSoXetNghiemSangLocHivForGridAsync(BaoCaoSoXetNghiemSangLocHivQueryInfo queryInfo)
        {
            var yeuCauDichVuKyThuatKetQuaChiTiets = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => o.DichVuKyThuatBenhVienId == queryInfo.DichVuKyThuatBenhVienId &&
                    o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemKetLuan != null && ct.ThoiDiemKetLuan >= queryInfo.FromDate && ct.ThoiDiemKetLuan <= queryInfo.ToDate))
                .Select(o => new BaoCaoSoXetNghiemTheoDichVuVo
                {
                    Id = o.Id,
                    TenDichVuKyThuat = o.TenDichVu,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = o.YeuCauTiepNhan.HoTen,
                    GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                    NamSinh = o.YeuCauTiepNhan.NamSinh,
                    Phong = o.NoiChiDinh.Ten,
                    Khoa = o.NoiChiDinh.KhoaPhong.Ten,
                    KetQuaPhienXetNghiemChiTietVos = o.PhienXetNghiemChiTiets.Select(ct =>
                        new KetQuaPhienXetNghiemChiTietVo
                        {
                            Id = ct.Id,
                            MaBarcode = ct.PhienXetNghiem.BarCodeId,
                            ThoiDiemKetLuan = ct.ThoiDiemKetLuan,
                            ThoiDiemLayMau = ct.ThoiDiemLayMau,
                            KetQuaChiSoXetNghiemChiTietVos = ct.KetQuaXetNghiemChiTiets.Select(kq=>new KetQuaChiSoXetNghiemChiTietVo
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
                        }).ToList()
                })
                .ToList();
            var ketnoichiso = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Where(o => o.HieuLuc).ToList();
            var dataReturn = new List<BaoCaoSoXetNghiemSangLocHivGridVo>();
            var stt = 1;
            foreach (var baoCaoSoXetNghiemTheoDichVuVo in yeuCauDichVuKyThuatKetQuaChiTiets.OrderBy(o=>o.KetQuaPhienXetNghiemChiTietVos.First().MaBarcode))
            {
                var ketQuaPhienXetNghiemChiTietLast = baoCaoSoXetNghiemTheoDichVuVo.KetQuaPhienXetNghiemChiTietVos.OrderBy(o => o.Id).Last();
                if (ketQuaPhienXetNghiemChiTietLast.ThoiDiemKetLuan != null &&
                    ketQuaPhienXetNghiemChiTietLast.ThoiDiemKetLuan >= queryInfo.FromDate &&
                    ketQuaPhienXetNghiemChiTietLast.ThoiDiemKetLuan <= queryInfo.ToDate)
                {
                    dataReturn.Add(new BaoCaoSoXetNghiemSangLocHivGridVo
                    {
                        STT = stt,
                        MaBN = baoCaoSoXetNghiemTheoDichVuVo.MaBN,
                        MaBarcode = ketQuaPhienXetNghiemChiTietLast.MaBarcode,
                        HoTen = baoCaoSoXetNghiemTheoDichVuVo.HoTen,
                        GioiTinh = baoCaoSoXetNghiemTheoDichVuVo.GioiTinh?.GetDescription(),
                        NamSinh = baoCaoSoXetNghiemTheoDichVuVo.NamSinh?.ToString(),
                        KhoaPhong = $"{baoCaoSoXetNghiemTheoDichVuVo.Khoa} - {baoCaoSoXetNghiemTheoDichVuVo.Phong}",
                        DoiTuong = baoCaoSoXetNghiemTheoDichVuVo.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ? "Nội trú" : "Ngoại trú",
                        NgayLayMau = ketQuaPhienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDate(),
                        NgayXetNghiem = ketQuaPhienXetNghiemChiTietLast.ThoiDiemKetLuan?.ApplyFormatDate(),
                        KetQua = LISHelper.GetKetQuaDichVuXetNghiem(baoCaoSoXetNghiemTheoDichVuVo.TenDichVuKyThuat, ketQuaPhienXetNghiemChiTietLast.KetQuaChiSoXetNghiemChiTietVos.ToList(), ketnoichiso)
                    });
                    stt++;
                }
            }
            
            return new GridDataSource { Data = dataReturn.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count };
        }
        
        public virtual byte[] ExportBaoCaoSoXetNghiemSangLocHiv(GridDataSource gridDataSource, BaoCaoSoXetNghiemSangLocHivQueryInfo query)
        {
            var datas = (ICollection<BaoCaoSoXetNghiemSangLocHivGridVo>)gridDataSource.Data;
            var dvkt = _dichVuKyThuatBenhVienRepository.GetById(query.DichVuKyThuatBenhVienId);
            int ind = 1;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO SỐ XÉT NGHIỆM SÀNG LỌC HIV");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        //                        var url = hostingName + "/assets/img/logo-bacha-full.png";
                        //                        WebClient wc = new WebClient();
                        //                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        //                        MemoryStream ms = new MemoryStream(bytes); //
                        //                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        //                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        //                        pic.SetPosition(0, 0, 0, 0);
                        //                        var height = 120; // chiều cao từ A1 đến A6
                        //                        var width = 510; // chiều rộng từ A1 đến D1
                        //                        pic.SetSize(width, height);
                        //                        range.Worksheet.Protection.IsProtected = false;
                        //                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "SỐ XÉT NGHIỆM SÀNG LỌC HIV";
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:M4"])
                    {
                        range.Worksheet.Cells["A4:M4"].Merge = true;
                        range.Worksheet.Cells["A4:M4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:M4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:M4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:M5"])
                    {
                        range.Worksheet.Cells["A5:M5"].Merge = true;
                        range.Worksheet.Cells["A5:M5"].Value = "Dịch vụ: " + dvkt?.Ten;
                        range.Worksheet.Cells["A5:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:M5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:M5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:M5"].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells["A7:M7"])
                    {
                        range.Worksheet.Cells["A7:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:M7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:M7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:M7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["A7"].Value = "STT";

                        worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B7"].Value = "MÃ BN";

                        worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C7"].Value = "MÃ BARCODE";

                        worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D7"].Value = "HỌ VÀ TÊN";

                        worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E7"].Value = "GIỚI TÍNH";

                        worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F7"].Value = "NĂM SINH";

                        worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G7"].Value = "KHOA PHÒNG";

                        worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H7"].Value = "ĐỐI TƯỢNG";

                        worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I7"].Value = "NGÀY LẤY MẪU";

                        worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J7"].Value = "NGÀY XÉT NGHIỆM";

                        worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K7"].Value = "KẾT QUẢ";

                        worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L7"].Value = "KẾT QUẢ KHẲNG ĐỊNH";

                        worksheet.Cells["M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M7"].Value = "GHI CHÚ";
                    }

                    int index = 8; // bắt đầu đổ data từ dòng 7

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":M" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.VerticalAlignment =
                                    ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font
                                    .SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":M" + index].Style.Font.Color
                                    .SetColor(Color.Black);

                                worksheet.Cells["A" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaBN;

                                worksheet.Cells["C" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaBarcode;

                                worksheet.Cells["D" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.HoTen;

                                worksheet.Cells["E" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.GioiTinh;

                                worksheet.Cells["F" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NamSinh;

                                worksheet.Cells["G" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.KhoaPhong;

                                worksheet.Cells["H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.DoiTuong;

                                worksheet.Cells["I" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.NgayLayMau;

                                worksheet.Cells["J" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.NgayXetNghiem;

                                worksheet.Cells["K" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.KetQua;

                                worksheet.Cells["L" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.KetQuaKhangDinh;

                                worksheet.Cells["M" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.GhiChu;
                                index++;
                            }
                            stt++;
                        }
                    }
                    index++;
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":M" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["K" + index + ":M" + index].Value = "Người lập";
                    worksheet.Cells["K" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":M" + index].Merge = true;
                    index++;

                    //value
                    worksheet.Cells["K" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["K" + index + ":M" + index].Style.Font.Italic = true;
                    worksheet.Cells["K" + index + ":M" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["K" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K" + index + ":M" + index].Merge = true;
                    index++;
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
