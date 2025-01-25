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

namespace Camino.Services.ChucDanh
{
    [ScopedDependency(ServiceType = typeof(IChucDanhService))]
    public class ChucDanhService : MasterFileService<Core.Domain.Entities.ChucDanhs.ChucDanh>, IChucDanhService
    {
        IRepository<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh> _nhomChucDanhRepository;
        public ChucDanhService(IRepository<Core.Domain.Entities.ChucDanhs.ChucDanh> repository , IRepository<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh> nhomChucDanhRepository) : base(repository)
        {
            _nhomChucDanhRepository = nhomChucDanhRepository;
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

            var query = BaseRepository.TableNoTracking.Include(p=> p.NhomChucDanh).Select(s => new ChucDanhGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                HideCheckbox = s.IsDefault,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled,
                IsDefault = s.IsDefault,
                TenNhomChucDanh = s.NhomChucDanh.Ten
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma,g=>g.TenNhomChucDanh);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource {Data = queryTask.Result, TotalRowCount = countTask.Result};
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Include(p => p.NhomChucDanh).Select(s => new ChucDanhGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                //NhomChucDanhId = s.NhomChucDanhId,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled,
                IsDefault = s.IsDefault,
                TenNhomChucDanh = s.NhomChucDanh.Ten
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenNhomChucDanh);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource {TotalRowCount = countTask.Result};
        }

        public async Task<ICollection<LookupItemVo>> GetLookupAsync()
        {
            var list = await BaseRepository.TableNoTracking
                .Select(o => new LookupItemVo {DisplayName = o.Ten, KeyId = o.Id}).ToListAsync();
            return list;
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) && queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?.Replace("Format", "") ?? "";
            }
        }

        public async Task<bool> IsTenExists(string ten ,long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id !=id);
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

        public Task<List<LookupItemVo>> GetListChucDanh()
        {
            var list = BaseRepository.TableNoTracking
               .Select(i => new LookupItemVo()
               {
                   DisplayName = i.Ten,
                   KeyId = i.Id
               }).ToList();
            return Task.FromResult(list);
        }

        public async Task<List<ChucDanhItemVo>> GetListNhomChucDanh(DropDownListRequestModel model)
        {
            var lstChucDanh = await _nhomChucDanhRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = lstChucDanh.Select(item => new ChucDanhItemVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();
            return query;
        }

        public async Task<List<LookupItemTemplateVo>> GetListChucDanh(DropDownListRequestModel model)
        {
            //var list = await BaseRepository.TableNoTracking
            //   .Where(p => (p.Ma.Contains(model.Query ?? "") && p.IsDisabled == false) || (p.Ten.Contains(model.Query ?? "") && p.IsDisabled == false))
            //   .Take(model.Take).ApplyLike(model.Query, g => g.Ma,  g => g.Ten)
            //   .ToListAsync();

            var list = await BaseRepository.TableNoTracking
              .Where(p => p.IsDisabled != true)
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
