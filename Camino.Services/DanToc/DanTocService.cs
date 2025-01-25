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
using Camino.Core.Domain.ValueObject.NhanVien;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.DanToc;

namespace Camino.Services.DanToc
{
    [ScopedDependency(ServiceType = typeof(IDanTocService))]
    public class DanTocService : MasterFileService<Core.Domain.Entities.DanTocs.DanToc>, IDanTocService
    {
        IRepository<Core.Domain.Entities.QuocGias.QuocGia> _quocGiaRepo;
        public DanTocService(IRepository<Core.Domain.Entities.DanTocs.DanToc> repository, IRepository<Core.Domain.Entities.QuocGias.QuocGia> quocGiaRepo) : base(repository)
        {
            _quocGiaRepo = quocGiaRepo;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            //
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Include(p=>p.QuocGia).Select(s => new DanTocGridVo()
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
                QuocGiaId = s.QuocGiaId,
                TenQuocGia = s.QuocGia.Ten,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenQuocGia);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Include(p => p.QuocGia).Select(s => new DanTocGridVo()
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
                QuocGiaId = s.QuocGiaId,
                TenQuocGia = s.QuocGia.Ten,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenQuocGia);

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
        public async Task<List<LookupItemVo>> GetListQuocGia(DropDownListRequestModel model)
        {
            var lstQuocGia = await _quocGiaRepo.TableNoTracking.Where(x=>x.IsDisabled == false)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = lstQuocGia.Select(item => new LookupItemVo()
            { 
                DisplayName = item.Ten,
                KeyId = item.Id,
               
            }).ToList();
            return query;
        }
        public async Task<bool> IsTenExists(string ma = null, long Id = 0)
        {
            bool result;

            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ma) && p.Id != Id);
            }

            return result;
        }
        public async Task<bool> IsMaExists(string ma = null, long Id = 0)
        {
            bool result;

            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != Id);
            }

            return result;
        }

    }
}
