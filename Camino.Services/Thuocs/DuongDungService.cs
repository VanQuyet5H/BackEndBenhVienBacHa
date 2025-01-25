using System.Collections.Generic;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    [ScopedDependency(ServiceType = typeof(IDuongDungService))]
    public class DuongDungService : MasterFileService<DuongDung>, IDuongDungService
    {
        public DuongDungService(IRepository<DuongDung> repository) : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Select(s => new DuongDungGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    MoTa = s.MoTa,
                    IsDisabled = s.IsDisabled
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.MoTa);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DuongDungGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    MoTa = s.MoTa,
                    IsDisabled = s.IsDisabled
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.MoTa);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsTenExists(string tenDuongDung = null, long id = 0)
        {
            bool result;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenDuongDung));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenDuongDung) && p.Id != id);
            }

            if (result)
                return false;
            return true;
        }

        public async Task<bool> IsMaExists(string maDuongDung = null, long id = 0)
        {
            bool result;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maDuongDung));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maDuongDung) && p.Id != id);
            }

            if (result)
                return false;
            return true;
        }

        public async Task<List<DuongDungTemplateVo>> GetListDuongDung(DropDownListRequestModel model)
        {
            var listDuongDung = await BaseRepository.TableNoTracking
                .Where(p => p.IsDisabled != true && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listDuongDung.Select(item => new DuongDungTemplateVo
            {
                //DisplayName = item.Ma + " - " + item.Ten,
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return query;
        }
    }
}