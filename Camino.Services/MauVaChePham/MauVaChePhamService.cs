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
using Camino.Core.Domain.ValueObject.MauVaChePham;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using Camino.Core.Helpers;


namespace Camino.Services.MauVaChePham
{
    [ScopedDependency(ServiceType = typeof(IMauVaChePhamService))]
    public class MauVaChePhamService : MasterFileService<Core.Domain.Entities.MauVaChePhams.MauVaChePham>, IMauVaChePhamService
    {
        public MauVaChePhamService(IRepository<Core.Domain.Entities.MauVaChePhams.MauVaChePham> repository) : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo,bool isPrint)
        {
            //
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new MauVaChePhamGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                TheTichs = s.TheTich.ApplyNumber(),
                GiaTriToiDas = s.GiaTriToiDa.ApplyFormatMoneyVND(),
                GhiChu =  s.GhiChu
            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isPrint== true ? query.OrderBy(queryInfo.SortString).ToArrayAsync() :query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new MauVaChePhamGridVo()
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                TheTichs = s.TheTich.ApplyNumber(),
                GiaTriToiDas = s.GiaTriToiDa.ApplyFormatMoneyVND(),
                GhiChu = s.GhiChu
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
        public List<LookupItemVo> GetListPhanLoaiMau(LookupQueryInfo queryInfo)
        {
            var listEnumPhanLoaiMau = EnumHelper.GetListEnum<Enums.PhanLoaiMau>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumPhanLoaiMau = listEnumPhanLoaiMau.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                                         .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumPhanLoaiMau;
        }
    }
}
