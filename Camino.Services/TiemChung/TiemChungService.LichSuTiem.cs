using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.TiemChung
{
    public partial class TiemChungService
    {
        public async Task<GridDataSource> GetDataForGridLichSuTiemChungAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            LichSuTiemChungGridSearchVo queryObject = null;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<LichSuTiemChungGridSearchVo>(queryInfo.AdditionalSearchString);
            }

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking.Include(p => p.KhamSangLocTiemChung)
                                                                      .Where(p => (p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || p.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(o => o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)) &&
                                                                                  p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung);

            if (queryObject != null && queryObject.YeuCauTiepNhanId != null && queryObject.BenhNhanId != null && queryObject.YeuCauDichVuKyThuatKhamSangLocId != null)
            {
                var yeuCauDichVuKythuat = _yeuCauDichVuKyThuatRepository.GetById(queryObject.YeuCauDichVuKyThuatKhamSangLocId.Value);

                query = query.Where(p => p.YeuCauTiepNhanId != queryObject.YeuCauTiepNhanId &&
                                         p.YeuCauTiepNhan.BenhNhanId == queryObject.BenhNhanId &&
                                         p.Id != queryObject.YeuCauDichVuKyThuatKhamSangLocId);
            }

            var queryLichSuTiemChung = query.Select(p => new LichSuTiemChungGridVo
            {
                Id = p.Id,
                MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaNguoiBenh = p.YeuCauTiepNhan.BenhNhan.MaBN,
                HoTen = p.YeuCauTiepNhan.BenhNhan.HoTen,
                NamSinh = p.YeuCauTiepNhan.NamSinh,
                DiaChi = p.YeuCauTiepNhan.DiaChiDayDu,
                //MuiTiem = p.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any() ? string.Join("; ", p.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Select(o => o.TenDichVu).GroupBy(o => o).Select(o => o.Key).ToList()) : string.Empty,
                ThoiDiemKham = p.ThoiDiemThucHien,
                BacSiKhamId = p.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId,
                BacSiKhamDisplay = p.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLoc != null ? p.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLoc.User.HoTen : string.Empty,
                LoaiPhanUngSauTiem = p.KhamSangLocTiemChung.LoaiPhanUngSauTiem,
                ThoiGianHenTiem = p.KhamSangLocTiemChung.SoNgayHenTiemMuiTiepTheo != null ? (DateTime?)p.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.GetValueOrDefault().AddDays(p.KhamSangLocTiemChung.SoNgayHenTiemMuiTiepTheo.GetValueOrDefault()) : null
            });

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    queryLichSuTiemChung = queryLichSuTiemChung.ApplyLike(searchTerms, p => p.MaYeuCauTiepNhan, p => p.MaNguoiBenh, p => p.HoTen);
                }

                if (queryObject.ThoiDiemKham != null)
                {
                    if (queryObject.ThoiDiemKham.startDate != null)
                    {
                        queryLichSuTiemChung = queryLichSuTiemChung.Where(p => queryObject.ThoiDiemKham.startDate.Value.Date <= p.ThoiDiemKham.Value.Date);
                    }

                    if (queryObject.ThoiDiemKham.endDate != null)
                    {
                        queryLichSuTiemChung = queryLichSuTiemChung.Where(p => queryObject.ThoiDiemKham.endDate.Value.Date >= p.ThoiDiemKham.Value.Date);
                    }
                }

                if (queryObject.ThoiGianCachLichHen != null)
                {
                    queryLichSuTiemChung = queryLichSuTiemChung.Where(p => p.ThoiGianHenTiem != null &&
                                                                           p.ThoiGianHenTiem.Value.Date == DateTime.Now.Date.AddDays((double)queryObject.ThoiGianCachLichHen.Value));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = queryLichSuTiemChung.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip) .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(queryTask, countTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridLichSuTiemChungAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            LichSuTiemChungGridSearchVo queryObject = null;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<LichSuTiemChungGridSearchVo>(queryInfo.AdditionalSearchString);
            }

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking.Include(p => p.KhamSangLocTiemChung)
                                                                      .Where(p => (p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || p.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(o => o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)) &&
                                                                                  p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung);

            if (queryObject != null && queryObject.YeuCauTiepNhanId != null && queryObject.BenhNhanId != null && queryObject.YeuCauDichVuKyThuatKhamSangLocId != null)
            {
                var yeuCauDichVuKythuat = _yeuCauDichVuKyThuatRepository.GetById(queryObject.YeuCauDichVuKyThuatKhamSangLocId.Value);

                query = query.Where(p => p.YeuCauTiepNhanId != queryObject.YeuCauTiepNhanId &&
                                         p.YeuCauTiepNhan.BenhNhanId == queryObject.BenhNhanId &&
                                         p.Id != queryObject.YeuCauDichVuKyThuatKhamSangLocId);
            }

            var queryLichSuTiemChung = query.Select(p => new LichSuTiemChungGridVo
            {
                Id = p.Id,
                MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaNguoiBenh = p.YeuCauTiepNhan.BenhNhan.MaBN,
                HoTen = p.YeuCauTiepNhan.BenhNhan.HoTen,
                NamSinh = p.YeuCauTiepNhan.NamSinh,
                DiaChi = p.YeuCauTiepNhan.DiaChiDayDu,
                //MuiTiem = p.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any() ? string.Join("; ", p.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Select(o => o.TenDichVu).GroupBy(o => o).Select(o => o.Key).ToList()) : string.Empty,
                ThoiDiemKham = p.ThoiDiemThucHien,
                BacSiKhamId = p.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId,
                BacSiKhamDisplay = p.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLoc != null ? p.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLoc.User.HoTen : string.Empty,
                LoaiPhanUngSauTiem = p.KhamSangLocTiemChung.LoaiPhanUngSauTiem,
                ThoiGianHenTiem = p.KhamSangLocTiemChung.SoNgayHenTiemMuiTiepTheo != null ? (DateTime?)p.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.GetValueOrDefault().AddDays(p.KhamSangLocTiemChung.SoNgayHenTiemMuiTiepTheo.GetValueOrDefault()) : null
            });

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    queryLichSuTiemChung = queryLichSuTiemChung.ApplyLike(searchTerms, p => p.MaYeuCauTiepNhan, p => p.MaNguoiBenh, p => p.HoTen);
                }

                if (queryObject.ThoiDiemKham != null)
                {
                    if (queryObject.ThoiDiemKham.startDate != null)
                    {
                        queryLichSuTiemChung = queryLichSuTiemChung.Where(p => queryObject.ThoiDiemKham.startDate.Value.Date <= p.ThoiDiemKham.Value.Date);
                    }

                    if (queryObject.ThoiDiemKham.endDate != null)
                    {
                        queryLichSuTiemChung = queryLichSuTiemChung.Where(p => queryObject.ThoiDiemKham.endDate.Value.Date >= p.ThoiDiemKham.Value.Date);
                    }
                }

                if (queryObject.ThoiGianCachLichHen != null)
                {
                    queryLichSuTiemChung = queryLichSuTiemChung.Where(p => p.ThoiGianHenTiem != null &&
                                                                           p.ThoiGianHenTiem.Value.Date == DateTime.Now.Date.AddDays((double)queryObject.ThoiGianCachLichHen.Value));
                }
            }

            var countTask = queryLichSuTiemChung.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
    }
}
