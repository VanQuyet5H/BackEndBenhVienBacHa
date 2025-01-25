using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoTongHopSoLuongXetNghiemTheoThoiGianForGridAsync(BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo queryInfo)
        {
            var nhomDichVuXetNghiems = _nhomDichVuBenhVienRepository.TableNoTracking.Where(o => o.NhomDichVuBenhVienChaId == (long) Enums.LoaiDichVuKyThuat.XetNghiem).ToList();
            var nhomDichVuXetNghiemIds = nhomDichVuXetNghiems.Select(o => o.Id);
            var dichVuKyThuatBenhViens = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(o => nhomDichVuXetNghiemIds.Contains(o.NhomDichVuBenhVienId)).Include(o=>o.DichVuXetNghiem).ToList();
            var yeuCauDichVuKyThuatKetQuaChiTiets = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o =>
                    o.PhienXetNghiemChiTiets.Any(ct =>ct.ThoiDiemCoKetQua != null && ct.ThoiDiemCoKetQua >= queryInfo.FromDate && ct.ThoiDiemCoKetQua <= queryInfo.ToDate))
                .Select(o => new BaoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo
                {
                    Id = o.Id,
                    DichVuKyThuatBenhVienId = o.DichVuKyThuatBenhVienId,
                    LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    DuocHuongBaoHiem = o.DuocHuongBaoHiem,
                    BaoHiemChiTra = o.BaoHiemChiTra,
                    NoiTruPhieuDieuTriId = o.NoiTruPhieuDieuTriId,
                    YeuCauKhamBenhId = o.YeuCauKhamBenhId,
                    KetQuaPhienXetNghiemChiTietVos = o.PhienXetNghiemChiTiets.Select(ct =>
                        new KetQuaPhienXetNghiemChiTietVo
                        {
                            Id = ct.Id,
                            ThoiDiemCoKetQua = ct.ThoiDiemCoKetQua,
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
                        }).ToList()
                })
                .ToList();
            var dataReturn = new List<BaoCaoTongHopSoLuongXetNghiemTheoThoiGianGridVo>();
            foreach (var nhomDichVuXetNghiem in nhomDichVuXetNghiems.OrderBy(o=>o.Id))
            {
                dataReturn.Add(new BaoCaoTongHopSoLuongXetNghiemTheoThoiGianGridVo
                {
                    NhomDichVuBenhVienId = nhomDichVuXetNghiem.Id,
                    DichVuKyThuatBenhVienId = 0,
                    TenDichVu = nhomDichVuXetNghiem.Ten,
                    ToDam = true,
                    STT = 1
                });
                var stt = 2;
                foreach (var dichVuKyThuatBenhVien in dichVuKyThuatBenhViens.Where(o=>o.NhomDichVuBenhVienId == nhomDichVuXetNghiem.Id).OrderBy(o=>o.DichVuXetNghiem?.SoThuTu ?? 0).ThenBy(o=>o.Id))
                {
                    dataReturn.Add(new BaoCaoTongHopSoLuongXetNghiemTheoThoiGianGridVo
                    {
                        NhomDichVuBenhVienId = nhomDichVuXetNghiem.Id,
                        DichVuKyThuatBenhVienId = dichVuKyThuatBenhVien.Id,
                        TenDichVu = dichVuKyThuatBenhVien.Ten,
                        ToDam = false,
                        STT = stt,
                        SoLanThucHienXetNghiem = (dichVuKyThuatBenhVien.SoLanThucHienXetNghiem == null || dichVuKyThuatBenhVien.SoLanThucHienXetNghiem < 1) ? 1 : dichVuKyThuatBenhVien.SoLanThucHienXetNghiem.Value
                    });
                    stt++;
                }
            }
            foreach (var baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo in yeuCauDichVuKyThuatKetQuaChiTiets)
            {
                var ketQuaPhienXetNghiemChiTietLast = baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.KetQuaPhienXetNghiemChiTietVos.OrderBy(o => o.Id).Last();
                if (ketQuaPhienXetNghiemChiTietLast.ThoiDiemCoKetQua != null &&
                    ketQuaPhienXetNghiemChiTietLast.ThoiDiemCoKetQua >= queryInfo.FromDate &&
                    ketQuaPhienXetNghiemChiTietLast.ThoiDiemCoKetQua <= queryInfo.ToDate &&
                    ketQuaPhienXetNghiemChiTietLast.KetQuaChiSoXetNghiemChiTietVos.Any(o=> !string.IsNullOrEmpty(o.GiaTriTuMay) || !string.IsNullOrEmpty(o.GiaTriNhapTay)))
                {
                    var dichVu = dataReturn.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.DichVuKyThuatBenhVienId);
                    if (dichVu != null)
                    {
                        dichVu.SoLuongMauNoiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauNoiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                        dichVu.SoLuongMauNgoaiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauNgoaiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                        dichVu.SoLuongMauBHYTNoiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauBHYTNoiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                        dichVu.SoLuongMauBHYTNgoaiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauBHYTNgoaiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                        dichVu.SoLuongDichVu += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.DichVu ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                        dichVu.SoLuongKhamSucKhoe += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.KhamSucKhoe ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;

                        var nhomDichVu = dataReturn.FirstOrDefault(o => o.NhomDichVuBenhVienId == dichVu.NhomDichVuBenhVienId && o.DichVuKyThuatBenhVienId == 0);
                        if (nhomDichVu != null)
                        {
                            nhomDichVu.SoLuongMauNoiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauNoiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                            nhomDichVu.SoLuongMauNgoaiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauNgoaiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                            nhomDichVu.SoLuongMauBHYTNoiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauBHYTNoiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                            nhomDichVu.SoLuongMauBHYTNgoaiTru += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.MauBHYTNgoaiTru ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                            nhomDichVu.SoLuongDichVu += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.DichVu ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                            nhomDichVu.SoLuongKhamSucKhoe += (baoCaoTongHopSoLuongXetNghiemTheoThoiGianChiTietVo.KhamSucKhoe ? 1 : 0) * dichVu.SoLanThucHienXetNghiem;
                        }
                    }
                }
            }
            return new GridDataSource { Data = dataReturn.ToArray(), TotalRowCount = dataReturn.Count };
        }

        public virtual byte[] ExportBaoCaoTongHopSoLuongXetNghiemTheoThoiGian(GridDataSource gridDataSource, BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTongHopSoLuongXetNghiemTheoThoiGianGridVo>)gridDataSource.Data;
            int ind = 1;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỔNG HỢP SỐ LƯỢNG XÉT NGHIỆM");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
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
                    using (var range = worksheet.Cells["A3:I3"])
                    {
                        range.Worksheet.Cells["A3:I3"].Merge = true;
                        range.Worksheet.Cells["A3:I3"].Value = "BÁO CÁO TỔNG HỢP SỐ LƯỢNG XÉT NGHIỆM";
                        range.Worksheet.Cells["A3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:I3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:I3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:I4"])
                    {
                        range.Worksheet.Cells["A4:I4"].Merge = true;
                        range.Worksheet.Cells["A4:I4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:I4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells["A6:I6"])
                    {
                        range.Worksheet.Cells["A6:I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:I6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:I6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:I6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:I6"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A6:I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["A6"].Value = "STT";

                        worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B6"].Value = "TÊN DV";

                        worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C6"].Value = "SL MẪU NỘI TRÚ";

                        worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D6"].Value = "SL MẪU NGOẠI TRÚ";

                        worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E6"].Value = "SL MẪU BHYT NỘI TRÚ";

                        worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F6"].Value = "SL MẪU BHYT NGOẠI TRÚ";

                        worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G6"].Value = "DV";

                        worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H6"].Value = "KHÁM SỨC KHỎE";

                        worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I6"].Value = "TỔNG HỢP";
                    }
                    
                    int index = 7; // bắt đầu đổ data từ dòng 13

                    ///////Đổ data vào bảng excel
                    ///
                    var stt = 1;

                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":I" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = item.ToDam;

                                worksheet.Cells["A" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Value = item.STT;

                                worksheet.Cells["B" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.TenDichVu;

                                worksheet.Cells["C" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.SoLuongMauNoiTru;

                                worksheet.Cells["D" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.SoLuongMauNgoaiTru;

                                worksheet.Cells["E" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.SoLuongMauBHYTNoiTru;

                                worksheet.Cells["F" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.SoLuongMauBHYTNgoaiTru;

                                worksheet.Cells["G" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.SoLuongDichVu;

                                worksheet.Cells["H" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.SoLuongKhamSucKhoe;

                                worksheet.Cells["I" + index].Style.Border
                                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.SoLuongTongHop;
                                index++;
                            }
                            stt ++ ;
                        }

                        //footer tính tổng số tiền
                        //set font size, merge,...
                        worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black); //set border
                        worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                        //value
                        worksheet.Cells["A" + index].Value = "Tổng cộng";
                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Value = datas.Where(o=>o.ToDam).Sum(p => p.SoLuongMauNoiTru);

                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Value = datas.Where(o => o.ToDam).Sum(p => p.SoLuongMauNgoaiTru);

                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Value = datas.Where(o => o.ToDam).Sum(p => p.SoLuongMauBHYTNoiTru);

                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Value = datas.Where(o => o.ToDam).Sum(p => p.SoLuongMauBHYTNgoaiTru);

                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Value = datas.Where(o => o.ToDam).Sum(p => p.SoLuongDichVu);

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Value = datas.Where(o => o.ToDam).Sum(p => p.SoLuongKhamSucKhoe);

                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Value = datas.Where(o => o.ToDam).Sum(p => p.SoLuongTongHop);

                        index++;
                    }


                    index++;
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                    //value
                    worksheet.Cells["G" + index + ":I" + index].Value = "Người lập";
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    index++;

                    //value
                    worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["I" + index + ":I" + index].Style.Font.Italic = true;
                    worksheet.Cells["G" + index + ":I" + index].Value = "(ký, ghi rõ họ tên)";
                    worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + index + ":I" + index].Merge = true;
                    index++;

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
