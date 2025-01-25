using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhamViHanhNghe;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.PhamViHanhNghe
{
    [ScopedDependency(ServiceType = typeof(IPhamViHanhNgheService))]
    public class PhamViHanhNgheService
        : MasterFileService<Core.Domain.Entities.PhamViHanhNghes.PhamViHanhNghe>
        , IPhamViHanhNgheService
    {
        public PhamViHanhNgheService
            (IRepository<Core.Domain.Entities.PhamViHanhNghes.PhamViHanhNghe> repository)
            : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            var query = BaseRepository.TableNoTracking.Select(s => new PhamViHanhNgheGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa
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
            var result = BaseRepository.TableNoTracking.Select(s => new PhamViHanhNgheGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa
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

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }

            return result;
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

        
        public async Task<List<LookupItemTemplateVo>> GetListPhamViHanhNghe(DropDownListRequestModel model)
        {
            var list = await BaseRepository.TableNoTracking
               .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
               .ToListAsync();

            var query = list.Select(item => new LookupItemTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return query;
        }
    }
}
