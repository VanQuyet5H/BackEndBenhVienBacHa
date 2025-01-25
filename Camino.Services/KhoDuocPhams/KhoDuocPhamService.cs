using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.KhoDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IKhoDuocPhamService))]
    public class KhoDuocPhamService : MasterFileService<Kho>, IKhoDuocPhamService
    {
        private readonly IRepository<KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        public KhoDuocPhamService(IRepository<Kho> repository, IRepository<KhoNhanVienQuanLy> khoNhanVienQuanLyRepository) : base(repository)
        {
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
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
            var query = BaseRepository.TableNoTracking.Select(p => new KhoDuocPhamGridVo
            {
                Id = p.Id,
                Ten = p.Ten,
                LoaiKho = p.LoaiKho,
                KhoaPhong = p.KhoaPhong.Ten,
                PhongBenhVien = p.PhongBenhVien.Ten,
                IsDefault = p.IsDefault,
                LoaiDuocPham = p.LoaiDuocPham,
                LoaiVatTu = p.LoaiVatTu
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.KhoaPhong, p => p.PhongBenhVien, p => p.LoaiDuocPhamVatTu);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(p => new KhoDuocPhamGridVo
            {
                Id = p.Id,
                Ten = p.Ten,
                LoaiKho = p.LoaiKho,
                KhoaPhong = p.KhoaPhong.Ten,
                PhongBenhVien = p.PhongBenhVien.Ten,
                IsDefault = p.IsDefault,
                LoaiDuocPham = p.LoaiDuocPham,
                LoaiVatTu = p.LoaiVatTu
            }).ApplyLike(queryInfo.SearchTerms, p => p.Ten, p => p.KhoaPhong, p => p.PhongBenhVien, p => p.LoaiDuocPhamVatTu);

            var countTask = query.CountAsync();
            return new GridDataSource { TotalRowCount = await countTask };
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

        public async Task<List<LookupItemVo>> GetListKhoDuocPham(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking.ApplyLike(model.Query, p => p.Ten)
                .Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2) && p.LoaiDuocPham == true)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });
            return await list.ToListAsync();
        }

        public async Task<bool> CheckExistKhoTongAsync(EnumLoaiKhoDuocPham loaiKhoDuocPham)
        {
            return await BaseRepository.Table.FirstOrDefaultAsync(o => o.LoaiKho == loaiKhoDuocPham) != null;
        }

        public async Task<bool> CheckIsExistTenKho(string strTen, long id)
        {
            if (id == 0)
            {
                return !(await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(strTen)));
            }
            else
            {
                return !(await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(strTen) && p.Id != id));
            }
        }

        public async Task<List<LookupItemVo>> GetLoaiKhos(DropDownListRequestModel model)
        {
            var loaiKhoDuocPhams = EnumHelper.GetListEnum<EnumLoaiKhoDuocPham>();
            var loaiKhos = loaiKhoDuocPhams.Where(z => z == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || z == EnumLoaiKhoDuocPham.KhoLe || z == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                .Select(item => new LookupItemVo
             {
                 KeyId = (int)item,
                 DisplayName = item.GetDescription()
             });
            if (!string.IsNullOrEmpty(model.Query))
            {
                loaiKhos = loaiKhos.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return loaiKhos.ToList();
        }
    }
}
