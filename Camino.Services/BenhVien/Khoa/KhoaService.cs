using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BenhVien.Khoa;

namespace Camino.Services.BenhVien.Khoa
{
    [ScopedDependency(ServiceType = typeof(IKhoaService))]
    public class KhoaService
         : MasterFileService<Core.Domain.Entities.BenhVien.Khoas.Khoa>,
        IKhoaService
    {
        public KhoaService
            (IRepository<Core.Domain.Entities.BenhVien.Khoas.Khoa> repository)
            : base(repository)
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
            var query = BaseRepository.TableNoTracking.Select(s => new KhoaGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.MoTa,
                g => g.Ma);

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

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var result = BaseRepository.TableNoTracking.Select(s => new KhoaGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.MoTa,
                g => g.Ma);

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
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

        public async Task<List<KhoaTemplateVo>> GetListKhoa(DropDownListRequestModel model)
        {
            var listKhoa = await BaseRepository.TableNoTracking
                .Where(p => p.IsDisabled != true)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoa.Select(item => new KhoaTemplateVo
            {
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return query;
        }
    }
}
