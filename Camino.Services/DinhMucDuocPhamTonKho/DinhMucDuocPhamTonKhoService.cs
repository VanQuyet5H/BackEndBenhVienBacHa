using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Helpers;
using System;

namespace Camino.Services.DinhMucDuocPhamTonKho
{
    [ScopedDependency(ServiceType = typeof(IDinhMucDuocPhamTonKhoService))]

    public class DinhMucDuocPhamTonKhoService : MasterFileService<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho>, IDinhMucDuocPhamTonKhoService
    {
        private readonly IRepository<Kho> _khoDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBVRepository;
        public DinhMucDuocPhamTonKhoService(IRepository<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho> repository,
            IRepository<Kho> khoDuocPhamRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBVRepository
            ) : base(repository)
        {
            _khoDuocPhamRepository = khoDuocPhamRepository;
            _duocPhamBVRepository = duocPhamBVRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Include(o => o.KhoDuocPham)
                .Include(o => o.DuocPhamBenhVien)
                .ThenInclude(o => o.DuocPham)
                .Select(s => new DinhMucDuocPhamTonKhoGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                    KhoDuocPhamId = s.KhoId,
                    TenKhoDuocPham = s.KhoDuocPham.Ten,
                    MoTa = s.MoTa,
                    TonToiThieu = s.TonToiThieu,
                    TonToiDa = s.TonToiDa,
                    SoNgayTruocKhiHetHan = s.SoNgayTruocKhiHetHan
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenDuocPham, g => g.TenKhoDuocPham, g => g.MoTa);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(o => o.KhoDuocPham)
                .Include(o => o.DuocPhamBenhVien)
                .ThenInclude(o => o.DuocPham)
                .Select(s => new DinhMucDuocPhamTonKhoGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                    KhoDuocPhamId = s.KhoId,
                    TenKhoDuocPham = s.KhoDuocPham.Ten,
                    MoTa = s.MoTa,
                    TonToiThieu = s.TonToiThieu,
                    TonToiDa = s.TonToiDa,
                    SoNgayTruocKhiHetHan = s.SoNgayTruocKhiHetHan
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenDuocPham, g => g.TenKhoDuocPham, g => g.MoTa);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<LookupItemVo>> GetTenKhoDuocPhamAsync(DropDownListRequestModel queryInfo)
        {
            var result = _khoDuocPhamRepository.TableNoTracking
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).Distinct().ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take).ToListAsync();
            await Task.WhenAll(result);
            return result.Result;
        }
        public async Task<List<VatTuDropdownTemplateVo>> GetListDuocPham(DropDownListRequestModel queryInfo)
        {
            var lst = await _duocPhamBVRepository.TableNoTracking
                .Include(x => x.DuocPham)
                .Where(o => o.HieuLuc != false)
                .ApplyLike(queryInfo.Query, o => o.DuocPham.Ten).Take(queryInfo.Take).ToListAsync();
            var result = lst.Select(item => new VatTuDropdownTemplateVo
            {
                DisplayName = item.DuocPham.Ten,
                Ten = item.DuocPham.Ten,
                HoatChat = item.DuocPham.HoatChat,
                NhaSanXuat = item.DuocPham.NhaSanXuat,
                KeyId = item.Id,
            });
            result = result.GroupBy(p => new
            {
                p.DisplayName
            }).Select(g => g.First());
            return result.ToList();
        }
        public async Task<bool> CheckHoatChatExist(long id)
        {
            var check = await _duocPhamBVRepository.TableNoTracking
                   .Include(o => o.DuocPham)
                .Where(x => x.Id == id).ToListAsync();
            if (check.Count > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> CheckHieuLuc(long id)
        {
            var check = await _duocPhamBVRepository.TableNoTracking
                   .Include(o => o.DuocPham)
                .Where(x => x.Id == id && x.HieuLuc == false).FirstOrDefaultAsync();
            if (check != null)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> IsTenExists(long duocPhamId = 0, long Id = 0, long khoDuocPhamId = 0)
        {
            bool result;
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.DuocPhamBenhVienId == duocPhamId && p.KhoId == khoDuocPhamId);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.DuocPhamBenhVienId == duocPhamId && p.Id != Id && p.KhoId == khoDuocPhamId);
            }
            return result;
        }
    }
}
