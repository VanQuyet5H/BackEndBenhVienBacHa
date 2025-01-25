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
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachGoiKhamChungSucKhoe(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = _goiKhamChungSucKhoeRepository.TableNoTracking
            .Select(s => new GoiKhamSucKhoeChungDoanVo
            {
                Id = s.Id,
                MaGoiKham = s.Ma,
                TenGoiKham = s.Ten
            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiKhamSucKhoeChungDoanVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                       g => g.MaGoiKham,
                       g => g.TenGoiKham
                   );
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaGoiKham,
                    g => g.TenGoiKham);

            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachGoiKhamChungSucKhoe(QueryInfo queryInfo)
        {
            var query = _goiKhamChungSucKhoeRepository.TableNoTracking
            .Select(s => new GoiKhamSucKhoeChungDoanVo
            {
                Id = s.Id,
                MaGoiKham = s.Ma,
                TenGoiKham = s.Ten
            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiKhamSucKhoeChungDoanVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                       g => g.MaGoiKham,
                       g => g.TenGoiKham
                   );
                }
            }

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.MaGoiKham,
                    g => g.TenGoiKham);

            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task CheckGoiKhamDaKetThucHoacDaXoaChung(long GoiKhamChungId)
        {
            var GoiKhamChungSucKhoe = await _goiKhamChungSucKhoeRepository.TableNoTracking
                .Where(p => p.Id == GoiKhamChungId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (GoiKhamChungSucKhoe == null)
            {
                resourceName = "KhamDoan.GoiKhamChung.NotExists";
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

        public async Task<bool> IsTenGoiKhamExistsChung(string tenGoiKhamChung = null, long GoiKhamChungId = 0)
        {
            var result = false;
            if (GoiKhamChungId == 0)
            {
                result = await _goiKhamChungSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenGoiKhamChung));
            }
            else
            {
                result = await _goiKhamChungSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenGoiKhamChung) && p.Id != GoiKhamChungId);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> IsMaGoiKhamExistsChung(string maGoiKhamChung = null, long GoiKhamChungId = 0)
        {
            var result = false;
            if (GoiKhamChungId == 0)
            {
                result = await _goiKhamChungSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maGoiKhamChung));
            }
            else
            {
                result = await _goiKhamChungSucKhoeRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maGoiKhamChung) && p.Id != GoiKhamChungId);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> CheckGoiDichVuExistsChung(long? dichVuKyThuatBenhVienId, List<long> dichVuKhamBenhVaKyThuatIds)
        {
            if (dichVuKhamBenhVaKyThuatIds == null)
            {
                return true;
            }
            if (dichVuKhamBenhVaKyThuatIds != null)
            {
                foreach (var item in dichVuKhamBenhVaKyThuatIds)
                {
                    if (dichVuKhamBenhVaKyThuatIds.Any(p => p == dichVuKyThuatBenhVienId))
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
