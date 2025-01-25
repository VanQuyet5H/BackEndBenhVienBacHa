using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamViTris;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.KhoDuocPhams;

namespace Camino.Services.KhoDuocPhamViTri
{
    [ScopedDependency(ServiceType = typeof(IKhoduocPhamViTriService))]
    public class KhoduocPhamViTriService : MasterFileService<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri>, IKhoduocPhamViTriService
    {
        private IRepository<Kho> _repositoryKhoDuocPham;
        public KhoduocPhamViTriService(IRepository<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri> repository, IRepository<Kho> repositoryKhoDuocPham) :
            base(repository)
        {
            _repositoryKhoDuocPham = repositoryKhoDuocPham;
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

            var query = BaseRepository.TableNoTracking.Select(s => new KhoDuocPhamViTriGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled,
                Kho = s.KhoDuocPham.Ten
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Kho);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new KhoDuocPhamViTriGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled,
                Kho = s.KhoDuocPham.Ten
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Kho);

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
        public Task<List<LookupItemVo>> GetListTenKhoDuocPham(DropDownListRequestModel model)
        {
            var list = _repositoryKhoDuocPham.TableNoTracking
                .Select(i => new LookupItemVo()
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                }).ApplyLike(model.Query, g => g.DisplayName).ToList();
            return Task.FromResult(list);
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
        public async Task<List<LookupItemVo>> GetListViTriKhoDuocPham(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking.ApplyLike(model.Query,g=>g.Ten).Where(p=>p.IsDisabled != true)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });

            return await list.ToListAsync();
        }
        public async Task<List<LookupItemViTriVo>> GetListDataViTri()
        {
            var list = BaseRepository.TableNoTracking.Where(p=>p.IsDisabled != true).Select(i => new LookupItemViTriVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id,
                    KhoDuocPhamId = i.KhoId
            });

            return await list.ToListAsync();
        }
    }
}
