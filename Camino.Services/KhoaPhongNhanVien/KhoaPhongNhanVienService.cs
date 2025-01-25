using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoaPhongNhanVien;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KhoaPhongNhanVien
{
    [ScopedDependency(ServiceType = typeof(IKhoaPhongNhanVienService))]
    public class KhoaPhongNhanVienService
        : MasterFileService<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien>
            , IKhoaPhongNhanVienService
    {
        public KhoaPhongNhanVienService
            (IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> repository)
            : base(repository)
        { }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Include(p => p.NhanVien).ThenInclude(p => p.User)
                    .Select(s => new KhoaPhongNhanVienGridVo
                    {
                        Id = s.Id,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        TenNhanVien = s.NhanVien.User.HoTen
                    }).ApplyLike(queryInfo.SearchTerms,
                g => g.TenKhoaPhong,
                g => g.TenNhanVien);

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
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Include(p => p.NhanVien).ThenInclude(p => p.User)
                    .Where(p => p.KhoaPhong.Ten.Contains(searchString)
                                || p.NhanVien.User.HoTen.Contains(searchString))
                    .Select(s => new KhoaPhongNhanVienGridVo
                    {
                        Id = s.Id,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        TenNhanVien = s.NhanVien.User.HoTen
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.TenKhoaPhong,
                        g => g.TenNhanVien);

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
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var result = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Include(p => p.NhanVien).ThenInclude(p => p.User)
                    .Select(s => new KhoaPhongNhanVienGridVo
                    {
                        Id = s.Id,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        TenNhanVien = s.NhanVien.User.HoTen
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.TenKhoaPhong,
                        g => g.TenNhanVien);

                var countTask = result.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            else
            {
                var searchString = queryInfo.SearchTerms.Trim();

                var query = BaseRepository.TableNoTracking.Include(p => p.KhoaPhong)
                    .Include(p => p.NhanVien).ThenInclude(p => p.User)
                    .Where(p => p.KhoaPhong.Ten.Contains(searchString)
                                || p.NhanVien.User.HoTen.Contains(searchString))
                    .Select(s => new KhoaPhongNhanVienGridVo
                    {
                        Id = s.Id,
                        TenKhoaPhong = s.KhoaPhong.Ten,
                        TenNhanVien = s.NhanVien.User.HoTen
                    }).ApplyLike(queryInfo.SearchTerms,
                        g => g.TenKhoaPhong,
                        g => g.TenNhanVien);

                var countTask = query.CountAsync();

                await Task.WhenAll(countTask);

                return new GridDataSource
                {
                    TotalRowCount = countTask.Result
                };
            }
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

        public async Task<bool> CheckExistAsync(long khoaPhongId, long nhanVienId, long id)
        {
            if (id != 0)
            {
                return await BaseRepository.Table.FirstOrDefaultAsync(o => o.KhoaPhong.Id == khoaPhongId && o.NhanVien.Id == nhanVienId && o.Id != id) != null;
            }

            return await BaseRepository.Table.FirstOrDefaultAsync(o => o.KhoaPhong.Id == khoaPhongId && o.NhanVien.Id == nhanVienId) != null;
        }

        public async Task<bool> CheckNhanVienExistAsync(long khoaPhongId, long[] nhanVienIds, long[] input)
        {
            if (nhanVienIds != null && nhanVienIds.Length != 0)
            {
                var queryforNhanViens = await BaseRepository.TableNoTracking
                    .Where(x => x.KhoaPhongId == khoaPhongId)
                    .Select(p => p.NhanVienId).ToListAsync();

                if (queryforNhanViens.Any())
                {
                    foreach (var queryNhanVien in queryforNhanViens)
                    {
                        foreach (var nhanVien in nhanVienIds)
                        {
                            if (nhanVien == queryNhanVien)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
