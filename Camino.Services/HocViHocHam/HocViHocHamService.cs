using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.HocViHocHam;
using System.Collections.Generic;

namespace Camino.Services.HocViHocHam
{
    [ScopedDependency(ServiceType = typeof(IHocViHocHamService))]
    public class HocViHocHamService : MasterFileService<Core.Domain.Entities.HocViHocHams.HocViHocHam>, IHocViHocHamService
    {
        public HocViHocHamService(IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new HocViHocHamGripVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.Ma, p => p.MoTa);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new HocViHocHamGripVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.Ma, p => p.MoTa);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public bool CheckTenExits(string ten, long id)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(ten) || id != 0)
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ten == tenCheck.Trim() && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ten == tenCheck.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }

        public bool CheckMaSoExits(string maso, long id)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(maso) || id != 0)
            {
                string result = !string.IsNullOrEmpty(maso) ? maso.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ma == result.Trim() && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string result = !string.IsNullOrEmpty(maso) ? maso.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ma == result.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }

        public Task<List<LookupItemVo>> GetListHocViHocHam()
        {
            var list = BaseRepository.TableNoTracking
               .Select(i => new LookupItemVo()
               {
                   DisplayName = i.Ten,
                   KeyId = i.Id
               }).ToListAsync();
            return list;
        }

        public async Task<List<LookupItemTemplateVo>> GetListHocViHocHam(DropDownListRequestModel model)
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
