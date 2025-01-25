using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.VatTu;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.VatTu
{
    [ScopedDependency(ServiceType = typeof(IVatTuService))]

    public class VatTuService : MasterFileService<Core.Domain.Entities.VatTus.VatTu>, IVatTuService
    {
        IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> _repositoryNhomVatTu;
        public VatTuService(IRepository<Core.Domain.Entities.VatTus.VatTu> repository, IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> repositoryNhomVatTu) : base(repository)
        {
            _repositoryNhomVatTu = repositoryNhomVatTu;
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
                .Select(s => new VatTuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    NhomVatTuId = s.NhomVatTu.Id,
                    TenNhomVatTu = s.NhomVatTu.Ten,
                    TenDonViTinh = s.DonViTinh,
                    TyLeBaoHiemThanhToan = s.TyLeBaoHiemThanhToan,
                    QuyCach = s.QuyCach,
                    NhaSanXuat = s.NhaSanXuat,
                    NuocSanXuat = s.NuocSanXuat,
                    MoTa = s.MoTa,
                    IsDisabled = s.IsDisabled,
                    HeSoDinhMucDonViTinh = s.HeSoDinhMucDonViTinh
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenNhomVatTu, g => g.TenDonViTinh, g => g.TyLeBaoHiemThanhToan.ToString(), g => g.QuyCach, g => g.NhaSanXuat, g => g.NuocSanXuat, g => g.MoTa);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new VatTuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    NhomVatTuId = s.NhomVatTu.Id,
                    TenNhomVatTu = s.NhomVatTu.Ten,
                    TenDonViTinh = s.DonViTinh,
                    TyLeBaoHiemThanhToan = s.TyLeBaoHiemThanhToan,
                    QuyCach = s.QuyCach,
                    NhaSanXuat = s.NhaSanXuat,
                    NuocSanXuat = s.NuocSanXuat,
                    MoTa = s.MoTa,
                    IsDisabled = s.IsDisabled,
                    HeSoDinhMucDonViTinh = s.HeSoDinhMucDonViTinh
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.TenNhomVatTu, g => g.TenDonViTinh, g => g.TyLeBaoHiemThanhToan.ToString(), g => g.QuyCach, g => g.NhaSanXuat, g => g.NuocSanXuat, g => g.MoTa);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<NhomVatTuTreeViewVo>> GetListNhomVatTuAsync(DropDownListRequestModel model)
        {
            var lstNhomVatTu = await _repositoryNhomVatTu.TableNoTracking
                .Select(item => new NhomVatTuTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    ParentId = item.NhomVatTuChaId
                })
                .ToListAsync();

            var query = lstNhomVatTu.Select(item => new NhomVatTuTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                Items = GetChildrenTree(lstNhomVatTu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
            })
            .Where(x =>
                x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                       (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .Take(model.Take).ToList();
            return query;
        }
        public static List<NhomVatTuTreeViewVo> GetChildrenTree(List<NhomVatTuTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomVatTuTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString)
                            || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }
        public async Task<bool> CheckMaVatTuBenhVien(string maVatTuBenhVien)
        {
            if (maVatTuBenhVien == null || maVatTuBenhVien == "")
            {
                return true;
            }
            return false ;
        }
    }
}
