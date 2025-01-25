using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        //update 22/06/2022: update bao cáo theo logic moi
        public async Task<GridDataSource> GetDataBaoCaoThongKeSoLuongThuThuatForGridAsync(QueryInfo queryInfo)
        {
            var phongThucHienThuThuats = new List<BaoCaoThongKeSoLuongThuThuatGridVo>();
            var timKiemNangCaoObj = new BaoCaoThongKeSoLuongThuThuatQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThongKeSoLuongThuThuatQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var nhomDichVuIds = new List<long>();
                var arrTenLoai1 = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription() };
                var arrTenLoai2 = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription() };
                var arrTenLoai3 = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription() };
                var arrTenLoaiDacBiet = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription() };
                var arrTatCaLoai = arrTenLoai1.Union(arrTenLoai2).Union(arrTenLoai3).Union(arrTenLoaiDacBiet);


                var cauHinhNhomThuThuat = _cauHinhService.GetSetting("CauHinhBaoCao.NhomThuThuat");
                long.TryParse(cauHinhNhomThuThuat?.Value, out long nhomThuThuatId);
                var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                    .Select(item => new NhomDichVuBenhVienTreeViewVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        ParentId = item.NhomDichVuBenhVienChaId,
                        Ma = item.Ma,
                        IsDefault = item.IsDefault
                    })
                    .ToListAsync();
                GetFullNhomThuThuat(lstNhomDichVu, nhomThuThuatId, nhomDichVuIds);

                var thongTinPhongBenhViens = _phongBenhVienRepository.TableNoTracking.Select(o => new { o.Id, TenPhong = o.Ten, o.KhoaPhongId, TenKhoa = o.KhoaPhong.Ten }).ToList();
                var loaiThuThuats = new List<string> { LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription() };
                var dvktbvs = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(o => loaiThuThuats.Contains(o.LoaiPhauThuatThuThuat))
                    .Select(o => new { o.Id, o.Ten, o.LoaiPhauThuatThuThuat, ChuyenKhoaChuyenNganh = o.ChuyenKhoaChuyenNganhId != null ? o.ChuyenKhoaChuyenNganh.Ten : "", o.ThongTu })
                    .ToList();
                var dvktbvIds = dvktbvs.Select(o => o.Id).ToList();

                phongThucHienThuThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                && (nhomDichVuIds.Contains(x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId) || dvktbvIds.Contains(x.DichVuKyThuatBenhVienId))
                                && ((x.ThoiDiemHoanThanh != null && tuNgay <= x.ThoiDiemHoanThanh && x.ThoiDiemHoanThanh < denNgay)
                                    || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && tuNgay <= x.ThoiDiemThucHien && x.ThoiDiemThucHien < denNgay))                                
                                && (timKiemNangCaoObj.KhoaPhongId == null || timKiemNangCaoObj.KhoaPhongId == 0 || x.NoiThucHien.KhoaPhongId == timKiemNangCaoObj.KhoaPhongId))
                    .Select(item => new BaoCaoThongKeSoLuongThuThuatGridVo
                    {
                        PhongThucHien = item.NoiThucHien.Ten,
                        LoaiI = arrTenLoai1.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        LoaiII = arrTenLoai2.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        LoaiIII = arrTenLoai3.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        DacBiet = arrTenLoaiDacBiet.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        Khac = (string.IsNullOrEmpty(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) || !arrTatCaLoai.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat)) ? item.SoLan : 0,
                    })
                    .GroupBy(item => item.PhongThucHien)
                    .Select(item => new BaoCaoThongKeSoLuongThuThuatGridVo
                    {
                        PhongThucHien = item.Key,
                        LoaiI = item.Sum(a => a.LoaiI.GetValueOrDefault()),
                        LoaiII = item.Sum(a => a.LoaiII.GetValueOrDefault()),
                        LoaiIII = item.Sum(a => a.LoaiIII.GetValueOrDefault()),
                        DacBiet = item.Sum(a => a.DacBiet.GetValueOrDefault()),
                        Khac = item.Sum(a => a.Khac.GetValueOrDefault()),
                    })
                    .OrderBy(x => x.PhongThucHien)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();
            }

            return new GridDataSource
            {
                Data = phongThucHienThuThuats.ToArray(),
                TotalRowCount = phongThucHienThuThuats.Count()
            };
        }
        public async Task<GridDataSource> GetDataBaoCaoThongKeSoLuongThuThuatForGridAsyncOld(QueryInfo queryInfo)
        {
            var phongThucHienThuThuats = new List<BaoCaoThongKeSoLuongThuThuatGridVo>();
            var timKiemNangCaoObj = new BaoCaoThongKeSoLuongThuThuatQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThongKeSoLuongThuThuatQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var nhomDichVuIds = new List<long>();
                var arrTenLoai1 = new string[] {"T1", Enums.LoaiPTTT.Loai1.GetDescription()};
                var arrTenLoai2 = new string[] { "T2", Enums.LoaiPTTT.Loai2.GetDescription()};
                var arrTenLoai3 = new string[] { "T3", Enums.LoaiPTTT.Loai3.GetDescription()};
                var arrTenLoaiDacBiet = new string[] { "TDB", Enums.LoaiPTTT.DacBiet.GetDescription()};
                var arrTatCaLoai = arrTenLoai1.Union(arrTenLoai2).Union(arrTenLoai3).Union(arrTenLoaiDacBiet);


                var cauHinhNhomThuThuat = _cauHinhService.GetSetting("CauHinhBaoCao.NhomThuThuat");
                long.TryParse(cauHinhNhomThuThuat?.Value, out long nhomThuThuatId);
                var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                    .Select(item => new NhomDichVuBenhVienTreeViewVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        ParentId = item.NhomDichVuBenhVienChaId,
                        Ma = item.Ma,
                        IsDefault = item.IsDefault
                    })
                    .ToListAsync();
                GetFullNhomThuThuat(lstNhomDichVu, nhomThuThuatId, nhomDichVuIds);

                phongThucHienThuThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                && nhomDichVuIds.Contains(x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId)
                                && x.ThoiDiemHoanThanh >= tuNgay
                                && x.ThoiDiemHoanThanh <= denNgay
                                && (timKiemNangCaoObj.KhoaPhongId == null || x.NoiThucHien.KhoaPhongId == timKiemNangCaoObj.KhoaPhongId))
                    .Select(item => new BaoCaoThongKeSoLuongThuThuatGridVo
                    {
                        PhongThucHien = item.NoiThucHien.Ten,
                        LoaiI = arrTenLoai1.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? 1 : (int?)null,
                        LoaiII = arrTenLoai2.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? 1 : (int?)null,
                        LoaiIII = arrTenLoai3.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? 1 : (int?)null,
                        DacBiet = arrTenLoaiDacBiet.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? 1 : (int?)null,
                        Khac = (string.IsNullOrEmpty(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) || !arrTatCaLoai.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat)) ? 1 : (int?)null,
                    })
                    .GroupBy(item => item.PhongThucHien)
                    .Select(item => new BaoCaoThongKeSoLuongThuThuatGridVo
                    {
                        PhongThucHien = item.Key,
                        LoaiI = item.Any(a => a.LoaiI != null) ? item.Count(a => a.LoaiI != null) : (int?)null,
                        LoaiII = item.Any(a => a.LoaiII != null) ? item.Count(a => a.LoaiII != null) : (int?)null,
                        LoaiIII = item.Any(a => a.LoaiIII != null) ? item.Count(a => a.LoaiIII != null) : (int?)null,
                        DacBiet = item.Any(a => a.DacBiet != null) ? item.Count(a => a.DacBiet != null) : (int?)null,
                        Khac = item.Any(a => a.Khac != null) ? item.Count(a => a.Khac != null) : (int?)null,
                    })
                    .OrderBy(x => x.PhongThucHien)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();
            }

            return new GridDataSource
            {
                Data = phongThucHienThuThuats.ToArray(),
                TotalRowCount = phongThucHienThuThuats.Count()
            };
        }

        public async Task<GridDataSource> GetTotalBaoCaoThongKeSoLuongThuThuatAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoThongKeSoLuongThuThuatQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThongKeSoLuongThuThuatQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (tuNgay != null && denNgay != null)
            {
                var nhomDichVuIds = new List<long>();
                var arrTenLoai1 = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription() };
                var arrTenLoai2 = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription() };
                var arrTenLoai3 = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription() };
                var arrTenLoaiDacBiet = new string[] { LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription() };
                var arrTatCaLoai = arrTenLoai1.Union(arrTenLoai2).Union(arrTenLoai3).Union(arrTenLoaiDacBiet);


                var cauHinhNhomThuThuat = _cauHinhService.GetSetting("CauHinhBaoCao.NhomThuThuat");
                long.TryParse(cauHinhNhomThuThuat?.Value, out long nhomThuThuatId);
                var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                    .Select(item => new NhomDichVuBenhVienTreeViewVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        ParentId = item.NhomDichVuBenhVienChaId,
                        Ma = item.Ma,
                        IsDefault = item.IsDefault
                    })
                    .ToListAsync();
                GetFullNhomThuThuat(lstNhomDichVu, nhomThuThuatId, nhomDichVuIds);

                var thongTinPhongBenhViens = _phongBenhVienRepository.TableNoTracking.Select(o => new { o.Id, TenPhong = o.Ten, o.KhoaPhongId, TenKhoa = o.KhoaPhong.Ten }).ToList();
                var loaiThuThuats = new List<string> { LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription(), LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription() };
                var dvktbvs = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(o => loaiThuThuats.Contains(o.LoaiPhauThuatThuThuat))
                    .Select(o => new { o.Id, o.Ten, o.LoaiPhauThuatThuThuat, ChuyenKhoaChuyenNganh = o.ChuyenKhoaChuyenNganhId != null ? o.ChuyenKhoaChuyenNganh.Ten : "", o.ThongTu })
                    .ToList();
                var dvktbvIds = dvktbvs.Select(o => o.Id).ToList();

                var phongThucHienThuThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                && (nhomDichVuIds.Contains(x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId) || dvktbvIds.Contains(x.DichVuKyThuatBenhVienId))
                                && ((x.ThoiDiemHoanThanh != null && tuNgay <= x.ThoiDiemHoanThanh && x.ThoiDiemHoanThanh < denNgay)
                                    || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && tuNgay <= x.ThoiDiemThucHien && x.ThoiDiemThucHien < denNgay))
                                && (timKiemNangCaoObj.KhoaPhongId == null || timKiemNangCaoObj.KhoaPhongId == 0 || x.NoiThucHien.KhoaPhongId == timKiemNangCaoObj.KhoaPhongId))
                    .Select(item => new BaoCaoThongKeSoLuongThuThuatGridVo
                    {
                        PhongThucHien = item.NoiThucHien.Ten,
                        LoaiI = arrTenLoai1.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        LoaiII = arrTenLoai2.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        LoaiIII = arrTenLoai3.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        DacBiet = arrTenLoaiDacBiet.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) ? item.SoLan : 0,
                        Khac = (string.IsNullOrEmpty(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) || !arrTatCaLoai.Contains(item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat)) ? item.SoLan : 0,
                    })
                    .GroupBy(item => item.PhongThucHien)
                    .Select(item => new BaoCaoThongKeSoLuongThuThuatGridVo
                    {
                        PhongThucHien = item.Key,
                        LoaiI = item.Sum(a => a.LoaiI.GetValueOrDefault()),
                        LoaiII = item.Sum(a => a.LoaiII.GetValueOrDefault()),
                        LoaiIII = item.Sum(a => a.LoaiIII.GetValueOrDefault()),
                        DacBiet = item.Sum(a => a.DacBiet.GetValueOrDefault()),
                        Khac = item.Sum(a => a.Khac.GetValueOrDefault()),
                    });

                var countTask = phongThucHienThuThuats.Count();
                return new GridDataSource { TotalRowCount = countTask };
            }
            else
            {
                return new GridDataSource { TotalRowCount = 0 };
            }
        }

        public static List<NhomDichVuBenhVienTreeViewVo> GetFullNhomThuThuat(List<NhomDichVuBenhVienTreeViewVo> lstNhom, long parentId, List<long> nhomDichVuIds)
        {
            var query = lstNhom
                .Where(c => c.ParentId != null && c.ParentId == parentId)
                .Select(c => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = parentId,
                    Ma = c.Ma,
                    Items = GetFullNhomThuThuat(lstNhom, c.KeyId, nhomDichVuIds)
                })
                .ToList();
            nhomDichVuIds.AddRange(query.Select(x => x.KeyId));
            return query;
        }


        public virtual byte[] ExportBaoCaoThongKeSoLuongThuThuat(GridDataSource gridDataSource, QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoThongKeSoLuongThuThuatQueryInfo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoThongKeSoLuongThuThuatQueryInfo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)) //) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var datas = (ICollection<BaoCaoThongKeSoLuongThuThuatGridVo>)gridDataSource.Data;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoThongKeSoLuongThuThuatGridVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using(var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO THỐNG KÊ SỐ LƯỢNG THỦ THUẬT");

                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;

                    worksheet.Row(3).Height = 30;
                    worksheet.DefaultColWidth = 7;


                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:G3"])
                    {
                        range.Worksheet.Cells["A3:G3"].Merge = true;
                        range.Worksheet.Cells["A3:G3"].Value = "Báo cáo thống kê số lượng thủ thuật";
                        range.Worksheet.Cells["A3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:G3"].Style.Font.SetFromFont(new Font("Times New Roman", 18));
                        range.Worksheet.Cells["A3:G3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:G3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:G4"])
                    {
                        range.Worksheet.Cells["A4:G4"].Merge = true;
                        range.Worksheet.Cells["A4:G4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:G4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var tenKhoaPhong = string.Empty;
                    if (timKiemNangCaoObj.KhoaPhongId == 0)
                    {
                        tenKhoaPhong = "Tất cả";
                    }
                    else
                    {
                        tenKhoaPhong = _KhoaPhongRepository.TableNoTracking.Where(p => p.Id == timKiemNangCaoObj.KhoaPhongId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A5:G5"])
                    {
                        range.Worksheet.Cells["A5:G5"].Merge = true;
                        range.Worksheet.Cells["A5:G5"].Value = "Khoa thực hiện: " + tenKhoaPhong;
                        range.Worksheet.Cells["A5:G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:G5"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A5:G5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:G7"])
                    {
                        range.Worksheet.Cells["A7:G7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:G7"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A7:G7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Phòng thực hiện";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Loại I";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Loại II";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Loại III";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Đặc biệt";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Khác";

                        var manager = new PropertyManager<BaoCaoThongKeSoLuongThuThuatGridVo>(requestProperties);
                        int index = 8;
                        ////Đổ data vào
                        ///
                        var stt = 1;
                        if (datas.Any())
                        {
                            foreach(var item in datas)
                            {
                                range.Worksheet.Cells["A" + index + ":G" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":G" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Value = stt;

                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Value = item.PhongThucHien;

                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Value = item.LoaiI;

                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Value = item.LoaiII;

                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Value = item.LoaiIII;

                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Value = item.DacBiet;

                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Value = item.Khac;


                                index++;
                                stt++;
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
