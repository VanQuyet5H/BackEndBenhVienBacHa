using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.ChiSoXetNghiems;
using System;
using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Camino.Core.Helpers;

namespace Camino.Services.ChiSoXetNghiems
{
    [ScopedDependency(ServiceType = typeof(IChiSoXetNghiemService))]
    public class ChiSoXetNghiemService : MasterFileService<ChiSoXetNghiem>, IChiSoXetNghiemService
    {
        public ChiSoXetNghiemService(IRepository<ChiSoXetNghiem> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new ChiSoXetNghiemGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa,
                TenTiengAnh = s.TenTiengAnh,
                ChiSoBinhThuongNu = s.ChiSoBinhThuongNu,
                ChiSoBinhThuongNam = s.ChiSoBinhThuongNam,
                TenLoaiXetNghiem = s.LoaiXetNghiem.GetDescription(),
                HieuLuc = s.HieuLuc
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.Ma, p => p.MoTa,x =>x.TenTiengAnh,x=>x.ChiSoBinhThuongNam, x => x.ChiSoBinhThuongNu);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Select(s => new ChiSoXetNghiemGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                MoTa = s.MoTa,
                TenTiengAnh = s.TenTiengAnh,
                ChiSoBinhThuongNu = s.ChiSoBinhThuongNu,
                ChiSoBinhThuongNam = s.ChiSoBinhThuongNam,
                TenLoaiXetNghiem = s.LoaiXetNghiem.GetDescription(),
                HieuLuc = s.HieuLuc
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.Ma, p => p.MoTa, 
                                                x => x.TenTiengAnh, 
                                                x => x.ChiSoBinhThuongNam, 
                                                x => x.ChiSoBinhThuongNu);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
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

        public bool CheckTenExits(string ten, long id)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(ten) || id != 0)
            {
                string result = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ten == result.Trim() && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string result = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ten == result.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }

    }
}
