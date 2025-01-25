using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.NhaThau
{
    [ScopedDependency(ServiceType = typeof(INhaThauService))]
    public class NhaThauService : MasterFileService<Core.Domain.Entities.NhaThaus.NhaThau>, INhaThauService
    {
        public NhaThauService(IRepository<Core.Domain.Entities.NhaThaus.NhaThau> repository) : base(repository)
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
            var query = BaseRepository.TableNoTracking
                .Select(source => new NhaThauGridVo
                {
                    Id = source.Id,
                    Ten = source.Ten,
                    DiaChi = source.DiaChi,
                    MaSoThue = source.MaSoThue,
                    TaiKhoanNganHang = source.TaiKhoanNganHang,
                    NguoiDaiDien = source.NguoiDaiDien,
                    NguoiLienHe = source.NguoiLienHe,
                    SoDienThoaiLienHe = source.SoDienThoaiLienHe,
                    SoDienThoaiDisplay = source.SoDienThoaiDisplay,
                    EmailLienHe = source.EmailLienHe
                }).ApplyLike(queryInfo.SearchTerms,
                    g => g.Ten,
                    g => g.DiaChi,
                    g => g.MaSoThue,
                    g => g.TaiKhoanNganHang,
                    g => g.NguoiDaiDien,
                    g => g.NguoiLienHe,
                    g => g.SoDienThoaiLienHe,
                    g => g.SoDienThoaiDisplay,
                    g => g.EmailLienHe);

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
            var query = BaseRepository.TableNoTracking
                .Select(source => new NhaThauGridVo
                {
                    Id = source.Id,
                    Ten = source.Ten,
                    DiaChi = source.DiaChi,
                    MaSoThue = source.MaSoThue,
                    TaiKhoanNganHang = source.TaiKhoanNganHang,
                    NguoiDaiDien = source.NguoiDaiDien,
                    NguoiLienHe = source.NguoiLienHe,
                    SoDienThoaiLienHe = source.SoDienThoaiLienHe,
                    SoDienThoaiDisplay = source.SoDienThoaiDisplay,
                    EmailLienHe = source.EmailLienHe
                }).ApplyLike(queryInfo.SearchTerms,
                    g => g.Ten,
                    g => g.DiaChi,
                    g => g.MaSoThue,
                    g => g.TaiKhoanNganHang,
                    g => g.NguoiDaiDien,
                    g => g.NguoiLienHe,
                    g => g.SoDienThoaiLienHe,
                    g => g.SoDienThoaiDisplay,
                    g => g.EmailLienHe);

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
    }
}
