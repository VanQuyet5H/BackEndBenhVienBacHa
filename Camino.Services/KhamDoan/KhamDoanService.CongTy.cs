using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task<GridDataSource> GetDataListCongTyForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var timKiemNangCaoObj = new KhamDoanCongTyTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<KhamDoanCongTyTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            var query = _congTyKhamSucKhoeRepository.TableNoTracking.Select(s => new KhamDoanCongTyGridVo
            {
                Id = s.Id,
                DiaChi = s.DiaChi,
                CoHoatDong = s.CoHoatDong,
                DaiDien = s.NguoiDaiDien,
                MaSoThue = s.MaSoThue,
                NguoiLienHe = s.NguoiLienHe,
                LoaiCongTy = s.LoaiCongTy.GetDescription(),
                TaiKhoanNganHang = s.SoTaiKhoanNganHang,
                DienThoai = s.SoDienThoai,
                TenCongTy = s.Ten,
                MaCongTy = s.Ma
            }).ApplyLike(timKiemNangCaoObj.SearchString?.Trim(),
                g => g.DiaChi,
                g => g.MaCongTy,
                g => g.TenCongTy,
                g => g.DienThoai,
                g => g.TaiKhoanNganHang,
                g => g.NguoiLienHe,
                g => g.MaSoThue,
                g => g.DaiDien);

            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();

            var queryTask = exportExcel == true ? query.OrderBy(queryInfo.SortString).ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageListCongTyForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new KhamDoanCongTyTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<KhamDoanCongTyTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }
            var query = _congTyKhamSucKhoeRepository.TableNoTracking.Select(s => new KhamDoanCongTyGridVo
            {
                Id = s.Id,
                DiaChi = s.DiaChi,
                CoHoatDong = s.CoHoatDong,
                DaiDien = s.NguoiDaiDien,
                MaSoThue = s.MaSoThue,
                NguoiLienHe = s.NguoiLienHe,
                LoaiCongTy = s.LoaiCongTy.GetDescription(),
                TaiKhoanNganHang = s.SoTaiKhoanNganHang,
                DienThoai = s.SoDienThoai,
                TenCongTy = s.Ten,
                MaCongTy = s.Ma
            }).ApplyLike(timKiemNangCaoObj.SearchString?.Trim(),
                g => g.DiaChi,
                g => g.MaCongTy,
                g => g.TenCongTy,
                g => g.DienThoai,
                g => g.TaiKhoanNganHang,
                g => g.NguoiLienHe,
                g => g.MaSoThue,
                g => g.DaiDien);

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<CongTyKhamSucKhoe> GetByCongTyIdAsync(long id, Func<IQueryable<CongTyKhamSucKhoe>, IIncludableQueryable<CongTyKhamSucKhoe, object>> includes)
        {
            return await _congTyKhamSucKhoeRepository.GetByIdAsync(id, includes);
        }

        public async Task AddCongTyAsync(CongTyKhamSucKhoe entity)
        {
            await _congTyKhamSucKhoeRepository.AddAsync(entity);
        }

        public async Task UpdateCongTyAsync(CongTyKhamSucKhoe entity)
        {
            await _congTyKhamSucKhoeRepository.UpdateAsync(entity);
        }

        public async Task DeleteByCongTyIdAsync(long id)
        {
            await _congTyKhamSucKhoeRepository.DeleteByIdAsync(id);
        }
    }
}
