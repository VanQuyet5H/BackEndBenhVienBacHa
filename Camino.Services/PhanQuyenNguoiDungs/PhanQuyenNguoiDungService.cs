using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhanQuyenNguoiDungs;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.PhanQuyenNguoiDungs
{
    [ScopedDependency(ServiceType = typeof(IPhanQuyenNguoiDungService))]
    public class PhanQuyenNguoiDungService : MasterFileService<Role>
        , IPhanQuyenNguoiDungService
    {
        public PhanQuyenNguoiDungService
            (IRepository<Role> repository)
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
            var query = BaseRepository.TableNoTracking.Select(s => new PhanQuyenNguoiDungGridVo
            {
                Id = s.Id,
                Ten = s.Name,
                LoaiNguoiDung = s.UserType.GetDescription(),
                Quyen = s.IsDefault
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten);

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
            var query = BaseRepository.TableNoTracking.Select(s => new PhanQuyenNguoiDungGridVo
            {
                Id = s.Id,
                Ten = s.Name,
                LoaiNguoiDung = s.UserType.GetDescription(),
                Quyen = s.IsDefault
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten);

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

        public List<LookupItemVo> GetListDocumentType()
        {
            var listEnumDocumentType = EnumHelper.GetListEnum<Enums.DocumentType>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                })
                .Where(p => p.KeyId != 0).ToList();

            return listEnumDocumentType;
        }

        public List<LookupItemVo> GetListUserType(LookupQueryInfo model)
        {
            var listUserType = EnumHelper.GetListEnum<Enums.UserType>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            return listUserType;
        }

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Name.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Name.Equals(ten) && p.Id != id);
            }

            return result;
        }

        public async Task<bool> AddRoleAsync(Role phanQuyenEntity)
        {
            BaseRepository.AutoCommitEnabled = false;
            await BaseRepository.AddAsync(phanQuyenEntity);
            await BaseRepository.Context.SaveChangesAsync();
            return true;
        }
    }
}
