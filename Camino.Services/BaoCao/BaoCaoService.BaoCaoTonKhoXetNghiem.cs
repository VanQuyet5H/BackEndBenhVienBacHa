using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BaoCaos;
using LoaiGroupVo = Camino.Core.Domain.ValueObject.BaoCao.LoaiGroupVo;
using NhomGroupVo = Camino.Core.Domain.ValueObject.BaoCao.NhomGroupVo;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.KhoDuocPhams;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<LookupItemVo> GetTatCaKhoTheoKhoaXetNghiems(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Kho.Ten),
            };

            var khoaXetNghiem = _cauHinhService.GetSetting("CauHinhXetNghiem.KhoaXetNghiem");
            var khoaXetNghiemId = long.Parse(khoaXetNghiem.Value);

            var result = _khoRepository.TableNoTracking.Where(c => c.KhoaPhongId == khoaXetNghiemId)
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);

            return result.ToList();
        }
        public async Task<GridDataSource> GetDataBaoCaoTonKhoXetNghiemForGridAsync(BaoCaoTonKhoXetNghiemQueryInfo queryInfo)
        {
            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o =>
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= queryInfo.ToDate)
                    .Select(o => new BaoCaoChiTietTonKhoGridVo
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        //Ma = o.DuocPhamBenhViens.Ma,
                        //Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        //DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        HanSuDungDateTime = o.HanSuDung,
                        NgayNhapXuat = o.NgayNhap,
                        LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        //DuocPhamBenhVienPhanNhomId = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= queryInfo.ToDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= queryInfo.ToDate)))
                .Select(o => new BaoCaoChiTietTonKhoGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    //Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    //Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    //DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    HanSuDungDateTime = o.NhapKhoDuocPhamChiTiet.HanSuDung,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    LaDuocPhamBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    //DuocPhamBenhVienPhanNhomId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => new { o.LaDuocPhamBHYT, o.DuocPhamBenhVienId });
            //var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var thongTinDuocPham = _duocPhamBenhVienRepository.TableNoTracking.Select(o =>
                new
                {
                    o.Id,
                    Nhom = o.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    DVT = o.DuocPham.DonViTinh.Ten,
                    o.DuocPham.Ten,
                    o.Ma
                }).ToList();
            var dataReturn = new List<BaoCaoTonKhoXetNghiemGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var dp = thongTinDuocPham.First(o => o.Id == xuatNhapDuocPham.Key.DuocPhamBenhVienId);

                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < queryInfo.FromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= queryInfo.FromDate).ToList();
                var baoCaoTonKhoGridVo = new BaoCaoTonKhoXetNghiemGridVo
                {
                    MaHoaChat = dp.Ma,
                    TenHoaChat = dp.Ten,
                    DVT = dp.DVT,
                    TonDau = tonDau,
                    Nhap = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Xuat = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum().MathRoundNumber(2),
                    Loai = xuatNhapDuocPham.Key.LaDuocPhamBHYT ? "Thuốc BHYT" : "Viện phí",
                    Nhom = dp.Nhom
                };
                if (baoCaoTonKhoGridVo.TonCuoi != null && !baoCaoTonKhoGridVo.TonCuoi.Value.AlmostEqual(0))
                {
                    dataReturn.Add(baoCaoTonKhoGridVo);
                }
            }
            return new GridDataSource { Data = dataReturn.OrderBy(s => s.Loai).ThenBy(s => s.Nhom).ThenBy(s => s.TenHoaChat).ToArray(), TotalRowCount = dataReturn.Count };
        }
        public virtual byte[] ExportBaoCaoTonKhoXetNghiemGridVo(GridDataSource gridDataSource, BaoCaoTonKhoXetNghiemQueryInfo query)
        {
            var datas = (ICollection<BaoCaoTonKhoXetNghiemGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoTonKhoXetNghiemGridVo>("STT", p => ind++)
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
                    worksheet.Column(9).Width = 15;
                    worksheet.DefaultColWidth = 7;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:I3"])
                    {
                        range.Worksheet.Cells["A3:I3"].Merge = true;
                        range.Worksheet.Cells["A3:I3"].Value = "BÁO CÁO TỒN KHO";
                        range.Worksheet.Cells["A3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:I3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:I3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:I3"].Style.Font.Bold = true;
                    }

                    var kho = _khoRepository.TableNoTracking.Where(c => c.Id == query.KhoId).Select(c => c.Ten).FirstOrDefault();
                    using (var range = worksheet.Cells["A4:I4"])
                    {
                        range.Worksheet.Cells["A4:I4"].Merge = true;
                        range.Worksheet.Cells["A4:I4"].Value = "Kho: " + kho;
                        range.Worksheet.Cells["A4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:I4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:I5"])
                    {
                        range.Worksheet.Cells["A5:I5"].Merge = true;
                        range.Worksheet.Cells["A5:I5"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                      + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A5:I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:I5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:I5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:I5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:I7"])
                    {
                        range.Worksheet.Cells["A7:I7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:I7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:I7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:I7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7:C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7:C7"].Merge = true;
                        range.Worksheet.Cells["B7:C7"].Value = "Tên thuốc, nồng độ , hàm lượng";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Đơn vị tính";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Tồn đầu";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Nhập";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Tổng số";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Xuất";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Tồn cuối";
                    }

                    //write data from line 8
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

                            //loai HC
                            worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            //worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                            worksheet.Cells["A" + index + ":I" + index].Merge = true;
                            worksheet.Cells["A" + index + ":I" + index].Value = loai.Loai;
                            index++;

                            var lstNhomTheoLoai = datas.Where(o => o.Loai == loai.Loai)
                              .GroupBy(x => new { x.Nhom })
                                 .Select(item => new NhomGroupVo
                                 {
                                     Loai = item.First().Loai,
                                     Nhom = item.First().Nhom

                                 }).OrderBy(p => p.Nhom).ToList();
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
                                            worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                            worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                            worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Row(index).Height = 20.5;

                                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A" + index].Value = stt;

                                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["B" + index].Value = item.MaHoaChat;

                                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["C" + index].Value = item.TenHoaChat;

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
                                            worksheet.Cells["G" + index].Value = item.TongSo;

                                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["H" + index].Value = item.Xuat;

                                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["I" + index].Value = item.TonCuoi;
                                            stt++;
                                            index++;

                                        }
                                    }
                                }
                            }
                        }
                        xlPackage.Save();
                    }
                    return stream.ToArray();
                }
            }
        }
    }
}