using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.Helpers;
using XuatKhoDuocPham = Camino.Core.Domain.Entities.XuatKhos.XuatKhoDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Globalization;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Services.KhoDuocPhams;
using static Camino.Core.Domain.ValueObject.QuayThuoc.DanhSachChoXuatThuocVO;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Services.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.QuayThuoc
{
    [ScopedDependency(ServiceType = typeof(IQuayThuocService))]
    public partial class QuayThuocService : MasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>, IQuayThuocService
    {
        public GridDataSource GetDanhSachchoCapThuocBHYT(QueryInfo queryInfo, bool isPrint)
        {
            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "TrangThai", Dir = "asc" }, new Sort { Field = "Id", Dir = "asc" } };
            }
            if (queryInfo.AdditionalSearchString != null)
            {
                var queryString = JsonConvert.DeserializeObject<QuayThuocBHYTGridVo>(queryInfo.AdditionalSearchString);
                var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                .Where(o => (o.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                           o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                                                                         ((x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                         x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)))))
                                                                         ||
                                            (o.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                            o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                       u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                  .Select(s => new DonThuocThanhToanGridVo
                  {
                      Id = s.Id,
                      BenhNhanId = s.BenhNhanId,
                      MaBN = s.BenhNhan.MaBN,
                      MaTN = s.MaYeuCauTiepNhan,
                      YeuCauTiepNhanId = s.Id,
                      HoTen = s.HoTen,
                      NamSinh = s.NamSinh,
                      GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                      DiaChi = s.DiaChiDayDu,
                      SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                      DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",

                      TrangThai = TrangThaiThanhToan.ChuaThanhToan,
                      IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                      TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                            + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                      CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                  });
                if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                {
                    querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                }
                if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                {

                    querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                }
                if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                {
                    querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                }
                if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                {
                    querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                }

                var startDate = DateTime.MinValue;
                var endDate = DateTime.Now;

                if (queryString.RangeDate != null)
                {
                    if (queryString.RangeDate.StartDate != null)
                    {
                        startDate = (queryString.RangeDate.StartDate ?? endDate).Date.AddMilliseconds(0);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= startDate));
                    }
                    if (queryString.RangeDate.EndDate != null)
                    {
                        endDate = (queryString.RangeDate.EndDate ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= endDate));
                    }
                }

                var queryTask = isPrint == true ? querydata.OrderBy(queryInfo.SortString).ToArray() : querydata.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArray();

                return new GridDataSource { Data = queryTask };

            }
            else
            {

                var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                 .Where(o => (o.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                            o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                                                                          ((x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                          x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)))))
                                                                          ||
                                             (o.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                             o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                        u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                   .Select(s => new DonThuocThanhToanGridVo
                   {
                       Id = s.Id,
                       BenhNhanId = s.BenhNhanId,
                       MaBN = s.BenhNhan.MaBN,
                       MaTN = s.MaYeuCauTiepNhan,
                       YeuCauTiepNhanId = s.Id,
                       HoTen = s.HoTen,
                       NamSinh = s.NamSinh,
                       GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                       DiaChi = s.DiaChiDayDu,
                       SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                       DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",

                       TrangThai = TrangThaiThanhToan.ChuaThanhToan,
                       IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                       TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                             + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                       CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                   });
                var queryTask = isPrint == true ? querydata.OrderBy(queryInfo.SortString).ToArray() : querydata.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = queryTask };
            }
        }

        public GridDataSource GetTotalDanhSachchoCapThuocBHYT(QueryInfo queryInfo)
        {
            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "TrangThai", Dir = "asc" }, new Sort { Field = "Id", Dir = "asc" } };
            }
            if (queryInfo.AdditionalSearchString != null)
            {
                var queryString = JsonConvert.DeserializeObject<QuayThuocBHYTGridVo>(queryInfo.AdditionalSearchString);
                var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                .Where(o => (o.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                           o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                                                                         ((x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                         x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)))))
                                                                         ||
                                            (o.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                            o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                       u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                  .Select(s => new DonThuocThanhToanGridVo
                  {
                      Id = s.Id,
                      BenhNhanId = s.BenhNhanId,
                      MaBN = s.BenhNhan.MaBN,
                      MaTN = s.MaYeuCauTiepNhan,
                      YeuCauTiepNhanId = s.Id,
                      HoTen = s.HoTen,
                      NamSinh = s.NamSinh,
                      GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                      DiaChi = s.DiaChiDayDu,
                      SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                      DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",

                      TrangThai = TrangThaiThanhToan.ChuaThanhToan,
                      IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                      TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                            + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                      CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                  });
                if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                {
                    querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                }
                if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                {

                    querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                }
                if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                {
                    querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                }
                if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                {
                    querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                }

                var startDate = DateTime.MinValue;
                var endDate = DateTime.Now;

                if (queryString.RangeDate != null)
                {
                    if (queryString.RangeDate.StartDate != null)
                    {
                        startDate = (queryString.RangeDate.StartDate ?? endDate).Date.AddMilliseconds(0);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= startDate));
                    }
                    if (queryString.RangeDate.EndDate != null)
                    {
                        endDate = (queryString.RangeDate.EndDate ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= endDate));
                    }
                }

                var queryTask = querydata.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }
            else
            {

                var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                 .Where(o => (o.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                            o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                                                                          ((x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                          x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)))))
                                                                          ||
                                             (o.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                             o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                        u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                   .Select(s => new DonThuocThanhToanGridVo
                   {
                       Id = s.Id,
                       BenhNhanId = s.BenhNhanId,
                       MaBN = s.BenhNhan.MaBN,
                       MaTN = s.MaYeuCauTiepNhan,
                       YeuCauTiepNhanId = s.Id,
                       HoTen = s.HoTen,
                       NamSinh = s.NamSinh,
                       GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                       DiaChi = s.DiaChiDayDu,
                       SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                       DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",

                       TrangThai = TrangThaiThanhToan.ChuaThanhToan,
                       IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                       TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                             + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                       CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                   });
                var queryTask = querydata.Count();
                return new GridDataSource { TotalRowCount = queryTask };
            }

        }

        public virtual byte[] ExportDanhSachChoCapThuocBHYT(ICollection<DonThuocThanhToanGridVo> datas)
        {
            var queryInfo = new DonThuocThanhToanGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DonThuocThanhToanGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH ĐƠN THUỐC CẤP THUỐC BHYT");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH ĐƠN THUỐC CẤP THUỐC BHYT".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "Mã TN" }, { "B", "Mã BN" }, { "C", "Họ Tên" } , { "D", "Năm Sinh" },
                                    { "E", "Giới Tính" }, { "F", "Địa Chỉ" },{ "G", "Điện Thoại" },{ "H", "Đối Tượng" },{ "I", "Tổng Giá Trị Đơn Thuốc" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<DonThuocThanhToanGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var donThuocTrongNgay in datas)
                    {
                        manager.CurrentObject = donThuocTrongNgay;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = donThuocTrongNgay.MaTN;
                        worksheet.Cells["B" + index].Value = donThuocTrongNgay.MaBN;
                        worksheet.Cells["C" + index].Value = donThuocTrongNgay.HoTen;
                        worksheet.Cells["D" + index].Value = donThuocTrongNgay.NamSinh + "";
                        worksheet.Cells["E" + index].Value = donThuocTrongNgay.GioiTinhHienThi;
                        worksheet.Cells["F" + index].Value = donThuocTrongNgay.DiaChi;
                        worksheet.Cells["G" + index].Value = donThuocTrongNgay.SoDienThoai;
                        worksheet.Cells["H" + index].Value = donThuocTrongNgay.DoiTuong;
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Value = Convert.ToDouble(donThuocTrongNgay.TongGiaTriDonThuoc).ApplyFormatMoneyToDouble();
                        //worksheet.Cells["J" + index].Value = donThuocTrongNgay.TrangThaiHienThi;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;
                        int sttItems = 1;


                        using (var range = worksheet.Cells["B" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            string[,] SetColumnLinhDuTrus = { { "B", "#" },{ "C", "Mã Đơn" }, { "D", "Ngày Kê Đơn" },{ "E", "Dịch Vụ Khám" } , { "F", "Bác Sĩ Khám" } ,
                                { "G", "Số Tiền" },{ "H", "Tình Trạng" }};

                            for (int i = 0; i < SetColumnLinhDuTrus.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnLinhDuTrus[i, 0]).ToString() + index + ":" + (SetColumnLinhDuTrus[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnLinhDuTrus[i, 1];
                            }
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;
                        var donThuocTrongNgayBHYT = donThuocTrongNgay.ListChilDonThuocTrongNgay.Where(s => s.LoaiDonThuoc == "Thuốc BHYT")
                           .Select(k => new DonThuocCuaBenhNhanGridVo()
                           {
                               MaDon = k.MaDon,
                               NgayKeDon = k.NgayKeDon,
                               DVKham = k.DVKham,
                               BSKham = k.BSKham,
                               SoTien = k.SoTien,
                               TinhTrang = k.TinhTrang
                           }).ToList();
                        var donThuocTrongNgayKhongBHYT = donThuocTrongNgay.ListChilDonThuocTrongNgay.Where(s => s.LoaiDonThuoc == "Thuốc Không BHYT")
                            .Select(k => new DonThuocCuaBenhNhanGridVo()
                            {
                                MaDon = k.MaDon,
                                NgayKeDon = k.NgayKeDon,
                                DVKham = k.DVKham,
                                BSKham = k.BSKham,
                                SoTien = k.SoTien,
                                TinhTrang = k.TinhTrang
                            }).ToList();
                        if (donThuocTrongNgayBHYT.Count() > 0)
                        {
                            using (var range = worksheet.Cells["B" + index + ":H" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                //Set column A to K
                                string[,] SetColumnLoaiDuocPham = { { "B", "BHYT" } };

                                for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            foreach (var nhom in donThuocTrongNgayBHYT)
                            {
                                worksheet.Cells["B" + index].Value = sttItems++;
                                worksheet.Cells["C" + index].Value = nhom.MaDon;  // to do
                                worksheet.Cells["D" + index].Value = nhom.NgayKeDon;
                                worksheet.Cells["E" + index].Value = nhom.DVKham;
                                worksheet.Cells["F" + index].Value = nhom.BSKham;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Value = nhom.SoTien.ApplyFormatMoneyToDouble();
                                worksheet.Cells["H" + index].Value = nhom.TinhTrang;

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                            }
                        }
                        if (donThuocTrongNgayKhongBHYT.Count() > 0)
                        {
                            using (var range = worksheet.Cells["B" + index + ":H" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                //Set column A to K
                                string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            foreach (var nhom in donThuocTrongNgayKhongBHYT)
                            {
                                worksheet.Cells["B" + index].Value = sttItems++;
                                worksheet.Cells["C" + index].Value = nhom.MaDon;  // to do
                                worksheet.Cells["D" + index].Value = nhom.NgayKeDon;
                                worksheet.Cells["E" + index].Value = nhom.DVKham;
                                worksheet.Cells["F" + index].Value = nhom.BSKham;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Value = nhom.SoTien.ApplyFormatMoneyToDouble();
                                worksheet.Cells["H" + index].Value = nhom.TinhTrang;

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                            }
                        }
                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #region  DANH SÁCH LỊCH SỬ CẤP THUỐC 28/02/2022

        public GridDataSource GetDanhSachLichSuCapThuocBHYT(QueryInfo queryInfo, bool isPrint)
        {
            var queryDonThuoc = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc &&
                           o.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                .GroupBy(
                    o => new
                    {
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.Id,
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        o.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                        o.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.GioiTinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                        o.DonThuocThanhToan.YeuCauTiepNhan.SoDienThoai,
                        o.DonThuocThanhToan.YeuCauTiepNhan.CoBHYT,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                    }, o => o,
                    (k, v) => new DanhSachLichSuXuatThuocGridVo
                    {
                        Id = k.Id,
                        LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                        SoChungTu = k.SoPhieu,
                        NgayXuatThuoc = k.NgayXuat,
                        MaTN = k.MaYeuCauTiepNhan,
                        MaBN = k.MaBN,
                        HoTen = k.HoTen,
                        NamSinh = k.NamSinh,
                        DiaChi = k.DiaChiDayDu,
                        SoDienThoai = k.SoDienThoai,
                        DoiTuong = k.CoBHYT == true ? "BHYT (" + k.BHYTMucHuong + "%)" : "Viện phí",
                        LoaiGioiTinh = k.GioiTinh
                    })
                .Union(
                    _donVTYTThanhToanChiTietRepository.TableNoTracking
                        .Where(o => o.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
                        .GroupBy(
                            o => new
                            {
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.Id,
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.HoTen,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.NamSinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.GioiTinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.SoDienThoai,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.CoBHYT,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                            }, o => o,
                            (k, v) => new DanhSachLichSuXuatThuocGridVo
                            {
                                Id = k.Id,
                                LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiVatTu,
                                SoChungTu = k.SoPhieu,
                                NgayXuatThuoc = k.NgayXuat,
                                MaTN = k.MaYeuCauTiepNhan,
                                MaBN = k.MaBN,
                                HoTen = k.HoTen,
                                NamSinh = k.NamSinh,
                                DiaChi = k.DiaChiDayDu,
                                SoDienThoai = k.SoDienThoai,
                                DoiTuong = k.CoBHYT == true ? "BHYT (" + k.BHYTMucHuong + "%)" : "Viện phí",
                                LoaiGioiTinh = k.GioiTinh
                            })
                    );

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                queryDonThuoc = queryDonThuoc.Where(o =>
                        EF.Functions.Like(o.MaBN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.HoTen, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.MaTN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoDienThoai, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoChungTu, $"%{queryInfo.SearchTerms}%")
                );
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                    queryDonThuoc = queryDonThuoc.Where(p => p.NgayXuatThuoc >= tuNgay && p.NgayXuatThuoc <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }

            var dataOrderBy = queryDonThuoc.OrderByDescending(cc => cc.NgayXuatThuoc);
            var donthuoc = isPrint == true ? dataOrderBy.ToArray() : dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryDonThuoc.Count();
            return new GridDataSource { Data = donthuoc, TotalRowCount = countTask };
        }

        public GridDataSource GetTotalSachLichSuCapThuocBHYT(QueryInfo queryInfo)
        {
            var queryDonThuoc = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc &&
                           o.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                           && o.DonThuocThanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                .GroupBy(
                    o => new
                    {
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        o.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                        o.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.GioiTinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                        o.DonThuocThanhToan.YeuCauTiepNhan.SoDienThoai,
                        o.DonThuocThanhToan.YeuCauTiepNhan.CoBHYT,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                    }, o => o,
                    (k, v) => new DanhSachLichSuXuatThuocGridVo
                    {
                        LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                        SoChungTu = k.SoPhieu,
                        NgayXuatThuoc = k.NgayXuat,
                        MaTN = k.MaYeuCauTiepNhan,
                        MaBN = k.MaBN,
                        HoTen = k.HoTen,
                        NamSinh = k.NamSinh,
                        SoDienThoai = k.SoDienThoai,
                    })
                .Union(
                    _donVTYTThanhToanChiTietRepository.TableNoTracking
                        .Where(o => o.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
                        .GroupBy(
                            o => new
                            {
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.HoTen,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.NamSinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.GioiTinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.SoDienThoai,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.CoBHYT,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                            }, o => o,
                            (k, v) => new DanhSachLichSuXuatThuocGridVo
                            {
                                LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                                SoChungTu = k.SoPhieu,
                                NgayXuatThuoc = k.NgayXuat,
                                MaTN = k.MaYeuCauTiepNhan,
                                MaBN = k.MaBN,
                                HoTen = k.HoTen,
                                NamSinh = k.NamSinh,
                                SoDienThoai = k.SoDienThoai,
                            })
                    );

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                queryDonThuoc = queryDonThuoc.Where(o =>
                        EF.Functions.Like(o.MaBN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.HoTen, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.MaTN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoDienThoai, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoChungTu, $"%{queryInfo.SearchTerms}%")
                );
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                    queryDonThuoc = queryDonThuoc.Where(p => p.NgayXuatThuoc >= tuNgay && p.NgayXuatThuoc <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }
            return new GridDataSource { TotalRowCount = queryDonThuoc.Count() };
        }

        #endregion
    }
}

