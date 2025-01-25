using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        private YeuCauDichVuKyThuat getFirstYeuCauDichVuKyThuat(YeuCauTiepNhan o, DateTime? tuNgay, DateTime? denNgay)
        {
            var f = o.YeuCauDichVuKyThuats.FirstOrDefault(yckt => yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && (tuNgay == null || yckt.ThoiDiemHoanThanh >= tuNgay) && (denNgay == null || yckt.ThoiDiemHoanThanh <= denNgay));
            if (f == null)
            {
            }
            return f;
        }
        public async Task<GridDataSource> GetDataForGridAsyncLichSuPhauThuatThuThuat(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BuildQueryLichSuPTTT(queryInfo);
            //var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
            //                                               .Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //                                               .Include(c => c.TheoDoiSauPhauThuatThuThuat)
            //                                               .Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)
            //                                               .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
            //        .Where(yckt => yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && (yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien));

            //if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            //{
            //    var searchTerms = queryInfo.SearchTerms.Replace("\t", "").Trim();
            //    dataSreach = dataSreach.ApplyLike(searchTerms,
            //        g => g.YeuCauTiepNhan.HoTen,
            //        g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //        g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //        g => g.YeuCauTiepNhan.DiaChiDayDu
            //   );
            //}

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{

            //    var queryString = JsonConvert.DeserializeObject<LichSuPhauThuatThuThuatGridVo>(queryInfo.AdditionalSearchString);

            //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //    {
            //        DateTime denNgay;
            //        queryString.FromDate.TryParseExactCustom(out var tuNgay);
            //        //                    DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
            //        //                        out var tuNgay);
            //        if (string.IsNullOrEmpty(queryString.ToDate))
            //        {
            //            denNgay = DateTime.Now;
            //        }
            //        else
            //        {
            //            queryString.ToDate.TryParseExactCustom(out denNgay);
            //            //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //        }
            //        denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            //        dataSreach = dataSreach.Where(p => p.YeuCauTiepNhan.ThoiDiemTiepNhan >= tuNgay && p.YeuCauTiepNhan.ThoiDiemTiepNhan <= denNgay);
            //    }
            //    if (!string.IsNullOrEmpty(queryString.SearchString))
            //    {
            //        var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //        dataSreach = dataSreach.ApplyLike(searchTerms,
            //            g => g.YeuCauTiepNhan.HoTen,
            //            g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //            g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //            g => g.YeuCauTiepNhan.DiaChiDayDu
            //       );
            //    }
            //}
            //var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId }).OrderByDescending(g => g.Key.YeuCauTiepNhanId).ThenBy(g => g.Key.LanThucHien)
            //                                     .Select(g => new { YeuCauKyThuats = g.FirstOrDefault(), ListYeuCauKTIds = g.Select(c => c.Id) })
            //                                     .Select(group=> new LichSuPhauThuatThuThuatGridVo
            //                                     {
            //                                         Id = group.YeuCauKyThuats.Id,
            //                                         MaYeuCauTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //                                         BenhNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhanId,
            //                                         MaBN = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhan.MaBN,
            //                                         HoTen = group.YeuCauKyThuats.YeuCauTiepNhan.HoTen,
            //                                         NamSinh = group.YeuCauKyThuats.YeuCauTiepNhan.NamSinh,
            //                                         DiaChi = group.YeuCauKyThuats.YeuCauTiepNhan.DiaChiDayDu,
            //                                         DoiTuong = group.YeuCauKyThuats.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + group.YeuCauKyThuats.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
            //                                         TuVongTrongPTTT = group.YeuCauKyThuats.YeuCauTiepNhan.TuVongTrongPTTT,
            //                                         TrangThaiPTTTSearch = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || (group.YeuCauKyThuats.TheoDoiSauPhauThuatThuThuat!=null && group.YeuCauKyThuats.TheoDoiSauPhauThuatThuThuat.ThoiDiemTuVong != null)) ? "Đã tử vong" : "Đã chuyển giao",
            //                                         LanThucThien = group.YeuCauKyThuats.LanThucHien,
            //                                         TenDichVu = group.YeuCauKyThuats.YeuCauKhamBenh!=null?group.YeuCauKyThuats.YeuCauKhamBenh.TenDichVu:"",
            //                                         ThoiDiemTiepNhanDisplay = group.YeuCauKyThuats.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
            //                                         ThoiDiemTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.ThoiDiemTiepNhan,
            //                                         TrieuChungTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.TrieuChungTiepNhan,
            //                                         ThoiDiemThucHien = group.YeuCauKyThuats.ThoiDiemThucHien,
            //                                         ThoiDiemHoanThanh = group.YeuCauKyThuats.ThoiDiemHoanThanh,
            //                                         NoiChuyenGiao = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || (group.YeuCauKyThuats.TheoDoiSauPhauThuatThuThuat!=null && group.YeuCauKyThuats.TheoDoiSauPhauThuatThuThuat.ThoiDiemTuVong != null)) ? "" : group.YeuCauKyThuats.NoiChiDinh.KhoaPhong.Ten,
            //                                         //TuongTrinhLaiYeuCauKyThuatIds = group.ListYeuCauKTIds.ToList(),
            //                                         //DuocTuongTrinhLai = group.YeuCauKyThuats.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien ||
            //                                         //                                        group.YeuCauKyThuats.YeuCauTiepNhan?.NoiTruBenhAn?.ThoiDiemRaVien != null,
            //                                         DuocTuongTrinhLai = group.YeuCauKyThuats.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
            //                                                    (group.YeuCauKyThuats.YeuCauTiepNhan.NoiTruBenhAn == null || group.YeuCauKyThuats.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null),
            //                                         PhongBenhVienId = group.YeuCauKyThuats.NoiThucHienId.GetValueOrDefault()
            //                                     });
            //var query = dataGroupBySoLans;

            //var lichSuPhauThuatThuThuatGridVos = new List<LichSuPhauThuatThuThuatGridVo>();
            //foreach (var group in dataGroupBySoLans)
            //{
            //    var phongBenhVienHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
            //                                                            .Where(x => x.YeuCauTiepNhanId == group.YeuCauKyThuats.YeuCauTiepNhanId &&
            //                                                                   x.YeuCauDichVuKyThuatId == group.YeuCauKyThuats.Id);
            //    if (!phongBenhVienHangDoi.Any())
            //    {
            //        var lichSuPhauThuatThuThuatGridVo = new LichSuPhauThuatThuThuatGridVo
            //        {
            //            Id = group.YeuCauKyThuats.Id,
            //            MaYeuCauTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            BenhNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhanId,
            //            MaBN = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhan.MaBN,
            //            HoTen = group.YeuCauKyThuats.YeuCauTiepNhan.HoTen,
            //            NamSinh = group.YeuCauKyThuats.YeuCauTiepNhan.NamSinh,
            //            DiaChi = group.YeuCauKyThuats.YeuCauTiepNhan.DiaChiDayDu,
            //            DoiTuong = group.YeuCauKyThuats.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + group.YeuCauKyThuats.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
            //            TuVongTrongPTTT = group.YeuCauKyThuats.YeuCauTiepNhan.TuVongTrongPTTT,
            //            TrangThaiPTTTSearch = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "Đã tử vong" : "Đã chuyển giao",
            //            LanThucThien = group.YeuCauKyThuats.LanThucHien,
            //            TenDichVu = group.YeuCauKyThuats.YeuCauKhamBenh?.TenDichVu,
            //            ThoiDiemTiepNhanDisplay = group.YeuCauKyThuats.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
            //            ThoiDiemTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.ThoiDiemTiepNhan,
            //            TrieuChungTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.TrieuChungTiepNhan,
            //            ThoiDiemThucHien = group.YeuCauKyThuats.ThoiDiemThucHien,
            //            ThoiDiemHoanThanh = group.YeuCauKyThuats.ThoiDiemHoanThanh,
            //            NoiChuyenGiao = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "" : group.YeuCauKyThuats.NoiChiDinh.KhoaPhong.Ten,
            //            TuongTrinhLaiYeuCauKyThuatIds = group.ListYeuCauKTIds.ToList(),
            //            //DuocTuongTrinhLai = group.YeuCauKyThuats.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien ||
            //            //                                        group.YeuCauKyThuats.YeuCauTiepNhan?.NoiTruBenhAn?.ThoiDiemRaVien != null,
            //            DuocTuongTrinhLai = group.YeuCauKyThuats.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
            //                                                    (group.YeuCauKyThuats.YeuCauTiepNhan.NoiTruBenhAn == null || group.YeuCauKyThuats.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null),
            //        PhongBenhVienId = group.YeuCauKyThuats.NoiThucHienId.GetValueOrDefault()
            //    };
            //        lichSuPhauThuatThuThuatGridVos.Add(lichSuPhauThuatThuThuatGridVo);
            //    }
            //}

            //var query = lichSuPhauThuatThuThuatGridVos.AsQueryable();

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuPhauThuatThuThuat(QueryInfo queryInfo)
        {

            //var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan).Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //       .Include(c => c.TheoDoiSauPhauThuatThuThuat)
            //      .Where(yckt => yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && (yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien));

            //if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            //{
            //    var searchTerms = queryInfo.SearchTerms.Replace("\t", "").Trim();
            //    dataSreach = dataSreach.ApplyLike(searchTerms,
            //        g => g.YeuCauTiepNhan.HoTen,
            //        g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //        g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //        g => g.YeuCauTiepNhan.DiaChiDayDu
            //   );
            //}

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{

            //    var queryString = JsonConvert.DeserializeObject<LichSuPhauThuatThuThuatGridVo>(queryInfo.AdditionalSearchString);

            //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //    {
            //        DateTime denNgay;
            //        queryString.FromDate.TryParseExactCustom(out var tuNgay);
            //        //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
            //        //    out var tuNgay);
            //        if (string.IsNullOrEmpty(queryString.ToDate))
            //        {
            //            denNgay = DateTime.Now;
            //        }
            //        else
            //        {
            //            queryString.ToDate.TryParseExactCustom(out denNgay);
            //            //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //        }
            //        denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            //        dataSreach = dataSreach.Where(p => p.YeuCauTiepNhan.ThoiDiemTiepNhan >= tuNgay && p.YeuCauTiepNhan.ThoiDiemTiepNhan <= denNgay);
            //    }
            //    if (!string.IsNullOrEmpty(queryString.SearchString))
            //    {
            //        var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //        dataSreach = dataSreach.ApplyLike(searchTerms,
            //            g => g.YeuCauTiepNhan.HoTen,
            //            g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //            g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //            g => g.YeuCauTiepNhan.DiaChiDayDu
            //       );
            //    }
            //}
            //var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId }).OrderByDescending(g => g.Key.YeuCauTiepNhanId).ThenBy(g => g.Key.LanThucHien)
            //                                     .Select(g => new { YeuCauKyThuats = g.FirstOrDefault() });

            //var lichSuPhauThuatThuThuatGridVos = new List<LichSuPhauThuatThuThuatGridVo>();
            //foreach (var group in dataGroupBySoLans)
            //{
            //    var phongBenhVienHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
            //                                                            .Where(x => x.YeuCauTiepNhanId == group.YeuCauKyThuats.YeuCauTiepNhanId &&
            //                                                                   x.YeuCauDichVuKyThuatId == group.YeuCauKyThuats.Id);
            //    if (!phongBenhVienHangDoi.Any())
            //    {
            //        var lichSuPhauThuatThuThuatGridVo = new LichSuPhauThuatThuThuatGridVo
            //        {
            //            Id = group.YeuCauKyThuats.Id,
            //            MaYeuCauTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            BenhNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhanId,
            //            MaBN = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhan.MaBN,
            //            HoTen = group.YeuCauKyThuats.YeuCauTiepNhan.HoTen,
            //            NamSinh = group.YeuCauKyThuats.YeuCauTiepNhan.NamSinh,
            //            DiaChi = group.YeuCauKyThuats.YeuCauTiepNhan.DiaChiDayDu,
            //            DoiTuong = group.YeuCauKyThuats.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + group.YeuCauKyThuats.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
            //            TuVongTrongPTTT = group.YeuCauKyThuats.YeuCauTiepNhan.TuVongTrongPTTT,
            //            TrangThaiPTTTSearch = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "Đã tử vong" : "Đã chuyển giao",
            //            LanThucThien = group.YeuCauKyThuats.LanThucHien,
            //            TenDichVu = group.YeuCauKyThuats.YeuCauKhamBenh?.TenDichVu,
            //            ThoiDiemTiepNhanDisplay = group.YeuCauKyThuats.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
            //            ThoiDiemTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.ThoiDiemTiepNhan,
            //            TrieuChungTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.TrieuChungTiepNhan,
            //            ThoiDiemThucHien = group.YeuCauKyThuats.ThoiDiemThucHien,
            //            PhongBenhVienId = group.YeuCauKyThuats.NoiThucHienId.GetValueOrDefault()
            //        };
            //        lichSuPhauThuatThuThuatGridVos.Add(lichSuPhauThuatThuThuatGridVo);
            //    }
            //}
            //var query = lichSuPhauThuatThuThuatGridVos.AsQueryable();
            var query = BuildQueryLichSuPTTT(queryInfo);
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }
        private IQueryable<LichSuPhauThuatThuThuatGridVo> BuildQueryLichSuPTTT(QueryInfo queryInfo)
        {

            string searchString = null;
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            DanhSachKhamPTHoanThanhThucHienVo thongTinThucHien = null;

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<LichSuPhauThuatThuThuatGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    if (queryString.FromDate != null && !string.IsNullOrEmpty(queryString.FromDate))
                    {
                        DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                        tuNgay = tuNgayTemp;
                    }

                    if (queryString.ToDate != null && !string.IsNullOrEmpty(queryString.ToDate))
                    {
                        DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                        denNgay = denNgayTemp;
                    }
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    searchString = queryString.SearchString.Replace("\t", "").Trim();
                }

                //BVHD-3860
                thongTinThucHien = queryString.ThongTinThucHien;
            }
            //var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
            //                                               .Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //                                               .Include(c => c.TheoDoiSauPhauThuatThuThuat)
            //                                               .Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)
            //                                               .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
            //        .Where(yckt => yckt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat && yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //        (yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
            //        (tuNgay == null || yckt.ThoiDiemHoanThanh >= tuNgay) && (denNgay == null || yckt.ThoiDiemHoanThanh <= denNgay)).ApplyLike(searchString,
            //            g => g.YeuCauTiepNhan.HoTen,
            //            g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //            g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //            g => g.YeuCauTiepNhan.DiaChiDayDu
            //       );

            //var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId }).OrderByDescending(g => g.Key.YeuCauTiepNhanId).ThenBy(g => g.Key.LanThucHien)
            //                                     .Select(g => new { YeuCauKyThuat = g.FirstOrDefault(), ListYeuCauKTIds = g.Select(c => c.Id) });
            //var yeuCauDichVuKyThuatIds = dataSreach.Select(o => o.Id).ToList();



            #region BVHD-3860: 
            var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
                .Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Include(c => c.TheoDoiSauPhauThuatThuThuat)
                .Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)
                .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Where(yckt => yckt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                               && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                               && (
                                   // trường hợp dv không thực hiện
                                   (yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != null
                                       && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true
                                       && (tuNgay == null || yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh >= tuNgay)
                                       && (denNgay == null || yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh <= denNgay))

                                   // trường hợp dịch vụ đã tường trình
                                   || (yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                       && (tuNgay == null || yckt.ThoiDiemHoanThanh >= tuNgay)
                                       && (denNgay == null || yckt.ThoiDiemHoanThanh <= denNgay))
                                   )
                               && (
                                   thongTinThucHien == null
                                   || (thongTinThucHien.ThucHien == thongTinThucHien.KhongThucHien)
                                   || (thongTinThucHien.ThucHien == true && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
                                   || (thongTinThucHien.KhongThucHien == true && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != null && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true)
                                    )
                        )
                .ApplyLike(searchString,
                    g => g.YeuCauTiepNhan.HoTen,
                    g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    g => g.YeuCauTiepNhan.NamSinh.ToString(),
                    g => g.YeuCauTiepNhan.BenhNhan.MaBN,
                    g => g.YeuCauTiepNhan.DiaChiDayDu
                ).ToList();

            var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId })
                                                .OrderByDescending(g => g.Key.YeuCauTiepNhanId)
                                                .ThenBy(g => g.Key.LanThucHien)
                                                .Select(g => new
                                                {
                                                    YeuCauKyThuat = g.OrderByDescending(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true).ThenBy(a => a.Id).FirstOrDefault(),
                                                    ListYeuCauKTIds = g.Where(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true).Select(c => c.Id),
                                                    CountYeuCauKyThuatTuongTrinh = g.Count(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != null && a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true)
                                                });
            var yeuCauDichVuKyThuatIds = dataSreach
                .Where(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true)
                .Select(o => o.Id).ToList();
            #endregion

            var phongBenhVienHangDois = _phongBenhVienHangDoiRepository.TableNoTracking
                                                                         .Where(x => x.YeuCauDichVuKyThuatId != null && yeuCauDichVuKyThuatIds.Any(o => o == (long)x.YeuCauDichVuKyThuatId))
                                                                         .Select(o => new { YeuCauTiepNhanId = o.YeuCauTiepNhanId, YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId }).ToList();
            var lichSuPhauThuatThuThuatGridVos = new List<LichSuPhauThuatThuThuatGridVo>();
            foreach (var group in dataGroupBySoLans)
            {
                var phongBenhVienHangDoi = phongBenhVienHangDois.Where(x => x.YeuCauTiepNhanId == group.YeuCauKyThuat.YeuCauTiepNhanId &&
                                                                                x.YeuCauDichVuKyThuatId == group.YeuCauKyThuat.Id);
                if (!phongBenhVienHangDoi.Any())
                {
                    var lichSuPhauThuatThuThuatGridVo = new LichSuPhauThuatThuThuatGridVo();
                    lichSuPhauThuatThuThuatGridVo.Id = group.YeuCauKyThuat.Id;
                    lichSuPhauThuatThuThuatGridVo.YeuCauTiepNhanId = group.YeuCauKyThuat.YeuCauTiepNhanId;
                    lichSuPhauThuatThuThuatGridVo.PhongBenhVienId = group.YeuCauKyThuat.NoiThucHienId;
                    //lichSuPhauThuatThuThuatGridVo.NoiTiepNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.NoiTiepNhanId;
                    lichSuPhauThuatThuThuatGridVo.MaYeuCauTiepNhan = group.YeuCauKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan;
                    lichSuPhauThuatThuThuatGridVo.BenhNhanId = group.YeuCauKyThuat.YeuCauTiepNhan.BenhNhanId;
                    lichSuPhauThuatThuThuatGridVo.MaBN = group.YeuCauKyThuat.YeuCauTiepNhan.BenhNhan.MaBN;
                    lichSuPhauThuatThuThuatGridVo.HoTen = group.YeuCauKyThuat.YeuCauTiepNhan.HoTen;
                    lichSuPhauThuatThuThuatGridVo.NamSinh = group.YeuCauKyThuat.YeuCauTiepNhan.NamSinh;
                    lichSuPhauThuatThuThuatGridVo.DiaChi = group.YeuCauKyThuat.YeuCauTiepNhan.DiaChiDayDu;
                    lichSuPhauThuatThuThuatGridVo.ThoiDiemHoanThanh = group.YeuCauKyThuat.ThoiDiemHoanThanh ?? group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT?.ThoiDiemKetThucTuongTrinh;
                    lichSuPhauThuatThuThuatGridVo.DoiTuong = group.YeuCauKyThuat.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + group.YeuCauKyThuat.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)";
                    lichSuPhauThuatThuThuatGridVo.TrangThaiPTTTSearch = ((group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuat?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "Đã tử vong" : "Đã chuyển giao";
                    lichSuPhauThuatThuThuatGridVo.TuVongTrongPTTT = group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT : null;
                    //lichSuPhauThuatThuThuatGridVo.SoLan = group.YeuCauKyThuats.LanThucHien;
                    lichSuPhauThuatThuThuatGridVo.ThoiDiemThucHien = group.YeuCauKyThuat.ThoiDiemThucHien;

                    lichSuPhauThuatThuThuatGridVo.NoiChuyenGiao = ((group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT != null) || group.YeuCauKyThuat?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "" : group.YeuCauKyThuat.NoiChiDinh?.KhoaPhong?.Ten;

                    lichSuPhauThuatThuThuatGridVo.TuongTrinhLaiYeuCauKyThuatIds = group.ListYeuCauKTIds.Any() ? group.ListYeuCauKTIds.ToList() : null;
                    lichSuPhauThuatThuThuatGridVo.DuocTuongTrinhLai = group.YeuCauKyThuat.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien 
                                                                      && (group.YeuCauKyThuat.YeuCauTiepNhan.NoiTruBenhAn == null || group.YeuCauKyThuat.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)

                                                                      //BVHD-3860: nếu có 1 dv mà dv đó không thực hiện PT thì ko cho tường trình lại
                                                                      && group.ListYeuCauKTIds.Any();
                    //lichSuPhauThuatThuThuatGridVo.TuongTrinhLaiYeuCauKyThuatIds = group.ListYeuCauKTIds.Any() ? group.ListYeuCauKTIds.ToList() : null;
                    lichSuPhauThuatThuThuatGridVo.ThoiDiemTiepNhanDisplay = group.YeuCauKyThuat.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH();
                    lichSuPhauThuatThuThuatGridVo.ThoiDiemTiepNhan = group.YeuCauKyThuat.YeuCauTiepNhan.ThoiDiemTiepNhan;

                    //BVHD-3860
                    lichSuPhauThuatThuThuatGridVo.SoDichVuKhongTuongTrinh = group.CountYeuCauKyThuatTuongTrinh;
                    lichSuPhauThuatThuThuatGridVo.LaKhongThucHien = group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien;

                    lichSuPhauThuatThuThuatGridVos.Add(lichSuPhauThuatThuThuatGridVo);
                }

            }
            return lichSuPhauThuatThuThuatGridVos.AsQueryable();
        }
        public async Task<ThongTinBenhNhanPTTTVo> GetThongTinBenhNhan(long yeuCauDichVuKyThuatId)
        {
            var result = await BaseRepository.TableNoTracking
                          .Where(p => p.Id == yeuCauDichVuKyThuatId)
                          .Select(s => new ThongTinBenhNhanPTTTVo
                          {
                              YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                              MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                              BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                              MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                              HoTen = s.YeuCauTiepNhan.HoTen,
                              TenGioiTinh = s.YeuCauTiepNhan.GioiTinh == null ? null : s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                              Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                              SoDienThoai = s.YeuCauTiepNhan.SoDienThoai.ApplyFormatPhone(),
                              DanToc = s.YeuCauTiepNhan.DanToc != null ? s.YeuCauTiepNhan.DanToc.Ten : null,
                              BHYTMucHuong = s.YeuCauTiepNhan.BHYTMucHuong == null ? null : s.YeuCauTiepNhan.BHYTMucHuong,
                              NgheNghiep = s.YeuCauTiepNhan.NgheNghiep != null ? s.YeuCauTiepNhan.NgheNghiep.Ten : null,
                              DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                              TuyenKham = s.YeuCauTiepNhan.LyDoVaoVien == null ? null : s.YeuCauTiepNhan.LyDoVaoVien.GetDescription(),
                              TenLyDoTiepNhan = s.YeuCauTiepNhan.LyDoTiepNhan.Ten,
                              TenLyDoKhamBenh = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                              ThoiDiemTiepNhanDisplay = s.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                              NamSinh = s.YeuCauTiepNhan.NamSinh,
                              ThangSinh = s.YeuCauTiepNhan.ThangSinh,
                              NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                              TuoiThoiDiemHienTai = "",
                              TrangThai = s.YeuCauTiepNhan.TuVongTrongPTTT,
                              LanThucThien = s.LanThucHien,
                              SoBHYT = s.YeuCauTiepNhan.BHYTMaSoThe,
                              CoBHYT = s.YeuCauTiepNhan.CoBHYT,
                              BHYTNgayHetHan = s.YeuCauTiepNhan.BHYTNgayHetHan,
                              BHYTNgayHieuLuc = s.YeuCauTiepNhan.BHYTNgayHieuLuc,
                              TrangThaiTuVongTiepNhan = s.YeuCauTiepNhan.TuVongTrongPTTT.HasValue,
                              //CoPhauThuat = s.YeuCauTiepNhan.YeuCauDichVuKyThuats
                              //.Any(p => !string.IsNullOrEmpty(p.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) && p.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower().Contains("p")),
                              DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVienId,
                              TheoDoiSauPhauThuatThuThuatId = s.TheoDoiSauPhauThuatThuThuatId,
                              TrangThaiTuVongTuongTrinh = (s.YeuCauDichVuKyThuatTuongTrinhPTTT != null && s.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue)
                                                 || (s.TheoDoiSauPhauThuatThuThuat != null && s.TheoDoiSauPhauThuatThuThuat.ThoiDiemTuVong != null),
                              ThongTinKhamKhacChiTiet = s.TheoDoiSauPhauThuatThuThuat.KhamTheoDois.Select(o => new ThongTinKhamKhacChiTietVo
                              {
                                  Id = o.Id,
                                  ThongTinKhamTheoDoiData = o.ThongTinKhamTheoDoiData,
                                  ThongTinKhamTheoDoiTemplate = o.ThongTinKhamTheoDoiTemplate,
                                  KhamToanThan = o.KhamToanThan,
                              }).ToList(),

                              //BVHD-3941
                              CoBaoHiemTuNhan = s.YeuCauTiepNhan.CoBHTN
                          }).FirstOrDefaultAsync();
            if (result != null)
            {
                var tuoiThoiDiemHienTai = 0;
                if (result.NamSinh != null)
                {
                    tuoiThoiDiemHienTai = DateTime.Now.Year - result.NamSinh.Value;
                }
                var dobConvert = DateHelper.ConvertDOBToTimeJson(result.NgaySinh, result.ThangSinh, result.NamSinh);
                var jsonConvertString = new NgayThangNamSinhVo();

                if (!string.IsNullOrEmpty(dobConvert) && tuoiThoiDiemHienTai < 6)
                {
                    jsonConvertString = JsonConvert.DeserializeObject<NgayThangNamSinhVo>(dobConvert);
                }

                var tuoiBenhNhan = result.NamSinh != null ?
                                (tuoiThoiDiemHienTai < 6 ? jsonConvertString.Years + " Tuổi " + jsonConvertString.Months + " Tháng " + jsonConvertString.Days + " Ngày" : tuoiThoiDiemHienTai.ToString()) : tuoiThoiDiemHienTai.ToString();
                result.TuoiThoiDiemHienTai = tuoiBenhNhan;
                result.CoPhauThuat = IsPhauThuat(result.DichVuKyThuatBenhVienId);
            }
            return result;
        }

        public async Task<LichSuKetLuanPTTTVo> GetThongTinLichSuKetLuanPTTT(long yeuCauDichVuKyThuatId)
        {
            var result = await BaseRepository.TableNoTracking
                           .Where(p => p.Id == yeuCauDichVuKyThuatId)
                           .Select(s => new LichSuKetLuanPTTTVo
                           {
                               YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                               TinhTrang = s.TheoDoiSauPhauThuatThuThuat != null ? s.TheoDoiSauPhauThuatThuThuat.TrangThai : 0,
                               TinhTrangDisplay = s.TheoDoiSauPhauThuatThuThuat != null ? s.TheoDoiSauPhauThuatThuThuat.TrangThai.GetDescription() : null,
                               DieuDuongPhuTrachTheoDoiId = s.TheoDoiSauPhauThuatThuThuat != null ? s.TheoDoiSauPhauThuatThuThuat.DieuDuongPhuTrachTheoDoiId : null,
                               DieuDuongPhuTrachTheoDoi = s.TheoDoiSauPhauThuatThuThuat.DieuDuongPhuTrachTheoDoi != null ? s.TheoDoiSauPhauThuatThuThuat.DieuDuongPhuTrachTheoDoi.User.HoTen : null,
                               BacSiPhuTrachTheoDoiId = s.TheoDoiSauPhauThuatThuThuat != null ? s.TheoDoiSauPhauThuatThuThuat.BacSiPhuTrachTheoDoiId : null,
                               BacSiPhuTrachTheoDoi = s.TheoDoiSauPhauThuatThuThuat.BacSiPhuTrachTheoDoi != null ? s.TheoDoiSauPhauThuatThuThuat.BacSiPhuTrachTheoDoi.User.HoTen : null,
                               GhiChuTheoDoi = s.TheoDoiSauPhauThuatThuThuat != null ? s.TheoDoiSauPhauThuatThuThuat.GhiChuTheoDoi : null,
                               ThoiDiemTheoDoi = s.TheoDoiSauPhauThuatThuThuat != null && s.TheoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi != null ? s.TheoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi.Value.ApplyFormatDateTimeSACH() : null
                           }).FirstOrDefaultAsync();
            return result;
        }

        #region grid ChiSoSinhHieu
        public async Task<GridDataSource> GetDataForGridAsyncChiSoSinhHieuPTTT(QueryInfo queryInfo)
        {
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _ketQuaSinhHieuRepository.TableNoTracking
               .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId)
               .Select(s => new LichSuHoiTinhChiSoSinhTonPTTTVo
               {
                   Id = s.Id,
                   NhanVienThucHien = s.NhanVienThucHien.User.HoTen,
                   BMI = s.Bmi,
                   CanNang = s.CanNang,
                   ChieuCao = s.ChieuCao,
                   HuyetAp = s.HuyetApTamThu + "/" + s.HuyetApTamTruong,
                   NgayThucHien = s.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                   NhipTho = s.NhipTho,
                   NhipTim = s.NhipTim,
                   ThanNhiet = s.ThanNhiet,
                   Glassgow = s.Glassgow,
                   SpO2 = s.SpO2
               });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncChiSoSinhHieuPTTT(QueryInfo queryInfo)
        {
            return null;
        }
        #endregion

        #region grid CacCoQuan
        public async Task<GridDataSource> GetDataForGridAsyncLichSuKhamCacCoQuan(QueryInfo queryInfo)
        {
            long khamTheoDoiId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _khamTheoDoiBoPhanKhacRepository.TableNoTracking
                   .Where(s => s.KhamTheoDoiId == khamTheoDoiId)
                  .Select(s => new LichSuTheoDoiKhamCoQuanKhacPTTTVo
                  {
                      Id = s.Id,
                      BoPhan = s.Ten,
                      MoTa = s.NoiDung
                  });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamCacCoQuan(QueryInfo queryInfo)
        {
            return null;
        }
        #endregion

        #region Ekip
        public async Task<LichSuEkipPTTTVo> GetThongTinLichSuEkipPTTT(long yeuCauDichVuKyThuatId)
        {
            var result = BaseRepository.TableNoTracking
                         .Where(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT != null && p.YeuCauDichVuKyThuatTuongTrinhPTTT.Id == yeuCauDichVuKyThuatId && (p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy || p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
                         .Select(s => new LichSuEkipPTTTVo
                         {
                             Id = s.YeuCauDichVuKyThuatTuongTrinhPTTT.Id,
                             ChanDoanVaoKhoa = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.Icdchinh.TenTiengViet : null,
                             MoTaChanDoan = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.GhiChuICDChinh : null,
                             ChanDoanTruocPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.TenTiengViet : null,
                             MoTaTruocPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDTruocPhauThuat,
                             ChanDoanSauPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.TenTiengViet : null,
                             MoTaSauPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDSauPhauThuat,
                             ThoiGianTruocPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat != null ?
                                                s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat.Value.ApplyFormatDateTimeSACH() : null,
                             KhoaPhongId = s.NoiThucHienId,
                             TenKhoaPhong = s.NoiThucHien.Ten,
                             TenDichVu = s.TenDichVu,
                             TaiBienPTTT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT.GetDescription() : null,
                             TrinhTuPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.TrinhTuPhauThuat,
                             PhuongPhapVoCam = s.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ten : null,
                             PhuongPhapPTTT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT,
                             //LoaiPhauThuatThuThuat = s.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat.GetDescription() : null,
                             LoaiPhauThuatThuThuat = s.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat,
                             TinhHinhPTTT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT.GetDescription() : null,
                             PTTTVienChinh = s.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.Select(p => p.NhanVien.User.HoTen).FirstOrDefault(),
                             ThoiDiemPhauThuat = s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat.Value.ApplyFormatDateTime() : "",
                             ThoiDiemKetThucPhauThuat = s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat.Value.ApplyFormatDateTime() : "",
                             ThoiGianBatDauGayMe = s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe != null ? s.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe.Value.ApplyFormatDateTime() : "",
                             ImgLuocDoPT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.YeuCauDichVuKyThuatLuocDoPhauThuats.Select(p => new LichSuImgLuocDoPTVo
                             {
                                 LuocDo = p.LuocDo,
                                 MoTa = p.MoTa
                             }).ToList(),

                             //BVHD-3877
                             GhiChuCaPTTT = s.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT
                         });
            return await result.FirstOrDefaultAsync();
        }

        public async Task<bool> KiemTraCoDichVuHuy(long yeuCauTiepNhanId)
        {
            var yeuCauDichVuKyThuats = await BaseRepository.TableNoTracking.Include(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId).ToListAsync();
            if (yeuCauDichVuKyThuats.Any(p => p?.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true))
            {
                return true;
            }
            return false;
        }

        public async Task<GridDataSource> GetDataForGridAsyncLichSuEkipBacSi(QueryInfo queryInfo)
        {
            var query = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                 .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == long.Parse(queryInfo.AdditionalSearchString))
                 .Select(x => new LichSuNguoiThucHienEkipPTTTVo
                 {
                     ChucDanh = Enums.EnumNhomChucDanh.BacSi.GetDescription(),
                     HoTen = x.NhanVien.User.HoTen,
                     VaiTro = x.VaiTroBacSi.GetDescription(),
                 })
                 .Union(_phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
                     .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == long.Parse(queryInfo.AdditionalSearchString))
                     .Select(x => new LichSuNguoiThucHienEkipPTTTVo
                     {
                         ChucDanh = Enums.EnumNhomChucDanh.DieuDuong.GetDescription(),
                         HoTen = x.NhanVien.User.HoTen,
                         VaiTro = x.VaiTroDieuDuong.GetDescription(),
                     }));
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuEkipBacSi(QueryInfo queryInfo)
        {
            return null;
        }

        #endregion

        #region CanLamSang
        public async Task<GridDataSource> GetDataForGridAsyncLichSuCLS(QueryInfo queryInfo)
        {
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = BaseRepository.TableNoTracking
               .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && (p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
               .OrderBy(p => p.CreatedOn)
               .Select(s => new LichSuCLSPTTTVo
               {
                   Id = s.Id,
                   TenDichVu = s.TenDichVu,
                   MaDichVu = s.MaDichVu,
                   SoLan = s.SoLan,
                   SoLanDichVuThucHien = "",
                   DonGia = s.Gia,
                   NhanVienThucHienId = s.NhanVienThucHienId,
                   NhanVienThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                   ThoiDiemThucHienDisplay = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTime() : null,
                   NoiThucHienId = s.NoiThucHienId,
                   NoiThucHien = s.NoiThucHien != null ? s.NoiThucHien.Ma + " - " + s.NoiThucHien.Ten : null,
                   TenLoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                   DonGiaBaoHiem = s.DonGiaBaoHiem ?? 0,
                   DuocHuongBaoHiem = s.DuocHuongBaoHiem != false ? "Có" : "Không",
                   DHBH = s.DuocHuongBaoHiem,
                   TrangThai = s.TrangThai.GetDescription(),
                   ThanhTien = s.Gia * s.SoLan,
                   Nhom = (string.IsNullOrEmpty(s.NhomDichVuBenhVien.NhomDichVuBenhVienCha != null ? s.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten : "") ? "" : (s.NhomDichVuBenhVien.NhomDichVuBenhVienCha != null ? $"{s.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten} - " : "")) + s.NhomDichVuBenhVien.Ten,
                   NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                   TrangThaiDichVuId = (int)s.TrangThai,
                   //CoFileKetQuaCanLamSangWordExcel = s.FileKetQuaCanLamSangs.All(p => p.LoaiTapTin == LoaiTapTin.Khac),
                   LoaiDichVuKyThuat = (int)s.LoaiDichVuKyThuat,
                   TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                   ThoiDiemChiDinhDisplay = s.ThoiDiemChiDinh.ApplyFormatDateTime(),
                   CoFileKetQuaCanLamSang = s.FileKetQuaCanLamSangs.Any(),
                   FileKetQuaCanLamSangs = s.FileKetQuaCanLamSangs.Select(item => new FileKetQuaCanLamSangVo
                   {
                       Id = item.Id,
                       Url = _taiLieuDinhKemService.GetTaiLieuUrl(item.DuongDan, item.TenGuid),
                       DuongDan = item.DuongDan,
                       Ten = item.Ten,
                       TenGuid = item.TenGuid,
                       LoaiFile = (int)item.LoaiTapTin,
                       MoTa = item.MoTa,
                       IsDownload = true
                   }).ToList()
               });
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuCLS(QueryInfo queryInfo)
        {
            return null;
        }
        #endregion

        public async Task<List<DichVuPTTTsLookupItemVo>> GetDichVuPTTTs(DropDownListRequestModel queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var info = JsonConvert.DeserializeObject<YeuCauDichVuKTPTTTVo>(queryInfo.ParameterDependencies);
                var result = await BaseRepository.TableNoTracking
                            .Where(o => o.YeuCauTiepNhanId == info.YeuCauTiepNhanId && o.LanThucHien == info.LanThucHien && o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && (o.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null || o.ThoiDiemHoanThanh != null))
                            .Select(item => new DichVuPTTTsLookupItemVo
                            {
                                KeyId = item.Id,
                                //DisplayName = item.MaDichVu + " - " + item.TenDichVu,
                                DisplayName = item.TenDichVu,
                                MaDichVu = item.MaDichVu,
                                TenDichVu = item.TenDichVu,
                                TrangThai = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null,
                                BacSiChinhId = item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis
                                .Any(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh) ? item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis
                                .FirstOrDefault(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh).NhanVienId : (long?)null,
                                BacSiChinh = item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis
                                .Any(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh) ? item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis
                                .Where(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh && s.NhanVien != null).SelectMany(s => s.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.Select(bs => bs.NhanVien.User.HoTen)).FirstOrDefault() : null,
                                LoaiPhauThuatThuThuat = item.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat,
                                DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId
                            })
                            .ApplyLike(queryInfo.Query, g => g.DisplayName, g => g.BacSiChinh)
                            .Take(queryInfo.Take)
                            .ToListAsync();

                foreach (var item in result)
                {
                    item.LoaiPTTT = IsPhauThuat(item.DichVuKyThuatBenhVienId) ? "Phẫu thuật" : "Thủ thuật";
                }

                return result;
                //return await result.ToListAsync();
            }
            else
                return null;
        }

        public async Task<GridDataSource> GetDataForGridAsyncLichSuDVPTTTKhongThucHien(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var info = JsonConvert.DeserializeObject<YeuCauDichVuKTPTTTVo>(queryInfo.AdditionalSearchString);
                var query = BaseRepository.TableNoTracking
                            .Where(o => o.YeuCauTiepNhanId == info.YeuCauTiepNhanId && o.LanThucHien == info.LanThucHien && o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true)
                            .Select(s => new LichSuDichVuPTTTKhongThucHienGridVo
                            {
                                Id = s.Id,
                                TenDichVu = s.TenDichVu,
                                BacSiHuyId = s.YeuCauDichVuKyThuatTuongTrinhPTTT.NhanVienTuongTrinhId,
                                HoTenBacSiHuy = s.YeuCauDichVuKyThuatTuongTrinhPTTT.NhanVienTuongTrinh.User.HoTen,
                                LyDo = s.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien
                            });

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            else
                return null;
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuDVPTTTKhongThucHien(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<LichSuDichVuKyThuatDaTuongTrinhPTTT> GetDichVuDaTuongTrinh(LichSuDichVuKyThuatDaTuongTrinhVo lichSuDichVuKyThuatDaTuongTrinhVo)
        {
            var dichVuDaThucHien = await BaseRepository.TableNoTracking
                            .Where(o => o.YeuCauTiepNhanId == lichSuDichVuKyThuatDaTuongTrinhVo.YeuCauTiepNhanId
                                            && o.LanThucHien == lichSuDichVuKyThuatDaTuongTrinhVo.LanThucThien && (o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                            && o.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null && o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true).CountAsync();

            var tongDichVu = await BaseRepository.TableNoTracking
                            .Where(o => o.YeuCauTiepNhanId == lichSuDichVuKyThuatDaTuongTrinhVo.YeuCauTiepNhanId
                                            && o.LanThucHien == lichSuDichVuKyThuatDaTuongTrinhVo.LanThucThien).CountAsync();

            var lichSuDichVuKyThuatDaTuongTrinh = new LichSuDichVuKyThuatDaTuongTrinhPTTT
            {
                DichVuKyThuatDaTuongTrinh = dichVuDaThucHien,
                TongDichVuKyThuat = tongDichVu
            };
            return lichSuDichVuKyThuatDaTuongTrinh;
        }
    }
}
