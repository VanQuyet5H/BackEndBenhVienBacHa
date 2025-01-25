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
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<ICollection<LookupItemVo>> GetKhoBaoCaoTinhHinhNhapTuNCCLookupAsync(LookupQueryInfo queryInfo)
        {
            var dsKho = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(o => o.DuocKeToanDuyet == true)
                .SelectMany(o => o.YeuCauNhapKhoDuocPhamChiTiets).Where(o => o.KhoNhapSauKhiDuyetId != null)
                .Select(o => new { o.KhoNhapSauKhiDuyetId, o.KhoNhapSauKhiDuyet.Ten })
                .Union(_yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(o => o.DuocKeToanDuyet == true)
                    .SelectMany(o => o.YeuCauNhapKhoVatTuChiTiets).Where(o => o.KhoNhapSauKhiDuyetId != null)
                    .Select(o => new { o.KhoNhapSauKhiDuyetId, o.KhoNhapSauKhiDuyet.Ten })).Distinct().ToList();

            var result = dsKho.Select(o => new LookupItemVo { KeyId = o.KhoNhapSauKhiDuyetId.Value, DisplayName = o.Ten }).ToList();
            result.Insert(0, new LookupItemVo { KeyId = 0, DisplayName = "Toàn viện" });
            return result;
        }
        //private async Task<List<BaoCaoTinhHinhNhapTuNhaCungCapGridVo>> GetAllDataForBaoCaoTinhHinhNhapTuNhaCungCap(BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo queryInfo)
        //{
        //    var item1 = new BaoCaoTinhHinhNhapTuNhaCungCapGridVo()
        //    {
        //        Id = 1,
        //        NgayChungTu = DateTime.Now,
        //        SoChungTu = "KD-KVTN012101000 07",
        //        NgayHoaDon = DateTime.Now,
        //        SoHoaDon = "0003955",
        //        Thuoc = 5000,
        //        VTYT = 10000,
        //        HoaChat = 2000,
        //        ThueVAT = 0,
        //        NhaCungCapId = 1,
        //        TenNhaCungCap = "Chi Nhánh Công ty TNHH Dược Kim Đô"
        //    };
        //    var item2 = new BaoCaoTinhHinhNhapTuNhaCungCapGridVo()
        //    {
        //        Id = 2,
        //        NgayChungTu = DateTime.Now,
        //        SoChungTu = "KD-KVTN012101000 08",
        //        NgayHoaDon = DateTime.Now,
        //        SoHoaDon = "0003445",
        //        Thuoc = 1000,
        //        VTYT = 45000,
        //        HoaChat = 4000,
        //        ThueVAT = 0,
        //        NhaCungCapId = 1,
        //        TenNhaCungCap = "Chi Nhánh Công ty TNHH Dược Kim Đô"
        //    };
        //    var item3 = new BaoCaoTinhHinhNhapTuNhaCungCapGridVo()
        //    {
        //        Id = 3,
        //        NgayChungTu = DateTime.Now,
        //        SoChungTu = "KD-KVTN012101000 17",
        //        NgayHoaDon = DateTime.Now,
        //        SoHoaDon = "0001115",
        //        Thuoc = 2000,
        //        VTYT = 2000,
        //        HoaChat = 2000,
        //        ThueVAT = 0,
        //        NhaCungCapId = 2,
        //        TenNhaCungCap = "Chi nhánh công ty TNHH Mega Lifesciences (Việt Nam) tại Hà Nội"
        //    };
        //    var item4 = new BaoCaoTinhHinhNhapTuNhaCungCapGridVo()
        //    {
        //        Id = 4,
        //        NgayChungTu = DateTime.Now,
        //        SoChungTu = "KD-KVTN012101000 21",
        //        NgayHoaDon = DateTime.Now,
        //        SoHoaDon = "0011115",
        //        Thuoc = 3000,
        //        VTYT = 3000,
        //        HoaChat = 3000,
        //        ThueVAT = 0,
        //        NhaCungCapId = 2,
        //        TenNhaCungCap = "Chi nhánh công ty TNHH Mega Lifesciences (Việt Nam) tại Hà Nội"
        //    };
        //    var item5 = new BaoCaoTinhHinhNhapTuNhaCungCapGridVo()
        //    {
        //        Id = 5,
        //        NgayChungTu = DateTime.Now,
        //        SoChungTu = "KD-KVTN012101000 44",
        //        NgayHoaDon = DateTime.Now,
        //        SoHoaDon = "0001115",
        //        Thuoc = 4000,
        //        VTYT = 4000,
        //        HoaChat = 4000,
        //        ThueVAT = 0,
        //        NhaCungCapId = 2,
        //        TenNhaCungCap = "Chi nhánh công ty TNHH Mega Lifesciences (Việt Nam) tại Hà Nội"
        //    };

        //    var data = new List<BaoCaoTinhHinhNhapTuNhaCungCapGridVo>();
        //    data.Add(item1);
        //    data.Add(item2);
        //    data.Add(item3);
        //    data.Add(item4);
        //    data.Add(item5);

        //    return data;
        //}

        public async Task<GridDataSource> GetDataBaoCaoTinhHinhNhapTuNhaCungCapForGridAsync(BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo queryInfo)
        {
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();
            var nhaThaus = _nhaThauRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var khos = _khoRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            List<long> nhomHoaChats = new List<long> { cauHinhBaoCao.DuocPhamBenhVienNhomHoaChat };
            var nhomHoaChatCons = duocPhamBenhVienPhanNhoms.Where(o => o.NhomChaId != null && nhomHoaChats.Contains(o.NhomChaId.Value) && !nhomHoaChats.Contains(o.Id)).ToList();
            while (nhomHoaChatCons.Count > 0)
            {
                nhomHoaChats.AddRange(nhomHoaChatCons.Select(o => o.Id));
                nhomHoaChatCons = duocPhamBenhVienPhanNhoms.Where(o => o.NhomChaId != null && nhomHoaChats.Contains(o.NhomChaId.Value) && !nhomHoaChats.Contains(o.Id)).ToList();
            }

            var dsDuocPhamNhapKhoQuery = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true);

            if (queryInfo.TheoThoiGianNhap)
            {
                dsDuocPhamNhapKhoQuery = dsDuocPhamNhapKhoQuery.Where(o => o.NgayNhap >= queryInfo.FromDate && o.NgayNhap < queryInfo.ToDate);
            }
            else
            {
                dsDuocPhamNhapKhoQuery = dsDuocPhamNhapKhoQuery.Where(o => o.NgayDuyet >= queryInfo.FromDate && o.NgayDuyet < queryInfo.ToDate);
            }

            var dsDuocPhamNhapKho = dsDuocPhamNhapKhoQuery
                .Select(o => new BaoCaoTinhHinhNhapTuNhaCungCapQueryData
                {
                    NgayNhap = o.NgayNhap,
                    SoPhieu = o.SoPhieu,
                    NgayHoaDon = o.NgayHoaDon,
                    SoHoaDon = o.SoChungTu,
                    KyHieuHoaDon = o.KyHieuHoaDon,
                    BaoCaoTinhHinhNhapTuNhaCungCapChiTiets = o.YeuCauNhapKhoDuocPhamChiTiets.Select(ct => new BaoCaoTinhHinhNhapTuNhaCungCapChiTiet
                    {
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        NhaThauId = ct.HopDongThauDuocPham.NhaThauId,
                        DuocPhamBenhVienId = ct.DuocPhamBenhVienId,
                        DuocPhamBenhVienPhanNhomId = ct.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                        DonGiaNhap = ct.DonGiaNhap,
                        SoLuongNhap = ct.SoLuongNhap,
                        VAT = ct.VAT,
                        ThanhTienTruocVat = ct.ThanhTienTruocVat,
                        ThanhTienSauVat = ct.ThanhTienSauVat,
                        ThueVatLamTron = ct.ThueVatLamTron.GetValueOrDefault(),
                        Nhom = 1,
                        GhiChu = ct.GhiChu
                    }).ToList()
                }).ToList();
            foreach (var baoCaoTinhHinhNhapTuNhaCungCapQueryData in dsDuocPhamNhapKho)
            {
                foreach (var baoCaoTinhHinhNhapTuNhaCungCapChiTiet in baoCaoTinhHinhNhapTuNhaCungCapQueryData.BaoCaoTinhHinhNhapTuNhaCungCapChiTiets)
                {
                    if (baoCaoTinhHinhNhapTuNhaCungCapChiTiet.DuocPhamBenhVienPhanNhomId != null && nhomHoaChats.Contains(baoCaoTinhHinhNhapTuNhaCungCapChiTiet.DuocPhamBenhVienPhanNhomId.Value))
                    {
                        baoCaoTinhHinhNhapTuNhaCungCapChiTiet.Nhom = 3;
                    }
                }
            }

            var dsVatTuNhapKhoQuery = _yeuCauNhapKhoVatTuRepository.TableNoTracking
                .Where(o => o.DuocKeToanDuyet == true);

            if (queryInfo.TheoThoiGianNhap)
            {
                dsVatTuNhapKhoQuery = dsVatTuNhapKhoQuery.Where(o => o.NgayNhap >= queryInfo.FromDate && o.NgayNhap < queryInfo.ToDate);
            }
            else
            {
                dsVatTuNhapKhoQuery = dsVatTuNhapKhoQuery.Where(o => o.NgayDuyet >= queryInfo.FromDate && o.NgayDuyet < queryInfo.ToDate);
            }

            var dsVatTuNhapKho = dsVatTuNhapKhoQuery
                .Select(o => new BaoCaoTinhHinhNhapTuNhaCungCapQueryData
                {
                    NgayNhap = o.NgayNhap,
                    SoPhieu = o.SoPhieu,
                    NgayHoaDon = o.NgayHoaDon,
                    SoHoaDon = o.SoChungTu,
                    KyHieuHoaDon = o.KyHieuHoaDon,
                    BaoCaoTinhHinhNhapTuNhaCungCapChiTiets = o.YeuCauNhapKhoVatTuChiTiets.Select(ct => new BaoCaoTinhHinhNhapTuNhaCungCapChiTiet
                    {
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        NhaThauId = ct.HopDongThauVatTu.NhaThauId,
                        VatTuBenhVienId = ct.VatTuBenhVienId,
                        DonGiaNhap = ct.DonGiaNhap,
                        SoLuongNhap = ct.SoLuongNhap,
                        VAT = ct.VAT,
                        ThanhTienTruocVat = ct.ThanhTienTruocVat,
                        ThanhTienSauVat = ct.ThanhTienSauVat,
                        ThueVatLamTron = ct.ThueVatLamTron.GetValueOrDefault(),
                        Nhom = 2,
                        GhiChu = ct.GhiChu
                    }).ToList()
                }).ToList();

            var dsNhapKho = dsDuocPhamNhapKho.Concat(dsVatTuNhapKho);

            var returnData = new List<BaoCaoTinhHinhNhapTuNhaCungCapGridVo>();
            foreach (var baoCaoTinhHinhNhapTuNhaCungCapQueryData in dsNhapKho)
            {
                var chiTiets = baoCaoTinhHinhNhapTuNhaCungCapQueryData.BaoCaoTinhHinhNhapTuNhaCungCapChiTiets
                    .Where(o => queryInfo.KhoId == 0 || o.KhoNhapSauKhiDuyetId == queryInfo.KhoId).ToList();
                if (chiTiets.Any())
                {
                    var chiTietTheoNhaThauGroup = chiTiets.GroupBy(o => new { o.NhaThauId, o.KhoNhapSauKhiDuyetId });
                    foreach (var chiTietTheoNhaThau in chiTietTheoNhaThauGroup)
                    {
                        var thanhTienThuoc = chiTietTheoNhaThau.Where(o => o.Nhom == 1).Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                        var thanhTienVatTu = chiTietTheoNhaThau.Where(o => o.Nhom == 2).Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                        var thanhTienHoaChat = chiTietTheoNhaThau.Where(o => o.Nhom == 3).Select(o => o.ThanhTienTruocVat).DefaultIfEmpty().Sum();
                        var thanhTienVAT = chiTietTheoNhaThau.Select(o => o.ThueVatLamTron).DefaultIfEmpty().Sum();
                        var thanhTien = chiTietTheoNhaThau.Select(o => o.ThanhTienSauVat).DefaultIfEmpty().Sum();
                        var ghiChus = chiTietTheoNhaThau.Where(o => !string.IsNullOrEmpty(o.GhiChu)).Select(o => o.GhiChu).Distinct().ToList();
                        var gridItem = new BaoCaoTinhHinhNhapTuNhaCungCapGridVo
                        {
                            NgayChungTu = baoCaoTinhHinhNhapTuNhaCungCapQueryData.NgayNhap,
                            SoChungTu = baoCaoTinhHinhNhapTuNhaCungCapQueryData.SoPhieu,
                            NgayHoaDon = baoCaoTinhHinhNhapTuNhaCungCapQueryData.NgayHoaDon ??
                                         baoCaoTinhHinhNhapTuNhaCungCapQueryData.NgayNhap,
                            SoHoaDon =
                                $"{baoCaoTinhHinhNhapTuNhaCungCapQueryData.KyHieuHoaDon} - {baoCaoTinhHinhNhapTuNhaCungCapQueryData.SoHoaDon}",
                            Thuoc = thanhTienThuoc,
                            VTYT = thanhTienVatTu,
                            HoaChat = thanhTienHoaChat,
                            ThueVAT = thanhTienVAT,
                            ThanhTien = thanhTien,
                            NhaCungCapId = chiTietTheoNhaThau.Key.NhaThauId,
                            TenNhaCungCap = nhaThaus.FirstOrDefault(o => o.Id == chiTietTheoNhaThau.Key.NhaThauId)?.Ten,
                            KhoNhap = chiTietTheoNhaThau.Key.KhoNhapSauKhiDuyetId != null ? khos.FirstOrDefault(o => o.Id == chiTietTheoNhaThau.Key.KhoNhapSauKhiDuyetId)?.Ten : "",
                            GhiChu = string.Join(';', ghiChus)
                        };
                        returnData.Add(gridItem);
                    }
                }
            }

            return new GridDataSource { Data = returnData.ToArray(), TotalRowCount = returnData.Count() };
        }

        public virtual byte[] ExportBaoCaoBangTinhHinhNhapTuNhaCungCapGridVo(GridDataSource gridDataSource, BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTinhHinhNhapTuNhaCungCapGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTinhHinhNhapTuNhaCungCapGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("TÌNH HÌNH NHẬP TỪ NHÀ CUNG CẤP");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 25;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 20;
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
                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = "TÌNH HÌNH NHẬP DƯỢC TỪ NHÀ CUNG CẤP";
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
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
                        range.Worksheet.Cells["A4:M4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:M4"].Style.Font.Bold = true;
                    }

                    var tenKho = query.KhoId == 0 ? "Toàn viện" : _khoRepository.TableNoTracking.Where(c => c.Id == query.KhoId).Select(c => c.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A5:M5"])
                    {
                        range.Worksheet.Cells["A5:M5"].Merge = true;
                        range.Worksheet.Cells["A5:M5"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A5:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:M5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:M5"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A7:M7"])
                    {
                        range.Worksheet.Cells["A7:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:M7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:M7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:M7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:M7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Nhập kho";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Ngày chứng từ";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Số chứng từ";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Ngày hóa đơn";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Số seri - Số hoá đơn";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Thuốc";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "VTYT";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Hóa chất";

                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Thuế VAT";

                        range.Worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7"].Value = "Thành tiền";

                        range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7"].Value = "Tổng";

                        range.Worksheet.Cells["M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M7"].Value = "Ghi chú";
                    }

                    //write data from line 8
                    int index = 8;
                    var dataTheoNCC = datas.GroupBy(x => x.NhaCungCapId).Select(x => x.Key);
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var data in dataTheoNCC)
                        {
                            var listDataTheoNCC = datas.Where(x => x.NhaCungCapId == data.Value).ToList();
                            if (listDataTheoNCC.Any())
                            {
                                using (var range = worksheet.Cells["A" + index + ":C" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                                    range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                                    range.Worksheet.Cells["A" + index + ":C" + index].Value = listDataTheoNCC.FirstOrDefault().TenNhaCungCap;

                                }

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                               

                                ////sum VTYT
                                worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["G" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["G" + index].Style.Font.Bold = true;
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index].Value = listDataTheoNCC.Sum(x => x.Thuoc);

                                ////sum hóa chất
                                worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["H" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["H" + index].Style.Font.Bold = true;
                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["H" + index].Value = listDataTheoNCC.Sum(x => x.VTYT);

                                ////sum VTYT
                                worksheet.Cells["I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["I" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["I" + index].Style.Font.Bold = true;
                                worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["I" + index].Value = listDataTheoNCC.Sum(x => x.HoaChat);                     

                                worksheet.Cells["J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["J" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["J" + index].Value = listDataTheoNCC.Sum(x => x.ThueVAT);


                                //sum thuốc
                                worksheet.Cells["L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["L" + index].Style.Font.Color.SetColor(Color.Black);
                                worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                worksheet.Cells["L" + index].Style.Font.Bold = true;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["L" + index].Value = listDataTheoNCC.Sum(x => x.ThanhTien);

                                index++;

                                foreach (var item in listDataTheoNCC)
                                {
                                    // format border, font chữ,....
                                    worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Row(index).Height = 20.5;



                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = item.KhoNhap;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = item.NgayChungTuStr ;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = item.SoChungTu;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["E" + index].Value = item.NgayHoaDonStr;


                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;                          
                                    worksheet.Cells["F" + index].Value = item.SoHoaDon;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["G" + index].Value = item.Thuoc;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["H" + index].Value = item.VTYT;


                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["I" + index].Value = item.HoaChat;

                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["J" + index].Value = item.ThueVAT;

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["K" + index].Value = item.ThanhTien;

                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                                    worksheet.Cells["M" + index].Value = item.GhiChu; 

                                   stt++;
                                   index++;
                                }
                            }
                        }

                        using (var range = worksheet.Cells["A" + index + ":D" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":D" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":D" + index].Value = "Tổng cộng";
                        }

                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["G" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["G" + index].Style.Font.Bold = true;
                        worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index].Value = datas.Sum(x => x.Thuoc);

                        worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["H" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["H" + index].Style.Font.Bold = true;
                        worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index].Value = datas.Sum(x => x.VTYT);

                        worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["I" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["I" + index].Style.Font.Bold = true;
                        worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["I" + index].Value = datas.Sum(x => x.HoaChat);


                        worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["J" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["J" + index].Style.Font.Bold = true;
                        worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["J" + index].Value = datas.Sum(x => x.ThueVAT);

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["L" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Style.Font.Bold = true;
                        worksheet.Cells["L" + index].Value = datas.Sum(x => x.ThanhTien);

                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        var date = DateTime.Now;
                        index += 2;
                        using (var range = worksheet.Cells["A" + index + ":C" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":C" + index].Value = "Trưởng khoa dược";
                        }

                        using (var range = worksheet.Cells["D" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["D" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["D" + index + ":F" + index].Value = "Kế toán";
                        }

                        using (var range = worksheet.Cells["G" + index + ":I" + index])
                        {
                            range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["G" + index + ":I" + index].Value = "Thủ kho";
                        }

                        using (var range = worksheet.Cells["J" + index + ":M" + index])
                        {
                            range.Worksheet.Cells["J" + index + ":M" + index].Merge = true;
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["J" + index + ":M" + index].Value = "Người lập";
                        }

                        index++;
                        using (var range = worksheet.Cells["A" + index + ":C" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["A" + index + ":C" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["D" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["D" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["D" + index + ":F" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["G" + index + ":I" + index])
                        {
                            range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["G" + index + ":I" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["J" + index + ":M" + index])
                        {
                            range.Worksheet.Cells["J" + index + ":M" + index].Merge = true;
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["J" + index + ":M" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["J" + index + ":M" + index].Value = "(Ký, ghi rõ họ tên)";
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
