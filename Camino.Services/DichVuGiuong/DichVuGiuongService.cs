using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.DichVuGiuongThongTinGias;
using System.Collections.Generic;

namespace Camino.Services.DichVuGiuong
{
    [ScopedDependency(ServiceType = typeof(IDichVuGiuongService))]

    public class DichVuGiuongService : MasterFileService<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong>, IDichVuGiuongService
    {
        private readonly IRepository<DichVuGiuongThongTinGia> _dichVuGiuongThongTinGia;
        public DichVuGiuongService(IRepository<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong> repository,
            IRepository<DichVuGiuongThongTinGia> dichVuGiuongThongTinGia) : base(repository)
        {
            _dichVuGiuongThongTinGia = dichVuGiuongThongTinGia;
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
                .Include(o => o.Khoa)
                .Include(o => o.DichVuGiuongThongTinGias)
                .Select(s => new DichVuGiuongGridVo
                {
                    Id = s.Id,
                    Ma = s.MaChung,
                    MaTT37 = s.MaTT37,
                    Ten = s.TenChung,
                    HangBenhVien = s.HangBenhVien,
                    HangBenhVienDisplay = s.HangBenhVien.GetDescription(),
                    KhoaId = s.KhoaId,
                    Khoa = s.Khoa.Ten,
                    MoTa = s.MoTa,
                    //ListDichVuGiuongThongTinGia = s.DichVuGiuongThongTinGias.ToList()
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.MaTT37, g => g.Ten, g => g.Khoa,
                g => g.MoTa);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(o => o.Khoa)
                .Include(o => o.DichVuGiuongThongTinGias)
                .Select(s => new DichVuGiuongGridVo
                {
                    Id = s.Id,
                    Ma = s.MaChung,
                    MaTT37 = s.MaTT37,
                    Ten = s.TenChung,
                    HangBenhVien = s.HangBenhVien,
                    HangBenhVienDisplay = s.HangBenhVien.GetDescription(),
                    KhoaId = s.KhoaId,
                    Khoa = s.Khoa.Ten,
                    MoTa = s.MoTa,
                    //ListDichVuGiuongThongTinGia = s.DichVuGiuongThongTinGias.ToList()
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.MaTT37, g => g.Ten, g => g.Khoa,
                g => g.MoTa);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuGiuongId, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par = 0;
            if (dichVuGiuongId != null && dichVuGiuongId != 0)
            {
                par = dichVuGiuongId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var query = _dichVuGiuongThongTinGia.TableNoTracking
                //.Where(x => x.DichVuGiuongId == long.Parse(queryInfo.SearchTerms))
                .Where(x => x.DichVuGiuongId == dichVuGiuongId)
                .Select(s => new DichVuGiuongThongTinGiaGridVo()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaDisplay = Convert.ToInt64(s.Gia).ApplyNumber(),
                    TuNgay = s.TuNgay,
                    TuNgayDisplay = s.TuNgay.ApplyFormatDate(),
                    DenNgay = s.DenNgay,
                    DenNgayDisplay = s.DenNgay == null ? null : s.DenNgay.Value.ApplyFormatDate(),
                    MoTa = s.MoTa,
                    HieuLucDisplay = s.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _dichVuGiuongThongTinGia.TableNoTracking
                .Where(x => x.DichVuGiuongId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new DichVuGiuongThongTinGiaGridVo()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaDisplay = Convert.ToInt64(s.Gia).ApplyNumber(),
                    TuNgay = s.TuNgay,
                    TuNgayDisplay = s.TuNgay.ApplyFormatDate(),
                    DenNgay = s.DenNgay,
                    DenNgayDisplay = s.DenNgay == null ? null : s.DenNgay.Value.ApplyFormatDate(),
                    MoTa = s.MoTa,
                    HieuLucDisplay = s.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

    }
}
