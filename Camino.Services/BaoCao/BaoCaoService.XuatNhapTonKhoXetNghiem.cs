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
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<LookupItemVo> GetTatCaTuHoaChat(DropDownListRequestModel queryInfo)
        {
            var result = _khoRepository.TableNoTracking.Where(c => c.LoaiDuocPham == true && c.KhoaPhongId == (long)EnumKhoaPhong.KhoaXetNghiem)
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take).ToList();           
            return result;
        }   

        public async Task<GridDataSource> GetDataXuatNhapTonKhoXetNghiemForGrid(XuatNhapTonKhoXetNghiemQueryInfo queryInfo)
        {
            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.DenNgay)
                    .Select(o => new BaoCaoChiTietTonKhoGridVo
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        Ma = o.DuocPhamBenhViens.Ma,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        //HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        HanSuDungDateTime = o.HanSuDung,
                        NgayNhapXuat = o.NgayNhap,
                        //LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        DuocPhamBenhVienPhanNhomId = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.DenNgay) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.DenNgay)))
                .Select(o => new BaoCaoChiTietTonKhoGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    //HamLuong = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanSuDungDateTime = o.NhapKhoDuocPhamChiTiet.HanSuDung,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    //LaDuocPhamBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    DuocPhamBenhVienPhanNhomId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.Ten).ThenBy(o => o.HanSuDungDateTime).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.DuocPhamBenhVienId, o.SoLo, HanSuDung = o.HanSuDungDateTime.Date });
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var dataReturn = new List<DanhSachXuatNhapTonKhoXetNghiem>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < queryInfo.TuNgay)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= queryInfo.TuNgay).ToList();
                var baoCaoTonKhoGridVo = new DanhSachXuatNhapTonKhoXetNghiem
                {
                    MaDuocPham = xuatNhapDuocPham.First().Ma,
                    DuocPham = xuatNhapDuocPham.First().Ten,
                    DonViTinh = xuatNhapDuocPham.First().DVT,
                    SoLoSX = xuatNhapDuocPham.Key.SoLo,
                    HanDung = xuatNhapDuocPham.Key.HanSuDung,
                    TongDauKy = tonDau,
                    NhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    XuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                    Nhom = duocPhamBenhVienPhanNhoms.FirstOrDefault(o => o.Id == xuatNhapDuocPham.First().DuocPhamBenhVienPhanNhomId)?.Ten ?? "Hóa chất khác"
                };
                if (baoCaoTonKhoGridVo.TonCuoiKy != null && !baoCaoTonKhoGridVo.TonCuoiKy.Value.AlmostEqual(0))
                {
                    dataReturn.Add(baoCaoTonKhoGridVo);
                }
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.Nhom).ThenBy(s => s.MaDuocPham).ToArray(), TotalRowCount = dataReturn.Count };

            //var danhSachXuatNhapTonKhoXetNghiems = new List<DanhSachXuatNhapTonKhoXetNghiem>
            //{
            //    new DanhSachXuatNhapTonKhoXetNghiem {Nhom = "Hóa chất khác" , MaDuocPham = "DUNH003" , DuocPham = "Dung dịch tím Gentian" , DonViTinh = "Lọ" , TongDauKy = 10 , NhapTrongKy = 5 , XuatTrongKy = 3 ,TonCuoiKy = 12 , SoLoSX ="101001" , HanDung = DateTime.Now , GhiChu ="Có thông tin gì hiển thị thông tin đó ra"},
            //    new DanhSachXuatNhapTonKhoXetNghiem {Nhom = "Hóa chất nội kiểm" , MaDuocPham = "PREH001" , DuocPham = "PreciControl ClinChem Multi 1" , DonViTinh = "Lọ" , TongDauKy = 10 , NhapTrongKy = 5 , XuatTrongKy = 3 ,TonCuoiKy = 12 , SoLoSX ="1010021" , HanDung = DateTime.Now , GhiChu ="Có thông tin gì hiển thị thông tin đó ra"},
            //    new DanhSachXuatNhapTonKhoXetNghiem {Nhom = "Hóa chất nội kiểm" , MaDuocPham = "PREH002" , DuocPham = "PreciControl ClinChem Multi 2" , DonViTinh = "Lọ" , TongDauKy = 10 , NhapTrongKy = 5 , XuatTrongKy = 3 ,TonCuoiKy = 12 , SoLoSX ="10100231" , HanDung = DateTime.Now , GhiChu ="Có thông tin gì hiển thị thông tin đó ra"}
            //};

            //return new GridDataSource
            //{
            //    Data = danhSachXuatNhapTonKhoXetNghiems.OrderBy(o => o.HanDung).ToArray(),
            //    TotalRowCount = danhSachXuatNhapTonKhoXetNghiems.Count()
            //};
        }

        public virtual byte[] ExportXuatNhapTonKhoXetNghiem(GridDataSource gridDataSource, XuatNhapTonKhoXetNghiemQueryInfo query)
        {
            var datas = (ICollection<DanhSachXuatNhapTonKhoXetNghiem>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DanhSachXuatNhapTonKhoXetNghiem>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[XN]Xuất nhập tồn kho xét nghiệm");
                    worksheet.DefaultRowHeight = 16;
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 28;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 40;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "Báo cáo sử dụng hóa chất";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["J1:K1"])
                    {
                        range.Worksheet.Cells["J1:K1"].Merge = true;
                        range.Worksheet.Cells["J1:K1"].Value = "BM07.QTKT 5.3.2";
                        range.Worksheet.Cells["J1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["J1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J1:K1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["J1:K1"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A2:K2"])
                    {
                        range.Worksheet.Cells["A2:K2"].Merge = true;
                        range.Worksheet.Cells["A2:K2"].Value = "BỆNH VIỆN ĐA KHOA QUỐC TẾ BẮC HÀ";
                        range.Worksheet.Cells["A2:K2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:K2"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A2:K2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:K2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "KHOA XÉT NGHIỆM";
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 15));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Worksheet.Cells["A4:K4"].Merge = true;
                        range.Worksheet.Cells["A4:K4"].Value = "BÁO CÁO SỬ DỤNG HÓA CHẤT";
                        range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 15));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:K5"])
                    {
                        range.Worksheet.Cells["A5:K5"].Merge = true;
                        range.Worksheet.Cells["A5:K5"].Value = "Thời gian từ: " + query.TuNgay.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến: " + query.DenNgay.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:K5"].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells["A5:K5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:K5"].Style.Font.Bold = true;
                    }

                    var indexHeader = 8;
                    using (var range = worksheet.Cells["A" + indexHeader + ":K" + indexHeader])
                    {
                        range.Worksheet.Cells["A" + indexHeader + ":K" + indexHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":K" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + indexHeader + ":K" + indexHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A" + indexHeader + ":K" + indexHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + indexHeader + ":K" + indexHeader].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + indexHeader + ":K" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Merge = true;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Value = "STT";
                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A" + indexHeader + ":A" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Merge = true;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Value = "Mã dược phẩm";
                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B" + indexHeader + ":B" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Merge = true;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Value = "Dược phẩm";
                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C" + indexHeader + ":C" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Merge = true;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Value = "Đơn vị tính";
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D" + indexHeader + ":D" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Merge = true;
                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Value = "Tồn đầu kỳ";
                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E" + indexHeader + ":E" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Merge = true;
                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Value = "Nhập trong kỳ";
                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F" + indexHeader + ":F" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G" + indexHeader + ":G" + indexHeader].Merge = true;
                        range.Worksheet.Cells["G" + indexHeader + ":G" + indexHeader].Value = "Xuất trong kỳ";
                        range.Worksheet.Cells["G" + indexHeader + ":G" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G" + indexHeader + ":G" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H" + indexHeader + ":H" + indexHeader].Merge = true;
                        range.Worksheet.Cells["H" + indexHeader + ":H" + indexHeader].Value = "Tồn cuối kỳ";
                        range.Worksheet.Cells["H" + indexHeader + ":H" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H" + indexHeader + ":H" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I" + indexHeader + ":I" + indexHeader].Merge = true;
                        range.Worksheet.Cells["I" + indexHeader + ":I" + indexHeader].Value = "Lô sx";
                        range.Worksheet.Cells["I" + indexHeader + ":I" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I" + indexHeader + ":I" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J" + indexHeader + ":J" + indexHeader].Merge = true;
                        range.Worksheet.Cells["J" + indexHeader + ":J" + indexHeader].Value = "Hạn dùng";
                        range.Worksheet.Cells["J" + indexHeader + ":J" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J" + indexHeader + ":J" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K" + indexHeader + ":K" + indexHeader].Merge = true;
                        range.Worksheet.Cells["K" + indexHeader + ":K" + indexHeader].Value = "Ghi chú";
                        range.Worksheet.Cells["K" + indexHeader + ":K" + indexHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K" + indexHeader + ":K" + indexHeader].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                    }
                    var manager = new PropertyManager<DanhSachXuatNhapTonKhoXetNghiem>(requestProperties);
                    int index = indexHeader + 1;
                    var stt = 1;

                    var lstNhom = datas.GroupBy(x => new { x.Nhom }).Select(item => new { item.First().Nhom }).OrderBy(p => p.Nhom).ToList();
                    if (lstNhom.Any())
                    {
                        foreach (var itemNhom in lstNhom)
                        {
                            using (var range = worksheet.Cells["A" + index + ":K" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["A" + index + ":K" + index].Value = itemNhom.Nhom;
                                range.Worksheet.Cells["A" + index + ":K" + index].Merge = true;
                            }
                            index++;

                            var listDataTheoNhom = datas.Where(o => o.Nhom == itemNhom.Nhom).ToList();

                            if (listDataTheoNhom.Any())
                            {
                                foreach (var item in listDataTheoNhom)
                                {
                                    using (var range = worksheet.Cells["A" + index + ":K" + index])
                                    {
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells["A" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells["A" + index].Value = stt;

                                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["B" + index].Value = item.MaDuocPham;

                                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["C" + index].Value = item.DuocPham;

                                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["D" + index].Value = item.DonViTinh;

                                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["E" + index].Value = item.TongDauKy;

                                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["F" + index].Value = item.NhapTrongKy;

                                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["G" + index].Value = item.XuatTrongKy;

                                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["H" + index].Value = item.TonCuoiKy;

                                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["I" + index].Value = item.SoLoSX;

                                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["J" + index].Value = item.HanDungDisplay;

                                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["K" + index].Value = item.GhiChu;

                                        index++;
                                        stt++;
                                    }
                                }
                            }
                        }
                    }

                    index = index + 1;
                    using (var range = worksheet.Cells["A" + index + ":C" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":C" + index].Value = $"Lần BH/SX: 01/00";
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["F" + index + ":H" + index])
                    {
                        range.Worksheet.Cells["F" + index + ":H" + index].Merge = true;
                        range.Worksheet.Cells["F" + index + ":H" + index].Value = $"Ngày ban hành: 01/01/2022";
                        range.Worksheet.Cells["F" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["F" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["F" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["J" + index + ":K" + index])
                    {
                        range.Worksheet.Cells["J" + index + ":K" + index].Merge = true;
                        range.Worksheet.Cells["J" + index + ":K" + index].Value = $"Trang 1/1";
                        range.Worksheet.Cells["J" + index + ":K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["J" + index + ":K" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J" + index + ":K" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["J" + index + ":K" + index].Style.Font.Color.SetColor(Color.Black);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
    }
}
