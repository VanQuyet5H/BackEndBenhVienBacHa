using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhuongPhapVoCams;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.PhuongPhapVoCam
{
    [ScopedDependency(ServiceType = typeof(IPhuongPhapVoCamService))]
    public class PhuongPhapVoCamService : MasterFileService<Core.Domain.Entities.PhuongPhapVoCams.PhuongPhapVoCam>, IPhuongPhapVoCamService
    {
        public PhuongPhapVoCamService
            (IRepository<Core.Domain.Entities.PhuongPhapVoCams.PhuongPhapVoCam> repository) : base(repository)
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
            var query = BaseRepository.TableNoTracking.Select(s => new PhuongPhapVoCamGridVo
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
            var query = BaseRepository.TableNoTracking.Select(s => new PhuongPhapVoCamGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.MoTa,
                g => g.Ma);

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
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

        public async Task<PhuongPhapVoCamDataResult> DeletePhuongPhapVoCamMultiAsync(long[] modelIds)
        {
            var phuongPhapVoCams = await BaseRepository.GetByIdsAsync(modelIds);

            if (phuongPhapVoCams == null)
            {
                return new PhuongPhapVoCamDataResult
                {
                    Error = false,
                    ErrorType = 1 // not found
                };
            }

            var listPhuongPhapVoCam = phuongPhapVoCams.ToList();

            if (listPhuongPhapVoCam.Count != modelIds.Length)
            {
                return new PhuongPhapVoCamDataResult
                {
                    Error = false,
                    ErrorType = 2 // Common.WrongLengthMultiDelete
                };
            }

            await BaseRepository.DeleteAsync(listPhuongPhapVoCam);

            return new PhuongPhapVoCamDataResult
            {
                Error = true,
                ErrorType = 3 // success
            };
        }

        public async Task<bool> IsMaExists(string ma, long id)
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
    }
}
