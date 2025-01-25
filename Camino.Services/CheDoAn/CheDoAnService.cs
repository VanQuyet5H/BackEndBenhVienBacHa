using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.CheDoAn;
using Camino.Core.Domain.ValueObject.NhanVien;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;

namespace Camino.Services.CheDoAn
{
    [ScopedDependency(ServiceType = typeof(ICheDoAnService))]
    public class CheDoAnService : MasterFileService<Core.Domain.Entities.CheDoAns.CheDoAn>, ICheDoAnService
    {
        public CheDoAnService(IRepository<Core.Domain.Entities.CheDoAns.CheDoAn> repository) : base(repository)
        {
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            //
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new CheDoAnGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                KyHieu = s.KyHieu,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.KyHieu);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource {Data = queryTask.Result, TotalRowCount = countTask.Result};
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new CheDoAnGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                KyHieu = s.KyHieu,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.KyHieu);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource {TotalRowCount = countTask.Result};
        }

        public async Task<ICollection<LookupItemTemplateVo>> GetLookupAsync(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten)
                .Select(i => new LookupItemTemplateVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id,
                    Ma = i.KyHieu
                });
            return await list.ToListAsync();
        }
        public async Task<bool> IsTenExists(string ten, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }
        public async Task<bool> IsMaExists(string ma, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.KyHieu.Equals(ma));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.KyHieu.Equals(ma) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }
    }
}
