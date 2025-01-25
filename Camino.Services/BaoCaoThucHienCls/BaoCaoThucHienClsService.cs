using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.BaoCaoThucHienCls;
using static Camino.Core.Domain.Enums;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Services.BaoCaoThucHienCls
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoThucHienClsService))]
    public class BaoCaoThucHienClsService : MasterFileService<YeuCauDichVuKyThuat>, IBaoCaoThucHienClsService
    {
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> _noiTruBenhAnRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;

        public BaoCaoThucHienClsService(IRepository<YeuCauDichVuKyThuat> repository,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> noiTruBenhAnRepository,
            IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
            IRepository<User> userRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            ) : base(repository)
        {
            _khoaPhongRepository = khoaPhongRepository;
            _userAgentHelper = userAgentHelper;
            _phongBenhVienRepository = phongBenhVienRepository;
            _noiTruBenhAnRepository = noiTruBenhAnRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _userRepository = userRepository;
        }

        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            DateTime tuNgay = DateTime.Now;
            DateTime denNgay = DateTime.Now;
            var queryInfoBacSi = new BaoCaoThucHienCLSVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryInfoBacSi = JsonConvert.DeserializeObject<BaoCaoThucHienCLSVo>(queryInfo.AdditionalSearchString);
                queryInfoBacSi.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryInfoBacSi.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryInfoBacSi.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            }

            var query = BaseRepository.TableNoTracking
                  .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                  && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                     && z.NhanVienKetLuanId != null
                     && (queryInfoBacSi.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfoBacSi.BacSiKetLuanId)
                    && (
                        (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= tuNgay && z.ThoiDiemThucHien < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= tuNgay && z.ThoiDiemKetLuan < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= tuNgay && z.ThoiDiemHoanThanh < denNgay)
                       )
                      && z.NoiThucHien.KhoaPhongId == queryInfoBacSi.KhoaId
                     )
                .Select(s => new BaoCaoThucHienCLSVo
                {
                    Id = s.Id,
                    BacSiKetLuanId = s.NhanVienKetLuanId,
                    TenBacSiKetLuan = s.NhanVienKetLuan.User.HoTen,
                    KhoaId = s.NoiChiDinh.KhoaPhongId,
                    SoBacSiKetLuan = 1
                }).GroupBy(x => new { x.BacSiKetLuanId, x.TenBacSiKetLuan })
                .Select(item => new BaoCaoThucHienCLSVo()
                {
                    BacSiKetLuanId = item.First().BacSiKetLuanId,
                    TenBacSiKetLuan = item.First().TenBacSiKetLuan,
                    KhoaId = item.First().KhoaId,
                    SoBacSiKetLuan = item.Sum(x => x.SoBacSiKetLuan)
                }).OrderBy(x => x.TenBacSiKetLuan).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }      
        public async Task<GridDataSource> GetDataForGridAsyncChild(QueryInfo queryInfo, bool exportExcel = false)
        {
            DateTime tuNgay = DateTime.Now;
            DateTime denNgay = DateTime.Now;
            var queryInfoBacSi = new BaoCaoThucHienCLSVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryInfoBacSi = JsonConvert.DeserializeObject<BaoCaoThucHienCLSVo>(queryInfo.AdditionalSearchString);
                queryInfoBacSi.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryInfoBacSi.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryInfoBacSi.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            }

            long bacSiThucHienId = long.Parse(queryInfo.SearchTerms);

            var allBenhNhans = BaseRepository.TableNoTracking
                  .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                  && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)                    
                       && (
                           (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= tuNgay && z.ThoiDiemThucHien < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= tuNgay && z.ThoiDiemKetLuan < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= tuNgay && z.ThoiDiemHoanThanh < denNgay)
                           )
                   && z.NhanVienKetLuanId == bacSiThucHienId
                   && z.NoiThucHien.KhoaPhongId == queryInfoBacSi.KhoaId)
                  .Select(s => new BaoCaoThucHienCLSChiTietVo
                  {
                      Id = s.Id,
                      NgayThucHien = s.ThoiDiemThucHien,
                      MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      SoBA = s.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                      HoTen = s.YeuCauTiepNhan.HoTen,
                      NamSinh = s.YeuCauTiepNhan.NamSinh,
                      DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                      TenBacSiChiDinh = s.NhanVienChiDinh.User.HoTen,
                      KTV = s.NhanVienThucHien.User.HoTen,
                      ChiTietKetQuaObj = string.IsNullOrEmpty(s.DataKetQuaCanLamSang) ? new ChiTietKetLuanCDHATDCNJSON() : JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(s.DataKetQuaCanLamSang),
                      TenDichVu = s.TenDichVu,
                      SoLan = s.SoLan,
                      Nhom = s.NhomDichVuBenhVien.Ten,
                      NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                  });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : allBenhNhans.CountAsync();
            var queryTask = allBenhNhans.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        
        #region BÁC SĨ CLS

        public ICollection<BaoCaoThucHienCLSVo> DataBaoCaoThucHienCLSVo(BaoCaoThucHienCLSVo queryInfo)
        {
            var tuNgay = new DateTime(1990, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                queryInfo.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryInfo.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryInfo.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            }
            //var phongBenhVien = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId());
            var bacSis = BaseRepository.TableNoTracking
                  .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                  && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                     && z.NhanVienKetLuanId != null
                     && (queryInfo.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfo.BacSiKetLuanId)
                    //&& z.ThoiDiemThucHien != null
                    //&& z.ThoiDiemThucHien >= fromDate
                    //&& z.ThoiDiemThucHien <= toDate

                    && (
                        (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= tuNgay && z.ThoiDiemThucHien < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= tuNgay && z.ThoiDiemKetLuan < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= tuNgay && z.ThoiDiemHoanThanh < denNgay)
                       )
                     && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId
                     )
                  .Select(s => new BaoCaoThucHienCLSVo
                  {
                      Id = s.Id,
                      BacSiKetLuanId = s.NhanVienKetLuanId,
                      TenBacSiKetLuan = s.NhanVienKetLuan.User.HoTen,
                      KhoaId = s.NoiChiDinh.KhoaPhongId,
                      SoBacSiKetLuan = 1
                  }).GroupBy(x => new { x.BacSiKetLuanId, x.TenBacSiKetLuan })
                  .Select(item => new BaoCaoThucHienCLSVo()
                  {
                      BacSiKetLuanId = item.First().BacSiKetLuanId,
                      TenBacSiKetLuan = item.First().TenBacSiKetLuan,
                      KhoaId = item.First().KhoaId,
                      SoBacSiKetLuan = item.Sum(x => x.SoBacSiKetLuan)
                  }).OrderBy(x => x.TenBacSiKetLuan).ToList();
            foreach (var bacSi in bacSis)
            {
                var benhNhans = BaseRepository.TableNoTracking
                  .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                  && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                      || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                       //&& z.ThoiDiemThucHien != null
                       && (
                           (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= tuNgay && z.ThoiDiemThucHien < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= tuNgay && z.ThoiDiemKetLuan < denNgay)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= tuNgay && z.ThoiDiemHoanThanh < denNgay)
                           )
                   && z.NhanVienKetLuanId == bacSi.BacSiKetLuanId
                   && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId
                   )
                  .Select(s => new BaoCaoThucHienCLSChiTietVo
                  {
                      Id = s.Id,
                      NgayThucHien = s.ThoiDiemThucHien,
                      MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                      SoBA = s.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                      HoTen = s.YeuCauTiepNhan.HoTen,
                      NamSinh = s.YeuCauTiepNhan.NamSinh,
                      DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                      TenBacSiChiDinh = s.NhanVienChiDinh.User.HoTen,
                      KTV = s.NhanVienThucHien.User.HoTen,
                      //KetLuan = string.Join("; ", s.KetQuaChuanDoanHinhAnhs.SelectMany(z => z.KetLuan)),
                      //KetLuan = s.DichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua != null ? MaskHelper.RemoveHtmlFromString(s.DichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua.KetLuan) : "",
                      ChiTietKetQuaObj = string.IsNullOrEmpty(s.DataKetQuaCanLamSang) ? new ChiTietKetLuanCDHATDCNJSON() : JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(s.DataKetQuaCanLamSang),
                      TenDichVu = s.TenDichVu,
                      SoLan = s.SoLan,
                      Nhom = s.NhomDichVuBenhVien.Ten,
                      NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                  }).ToList();
                bacSi.BaoCaoThucHienCLSChiTietVos.AddRange(benhNhans);
            }
            return bacSis.ToArray();
        }
        public virtual byte[] ExportBaoCaoBangKeBacSiCLS(BaoCaoThucHienCLSVo queryInfo)
        {
            ICollection<BaoCaoThucHienCLSVo> dataBaoCaoThucHienCLSVos = DataBaoCaoThucHienCLSVo(queryInfo);
            var tuNgay = new DateTime(1990, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                queryInfo.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryInfo.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryInfo.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            }
            var dateNow = DateTime.Now.ApplyFormatNgayThangNam();
            var tenKhoa = string.Empty;
            if (queryInfo.KhoaId == null)
            {
                var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId, s => s.Include(z => z.KhoaPhong));
                tenKhoa = phongBenhVien?.KhoaPhong.Ten;
            }
            else
            {
                tenKhoa = _khoaPhongRepository.TableNoTracking.Where(z => z.Id == queryInfo.KhoaId).Select(z => z.Ten).FirstOrDefault();
            }
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[CĐHA -TDCN] Bảng Kê Bác Sĩ Thực Hiện Cận Lâm Sàng");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 50;

                    worksheet.DefaultColWidth = 7;


                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A1:B1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:B1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:B2"])
                    {
                        range.Worksheet.Cells["A2:B2"].Merge = true;
                        range.Worksheet.Cells["A2:B2"].Value = tenKhoa;
                        range.Worksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:B2"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A2:B2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:B2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:J3"])
                    {
                        range.Worksheet.Cells["A3:J3"].Merge = true;
                        range.Worksheet.Cells["A3:J3"].Value = "BẢNG KÊ BÁC SĨ THỰC HIỆN CẬN LÂM SÀNG";
                        range.Worksheet.Cells["A3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:J3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:J3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:J3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:J4"])
                    {
                        range.Worksheet.Cells["A4:J4"].Merge = true;
                        range.Worksheet.Cells["A4:J4"].Value = "Thời gian từ: " + tuNgay.ApplyFormatDateTime()
                                                          + " -  " + denNgay.ApplyFormatDateTime();
                        range.Worksheet.Cells["A4:J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:J4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:J4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:J4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:J4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6"])
                    {
                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";
                        range.Worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["B6"])
                    {
                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Ngày thực hiện";
                        range.Worksheet.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["C6"])
                    {
                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Mã TN";
                        range.Worksheet.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["D6"])
                    {
                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6"].Value = "Số BA";
                        range.Worksheet.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["E6"])
                    {
                        range.Worksheet.Cells["E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6"].Value = "Họ và Tên";
                        range.Worksheet.Cells["E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["E6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["F6"])
                    {
                        range.Worksheet.Cells["F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6"].Value = "Năm Sinh";
                        range.Worksheet.Cells["F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["F6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["G6"])
                    {
                        range.Worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6"].Value = "Địa Chỉ";
                        range.Worksheet.Cells["G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["H6"])
                    {
                        range.Worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6"].Value = "Bs.Chỉ Định";
                        range.Worksheet.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["H6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["I6"])
                    {
                        range.Worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6"].Value = "KTV I";
                        range.Worksheet.Cells["I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["I6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["I6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["I6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["J6"])
                    {
                        range.Worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6"].Value = "Kết Luận";
                        range.Worksheet.Cells["J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["J6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["J6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["J6"].Style.Font.Bold = true;
                    }

                    var index = 7; // bắt đầu đổ data từ dòng 7
                    var STT = 1;
                    foreach (var data in dataBaoCaoThucHienCLSVos)
                    {
                        using (var range = worksheet.Cells["A" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.Bold = true;
                        }
                        worksheet.Row(index).Height = 20.5;
                        using (var range = worksheet.Cells["A" + index + ":I" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":I" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":I" + index].Value = data.TenBacSiKetLuan;
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        using (var range = worksheet.Cells["J" + index])
                        {
                            range.Worksheet.Cells["J" + index].Value = data.SoBacSiKetLuan;
                            range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }
                        index++;
                        var nhomDichVus = data.BaoCaoThucHienCLSChiTietVos.Select(z => new { z.NhomDichVuBenhVienId, z.Nhom }).OrderBy(z => z.Nhom).Distinct().ToList();
                        foreach (var nhomDichVu in nhomDichVus)
                        {
                            var tenNhom = data.BaoCaoThucHienCLSChiTietVos.Where(z => z.NhomDichVuBenhVienId == nhomDichVu.NhomDichVuBenhVienId).Select(z => z.Nhom).FirstOrDefault();
                            var soLuongDichVu = data.BaoCaoThucHienCLSChiTietVos.Where(z => z.NhomDichVuBenhVienId == nhomDichVu.NhomDichVuBenhVienId).Sum(z => z.SoLan);
                            using (var range = worksheet.Cells["B" + index + ":I" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":I" + index].Merge = true;
                                range.Worksheet.Cells["B" + index + ":I" + index].Value = tenNhom;
                                range.Worksheet.Cells["B" + index + ":I" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }

                            using (var range = worksheet.Cells["J" + index])
                            {
                                range.Worksheet.Cells["J" + index].Value = soLuongDichVu;
                                range.Worksheet.Cells["J" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            index++;
                            foreach (var chitiet in data.BaoCaoThucHienCLSChiTietVos.Where(z => z.NhomDichVuBenhVienId == nhomDichVu.NhomDichVuBenhVienId))
                            {
                                using (var range = worksheet.Cells["A" + index + ":J" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":J" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["A" + index + ":J" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }


                                using (var range = worksheet.Cells["A" + index])
                                {
                                    range.Worksheet.Cells["A" + index].Value = STT;
                                    range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["B" + index])
                                {
                                    range.Worksheet.Cells["B" + index].Value = chitiet.NgayThucHienDisplay;
                                    range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["C" + index])
                                {
                                    range.Worksheet.Cells["C" + index].Value = chitiet.MaTN;
                                    range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["D" + index])
                                {
                                    range.Worksheet.Cells["D" + index].Value = chitiet.SoBA;
                                    range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["E" + index])
                                {
                                    range.Worksheet.Cells["E" + index].Value = chitiet.HoTen;
                                    range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["E" + index].Style.WrapText = true;

                                }

                                using (var range = worksheet.Cells["F" + index])
                                {
                                    range.Worksheet.Cells["F" + index].Value = chitiet.NamSinh;
                                    range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["G" + index])
                                {
                                    range.Worksheet.Cells["G" + index].Value = chitiet.DiaChi;
                                    range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["G" + index].Style.WrapText = true;
                                }

                                using (var range = worksheet.Cells["H" + index])
                                {
                                    range.Worksheet.Cells["H" + index].Value = chitiet.TenBacSiChiDinh;
                                    range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["H" + index].Style.WrapText = true;
                                }


                                using (var range = worksheet.Cells["I" + index])
                                {
                                    range.Worksheet.Cells["I" + index].Value = chitiet.KTV;
                                    range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["I" + index].Style.WrapText = true;
                                }

                                using (var range = worksheet.Cells["J" + index])
                                {
                                    range.Worksheet.Cells["J" + index].Value = chitiet.KetLuan;
                                    range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["J" + index].Style.WrapText = true;
                                }
                                STT++;
                                index++;
                            }
                        }

                    }

                    xlPackage.Save();
                }

                return stream.ToArray();

            }
        }

        #endregion
        
        #region HOẠT ĐỘNG CẬN LÂM SÀNG

        public async Task<GridDataSource> GetDataForGridAsyncHoatDongCLS(BaoCaoHoatDongCLSVo queryInfo, bool exportExcel = false)
        {
            var fromDate = new DateTime(1990, 1, 1);
            DateTime toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                queryInfo.FromDate.TryParseExactCustom(out fromDate);
                if (string.IsNullOrEmpty(queryInfo.ToDate))
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    queryInfo.ToDate.TryParseExactCustom(out toDate);
                }
                toDate = toDate.Date.AddDays(1).AddMilliseconds(-1);
            }
            var baoCaoHoatDongCLSChiTietVoKhongGoi = BaseRepository.TableNoTracking
                .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                   && z.NhanVienKetLuanId != null
                   && (queryInfo.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfo.BacSiKetLuanId)
                   && z.YeuCauGoiDichVuId == null
                   && (
                            (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= fromDate && z.ThoiDiemThucHien < toDate)
                            || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= fromDate && z.ThoiDiemKetLuan < toDate)
                            || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= fromDate && z.ThoiDiemHoanThanh < toDate)

                    )
                   && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId

                   )
                .Select(s => new BaoCaoHoatDongCLSChiTietVo
                {
                    Id = s.Id,
                    Ten = s.TenDichVu,
                    SoLan = s.SoLan,
                    DonGiaNiemYet = s.Gia,
                    DonGiaSauChietKhau = s.DonGiaSauChietKhau,
                    SoTienMienGiam = s.SoTienMienGiam,
                    YeuCauGoiDichVuId = null,
                    KhoaId = s.NoiChiDinh.KhoaPhongId,
                    NgayThucHien = s.ThoiDiemThucHien,
                    Nhom = s.NhomDichVuBenhVien.Ten,
                }).ToList();

            var baoCaoHoatDongCLSChiTietVoCoGoi = BaseRepository.TableNoTracking
               .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
               && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                   || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                   || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                  && z.NhanVienKetLuanId != null
                  && (queryInfo.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfo.BacSiKetLuanId)
                  && z.YeuCauGoiDichVuId != null
                  && ((z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= fromDate && z.ThoiDiemThucHien < toDate)
               || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= fromDate && z.ThoiDiemKetLuan < toDate)
               || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= fromDate && z.ThoiDiemHoanThanh < toDate))
                  && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId

                  )
               .Select(s => new BaoCaoHoatDongCLSChiTietVo
               {
                   Id = s.Id,
                   Ten = s.TenDichVu,
                   SoLan = s.SoLan,
                   DonGiaNiemYet = s.Gia,
                   DonGiaSauChietKhau = s.DonGiaSauChietKhau,
                   SoTienMienGiam = s.SoTienMienGiam,
                   YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                   KhoaId = s.NoiChiDinh.KhoaPhongId,
                   NgayThucHien = s.ThoiDiemThucHien,
                   Nhom = s.NhomDichVuBenhVien.Ten,
               }).ToList();
            var dataAll = baoCaoHoatDongCLSChiTietVoKhongGoi.Concat(baoCaoHoatDongCLSChiTietVoCoGoi);
            var dataGroup = dataAll.GroupBy(x => new { x.Ten }, x => x, (k, v) => new BaoCaoHoatDongCLSChiTietVo
            {
                Ten = v.First().Ten,
                DonGiaNiemYet = v.First().DonGiaNiemYet,
                TongThanhTienNiemYet = v.Sum(z => z.ThanhTienNiemYet),
                TongThanhTienThucThu = v.Sum(z => z.ThanhTienThucThu),
                SoLan = v.Sum(x => x.SoLan),
                Nhom = v.First().Nhom,
            }).OrderBy(x => x.Ten).Distinct().ToList();
            return new GridDataSource { Data = dataGroup.OrderBy(s => s.Nhom).ToArray(), TotalRowCount = dataGroup.Count() };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncHoatDongCLS(BaoCaoHoatDongCLSVo queryInfo)
        {
            return null;
        }
        private ICollection<BaoCaoHoatDongCLSVo> DataHoatDongCLS(BaoCaoHoatDongCLSVo queryInfo)
        {
            var fromDate = new DateTime(1990, 1, 1);
            DateTime toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                queryInfo.FromDate.TryParseExactCustom(out fromDate);
                if (string.IsNullOrEmpty(queryInfo.ToDate))
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    queryInfo.ToDate.TryParseExactCustom(out toDate);
                }
                toDate = toDate.Date.AddDays(1).AddMilliseconds(-1);
            }
            var nhomDichVus = BaseRepository.TableNoTracking
                .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                   && z.NhanVienKetLuanId != null
                  && (queryInfo.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfo.BacSiKetLuanId)
                     && (

                         (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= fromDate && z.ThoiDiemThucHien < toDate)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= fromDate && z.ThoiDiemKetLuan < toDate)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= fromDate && z.ThoiDiemHoanThanh < toDate)
                    )
                   && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId
                   )
                .Select(s => new BaoCaoHoatDongCLSVo
                {
                    Id = s.Id,
                    Nhom = s.NhomDichVuBenhVien.Ten,
                    NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                    KhoaId = s.NoiThucHien.KhoaPhongId,
                    SoLan = s.SoLan
                }).GroupBy(x => new { x.NhomDichVuBenhVienId })
                .Select(item => new BaoCaoHoatDongCLSVo()
                {
                    Nhom = item.First().Nhom,
                    NhomDichVuBenhVienId = item.First().NhomDichVuBenhVienId,
                    KhoaId = item.First().KhoaId,
                    SoLan = item.Sum(x => x.SoLan)
                }).OrderBy(x => x.Nhom).Distinct().ToList();
            foreach (var dichVu in nhomDichVus)
            {

                var baoCaoHoatDongCLSChiTietVoKhongGoi = BaseRepository.TableNoTracking
                 .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                 && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                     || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                     || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                    && z.NhanVienKetLuanId != null
                    && (queryInfo.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfo.BacSiKetLuanId)
                    && z.YeuCauGoiDichVuId == null
                    && dichVu.NhomDichVuBenhVienId == z.NhomDichVuBenhVienId
                     && (
                         (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= fromDate && z.ThoiDiemThucHien < toDate)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= fromDate && z.ThoiDiemKetLuan < toDate)
                        || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= fromDate && z.ThoiDiemHoanThanh < toDate)
                    )
                    && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId

                    )
                 .Select(s => new BaoCaoHoatDongCLSChiTietVo
                 {
                     Id = s.Id,
                     Ten = s.TenDichVu,
                     SoLan = s.SoLan,
                     DonGiaNiemYet = s.Gia,
                     DonGiaSauChietKhau = s.DonGiaSauChietKhau,
                     SoTienMienGiam = s.SoTienMienGiam,
                     YeuCauGoiDichVuId = null,
                     KhoaId = s.NoiChiDinh.KhoaPhongId,
                     NgayThucHien = s.ThoiDiemThucHien,
                     Nhom = s.NhomDichVuBenhVien.Ten,
                 }).ToList();

                var baoCaoHoatDongCLSChiTietVoCoGoi = BaseRepository.TableNoTracking
                   .Where(z => z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                   && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                       || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                       || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                      && z.NhanVienKetLuanId != null
                      && (queryInfo.BacSiKetLuanId == null || z.NhanVienKetLuanId == queryInfo.BacSiKetLuanId)
                      && dichVu.NhomDichVuBenhVienId == z.NhomDichVuBenhVienId
                      && z.YeuCauGoiDichVuId != null
                       && (
                       (z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= fromDate && z.ThoiDiemThucHien < toDate)
                    || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= fromDate && z.ThoiDiemKetLuan < toDate)
                    || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= fromDate && z.ThoiDiemHoanThanh < toDate)

                    )
                      && z.NoiThucHien.KhoaPhongId == queryInfo.KhoaId

                      )
                   .Select(s => new BaoCaoHoatDongCLSChiTietVo
                   {
                       Id = s.Id,
                       Ten = s.TenDichVu,
                       SoLan = s.SoLan,
                       DonGiaNiemYet = s.Gia,
                       DonGiaSauChietKhau = s.DonGiaSauChietKhau,
                       SoTienMienGiam = s.SoTienMienGiam,
                       YeuCauGoiDichVuId = s.YeuCauGoiDichVuId,
                       KhoaId = s.NoiThucHien.KhoaPhongId,
                       NgayThucHien = s.ThoiDiemThucHien,
                       Nhom = s.NhomDichVuBenhVien.Ten,
                   }).ToList();
                var dataAll = baoCaoHoatDongCLSChiTietVoKhongGoi.Concat(baoCaoHoatDongCLSChiTietVoCoGoi);
                var dataGroup = dataAll.GroupBy(x => new { x.Ten }, x => x, (k, v) => new BaoCaoHoatDongCLSChiTietVo
                {
                    Ten = v.First().Ten,
                    DonGiaNiemYet = v.First().DonGiaNiemYet,
                    TongThanhTienNiemYet = v.Sum(z => z.ThanhTienNiemYet),
                    TongThanhTienThucThu = v.Sum(z => z.ThanhTienThucThu),
                    SoLan = v.Sum(x => x.SoLan),
                    Nhom = v.First().Nhom,
                }).OrderBy(x => x.Ten).Distinct().ToList();
                dichVu.BaoCaoHoatDongCLSChiTietVos.AddRange(dataGroup);
            }
            return nhomDichVus.ToArray();
        }
        public virtual byte[] ExportBaoCaoHoatDongClsTheoKhoa(BaoCaoHoatDongCLSVo queryInfo)
        {
            ICollection<BaoCaoHoatDongCLSVo> dataBaoCaoHoatDongCLSVo = DataHoatDongCLS(queryInfo);
            var fromDate = new DateTime(1990, 1, 1);
            DateTime toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                queryInfo.FromDate.TryParseExactCustom(out fromDate);
                if (string.IsNullOrEmpty(queryInfo.ToDate))
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    queryInfo.ToDate.TryParseExactCustom(out toDate);
                }
                toDate = toDate.Date.AddDays(1).AddMilliseconds(-1);
            }
            var dateNow = DateTime.Now.ApplyFormatNgayThangNam();
            var tenKhoa = string.Empty;
            if (queryInfo.KhoaId == null)
            {
                var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
                var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId, s => s.Include(z => z.KhoaPhong));
                tenKhoa = phongBenhVien?.KhoaPhong.Ten;
            }
            else
            {
                tenKhoa = _khoaPhongRepository.TableNoTracking.Where(z => z.Id == queryInfo.KhoaId).Select(z => z.Ten).FirstOrDefault();
            }
            var dataChiTiets = dataBaoCaoHoatDongCLSVo.SelectMany(z => z.BaoCaoHoatDongCLSChiTietVos);
            var thanhTienChiTietNiemYet = dataChiTiets.Sum(z => z.TongThanhTienNiemYet) ?? 0;
            var thanhTienChiTietThucThu = dataChiTiets.Sum(z => z.TongThanhTienThucThu) ?? 0;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[CĐHA - TDCN] HOẠT ĐỘNG CẬN LÂM SÀNG THEO KHOA");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 60;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.DefaultColWidth = 7;


                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A1:B1"])
                    {
                        range.Worksheet.Cells["A1:B1"].Merge = true;
                        range.Worksheet.Cells["A1:B1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:B1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:B1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:B2"])
                    {
                        range.Worksheet.Cells["A2:B2"].Merge = true;
                        range.Worksheet.Cells["A2:B2"].Value = tenKhoa;
                        range.Worksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:B2"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A2:B2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:B2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:F3"])
                    {
                        range.Worksheet.Cells["A3:F3"].Merge = true;
                        range.Worksheet.Cells["A3:F3"].Value = "HOẠT ĐỘNG CẬN LÂM SÀNG THEO KHOA";
                        range.Worksheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:F3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:F3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:F3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:F4"])
                    {
                        range.Worksheet.Cells["A4:F4"].Merge = true;
                        range.Worksheet.Cells["A4:F4"].Value = "Thời gian từ: " + fromDate.ApplyFormatDateTime()
                                                          + " -  " + toDate.ApplyFormatDateTime();
                        range.Worksheet.Cells["A4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:F4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:F4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:F4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:F4"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A4:F4"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6"])
                    {
                        range.Worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Value = "STT";
                        range.Worksheet.Cells["A6:A7"].Merge = true;
                        range.Worksheet.Cells["A6:A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["B6"])
                    {
                        range.Worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Merge = true;
                        range.Worksheet.Cells["B6:B7"].Merge = true;
                        range.Worksheet.Cells["B6:B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6"].Value = "Các dịch vụ";
                        range.Worksheet.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["C6"])
                    {
                        range.Worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Merge = true;
                        range.Worksheet.Cells["C6:C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6"].Value = "Số lượng";
                        range.Worksheet.Cells["C6:C7"].Merge = true;
                        range.Worksheet.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C6"].Style.Font.Bold = true;
                    }
                    //using (var range = worksheet.Cells["D6:E6"])
                    //{
                    //    range.Worksheet.Cells["D6:E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    //    range.Worksheet.Cells["D6:E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells["D6:E6"].Merge = true;
                    //    range.Worksheet.Cells["D6:E6"].Value = "Đơn giá";
                    //    range.Worksheet.Cells["D6:E6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["D6:E6"].Style.Font.Bold = true;

                    //    range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    //    range.Worksheet.Cells["D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells["D7"].Value = "Niêm yết";
                    //    range.Worksheet.Cells["D7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["D7"].Style.Font.Bold = true;

                    //    range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    //    range.Worksheet.Cells["E7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells["E7"].Value = "Thực thu";
                    //    range.Worksheet.Cells["E7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["E7"].Style.Font.Bold = true;
                    //}

                    using (var range = worksheet.Cells["D6"])
                    {
                        range.Worksheet.Cells["D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6:D7"].Merge = true;
                        range.Worksheet.Cells["D6"].Value = "Đơn giá niêm yết";
                        range.Worksheet.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["E6:F6"])
                    {
                        range.Worksheet.Cells["E6:F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E6:F6"].Merge = true;
                        range.Worksheet.Cells["E6:F6"].Value = "Thành tiền";
                        range.Worksheet.Cells["E6:F6"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E6:F6"].Style.Font.Bold = true;

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E7"].Value = "Niêm yết";
                        range.Worksheet.Cells["E7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E7"].Style.Font.Bold = true;

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F7"].Value = "Thực thu";
                        range.Worksheet.Cells["F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F7"].Style.Font.Bold = true;
                    }
                    var index = 8; // bắt đầu đổ data từ dòng 
                    var STT = 1;
                    foreach (var data in dataBaoCaoHoatDongCLSVo)
                    {
                        using (var range = worksheet.Cells["A" + index + ":F" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Bold = true;
                        }
                        worksheet.Row(index).Height = 20.5;
                        using (var range = worksheet.Cells["A" + index + ":B" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = data.Nhom;
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        using (var range = worksheet.Cells["C" + index])
                        {
                            range.Worksheet.Cells["C" + index].Value = data.SoLan;
                            range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }
                        index++;
                        foreach (var chitiet in data.BaoCaoHoatDongCLSChiTietVos)
                        {
                            using (var range = worksheet.Cells["A" + index + ":F" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }

                            using (var range = worksheet.Cells["A" + index])
                            {
                                range.Worksheet.Cells["A" + index].Value = STT;
                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["B" + index])
                            {
                                range.Worksheet.Cells["B" + index].Value = chitiet.Ten;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["C" + index])
                            {
                                range.Worksheet.Cells["C" + index].Value = chitiet.SoLan;
                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["D" + index])
                            {
                                range.Worksheet.Cells["D" + index].Value = chitiet.DonGiaNiemYet;
                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
                            }

                            //using (var range = worksheet.Cells["E" + index])
                            //{
                            //    range.Worksheet.Cells["E" + index].Value = "";
                            //    range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //    range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //    range.Worksheet.Cells["E" + index].Style.WrapText = true;
                            //    range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            //}

                            using (var range = worksheet.Cells["E" + index])
                            {
                                range.Worksheet.Cells["E" + index].Value = chitiet.TongThanhTienNiemYet;
                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            }

                            using (var range = worksheet.Cells["F" + index])
                            {
                                range.Worksheet.Cells["F" + index].Value = chitiet.TongThanhTienThucThu;
                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                            }
                            STT++;
                            index++;
                        }
                    }
                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":B" + index].Value = "TỔNG CỘNG";
                        range.Worksheet.Cells["A" + index + ":B" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["C" + index + ":D" + index].Merge = true;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C" + index + ":D" + index].Value = dataBaoCaoHoatDongCLSVo.Sum(z => z.SoLan);
                        range.Worksheet.Cells["C" + index + ":D" + index].Style.Font.Bold = true;


                        range.Worksheet.Cells["E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["E" + index].Value = thanhTienChiTietNiemYet;
                        range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                        range.Worksheet.Cells["E" + index].Style.Font.Bold = true;


                        range.Worksheet.Cells["F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["F" + index].Value = thanhTienChiTietThucThu;
                        range.Worksheet.Cells["F" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
                    }

                    index += 2;
                    using (var range = worksheet.Cells["G" + index + ":I" + index])
                    {
                        range.Worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Value = "Trưởng khoa CĐHA ‑ TDCN";
                        range.Worksheet.Cells["B" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["D" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["D" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + index + ":F" + index].Value = "Hà Nội, " + dateNow;
                        index++;
                        range.Worksheet.Cells["E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E" + index].Value = "Người lập";
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();

            }
        }

        #endregion

        #region SỔ THỐNG KÊ
        public async Task<GridDataSource> GetDataSoThongKeCLSForGridAsync(BaoCaoSoThongKeCLSChiTietVo queryInfo, bool exportExcel = false)
        {

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
                queryInfo.Sort.Add(new Sort
                {
                    Field = "Nhom",
                    Dir = "asc"
                });
            }
            var queryString = JsonConvert.DeserializeObject<BaoCaoSoThongKeCLSChiTietVo>(queryInfo.AdditionalSearchString);
            var tuNgay = new DateTime(1990, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            {
                queryString.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryString.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryString.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.Date.AddDays(1).AddMilliseconds(-1);
            }
            if (queryString.ChuaThucHien == false && queryString.DaThucHien == false)
            {
                queryString.ChuaThucHien = true;
                queryString.DaThucHien = true;
            }

            var queryData = BaseRepository.TableNoTracking
                .Where(z => (
                (queryString.ChuaThucHien == true && queryString.DaThucHien == false && z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                || (queryString.ChuaThucHien == false && queryString.DaThucHien == true && z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                || (queryString.ChuaThucHien == true && queryString.DaThucHien == true && (z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
                )
                //&& ((!laNoiTru && z.NoiTruPhieuDieuTri == null) || (laNoiTru && z.NoiTruPhieuDieuTri != null))
                && z.NoiThucHien.KhoaPhongId == queryString.KhoaId
                && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                    && ((z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= tuNgay && z.ThoiDiemThucHien < denNgay)
                    || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= tuNgay && z.ThoiDiemKetLuan < denNgay)
                    || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= tuNgay && z.ThoiDiemHoanThanh < denNgay))
                   )
                .Select(s => new BaoCaoSoThongKeCLSChiTietVo
                {
                    Id = s.Id,
                    Nhom = s.NhomDichVuBenhVien.Ten,
                    NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBA = s.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                    BHYTMaSoThe = s.YeuCauTiepNhan.BHYTMaSoThe,
                    TenDichVu = s.TenDichVu,
                    YeuCauKhamBenhId = s.YeuCauKhamBenhId,
                    NoiTruPhieuDieuTriId = s.NoiTruPhieuDieuTriId,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    LoaiYeuCauTiepNhan = s.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    //TenKhoa = tenKhoa,
                    DataKetQuaCanLamSang = s.DataKetQuaCanLamSang,
                    //ChiTietKetQuaObj = string.IsNullOrEmpty(s.DataKetQuaCanLamSang) ? new ChiTietKetLuanCDHATDCNJSON() : JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(s.DataKetQuaCanLamSang),
                    SoLan = s.SoLan,
                    Gia = s.Gia,
                    //TenNguoiKetLuan = s.NhanVienKetLuan.User.HoTen,
                    //KTV = s.NhanVienThucHien.User.HoTen,
                    NhanVienChiDinhId = s.NhanVienChiDinhId,
                    NhanVienThucHienId = s.NhanVienThucHienId,
                    NhanVienKetLuanId = s.NhanVienKetLuanId,
                    ThoiDiemDangKy = s.ThoiDiemChiDinh,
                    ThoiDiemThucHien = s.ThoiDiemThucHien,
                    TrangThai = s.TrangThai,
                    GhiChu = "",
                    ChanDoan = (s.NoiTruPhieuDieuTriId == null && s.YeuCauKhamBenhId != null) ? s.YeuCauKhamBenh.ChanDoanSoBoGhiChu : "",
                    TenNoiChiDinh = (s.NoiTruPhieuDieuTriId == null && s.NoiChiDinhId != null) ? s.NoiChiDinh.Ten : "",
                    //TenNguoiChiDinh = s.NhanVienChiDinh.User.HoTen,
                    //ThoiDiemNhapVien = !laNoiTru ? (DateTime?)null : s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                    //ThoiDiemRaVien = !laNoiTru ? (DateTime?)null : s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien,
                }).ToList();
            var thongTinNhanViens = _userRepository.TableNoTracking
                .Select(o => new
                {
                    o.Id,
                    o.HoTen
                }).ToList();

            var yeuCauTiepNhanNoiTruIds = queryData.Where(o => o.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.YeuCauTiepNhanId).Distinct().ToList();
            var thongTinBenhAns = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNoiTruIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.SoBenhAn,
                    o.ThoiDiemNhapVien,
                    o.ThoiDiemRaVien
                }).ToList();

            var phieuDieuTriIds = queryData.Where(o => o.NoiTruPhieuDieuTriId != null).Select(o => o.NoiTruPhieuDieuTriId.Value).Distinct().ToList();
            var thongTinPhieuDieuTris = _noiTruPhieuDieuTriRepository.TableNoTracking
                .Where(o => phieuDieuTriIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.ChanDoanChinhGhiChu,
                    KhoaPhongDieuTri = o.KhoaPhongDieuTri.Ten,
                }).ToList();
            var tenKhoa = string.Empty;
            if (exportExcel)
            {
                var phongBenhVien = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId(), s => s.Include(z => z.KhoaPhong));
                tenKhoa = queryString.KhoaId != null ? _khoaPhongRepository.TableNoTracking.Where(z => z.Id == queryString.KhoaId).Select(z => z.Ten).FirstOrDefault() : _khoaPhongRepository.TableNoTracking.Where(z => z.Id == phongBenhVien.KhoaPhongId).Select(z => z.Ten).FirstOrDefault();
            }

            foreach (var item in queryData)
            {
                item.TenKhoa = tenKhoa;
                item.ChiTietKetQuaObj = string.IsNullOrEmpty(item.DataKetQuaCanLamSang) ? new ChiTietKetLuanCDHATDCNJSON() : JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(item.DataKetQuaCanLamSang);
                if(item.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                {
                    var thongTinBenhAn = thongTinBenhAns.FirstOrDefault(o => o.Id == item.YeuCauTiepNhanId);
                    if(thongTinBenhAn != null)
                    {
                        item.SoBA = thongTinBenhAn.SoBenhAn;
                        item.ThoiDiemNhapVien = thongTinBenhAn.ThoiDiemNhapVien;
                        item.ThoiDiemRaVien = thongTinBenhAn.ThoiDiemRaVien;
                    }
                }
                if(item.NoiTruPhieuDieuTriId != null)
                {
                    var phieuDieuTri = thongTinPhieuDieuTris.FirstOrDefault(o => o.Id == item.NoiTruPhieuDieuTriId);
                    if (phieuDieuTri != null)
                    {
                        item.ChanDoan = phieuDieuTri.ChanDoanChinhGhiChu;
                        item.TenNoiChiDinh = phieuDieuTri.KhoaPhongDieuTri;
                    }
                }
                if(item.NhanVienChiDinhId != null)
                {
                    item.TenNguoiChiDinh = thongTinNhanViens.FirstOrDefault(o => o.Id == item.NhanVienChiDinhId)?.HoTen;
                }
                if (item.NhanVienThucHienId != null)
                {
                    item.KTV = thongTinNhanViens.FirstOrDefault(o => o.Id == item.NhanVienThucHienId)?.HoTen;
                }
                if (item.NhanVienKetLuanId != null)
                {
                    item.TenNguoiKetLuan = thongTinNhanViens.FirstOrDefault(o => o.Id == item.NhanVienKetLuanId)?.HoTen;
                }
            }

            //var benhNhanNgoaiTrus = GetDataSoThongKeCLSQuery(queryString, false, queryString.ChuaThucHien, queryString.DaThucHien, tuNgay, denNgay, exportExcel);
            //var benhNhanNoiTrus = GetDataSoThongKeCLSQuery(queryString, true, queryString.ChuaThucHien, queryString.DaThucHien, tuNgay, denNgay, exportExcel);
            //var result = benhNhanNgoaiTrus.ToList().Concat(benhNhanNoiTrus.ToList());
            //var query = result.AsQueryable();

            //var countTask = result.Count();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = queryData.Count() };
        }
        public async Task<GridDataSource> GetDataSoThongKeCLSForGridAsyncOld(BaoCaoSoThongKeCLSChiTietVo queryInfo, bool exportExcel = false)
        {

            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
                queryInfo.Sort.Add(new Sort
                {
                    Field = "Nhom",
                    Dir = "asc"
                });
            }
            var queryString = JsonConvert.DeserializeObject<BaoCaoSoThongKeCLSChiTietVo>(queryInfo.AdditionalSearchString);
            var tuNgay = new DateTime(1990, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            {
                queryString.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryString.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryString.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.Date.AddDays(1).AddMilliseconds(-1);
            }
            if (queryString.ChuaThucHien == false && queryString.DaThucHien == false)
            {
                queryString.ChuaThucHien = true;
                queryString.DaThucHien = true;
            }
            var benhNhanNgoaiTrus = GetDataSoThongKeCLSQuery(queryString, false, queryString.ChuaThucHien, queryString.DaThucHien, tuNgay, denNgay, exportExcel);
            var benhNhanNoiTrus = GetDataSoThongKeCLSQuery(queryString, true, queryString.ChuaThucHien, queryString.DaThucHien, tuNgay, denNgay, exportExcel);
            var result = benhNhanNgoaiTrus.ToList().Concat(benhNhanNoiTrus.ToList());
            var query = result.AsQueryable();

            var countTask = result.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageSoThongKeCLSForGridAsyn(BaoCaoSoThongKeCLSChiTietVo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryString = JsonConvert.DeserializeObject<BaoCaoSoThongKeCLSChiTietVo>(queryInfo.AdditionalSearchString);
            var tuNgay = new DateTime(1990, 1, 1);
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            {
                queryString.FromDate.TryParseExactCustom(out tuNgay);
                if (string.IsNullOrEmpty(queryString.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryString.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.Date.AddDays(1).AddMilliseconds(-1);
            }
            if (queryString.ChuaThucHien == false && queryString.DaThucHien == false)
            {
                queryString.ChuaThucHien = true;
                queryString.DaThucHien = true;
            }
            var benhNhanNgoaiTrus = GetDataSoThongKeCLSQuery(queryString, false, queryString.ChuaThucHien, queryString.DaThucHien, tuNgay, denNgay);
            var benhNhanNoiTrus = GetDataSoThongKeCLSQuery(queryString, true, queryString.ChuaThucHien, queryString.DaThucHien, tuNgay, denNgay);
            var result = benhNhanNgoaiTrus.ToList().Concat(benhNhanNoiTrus.ToList());
            var query = result.AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }
        private IQueryable<BaoCaoSoThongKeCLSChiTietVo> GetDataSoThongKeCLSQuery(BaoCaoSoThongKeCLSChiTietVo queryString, bool laNoiTru, bool? chuaThucHien, bool? daThucHien, DateTime tuNgay, DateTime denNgay, bool exportExcel = false)
        {
            var tenKhoa = string.Empty;
            if (exportExcel)
            {
                var phongBenhVien = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId(), s => s.Include(z => z.KhoaPhong));
                tenKhoa = queryString.KhoaId != null ? _khoaPhongRepository.TableNoTracking.Where(z => z.Id == queryString.KhoaId).Select(z => z.Ten).FirstOrDefault() : _khoaPhongRepository.TableNoTracking.Where(z => z.Id == phongBenhVien.KhoaPhongId).Select(z => z.Ten).FirstOrDefault();
            }
            var query = BaseRepository.TableNoTracking
                .Where(z => (
                (chuaThucHien == true && daThucHien == false && z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                || (chuaThucHien == false && daThucHien == true && z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                || (chuaThucHien == true && daThucHien == true && (z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || z.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
                )
                && ((!laNoiTru && z.NoiTruPhieuDieuTri == null) || (laNoiTru && z.NoiTruPhieuDieuTri != null))
                && z.NoiThucHien.KhoaPhongId == queryString.KhoaId
                && (z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                    || z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang)
                    && ((z.ThoiDiemThucHien != null && z.ThoiDiemThucHien >= tuNgay && z.ThoiDiemThucHien < denNgay)
                    || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan != null && z.ThoiDiemKetLuan >= tuNgay && z.ThoiDiemKetLuan < denNgay)
                    || (z.ThoiDiemThucHien == null && z.ThoiDiemKetLuan == null && z.ThoiDiemHoanThanh != null && z.ThoiDiemHoanThanh >= tuNgay && z.ThoiDiemHoanThanh < denNgay))
                   )
                .Select(s => new BaoCaoSoThongKeCLSChiTietVo
                {
                    Id = s.Id,
                    Nhom = s.NhomDichVuBenhVien.Ten,
                    NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    SoBA = s.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                    BHYTMaSoThe = s.YeuCauTiepNhan.BHYTMaSoThe,
                    TenDichVu = s.TenDichVu,
                    TenKhoa = tenKhoa,
                    DataKetQuaCanLamSang = s.DataKetQuaCanLamSang,
                    //ChiTietKetQuaObj = string.IsNullOrEmpty(s.DataKetQuaCanLamSang) ? new ChiTietKetLuanCDHATDCNJSON() : JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(s.DataKetQuaCanLamSang),
                    SoLan = s.SoLan,
                    Gia = s.Gia,
                    TenNguoiKetLuan = s.NhanVienKetLuan.User.HoTen,
                    KTV = s.NhanVienThucHien.User.HoTen,
                    ThoiDiemDangKy = s.ThoiDiemChiDinh,
                    ThoiDiemThucHien = s.ThoiDiemThucHien,
                    TrangThai = s.TrangThai,
                    GhiChu = "",
                    ChanDoan = !laNoiTru ? s.YeuCauKhamBenh.ChanDoanSoBoGhiChu : s.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu,
                    TenNoiChiDinh = !laNoiTru ? (s.NoiChiDinh == null ? s.YeuCauKhamBenh.NoiChiDinh.Ten : s.NoiChiDinh.Ten) : s.NoiTruPhieuDieuTri.KhoaPhongDieuTri.Ten,
                    TenNguoiChiDinh = !laNoiTru ? (s.NhanVienChiDinh == null ? s.YeuCauKhamBenh.NhanVienChiDinh.User.HoTen : s.NhanVienChiDinh.User.HoTen) : s.NhanVienChiDinh.User.HoTen,
                    ThoiDiemNhapVien = !laNoiTru ? (DateTime?)null : s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                    ThoiDiemRaVien = !laNoiTru ? (DateTime?)null : s.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien,
                });
            return query;
        }
        public virtual byte[] ExportBaoCaoSoThongKeCls(BaoCaoSoThongKeCLSChiTietVo queryInfo)
        {
            var dataBaoCaoSoThongKeCls = GetDataSoThongKeCLSForGridAsync(queryInfo, true).Result.Data.Cast<BaoCaoSoThongKeCLSChiTietVo>().ToList();
            //var fromDate = queryInfo.RangeFromDate.startDate?.Date;
            //var toDate = queryInfo.RangeFromDate.endDate?.Date.AddDays(1).AddMilliseconds(-1);
            var queryString = JsonConvert.DeserializeObject<BaoCaoSoThongKeCLSChiTietVo>(queryInfo.AdditionalSearchString);

            var fromDate = new DateTime(1990, 1, 1);
            DateTime toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            {
                queryString.FromDate.TryParseExactCustom(out fromDate);
                if (string.IsNullOrEmpty(queryString.ToDate))
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    queryString.ToDate.TryParseExactCustom(out toDate);
                }
                toDate = toDate.Date.AddDays(1).AddMilliseconds(-1);
            }
            var dateNow = DateTime.Now.ApplyFormatNgayThangNam();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[CĐHA - TDCN] SỔ THỐNG KÊ CẬN LÂM SÀNG");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 10;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;
                    worksheet.Column(17).Width = 30;
                    worksheet.Column(18).Width = 30;
                    worksheet.Column(19).Width = 30;
                    worksheet.Column(20).Width = 30;
                    worksheet.Column(21).Width = 30;

                    worksheet.DefaultColWidth = 7;


                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A1:D1"])
                    {
                        range.Worksheet.Cells["A1:D1"].Merge = true;
                        range.Worksheet.Cells["A1:D1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:D1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:D11"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:D2"])
                    {
                        range.Worksheet.Cells["A2:D2"].Merge = true;
                        range.Worksheet.Cells["A2:D2"].Value = dataBaoCaoSoThongKeCls?.First().TenKhoa;
                        range.Worksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A2:D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:D2"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A2:D2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:D2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:U3"])
                    {
                        range.Worksheet.Cells["A3:U3"].Merge = true;
                        range.Worksheet.Cells["A3:U3"].Value = "SỔ THỐNG KÊ CẬN LÂM SÀNG";
                        range.Worksheet.Cells["A3:U3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:U3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:U3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:U3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:U3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:U4"])
                    {
                        range.Worksheet.Cells["A4:U4"].Merge = true;
                        range.Worksheet.Cells["A4:U4"].Value = dataBaoCaoSoThongKeCls?.First().TenKhoa;
                        range.Worksheet.Cells["A4:U4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:U4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:U4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:U4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:U4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:U5"])
                    {
                        range.Worksheet.Cells["A5:U5"].Merge = true;
                        range.Worksheet.Cells["A5:U5"].Value = "Thời gian từ: " + fromDate.ApplyFormatDateTime()
                                                          + " -  " + toDate.ApplyFormatDateTime();
                        range.Worksheet.Cells["A5:U5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:U5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:U5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:U5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:U5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:U5"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A6:A7"])
                    {
                        range.Worksheet.Cells["A6:A7"].Merge = true;
                        range.Worksheet.Cells["A6:A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A6:A7"].Value = "STT";
                        range.Worksheet.Cells["A6:A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:A7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:A7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:A7"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["B6:B7"])
                    {
                        range.Worksheet.Cells["B6:B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B6:B7"].Merge = true;
                        range.Worksheet.Cells["B6:B7"].Value = "Mã Y Tế";
                        range.Worksheet.Cells["B6:B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B6:B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B6:B7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B6:B7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B6:B7"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["C6:C7"])
                    {
                        range.Worksheet.Cells["C6:C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C6:C7"].Merge = true;
                        range.Worksheet.Cells["C6:C7"].Value = "Số bệnh án";
                        range.Worksheet.Cells["C6:C7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C6:C7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C6:C7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C6:C7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C6:C7"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["D6:D7"])
                    {
                        range.Worksheet.Cells["D6:D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D6:D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D6:D7"].Merge = true;
                        range.Worksheet.Cells["D6:D7"].Value = "Họ và Tên";
                        range.Worksheet.Cells["D6:D7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D6:D7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["D6:D7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["E6:E7"])
                    {
                        range.Worksheet.Cells["E6:E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E6:E7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E6:E7"].Merge = true;
                        range.Worksheet.Cells["E6:E7"].Value = "Năm sinh";
                        range.Worksheet.Cells["E6:E7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E6:E7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["E6:E7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["F6:F7"])
                    {
                        range.Worksheet.Cells["F6:F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F6:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F6:F7"].Merge = true;
                        range.Worksheet.Cells["F6:F7"].Value = "Giới tính";
                        range.Worksheet.Cells["F6:F7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F6:F7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["F6:F7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["G6:G7"])
                    {
                        range.Worksheet.Cells["G6:G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G6:G7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G6:G7"].Merge = true;
                        range.Worksheet.Cells["G6:G7"].Value = "Số BHYT";
                        range.Worksheet.Cells["G6:G7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G6:G7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["G6:G7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["H6:H7"])
                    {
                        range.Worksheet.Cells["H6:H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H6:H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H6:H7"].Merge = true;
                        range.Worksheet.Cells["H6:H7"].Value = "Nội dung thực hiện";
                        range.Worksheet.Cells["H6:H7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H6:H7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["H6:H7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["I6:I7"])
                    {
                        range.Worksheet.Cells["I6:I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I6:I7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I6:I7"].Merge = true;
                        range.Worksheet.Cells["I6:I7"].Value = "Chẩn đoán/diễn biến";
                        range.Worksheet.Cells["I6:I7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["I6:I7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["I6:I7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["J6:J7"])
                    {
                        range.Worksheet.Cells["J6:J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J6:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J6:J7"].Merge = true;
                        range.Worksheet.Cells["J6:J7"].Value = "Kết luận";
                        range.Worksheet.Cells["J6:J7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["J6:J7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["J6:J7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K6:K7"])
                    {
                        range.Worksheet.Cells["K6:K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K6:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K6:K7"].Merge = true;
                        range.Worksheet.Cells["K6:K7"].Value = "Số Lượng";
                        range.Worksheet.Cells["K6:K7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K6:K7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["K6:K7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["L6:L7"])
                    {
                        range.Worksheet.Cells["L6:L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L6:L7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L6:L7"].Merge = true;
                        range.Worksheet.Cells["L6:L7"].Value = "Đơn giá";
                        range.Worksheet.Cells["L6:L7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L6:L7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["L6:L7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["M6:M7"])
                    {
                        range.Worksheet.Cells["M6:M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M6:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M6:M7"].Merge = true;
                        range.Worksheet.Cells["M6:M7"].Value = "Nơi chỉ định";
                        range.Worksheet.Cells["M6:M7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["M6:M7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["M6:M7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["N6:N7"])
                    {
                        range.Worksheet.Cells["N6:N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N6:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N6:N7"].Merge = true;
                        range.Worksheet.Cells["N6:N7"].Value = "Người chỉ định";
                        range.Worksheet.Cells["N6:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["N6:N7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["N6:N7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["O6:O7"])
                    {
                        range.Worksheet.Cells["O6:O7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O6:O7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O6:O7"].Merge = true;
                        range.Worksheet.Cells["O6:O7"].Value = "Người trả lời KQ";
                        range.Worksheet.Cells["O6:O7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["O6:O7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["O6:O7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["P6:P7"])
                    {
                        range.Worksheet.Cells["P6:P7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P6:P7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["P6:P7"].Merge = true;
                        range.Worksheet.Cells["P6:P7"].Value = "KTV1";
                        range.Worksheet.Cells["P6:P7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["P6:P7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["P6:P7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["Q6:Q7"])
                    {
                        range.Worksheet.Cells["Q6:Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q6:Q7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q6:Q7"].Merge = true;
                        range.Worksheet.Cells["Q6:Q7"].Value = "Ngày chỉ định";
                        range.Worksheet.Cells["Q6:Q7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["Q6:Q7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["Q6:Q7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["R6:R7"])
                    {
                        range.Worksheet.Cells["R6:R7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R6:R7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["R6:R7"].Merge = true;
                        range.Worksheet.Cells["R6:R7"].Value = "Ngày thực hiện";
                        range.Worksheet.Cells["R6:R7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["R6:R7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["R6:R7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["S6:S7"])
                    {
                        range.Worksheet.Cells["S6:S7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["S6:S7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["S6:S7"].Merge = true;
                        range.Worksheet.Cells["S6:S7"].Value = "Ngày vào viện";
                        range.Worksheet.Cells["S6:S7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["S6:S7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["S6:S7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["T6:T7"])
                    {
                        range.Worksheet.Cells["T6:T7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["T6:T7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["T6:T7"].Merge = true;
                        range.Worksheet.Cells["T6:T7"].Value = "Ngày ra viện";
                        range.Worksheet.Cells["T6:T7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["T6:T7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["T6:T7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["U6:U7"])
                    {
                        range.Worksheet.Cells["U6:U7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["U6:U7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["U6:U7"].Merge = true;
                        range.Worksheet.Cells["U6:U7"].Value = "Ghi chú";
                        range.Worksheet.Cells["U6:U7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["U6:U7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["U6:U7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    var index = 8; // bắt đầu đổ data từ dòng 
                    var STT = 1;
                    var nhomDichVus = dataBaoCaoSoThongKeCls.Select(z => new { z.NhomDichVuBenhVienId, z.Nhom }).OrderBy(z => z.Nhom).Distinct().ToList();
                    foreach (var nhomDichVu in nhomDichVus)
                    {
                        var tenNhom = dataBaoCaoSoThongKeCls.Where(z => z.NhomDichVuBenhVienId == nhomDichVu.NhomDichVuBenhVienId).Select(z => z.Nhom).FirstOrDefault();
                        var dataBaoCaoSoThongKeClsTheoNhom = dataBaoCaoSoThongKeCls.Where(z => z.NhomDichVuBenhVienId == nhomDichVu.NhomDichVuBenhVienId).ToList();
                        var soLuongThongKeClsTheoNhom = dataBaoCaoSoThongKeClsTheoNhom.Select(o => o.SoLan.GetValueOrDefault()).DefaultIfEmpty().Sum();
                        using (var range = worksheet.Cells["A" + index + ":U" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":U" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":U" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":U" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":U" + index].Style.Font.Bold = true;
                        }
                        worksheet.Row(index).Height = 20.5;
                        using (var range = worksheet.Cells["A" + index + ":U" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":U" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":U" + index].Value = tenNhom + " (" + soLuongThongKeClsTheoNhom + ")";
                            range.Worksheet.Cells["A" + index + ":U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        index++;
                        foreach (var chitiet in dataBaoCaoSoThongKeClsTheoNhom)
                        {
                            using (var range = worksheet.Cells["A" + index + ":U" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":U" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":U" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":U" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["A" + index])
                            {
                                range.Worksheet.Cells["A" + index].Value = STT;
                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["B" + index])
                            {
                                range.Worksheet.Cells["B" + index].Value = chitiet.MaTN;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["C" + index])
                            {
                                range.Worksheet.Cells["C" + index].Value = chitiet.SoBA;
                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["D" + index])
                            {
                                range.Worksheet.Cells["D" + index].Value = chitiet.HoTen;
                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["D" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["E" + index])
                            {
                                range.Worksheet.Cells["E" + index].Value = chitiet.NamSinh;
                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["F" + index])
                            {
                                range.Worksheet.Cells["F" + index].Value = chitiet.GioiTinhDisplay;
                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            }

                            using (var range = worksheet.Cells["G" + index])
                            {
                                range.Worksheet.Cells["G" + index].Value = chitiet.BHYTMaSoThe;
                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["H" + index])
                            {
                                range.Worksheet.Cells["H" + index].Value = chitiet.TenDichVu;
                                range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["H" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["I" + index])
                            {
                                range.Worksheet.Cells["I" + index].Value = chitiet.ChanDoan;
                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["I" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["J" + index])
                            {
                                range.Worksheet.Cells["J" + index].Value = chitiet.KetLuan;
                                range.Worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["J" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["K" + index])
                            {
                                range.Worksheet.Cells["K" + index].Value = chitiet.SoLan;
                                range.Worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["L" + index])
                            {
                                range.Worksheet.Cells["L" + index].Value = chitiet.Gia;
                                range.Worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            }


                            using (var range = worksheet.Cells["M" + index])
                            {
                                range.Worksheet.Cells["M" + index].Value = chitiet.TenNoiChiDinh;
                                range.Worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["M" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["N" + index])
                            {
                                range.Worksheet.Cells["N" + index].Value = chitiet.TenNguoiChiDinh;
                                range.Worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["N" + index].Style.WrapText = true;
                            }


                            using (var range = worksheet.Cells["O" + index])
                            {
                                range.Worksheet.Cells["O" + index].Value = chitiet.TenNguoiKetLuan;
                                range.Worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["O" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["P" + index])
                            {
                                range.Worksheet.Cells["P" + index].Value = chitiet.KTV;
                                range.Worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["P" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["Q" + index])
                            {
                                range.Worksheet.Cells["Q" + index].Value = chitiet.ThoiDiemDangKyDisplay;
                                range.Worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["Q" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["R" + index])
                            {
                                range.Worksheet.Cells["R" + index].Value = chitiet.ThoiDiemThucHienDisplay;
                                range.Worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["R" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["S" + index])
                            {
                                range.Worksheet.Cells["S" + index].Value = chitiet.ThoiDiemNhapVienDisplay;
                                range.Worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["S" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["T" + index])
                            {
                                range.Worksheet.Cells["T" + index].Value = chitiet.ThoiDiemRaVienDisplay;
                                range.Worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["T" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["U" + index])
                            {
                                range.Worksheet.Cells["U" + index].Value = chitiet.GhiChu;
                                range.Worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["U" + index].Style.WrapText = true;
                            }

                            STT++;
                            index++;
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();

            }
        }

        #endregion

        public async Task<List<LookupItemVo>> KhoaPhongs(DropDownListRequestModel queryInfo)
        {
            var khoaPhongs = _khoaPhongRepository.TableNoTracking
                                .Select(s => new LookupItemVo
                                {
                                    KeyId = s.Id,
                                    DisplayName = s.Ten
                                }).ApplyLike(queryInfo.Query, z => z.DisplayName)
                                .Take(queryInfo.Take);
            return await khoaPhongs.ToListAsync();
        }
    }
}
