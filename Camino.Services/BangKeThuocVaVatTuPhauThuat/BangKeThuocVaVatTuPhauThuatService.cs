using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Services.BangKeThuocVaVatTuPhauThuat;
using Camino.Core.Domain.ValueObject.BaoCaos;
using OfficeOpenXml.Style;
using System.Drawing;
using System;
using Camino.Services.ExportImport.Help;
using System.IO;
using Camino.Core.Domain;
using OfficeOpenXml;

namespace Camino.Services.BangKeThuocVaVatTuPhauThuat
{
    [ScopedDependency(ServiceType = typeof(IBangKeThuocVaVatTuPhauThuatService))]
    public class BangKeThuocVaVatTuPhauThuatService : MasterFileService<YeuCauTiepNhan>, IBangKeThuocVaVatTuPhauThuatService
    {
        public IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        public BangKeThuocVaVatTuPhauThuatService(IRepository<YeuCauTiepNhan> repository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository) : base(repository)
        {
            _phongBenhVienRepository = phongBenhVienRepository;
        }
        public async Task<ICollection<LookupItemVo>> GetPhongPhauThuats()
        {
            var lookupItemVo =
                await _phongBenhVienRepository.TableNoTracking
                    .Where(x => x.IsDisabled != true)
                    .Select(item => new LookupItemVo()
                    {
                        DisplayName = item.Ten + " - " + item.Ma,
                        KeyId = item.Id
                    }).OrderBy(o=>o.DisplayName).ToListAsync();

            return lookupItemVo;
        }

        public async Task<ICollection<ThongTinBenhNhanLookupItemVo>> GetBenhNhanPhongPhauThuats(ThongTinBenhNhanPhauThuatQueryInfo queryInfo)
        {
            var thongTinBenhNhans = BaseRepository.TableNoTracking.Where(o => o.YeuCauDichVuKyThuats.Any(kt =>
                kt.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.NoiThucHienId == queryInfo.PhongBenhVienId && 
                ((kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= queryInfo.TuNgay && kt.ThoiDiemThucHien < queryInfo.DenNgay) || 
                 (kt.ThoiDiemThucHien == null && kt.ThoiDiemDangKy >= queryInfo.TuNgay && kt.ThoiDiemDangKy < queryInfo.DenNgay)) &&
                (o.YeuCauDuocPhamBenhViens.Any(dp=> dp.NoiChiDinhId == queryInfo.PhongBenhVienId && dp.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && dp.SoLuong!=0) ||
                 o.YeuCauVatTuBenhViens.Any(vt => vt.NoiChiDinhId == queryInfo.PhongBenhVienId && vt.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && vt.SoLuong != 0))))
                .Select(o=>new ThongTinBenhNhanLookupItemVo
                {
                    KeyId = o.MaYeuCauTiepNhan,
                    DisplayName = o.HoTen,
                    MaYeuCauTiepNhan = o.MaYeuCauTiepNhan,
                    MaBN = o.BenhNhan.MaBN,
                    MaBA = o.NoiTruBenhAn != null ? o.NoiTruBenhAn.SoBenhAn : ""
                }).ToList();
            var thongTinBenhNhanLookupItemVos = thongTinBenhNhans.GroupBy(o => new {o.KeyId, o.DisplayName, o.MaYeuCauTiepNhan, o.MaBN}, o => o,
                (k, v) => new ThongTinBenhNhanLookupItemVo
                {
                    KeyId = k.MaYeuCauTiepNhan,
                    DisplayName = k.DisplayName,
                    MaYeuCauTiepNhan = k.MaYeuCauTiepNhan,
                    MaBN = k.MaBN,
                    MaBA = v.FirstOrDefault(x=>x.MaBA != "") != null ? v.FirstOrDefault(x => x.MaBA != "").MaBA : ""
                }).ToList();
            if (thongTinBenhNhanLookupItemVos.Count > 1)
            {
                thongTinBenhNhanLookupItemVos.Insert(0, new ThongTinBenhNhanLookupItemVo
                {
                    KeyId = "",
                    DisplayName = "Tất cả",
                    MaYeuCauTiepNhan = "",
                    MaBN = "",
                    MaBA = ""
                });
            }
            return thongTinBenhNhanLookupItemVos;
        }

        private string GetDanhSachBenhNhanPhongPhauThuats(BaoCaoThuocVaVatTuPhauThuatQueryInfoVo queryInfo)
        {
            List<string> maYeuCauTiepNhans = new List<string>();
            if (!string.IsNullOrEmpty(queryInfo.MaYeuCauTiepNhan))
            {
                return BaseRepository.TableNoTracking.Where(o => o.MaYeuCauTiepNhan == queryInfo.MaYeuCauTiepNhan).Select(o => o.HoTen + "(" + o.BenhNhan.MaBN + ")").FirstOrDefault();
            }
            else
            {
                var thongTinBenhNhans = BaseRepository.TableNoTracking.Where(o => o.YeuCauDichVuKyThuats.Any(kt =>
                        kt.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.NoiThucHienId == queryInfo.PhongBenhVienId &&
                        ((kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= queryInfo.TuNgay && kt.ThoiDiemThucHien < queryInfo.DenNgay) ||
                         (kt.ThoiDiemThucHien == null && kt.ThoiDiemDangKy >= queryInfo.TuNgay && kt.ThoiDiemDangKy < queryInfo.DenNgay)) &&
                        (o.YeuCauDuocPhamBenhViens.Any(dp => dp.NoiChiDinhId == queryInfo.PhongBenhVienId && dp.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && dp.SoLuong != 0) ||
                         o.YeuCauVatTuBenhViens.Any(vt => vt.NoiChiDinhId == queryInfo.PhongBenhVienId && vt.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && vt.SoLuong != 0))))
                    .Select(o => o.HoTen + "(" + o.BenhNhan.MaBN + ")").ToList();
                return string.Join(", ", thongTinBenhNhans);
            }
        }

        public async Task<GridDataSource> GetDataForGridAsync(BaoCaoThuocVaVatTuPhauThuatQueryInfoVo queryInfo)
        {
            List<string> maYeuCauTiepNhans = new List<string>();
            if (!string.IsNullOrEmpty(queryInfo.MaYeuCauTiepNhan))
            {
                maYeuCauTiepNhans.Add(queryInfo.MaYeuCauTiepNhan);
            }
            else
            {
                var dsBN = await GetBenhNhanPhongPhauThuats(new ThongTinBenhNhanPhauThuatQueryInfo
                {
                    PhongBenhVienId = queryInfo.PhongBenhVienId, TuNgay = queryInfo.TuNgay, DenNgay = queryInfo.DenNgay
                });
                maYeuCauTiepNhans = dsBN.Select(o => o.MaYeuCauTiepNhan).ToList();
            }
            var thongTinDuocPhams = BaseRepository.TableNoTracking.Where(o => maYeuCauTiepNhans.Contains(o.MaYeuCauTiepNhan))
                .SelectMany(o=>o.YeuCauDuocPhamBenhViens)
                .Where(dp => dp.ThoiDiemChiDinh >= queryInfo.TuNgay && dp.ThoiDiemChiDinh < queryInfo.DenNgay && dp.NoiChiDinhId == queryInfo.PhongBenhVienId && dp.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && dp.SoLuong != 0
                             
                             //BVHD-3860
                             && (queryInfo.BangKeThuocPhi == null 
                                 || queryInfo.BangKeThuocPhi.TinhPhi == queryInfo.BangKeThuocPhi.KhongTinhPhi
                                 || (queryInfo.BangKeThuocPhi.TinhPhi == true && dp.KhongTinhPhi != true)
                                 || (queryInfo.BangKeThuocPhi.KhongTinhPhi == true && dp.KhongTinhPhi == true))
                             )
                .Select(dp => new DanhSachThuocVaVatTuPhauThuat
                {
                    Nhom = "Thuốc",
                    TenDichVu = dp.Ten,
                    HamLuongNoiSanXuat = dp.HamLuong,
                    DonViTinh = dp.DonViTinh.Ten,
                    LaThuocVatTuBHYT = dp.LaDuocPhamBHYT,
                    KhongTinhPhi = dp.KhongTinhPhi != null && dp.KhongTinhPhi == true,
                    GiaiDoanPhauThuat = dp.GiaiDoanPhauThuat,
                    SoLuong = dp.SoLuong,
                    DonGia = dp.DonGiaBan
                }).ToList();
            var thongTinVatTus = BaseRepository.TableNoTracking.Where(o => maYeuCauTiepNhans.Contains(o.MaYeuCauTiepNhan))
                .SelectMany(o => o.YeuCauVatTuBenhViens)
                .Where(vt => vt.ThoiDiemChiDinh >= queryInfo.TuNgay && vt.ThoiDiemChiDinh < queryInfo.DenNgay && vt.NoiChiDinhId == queryInfo.PhongBenhVienId && vt.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && vt.SoLuong != 0

                             //BVHD-3860
                             && (queryInfo.BangKeThuocPhi == null
                                 || queryInfo.BangKeThuocPhi.TinhPhi == queryInfo.BangKeThuocPhi.KhongTinhPhi
                                 || (queryInfo.BangKeThuocPhi.TinhPhi == true && vt.KhongTinhPhi != true)
                                 || (queryInfo.BangKeThuocPhi.KhongTinhPhi == true && vt.KhongTinhPhi == true))
                )
                .Select(vt => new DanhSachThuocVaVatTuPhauThuat
                {
                    Nhom = "Vật tư y tế",
                    TenDichVu = vt.Ten,
                    HamLuongNoiSanXuat = (vt.NhaSanXuat != null && vt.NhaSanXuat != "" && vt.NuocSanXuat != null && vt.NuocSanXuat != "") ? (vt.NhaSanXuat + ", " +vt.NuocSanXuat) : (vt.NhaSanXuat + vt.NuocSanXuat),
                    DonViTinh = vt.DonViTinh,
                    LaThuocVatTuBHYT = vt.LaVatTuBHYT,
                    KhongTinhPhi = vt.KhongTinhPhi != null && vt.KhongTinhPhi == true,
                    GiaiDoanPhauThuat = vt.GiaiDoanPhauThuat,
                    SoLuong = vt.SoLuong,
                    DonGia = vt.DonGiaBan
                }).ToList();
            var allData = thongTinDuocPhams.Concat(thongTinVatTus);
            var returnData = allData.GroupBy(
                o => new {o.LaThuocVatTuBHYT, o.Nhom, o.TenDichVu, o.HamLuongNoiSanXuat, o.DonViTinh, o.DonGia, o.KhongTinhPhi, o.GiaiDoanPhauThuat }, o => o,
                (k, v) => new DanhSachThuocVaVatTuPhauThuat
                {
                    Nhom = k.Nhom,
                    TenDichVu = k.TenDichVu,
                    HamLuongNoiSanXuat = k.HamLuongNoiSanXuat,
                    DonViTinh = k.DonViTinh,
                    LaThuocVatTuBHYT = k.LaThuocVatTuBHYT,
                    KhongTinhPhi = k.KhongTinhPhi,
                    GiaiDoanPhauThuat = k.GiaiDoanPhauThuat,
                    SoLuong = v.Sum(x=>x.SoLuong).MathRoundNumber(2),
                    DonGia = k.DonGia
                }).OrderByDescending(o=>o.Loai).ThenBy(o=>o.Nhom).ThenBy(o=>o.TenDichVu);
            return new GridDataSource { Data = returnData.ToArray() };
        }

        public virtual byte[] ExportBangKeThuocVatTuPT(GridDataSource gridDataSource, BaoCaoThuocVaVatTuPhauThuatQueryInfoVo query)
        {
            var datas = (ICollection<DanhSachThuocVaVatTuPhauThuat>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<ThongTinBenhNhanPhauThuatQueryInfo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BẢNG KÊ THUỐC VÀ VẬT TƯ PHẪU THUẬT/THỦ THUẬT");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 25;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 25;
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
                        range.Worksheet.Cells["A3:H3"].Value = "BẢNG KÊ THUỐC VÀ VẬT TƯ PHẪU THUẬT/THỦ THUẬT";
                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    var phongBenhVien = _phongBenhVienRepository.TableNoTracking.Where(c => c.Id == query.PhongBenhVienId).Select(o => o.Ten).FirstOrDefault();
                    var dsBN = GetDanhSachBenhNhanPhongPhauThuats(query);

                    using (var range = worksheet.Cells["A4:B4"])
                    {
                        range.Worksheet.Cells["A4:B4"].Merge = true;
                        range.Worksheet.Cells["A4:B4"].Value = "Nơi thực hiện";
                        range.Worksheet.Cells["A4:B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A3:B4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:B4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:B4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:B4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["C4:C4"])
                    {                     
                        range.Worksheet.Cells["C4:C4"].Value = phongBenhVien;
                        range.Worksheet.Cells["C4:C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C4:C4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C4:C4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["C4:C4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C4:C4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:B5"])
                    {
                        range.Worksheet.Cells["A5:B5"].Merge = true;
                        range.Worksheet.Cells["A5:B5"].Value = "Họ tên NB (Mã BN)";
                        range.Worksheet.Cells["A5:B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:B5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:B5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:B5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["C5:C5"])
                    {                    
                        range.Worksheet.Cells["C5:C5"].Value =  dsBN;
                        range.Worksheet.Cells["C5:C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C5:C5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["C5:C5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C5:C5"].Style.Font.Bold = true;
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
                        range.Worksheet.Cells["B7"].Value = "Loại";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tên thuốc, VTYT";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Hàm lượng/ Nơi sản xuất";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "ĐVT";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Số lượng";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Đơn giá";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Thành tiền";
                    }


                    //write data from line 9       
                    int index = 8;
                    var stt = 1;
                    var lstLoai = datas.GroupBy(x => new { x.Loai })
                       .Select(item => new NhomGroupVo
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
                                worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["A" + index + ":G" + index].Merge = true;
                                worksheet.Cells["A" + index + ":G" + index].Value = loai.Loai;

                                worksheet.Cells["G" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                worksheet.Cells["G" + index + ":H" + index].Style.Font.Bold = true;
                                worksheet.Cells["G" + index + ":H" + index].Value = 
                                    datas.Where(o => o.Loai == loai.Loai && o.Loai == loai.Loai).Sum(c=>c.ThanhTien);

                                //}
                                index++;

                                var lstNhomTheoLoai = datas.Where(o => o.Loai == loai.Loai)
                               .GroupBy(x => new { x.Loai, x.Nhom })
                                  .Select(item => new NhomGroupVo
                                  {
                                      Loai = item.First().Loai,
                                      Nhom = item.First().Nhom

                                  }).OrderBy(p => p.Nhom).ToList(); //demo desc
                                if (lstNhomTheoLoai.Any())
                                {
                                    foreach (var nhom in lstNhomTheoLoai)
                                    {
                                        var listThuocVatTuTheoNhom = datas.Where(o => o.Loai == loai.Loai && o.Nhom == nhom.Nhom).ToList();

                                        worksheet.Cells["A" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                        worksheet.Cells["A" + index + ":H" + index].Merge = true;
                                        worksheet.Cells["A" + index + ":H" + index].Value = nhom.Nhom;                           

                                        index++;


                                        if (listThuocVatTuTheoNhom.Any())
                                        {
                                            foreach (var item in listThuocVatTuTheoNhom)
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
                                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                worksheet.Cells["A" + index].Value = stt;

                                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["B" + index].Value = item.TenGiaiDoanPhauThuat;

                                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["C" + index].Value = item.TenDichVu;

                                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["D" + index].Value = item.HamLuongNoiSanXuat;

                                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["E" + index].Value = item.DonViTinh;

                                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["F" + index].Value = item.SoLuong;

                                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["G" + index].Value = item.DonGia;

                                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                                worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                                worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                                                worksheet.Cells["H" + index].Value = item.ThanhTien;
                                                stt++;
                                                index++;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var date = DateTime.Now;                        
                        using (var range = worksheet.Cells["A" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["A" + index + ":F" + index].Value = "Cộng khoản: " + datas.Count();
                        }
                        using (var range = worksheet.Cells["G" + index + ":G" + index])
                        {
                            range.Worksheet.Cells["G" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["G" + index + ":G" + index].Merge = true;
                            range.Worksheet.Cells["G" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["G" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["G" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["G" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["G" + index + ":G" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["G" + index + ":G" + index].Value = "Tổng cộng:";
                        }
                        using (var range = worksheet.Cells["H" + index + ":H" + index])
                        {
                            worksheet.Cells["H" + index + ":H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Font.Bold = true;
                            worksheet.Cells["H" + index + ":H" + index].Value = datas.Sum(c => c.ThanhTien);
                        }

                        index += 2;

                        using (var range = worksheet.Cells["H" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["H" + index + ":H" + index].Merge = true;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["H" + index + ":H" + index].Value = $"Ngày {date.Day}  tháng {date.Month}  năm {date.Year}";
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
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = "BS.PHẪU THUẬT VIÊN CHÍNH";
                        }

                        using (var range = worksheet.Cells["C" + index + ":D" + index])
                        {                            
                            range.Worksheet.Cells["C" + index + ":D" + index].Merge = true;
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["C" + index + ":D" + index].Value = "GÂY MÊ";
                        }

                        using (var range = worksheet.Cells["E" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["E" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["E" + index + ":F" + index].Value = "ĐIỀU DƯỠNG";
                        }

                        using (var range = worksheet.Cells["H" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["H" + index + ":H" + index].Value = "Người lập";
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

                        using (var range = worksheet.Cells["C" + index + ":D" + index])
                        {
                            range.Worksheet.Cells["C" + index + ":D" + index].Merge = true;
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["C" + index + ":D" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["E" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["E" + index + ":F" + index].Merge = true;
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["E" + index + ":F" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["E" + index + ":F" + index].Value = "(Ký, ghi rõ họ tên)";
                        }

                        using (var range = worksheet.Cells["G" + index + ":G" + index])
                        {
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["H" + index + ":H" + index].Style.Font.Italic = true;
                            range.Worksheet.Cells["H" + index + ":H" + index].Value = "(Ký, ghi rõ họ tên)";
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
