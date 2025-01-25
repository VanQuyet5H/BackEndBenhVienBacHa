using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho;
using Camino.Core.Domain.ValueObject.DinhMucVatTuTonKho;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.DinhMucVatTuTonKhos
{
    [ScopedDependency(ServiceType = typeof(IDinhMucVatTuTonKhoService))]
    public class DinhMucVatTuTonKhoService : MasterFileService<DinhMucVatTuTonKho>, IDinhMucVatTuTonKhoService
    {
        private readonly IRepository<Kho> _kho;
        private readonly IRepository<VatTuBenhVien> _vtbv;

        public DinhMucVatTuTonKhoService(IRepository<DinhMucVatTuTonKho> repository,
            IRepository<Kho> kho,
            IRepository<VatTuBenhVien> vtbv
        ) : base(repository)
        {
            _kho = kho;
            _vtbv = vtbv;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Select(s => new DinhMucVatTuTonKhoGridVo
                {
                    Id = s.Id,
                    Kho = s.Kho.Ten,
                    TenVt = s.VatTuBenhVien.VatTus.Ten,
                    MoTa = s.MoTa,
                    TonToiThieu = s.TonToiThieu,
                    TonToiDa = s.TonToiDa,
                    SoNgayTruocKhiHetHan = s.SoNgayTruocKhiHetHan
                }).ApplyLike(queryInfo.SearchTerms, w => w.Kho, w => w.TenVt, w => w.MoTa);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryEnumerable = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryEnumerable);
            return new GridDataSource { Data = queryEnumerable.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DinhMucVatTuTonKhoGridVo
                {
                    Id = s.Id,
                    Kho = s.Kho.Ten,
                    TenVt = s.VatTuBenhVien.VatTus.Ten,
                    MoTa = s.MoTa,
                    TonToiThieu = s.TonToiThieu,
                    TonToiDa = s.TonToiDa,
                    SoNgayTruocKhiHetHan = s.SoNgayTruocKhiHetHan
                }).ApplyLike(queryInfo.SearchTerms, w => w.Kho, w => w.TenVt, w => w.MoTa);
            var count = await query.CountAsync();
            return new GridDataSource { TotalRowCount = count };
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

        public async Task<List<LookupItemVo>> GetListKhoAsync(DropDownListRequestModel queryInfo)
        {
            var result = _kho.TableNoTracking
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).Distinct().ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take).ToListAsync();
            return await result;
        }

        public async Task<List<VatTuDinhMucVo>> GetListVatTu(DropDownListRequestModel queryInfo)
        {
            var lst = _vtbv.TableNoTracking
                .Where(o => o.HieuLuc)
                .Select(item => new VatTuDinhMucVo
                {
                    DisplayName = item.VatTus.Ten,
                    Ten = item.VatTus.Ten,
                    Ma = item.Ma,
                    NhaSanXuat = item.VatTus.NhaSanXuat,
                    KeyId = item.Id,
                })
                .ApplyLike(queryInfo.Query, w => w.Ten, w => w.Ma, w => w.NhaSanXuat);
            return await lst.ToListAsync();
        }

        public async Task<bool> IsTenVatTuExists(long vatTuId = 0, long id = 0, long khoId = 0)
        {
            if (id == 0)
            {
                return await BaseRepository.TableNoTracking.AnyAsync(p => p.VatTuBenhVienId == vatTuId && p.KhoId == khoId);
            }

            return await BaseRepository.TableNoTracking.AnyAsync(p => p.VatTuBenhVienId == vatTuId && p.Id != id && p.KhoId == khoId);
        }

        public async Task<bool> CheckVatTuExist(long id)
        {
            return await _vtbv.TableNoTracking
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> CheckHieuLuc(long id)
        {
            return await _vtbv.TableNoTracking
                .AnyAsync(x => x.Id == id && x.HieuLuc);
        }
    }
}
