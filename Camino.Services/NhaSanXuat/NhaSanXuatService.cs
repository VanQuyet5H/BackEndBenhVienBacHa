using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.NhaSanXuat;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;

namespace Camino.Services.NhaSanXuat
{
    [ScopedDependency(ServiceType = typeof(INhaSanXuatService))]
    public class NhaSanXuatService : MasterFileService<Core.Domain.Entities.NhaSanXuats.NhaSanXuat>, INhaSanXuatService
    {
        IRepository<Core.Domain.Entities.QuocGias.QuocGia> _quocGiaRepository;
        public NhaSanXuatService(IRepository<Core.Domain.Entities.NhaSanXuats.NhaSanXuat> repository,
             IRepository<Core.Domain.Entities.QuocGias.QuocGia> quocGiaRepository) : base(repository)
        {
            _quocGiaRepository = quocGiaRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo , bool isPrint)
        {
            //
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new NhaSanXuatGridVo()
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isPrint == true ? query.OrderBy(queryInfo.SortString).ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new NhaSanXuatGridVo()
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);


            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) && queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?.Replace("Format", "") ?? "";
            }
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
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != id);
            }

            if (result)
                return false;
            return true;
        }
        public async Task<List<ChucDanhItemVo>> GetListTenQuocGia(DropDownListRequestModel model)
        {
            var lstQuocGia = await _quocGiaRepository.TableNoTracking.Where(x=>x.IsDisabled != true)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();
            var query = lstQuocGia.Select(item => new ChucDanhItemVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();
            return query;
        }
      
    }
}
