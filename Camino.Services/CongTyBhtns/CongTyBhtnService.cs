using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CongTyBhtns;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.CongTyBhtns
{
    [ScopedDependency(ServiceType = typeof(ICongTyBhtnService))]
    public class CongTyBhtnService : MasterFileService<CongTyBaoHiemTuNhan>, ICongTyBhtnService
    {
        public CongTyBhtnService
            (IRepository<CongTyBaoHiemTuNhan> repository) : base(repository)
        { }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new CongTyBhtnGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                GhiChu = s.GhiChu,
                DiaChi = s.DiaChi,
                Email = s.Email,
                HinhThucBaoHiem = s.HinhThucBaoHiem,
                PhamViBaoHiem = s.PhamViBaoHiem,
                SoDienThoai = s.SoDienThoai,
                MaSoThue = s.MaSoThue,
                DonVi = s.DonVi
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma,
                g => g.DiaChi,
                g => g.Email,
                g => g.SoDienThoai,
                g => g.MaSoThue,
                g => g.DonVi);

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
            var query = BaseRepository.TableNoTracking.Select(s => new CongTyBhtnGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ma = s.Ma,
                DiaChi = s.DiaChi,
                Email = s.Email,
                SoDienThoai = s.SoDienThoai,
                MaSoThue = s.MaSoThue,
                DonVi = s.DonVi
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma,
                g => g.DiaChi,
                g => g.Email,
                g => g.SoDienThoai,
                g => g.MaSoThue,
                g => g.DonVi);
            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<bool> IsMaExists(string ma = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != id);
            }

            return result;
        }

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }

            return result;
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

        public List<LookupItemVo> GetHinhThucBaoHiem()
        {
            var result = EnumHelper.GetListEnum<Enums.EnumHinhThucBaoHiem>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            return result;
        }

        public List<LookupItemVo> GetPhamViBaoHiem()
        {
            var result = EnumHelper.GetListEnum<Enums.EnumPhamViBaoHiem>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            return result;
        }
    }
}
