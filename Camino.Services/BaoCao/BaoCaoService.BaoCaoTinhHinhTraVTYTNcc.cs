using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService : IBaoCaoService
    {
        public async Task<List<LookupItemVo>> GetKhoVatTuTraNCCLookupAsync(LookupQueryInfo queryInfo)
        {
            var lookup = await _khoRepository.TableNoTracking.Where(s => s.LoaiVatTu == true && (s.LoaiKho == EnumLoaiKhoDuocPham.KhoVTYTChoXuLy
                                                                                              || s.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1
                                                                                              || s.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                                                              || s.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc
                                                                                              || s.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh
                                                                                              || s.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK))
                                                             .Select(item => new LookupItemVo
                                                             {
                                                                 DisplayName = item.Ten,
                                                                 KeyId = Convert.ToInt32(item.Id),
                                                             })
                                                           .ApplyLike(queryInfo.Query, g => g.DisplayName)
                                                           .Take(queryInfo.Take)
                                                           .ToListAsync();
            return lookup;
        }

        public async Task<GridDataSource> GetDataBaoCaoTinhHinhTraVTYTNCCForGridAsync(BaoCaoTinhHinhTraVTYTNCCQueryInfo queryInfo)
        {
            var dataXuatKhos = _xuatKhoVatTuRepository.TableNoTracking
                .Where(o => o.TraNCC == true && o.KhoXuatId == queryInfo.KhoId && queryInfo.FromDate <= o.CreatedOn && o.CreatedOn < queryInfo.ToDate)
                .Select(o => new
                {
                    o.SoChungTu,
                    o.CreatedOn,
                    o.SoPhieu,
                    o.NhaThauId,
                    XuatKhoVatTuChiTiets = o.XuatKhoVatTuChiTiets
                        .Select(ct => new
                        {
                            ct.VatTuBenhVienId,
                            XuatKhoVatTuChiTietViTris = ct.XuatKhoVatTuChiTietViTris.Select(vt => new
                            {
                                vt.Id,
                                vt.SoLuongXuat,
                                vt.NhapKhoVatTuChiTiet.Solo,
                                vt.NhapKhoVatTuChiTiet.DonGiaNhap,
                            }).ToList()
                        }).ToList()
                }).ToList();

            var soChungTus = dataXuatKhos.Select(o => o.SoChungTu).ToList();
            var nhaThauIds = dataXuatKhos.Select(o => o.NhaThauId).ToList();
            var vatTuBenhVienIds = dataXuatKhos.SelectMany(o => o.XuatKhoVatTuChiTiets).Select(o => o.VatTuBenhVienId).ToList();

            var thongTinNhaThau = _nhaThauRepository.TableNoTracking.Where(o => nhaThauIds.Contains(o.Id)).Select(o => new { o.Id, o.Ten }).ToList();

            var thongTinHoaDon = _yeuCauNhapKhoVatTuRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true && soChungTus.Contains(o.SoChungTu) && o.YeuCauNhapKhoVatTuChiTiets.Any(ct => nhaThauIds.Contains(ct.HopDongThauVatTu.NhaThauId)))
                .Select(o => new
                {
                    o.SoChungTu,
                    o.NgayHoaDon,
                    NhaThauIds = o.YeuCauNhapKhoVatTuChiTiets.Select(ct => ct.HopDongThauVatTu.NhaThauId).ToList()
                });

            var thongTinVatTu = _vatTuBenhVienRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    o.VatTus.Ten,
                    o.VatTus.NhaSanXuat,
                    o.VatTus.NuocSanXuat,
                    DonViTinh = o.VatTus.DonViTinh,
                    NhomVatTu = o.VatTus.NhomVatTu.Ten
                });

            var allData = new List<BaoCaoTinhHinhTraVTYTNCCGridVo>();
            foreach (var dataXuatKho in dataXuatKhos)
            {
                foreach (var xuatKhoVatTuChiTiet in dataXuatKho.XuatKhoVatTuChiTiets)
                {
                    foreach (var xuatKhoVatTuChiTietViTri in xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
                    {
                        var vatTu = thongTinVatTu.First(o => o.Id == xuatKhoVatTuChiTiet.VatTuBenhVienId);

                        var baoCaoTinhHinhTraNCCGridVo = new BaoCaoTinhHinhTraVTYTNCCGridVo
                        {
                            Id = xuatKhoVatTuChiTietViTri.Id,
                            SoHoaDon = dataXuatKho.SoChungTu,
                            NgayTra = dataXuatKho.CreatedOn,
                            SoPhieuTra = dataXuatKho.SoPhieu,
                            CongTy = thongTinNhaThau.FirstOrDefault(o => o.Id == dataXuatKho.NhaThauId)?.Ten,
                            MaVTYT = vatTu.Ma,
                            TenVTYT = vatTu.Ten,
                            DVT = vatTu.DonViTinh,
                            SoLo = xuatKhoVatTuChiTietViTri.Solo,
                            SoLuongTra = xuatKhoVatTuChiTietViTri.SoLuongXuat,
                            DonGiaNhap = xuatKhoVatTuChiTietViTri.DonGiaNhap,
                            Nhom = vatTu.NhomVatTu
                        };
                        if(!string.IsNullOrEmpty(vatTu.NhaSanXuat) || !string.IsNullOrEmpty(vatTu.NuocSanXuat))
                        {
                            baoCaoTinhHinhTraNCCGridVo.TenVTYT = $"{baoCaoTinhHinhTraNCCGridVo.TenVTYT} ({(!string.IsNullOrEmpty(vatTu.NhaSanXuat) && !string.IsNullOrEmpty(vatTu.NuocSanXuat) ? $"{vatTu.NhaSanXuat}, {vatTu.NuocSanXuat}" : $"{vatTu.NhaSanXuat}{vatTu.NuocSanXuat}")})";
                        }
                        var hoaDon = thongTinHoaDon.Where(o => o.SoChungTu == dataXuatKho.SoChungTu && o.NhaThauIds.Contains(dataXuatKho.NhaThauId.GetValueOrDefault())
                        && o.NgayHoaDon < dataXuatKho.CreatedOn).OrderBy(o => o.NgayHoaDon).FirstOrDefault();
                        baoCaoTinhHinhTraNCCGridVo.NgayHoaDon = hoaDon?.NgayHoaDon;

                        allData.Add(baoCaoTinhHinhTraNCCGridVo);
                    }
                }
            }

            var returnData = allData
                .GroupBy(o => new { o.SoHoaDon, o.NgayHoaDon, o.SoPhieuTra, o.NgayTra, o.MaVTYT, o.SoLo, o.DonGiaNhap }, o => o,
                    (k, v) => new BaoCaoTinhHinhTraVTYTNCCGridVo
                    {
                        Id = v.First().Id,
                        NgayHoaDon = k.NgayHoaDon,
                        SoHoaDon = k.SoHoaDon,
                        NgayTra = k.NgayTra,
                        SoPhieuTra = k.SoPhieuTra,
                        CongTy = v.First().CongTy,
                        MaVTYT = k.MaVTYT,
                        TenVTYT = v.First().TenVTYT,
                        DVT = v.First().DVT,
                        SoLo = k.SoLo,
                        SoLuongTra = v.Sum(s => s.SoLuongTra),
                        DonGiaNhap = k.DonGiaNhap,
                        Nhom = v.First().Nhom,
                    }).ToArray();

            return new GridDataSource { Data = returnData, TotalRowCount = returnData.Count() };
        }

        public virtual byte[] ExportBaoCaoTinhHinhTraVTYTNCC(GridDataSource gridDataSource, BaoCaoTinhHinhTraVTYTNCCQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTinhHinhTraVTYTNCCGridVo>)gridDataSource.Data;
            var listNhom = datas.GroupBy(s => s.Nhom).Select(s => s.First().Nhom).ToList();

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[VTYT] Tình hình trả NCC");
                    //set row
                    worksheet.DefaultRowHeight = 16;

                    //set chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 7;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 10;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 40;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 10;
                    worksheet.Column(11).Width = 10;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 40;
                    worksheet.Row(3).Height = 20;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    }

                    using (var range = worksheet.Cells["A3:O3"])
                    {
                        range.Worksheet.Cells["A3:O3"].Style.Font.SetFromFont(new Font("Times New Roman", 17));
                        range.Worksheet.Cells["A3:O3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:O3"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A3:O3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:O3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A3:O3"].Merge = true;
                        range.Worksheet.Cells["A3:O3"].Value = "BẢNG KÊ TÌNH HÌNH TRẢ NHÀ CUNG CẤP";

                    }
                    using (var range = worksheet.Cells["A4:O4"])
                    {
                        range.Worksheet.Cells["A4:O4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:O4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:O4"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A4:O4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:O4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:O4"].Style.Font.Italic = true;
                        range.Worksheet.Cells["A4:O4"].Merge = true;
                        range.Worksheet.Cells["A4:O4"].Value = "Thời gian từ: " + query.FromDate.ApplyFormatDate()
                                                          + " - " + query.ToDate.ApplyFormatDate();

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
                    using (var range = worksheet.Cells["A5:O5"])
                    {
                        range.Worksheet.Cells["A5:O5"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A5:O5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:O5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:O5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:O5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:O5"].Style.Font.Italic = true;
                        range.Worksheet.Cells["A5:O5"].Merge = true;
                        range.Worksheet.Cells["A5:O5"].Value = "Kho trả: " + tenKho;

                    }

                    using (var range = worksheet.Cells["A8:N8"])
                    {
                        range.Worksheet.Cells["A8:N8"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A8:N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:N8"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A8:N8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8"].Value = "STT";

                        range.Worksheet.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B8"].Value = "Ngày hoá đơn";

                        range.Worksheet.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8"].Value = "Số hoá đơn";

                        range.Worksheet.Cells["D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D8"].Value = "Ngày trả";

                        range.Worksheet.Cells["E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E8"].Value = "Phiếu trả";

                        range.Worksheet.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8"].Value = "Công ty";

                        range.Worksheet.Cells["G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8"].Value = "Mã VTYT";

                        range.Worksheet.Cells["H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8"].Value = "Tên VTYT (Hãng SX, Nước SX)";

                        range.Worksheet.Cells["I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8"].Value = "ĐVT";

                        range.Worksheet.Cells["J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J8"].Value = "Số lô";

                        range.Worksheet.Cells["K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8"].Value = "Số lượng trả";

                        range.Worksheet.Cells["L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L8"].Value = "Đơn giá nhập";

                        range.Worksheet.Cells["M8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M8"].Value = "Thành tiền";

                        range.Worksheet.Cells["N8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N8"].Value = "Diễn Giải";
                    }

                    int index = 9; 
                    var stt = 1;

                    if (listNhom.Any())
                    {
                        foreach (var nhom in listNhom)
                        {
                            var listTheoNhom = datas.Where(s => s.Nhom == nhom).ToList();
                            if (listTheoNhom.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":N" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    range.Worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    range.Worksheet.Cells["B" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["B" + index + ":N" + index].Merge = true;
                                    range.Worksheet.Cells["B" + index + ":N" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["B" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["B" + index + ":N" + index].Value = nhom;

                                    index++;

                                }

                                foreach (var item in listTheoNhom)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":N" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["A" + index].Value = stt;

                                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["B" + index].Value = item.NgayHoaDonStr;

                                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["C" + index].Value = item.SoHoaDon;

                                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["D" + index].Value = item.NgayTraStr;

                                        range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["E" + index].Value = item.SoPhieuTra;

                                        range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["F" + index].Value = item.CongTy;
                                        range.Worksheet.Cells["F" + index].Style.WrapText = true;

                                        range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["G" + index].Value = item.MaVTYT;

                                        range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["H" + index].Value = item.TenVTYT;

                                        range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["I" + index].Value = item.DVT;

                                        range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["J" + index].Value = item.SoLo;

                                        range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["K" + index].Value = item.SoLuongTra;
                                        //range.Worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";


                                        range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        range.Worksheet.Cells["L" + index].Value = item.DonGiaNhap;
                                        range.Worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";


                                        range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        range.Worksheet.Cells["M" + index].Value = item.ThanhTien;
                                        range.Worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                                        range.Worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        range.Worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        range.Worksheet.Cells["N" + index].Value = item.DienGiai;
                                        range.Worksheet.Cells["N" + index].Style.WrapText = true;
                                        stt++;
                                        index++;

                                    }
                                }
                            }

                        }
                    }

                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("ARIAL", 10));
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["A" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["B" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["B" + index + ":L" + index].Style.Font.Bold = true;
                    worksheet.Cells["B" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["B" + index + ":L" + index].Value = "Tổng cộng";
                    worksheet.Cells["B" + index + ":L" + index].Merge = true;

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.Font.Bold = true;
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Value = datas.Sum(s => s.ThanhTien);
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";

                    index = index + 2;

                    worksheet.Cells["M" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["M" + index + ":N" + index].Merge = true;
                    worksheet.Cells["M" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["M" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["M" + index + ":N" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    index++;

                    worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells["A" + index + ":N" + index].Style.Font.Bold = true;

                    worksheet.Cells["C" + index].Style.Font.Bold = true;
                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C" + index].Value = "Người lập";

                    worksheet.Cells["F" + index].Style.Font.Bold = true;
                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["F" + index].Value = "Thủ kho";

                    worksheet.Cells["I" + index].Style.Font.Bold = true;
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["I" + index].Value = "Kế toán";

                    worksheet.Cells["M" + index + ":N" + index].Style.Font.Bold = true;
                    worksheet.Cells["M" + index + ":N" + index].Merge = true;
                    worksheet.Cells["M" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["M" + index + ":N" + index].Value = "Trưởng khoa";

                    xlPackage.Save();

                }
                return stream.ToArray();

            }
        }
    }
}
