using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System.Collections.Generic;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachGoiKhamSucKhoe(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = _goiKhamSucKhoeRepository.TableNoTracking
            .Select(s => new GoiKhamSucKhoeDoanVo
            {
                Id = s.Id,
                MaGoiKham = s.Ma,
                TenGoiKham = s.Ten,
                TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                SoHopDong = s.HopDongKhamSucKhoe.SoHopDong,
                NgayHieuLuc = s.HopDongKhamSucKhoe.NgayHieuLuc,
                NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId
            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);
                if (queryString.IsHopDongKhamSucKhoe == true)
                {
                    query = query.Where(p => p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                       g => g.MaGoiKham,
                       g => g.TenGoiKham,
                       g => g.TenCongTy,
                       g => g.TenCongTy,
                       g => g.SoHopDong
                   );
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaGoiKham,
                    g => g.TenGoiKham,
                    g => g.TenCongTy,
                    g => g.TenCongTy,
                    g => g.SoHopDong
                    );

            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachGoiKhamSucKhoe(QueryInfo queryInfo)
        {
            var query = _goiKhamSucKhoeRepository.TableNoTracking
               .Select(s => new GoiKhamSucKhoeDoanVo
               {
                   Id = s.Id,
                   MaGoiKham = s.Ma,
                   TenGoiKham = s.Ten,
                   TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                   SoHopDong = s.HopDongKhamSucKhoe.SoHopDong,
                   NgayHieuLuc = s.HopDongKhamSucKhoe.NgayHieuLuc,
                   NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                   HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId

               });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);
                if (queryString.IsHopDongKhamSucKhoe == true)
                {
                    query = query.Where(p => p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.MaGoiKham,
                       g => g.TenGoiKham,
                       g => g.TenCongTy,
                       g => g.TenCongTy,
                       g => g.SoHopDong
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                      g => g.MaGoiKham,
                       g => g.TenGoiKham,
                       g => g.TenCongTy,
                       g => g.TenCongTy,
                       g => g.SoHopDong
                    );

            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<string> GetNoiThucHienString(long phongBenhVienId)
        {
            var noiThucHien = await _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == phongBenhVienId).Select(p => p.Ten).FirstOrDefaultAsync();
            return noiThucHien;
        }

        public async Task CheckGoiKhamDaKetThucHoacDaXoa(long goiKhamId)
        {
            var goiKhamSucKhoe = await _goiKhamSucKhoeRepository.TableNoTracking
                .Include(p => p.HopDongKhamSucKhoe)
                .Where(p => p.Id == goiKhamId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (goiKhamSucKhoe == null)
            {
                resourceName = "KhamDoan.GoiKham.NotExists";
            }
            else
            {
                if (goiKhamSucKhoe.HopDongKhamSucKhoe.NgayKetThuc < DateTime.Now || goiKhamSucKhoe.HopDongKhamSucKhoe.DaKetThuc)
                {
                    resourceName = "KhamDoan.GoiKham.DaKetThuc";
                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                var currentUserLanguge = _userAgentHelper.GetUserLanguage();
                var mess = await _localeStringResourceRepository.TableNoTracking
                    .Where(x => x.ResourceName == resourceName && x.Language == (int)currentUserLanguge)
                    .Select(x => x.ResourceValue).FirstOrDefaultAsync();
                throw new Exception(mess ?? resourceName);
            }
        }

        public async Task<bool> IsTenGoiKhamExists(string tenGoiKham = null, long goiKhamId = 0, long? hopDongKhamSucKhoeId = 0)
        {
            var result = false;
            if (goiKhamId == 0)
            {
                result = await _goiKhamSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenGoiKham) && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId);
            }
            else
            {
                result = await _goiKhamSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenGoiKham) && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && p.Id != goiKhamId);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> IsMaGoiKhamExists(string maGoiKham = null, long goiKhamId = 0, long? hopDongKhamSucKhoeId = 0)
        {
            var result = false;
            if (goiKhamId == 0)
            {
                result = await _goiKhamSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maGoiKham) && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId);
            }
            else
            {
                result = await _goiKhamSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maGoiKham) && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && p.Id != goiKhamId);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> CheckDichVuExists(long? dichVuKyThuatBenhVienId, bool laDichVuKham, List<long> dichVuKhamBenhIds, List<long> dichVuKyThuatIds)
        {
            if (dichVuKhamBenhIds == null && dichVuKyThuatIds == null)
            {
                return true;
            }
            if (laDichVuKham && dichVuKhamBenhIds != null)
            {
                foreach (var item in dichVuKhamBenhIds)
                {
                    if (dichVuKhamBenhIds.Any(p => p == dichVuKyThuatBenhVienId))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            if (!laDichVuKham && dichVuKyThuatIds != null)
            {
                foreach (var item in dichVuKyThuatIds)
                {
                    if (dichVuKyThuatIds.Any(p => p == dichVuKyThuatBenhVienId))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }
    }
}
