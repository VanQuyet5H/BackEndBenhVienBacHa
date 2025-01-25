using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.VanBangChuyenMon;
using Microsoft.EntityFrameworkCore;
using Camino.Services.VanBangChuyenMon;
using System.Collections.Generic;

namespace Camino.Services.TrinhDoChuyenMon
{
    [ScopedDependency(ServiceType = typeof(IVanBangChuyenMonService))]
    public class VanBangChuyenMonService : MasterFileService<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon>,IVanBangChuyenMonService
    {
        public VanBangChuyenMonService(IRepository<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new VanBangChuyenMonGridVo
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.TenVietTat, p => p.MoTa);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new VanBangChuyenMonGridVo
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms, p=>p.Ten,p=>p.TenVietTat, p => p.MoTa);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public bool CheckTrinhDoChuyenMonExits(string ten, long id, bool isTen)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(ten) || id != 0)
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => (isTen ? x.Ten == tenCheck.Trim() : x.TenVietTat == tenCheck.Trim()) && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => isTen ? x.Ten == tenCheck.Trim() : x.TenVietTat == tenCheck.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }

        public Task<List<LookupItemVo>> GetListVanBangChuyenMon()
        {
            var list = BaseRepository.TableNoTracking
               .Select(i => new LookupItemVo()
               {
                   DisplayName = i.Ten,
                   KeyId = i.Id
               }).ToList();
            return Task.FromResult(list);
        }
        public async Task<List<LookupItemTemplateVo>> GetListVanBangChuyenMon(DropDownListRequestModel model)
        {
            //var list = await BaseRepository.TableNoTracking
            //   .Where(p => p.TenVietTat.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? "") && p.IsDisabled == false)
            //   .Take(model.Take)
            //   .ToListAsync();

            var list = await BaseRepository.TableNoTracking
             .Where(p => p.IsDisabled != true)
             .ApplyLike(model.Query, g => g.TenVietTat, g => g.Ten)
                .Take(model.Take)
             .ToListAsync();
            var query = list.Select(item => new LookupItemTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.TenVietTat
            }).ToList();
            return query;
        }

        public async Task<bool> KiemTraMaTonTai(long id, string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return false;
            var vanBang = await BaseRepository.TableNoTracking.AnyAsync(x => (id == 0 || x.Id != id) && x.Ma == ma.Trim().ToLower());
            return vanBang;
        }
    }
}
