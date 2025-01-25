using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        #region Grid
        public async Task<GridDataSource> GetDataForGridNoiDungMauKhamBenhAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var query = _noiDungMauKhamBenhRepository.TableNoTracking
                .Where(x => x.BacSiId == currentUserId)
                .Select(s => new NoiDungMauKhamBenhGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridNoiDungMauKhamBenhAsync(QueryInfo queryInfo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var query = _noiDungMauKhamBenhRepository.TableNoTracking
                .Where(x => x.BacSiId == currentUserId)
                .Select(s => new NoiDungMauKhamBenhGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region Get data

        public async Task<NoiDungMauKhamBenh> GetThongTinNoiDungMauKhamBenhAsync(long id, bool isEditData = false)
        {
            var noiDungMau = new NoiDungMauKhamBenh();
            if (!isEditData)
            {
                noiDungMau = await _noiDungMauKhamBenhRepository.TableNoTracking
                    .FirstAsync(x => x.Id == id);
            }
            else
            {
                noiDungMau = await _noiDungMauKhamBenhRepository.Table
                    .FirstAsync(x => x.Id == id);
            }

            if (noiDungMau == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            return noiDungMau;
        }

        #endregion

        #region Xử lý data

        public async Task XoaThongTinNoiDungMauKhamBenhAsync(long id)
        {
            var noiDungMau = await GetThongTinNoiDungMauKhamBenhAsync(id, true);
            noiDungMau.WillDelete = true;
            await _noiDungMauKhamBenhRepository.Context.SaveChangesAsync();
        }

        public async Task LuuThongTinNoiDungMauKhamBenhAsync(NoiDungMauKhamBenh noiDungMau)
        {
            // trường hợp tạo mới
            if (noiDungMau.Id == 0)
            {
                noiDungMau.BacSiId = _userAgentHelper.GetCurrentUserId();
                await _noiDungMauKhamBenhRepository.AddAsync(noiDungMau);
            }
            else
            {
                await _noiDungMauKhamBenhRepository.Context.SaveChangesAsync();
            }
            
        }
        #endregion
    }
}
