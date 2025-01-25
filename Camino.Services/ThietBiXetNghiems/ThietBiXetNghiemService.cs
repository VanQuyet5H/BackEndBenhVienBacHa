using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ThietBiXetNghiems;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.ThietBiXetNghiems
{
    [ScopedDependency(ServiceType = typeof(IThietBiXetNghiemService))]
    public class ThietBiXetNghiemService : MasterFileService<MayXetNghiem>, IThietBiXetNghiemService
    {
        public static readonly string MAYXETNGHIEMS_PATTERN_KEY = "Camino.MayXetNghiems.";
        private readonly IRepository<MauMayXetNghiem> _mauMayXn;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDvBv;

        public ThietBiXetNghiemService(
            IRepository<MayXetNghiem> repository,
            IRepository<MauMayXetNghiem> mauMayXn,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDvBv
        ) : base(repository)
        {
            _mauMayXn = mauMayXn;
            _nhomDvBv = nhomDvBv;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new ThietBiXetNghiemGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ncc = s.NhaCungCap,
                Ma = s.Ma,
                NhomThietBiDisplay = s.MauMayXetNghiem.Ten,
                NhomXetNghiemDisplay = s.MauMayXetNghiem.NhomDichVuBenhVien.Ten,
                TinhTrang = s.ConnectionStatus ?? Enums.EnumConnectionStatus.Close,
                HieuLuc = s.HieuLuc
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma,
                g => g.Ncc,
                g => g.NhomThietBiDisplay);

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
            var result = BaseRepository.TableNoTracking.Select(s => new ThietBiXetNghiemGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                Ncc = s.NhaCungCap,
                Ma = s.Ma,
                NhomThietBiDisplay = s.MauMayXetNghiem.Ten,
                NhomXetNghiemDisplay = s.MauMayXetNghiem.NhomDichVuBenhVien.Ten,
                TinhTrang = s.ConnectionStatus ?? Enums.EnumConnectionStatus.Close,
                HieuLuc = s.HieuLuc
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.Ma,
                g => g.Ncc,
                g => g.NhomThietBiDisplay);

            var countTask = result.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
        }

        public async Task<List<MayXetNghiem>> GetTatCaMayXetNghiemAsync()
        {
            var mayXetNghiems = await BaseRepository.TableNoTracking.Include(o => o.MauMayXetNghiem).ToListAsync();
            return mayXetNghiems;
        }

        public async Task<List<MauMayXetNghiem>> GetListNhomThietBi(long id, DropDownListRequestModel model = null)
        {
            var modelEntity = _mauMayXn.TableNoTracking;
            if (model != null)
            {
                modelEntity = modelEntity.Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.NhaSanXuat.Contains(model.Query ?? ""));
            }
            if (id != 0)
            {
                modelEntity = modelEntity.Where(x => x.NhomDichVuBenhVienId == id);
            }
            var result = await modelEntity.Take(50).ToListAsync();
            return result;
        }

        public async Task<List<LookupItemTemplateVo>> GetNhomXetNghiem(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstNhomXetNghiemQuery = _nhomDvBv.TableNoTracking
                    .Where(w => w.NhomDichVuBenhVienChaId == 2)
                    .Select(item => new LookupItemTemplateVo
                    {
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.Ten,
                        DisplayName = item.Ten
                    })
                    .ApplyLike(model.Query, w => w.Ten, w => w.Ma);

                var lstNhomXetNghiemEnumerable = lstNhomXetNghiemQuery
                    .Take(model.Take).ToListAsync();

                return await lstNhomXetNghiemEnumerable;
            }

            var lstColumnNameSearch = new List<string> { "Ma", "Ten" };

            var lstId = _nhomDvBv
                .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien), lstColumnNameSearch)
                .Select(item => item.Id)
                .ToList();

            var dct = lstId.Select((p, i) => new
            {
                key = p,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);

            var lst = _nhomDvBv.TableNoTracking
                .Where(p => lstId.Contains(p.Id));
            var query = await lst
                .Where(w => w.NhomDichVuBenhVienChaId == 2)
                .Select(item => new LookupItemTemplateVo
                {
                    KeyId = item.Id,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DisplayName = item.Ten
                }).OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                .Take(model.Take)
                .ToListAsync();

            return query;
        }

        public async Task<bool> IsTenExists(long nhomThietBiId, string ten, long nhomXetNghiemId, long id = 0, bool isCopy = false)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) &&
                                   p.MauMayXetNghiem.NhomDichVuBenhVienId == nhomXetNghiemId && p.MauMayXetNghiemID == nhomThietBiId);
            }
            else
            {
                if (isCopy)
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) &&
                                                                                p.MauMayXetNghiem.NhomDichVuBenhVienId == nhomXetNghiemId && p.MauMayXetNghiemID == nhomThietBiId);
                }
                else
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) &&
                                                                                p.MauMayXetNghiem.NhomDichVuBenhVienId == nhomXetNghiemId && p.MauMayXetNghiemID == nhomThietBiId && p.Id != id);
                }
            }

            return result;
        }

        public async Task<bool> IsMaExists(long nhomThietBiId, string ma, long nhomXetNghiemId, long id = 0, bool isCopy = false)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) &&
                                                                            p.MauMayXetNghiem.NhomDichVuBenhVienId == nhomXetNghiemId && p.MauMayXetNghiemID == nhomThietBiId);
            }
            else
            {
                if (isCopy)
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) &&
                                                                                p.MauMayXetNghiem.NhomDichVuBenhVienId == nhomXetNghiemId && p.MauMayXetNghiemID == nhomThietBiId);
                }
                else
                {
                    result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) &&
                                                                                p.MauMayXetNghiem.NhomDichVuBenhVienId == nhomXetNghiemId && p.MauMayXetNghiemID == nhomThietBiId && p.Id != id);
                }
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

        #region lookup
        public async Task<List<LookupItemTemplateVo>> GetListMayXetNghiemAsync(DropDownListRequestModel model)
        {
            var lst = await
                BaseRepository.TableNoTracking
                    .Where(x => x.HieuLuc)
                    .Select(item => new LookupItemTemplateVo()
                    {
                        DisplayName = item.Ten,
                        Ma = item.Ma,
                        Ten = item.Ten,
                        KeyId = item.Id
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                    .Take(model.Take)
                    .ToListAsync();
            return lst;
        }
        #endregion
    }
}
