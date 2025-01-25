using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ChuanDoanHinhAnh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.ChuanDoanHinhAnhs
{
    [ScopedDependency(ServiceType = typeof(IChuanDoanHinhAnhService))]
    public class ChuanDoanHinhAnhService : MasterFileService<ChuanDoanHinhAnh>, IChuanDoanHinhAnhService
    {
        public ChuanDoanHinhAnhService(IRepository<ChuanDoanHinhAnh> repository) : base(repository)
        { }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var query = BaseRepository.TableNoTracking
                    .Select(source => new ChuanDoanHinhAnhGridVo
                    {
                        Id = source.Id,
                        Ma = source.Ma,
                        Ten = source.Ten,
                        TenTiengAnh = source.TenTiengAnh,
                        LoaiChuanDoanHinhAnhDisplay = source.LoaiChuanDoanHinhAnh.GetDescription(),
                        HieuLuc = source.HieuLuc,
                        MoTa = source.MoTa
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.TenTiengAnh,
                        g => g.MoTa);

                var countTask = queryInfo.LazyLoadPage == true ?
                    Task.FromResult(0) :
                    query.CountAsync();

                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);

                return new GridDataSource
                {
                    Data = queryTask.Result,
                    TotalRowCount = countTask.Result
                };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking
                    .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
                    || p.Ten.Contains(searchString) || p.TenTiengAnh.Contains(searchString)
                    || p.MoTa.Contains(searchString))
                    .Select(source => new ChuanDoanHinhAnhGridVo
                    {
                        Id = source.Id,
                        Ma = source.Ma,
                        Ten = source.Ten,
                        TenTiengAnh = source.TenTiengAnh,
                        LoaiChuanDoanHinhAnhDisplay = source.LoaiChuanDoanHinhAnh.GetDescription(),
                        HieuLuc = source.HieuLuc,
                        MoTa = source.MoTa
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.TenTiengAnh,
                        g => g.MoTa);

                var countTask = queryInfo.LazyLoadPage == true ?
                    Task.FromResult(0) :
                    query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();

                await Task.WhenAll(countTask, queryTask);

                return new GridDataSource
                {
                    Data = queryTask.Result,
                    TotalRowCount = countTask.Result
                };
            }
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var query = BaseRepository.TableNoTracking
                     .Select(source => new ChuanDoanHinhAnhGridVo
                     {
                         Id = source.Id,
                         Ma = source.Ma,
                         Ten = source.Ten,
                         TenTiengAnh = source.TenTiengAnh,
                         LoaiChuanDoanHinhAnhDisplay = source.LoaiChuanDoanHinhAnh.GetDescription(),
                         HieuLuc = source.HieuLuc,
                         MoTa = source.MoTa
                     }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.TenTiengAnh,
                        g => g.MoTa);

                var countTask = query.CountAsync();

                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking
                   .Where(p => p.Ten.Contains(searchString) || p.Ma.Contains(searchString)
                    || p.Ten.Contains(searchString) || p.TenTiengAnh.Contains(searchString)
                    || p.MoTa.Contains(searchString))
                     .Select(source => new ChuanDoanHinhAnhGridVo
                     {
                         Id = source.Id,
                         Ma = source.Ma,
                         Ten = source.Ten,
                         TenTiengAnh = source.TenTiengAnh,
                         LoaiChuanDoanHinhAnhDisplay = source.LoaiChuanDoanHinhAnh.GetDescription(),
                         HieuLuc = source.HieuLuc,
                         MoTa = source.MoTa
                     }).ApplyLike(queryInfo.SearchTerms,
                        g => g.Ma,
                        g => g.Ten,
                        g => g.TenTiengAnh,
                        g => g.MoTa);

                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) &&
                queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?
                    .Replace("Format", "");
            }
        }

        public async Task<bool> IsMaExists(string ma = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != id);
            }

            return result;
        }

        public List<LookupItemVo> GetListLoaiChuanDoanHinhAnh(LookupQueryInfo queryInfo)
        {
            var listEnumLoaiChuanDoanHinhAnh = EnumHelper.GetListEnum<Enums.EnumLoaiChuanDoanHinhAnh>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumLoaiChuanDoanHinhAnh = listEnumLoaiChuanDoanHinhAnh.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                      .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumLoaiChuanDoanHinhAnh;
        }
    }
}
