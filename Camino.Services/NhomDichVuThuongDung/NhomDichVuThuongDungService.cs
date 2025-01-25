using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhomDichVuBenhVien;
using Camino.Core.Domain.ValueObject.NhomVatTu;
using Camino.Core.Domain.ValueObject.VatTu;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.NhomDichVuThuongDung
{
    [ScopedDependency(ServiceType = typeof(INhomDichVuThuongDungService))]
    public class NhomDichVuThuongDungService : MasterFileService<Core.Domain.Entities.GoiDichVus.GoiDichVu>, INhomDichVuThuongDungService
    {
        private readonly IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuKhamBenh> _goiDichVuChiTietDichVuKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuKyThuat> _goiDichVuChiTietDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuGiuong> _goiDichVuChiTietDichVuGiuongRepository;

        public NhomDichVuThuongDungService(IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVu> repository,
            IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuKhamBenh> goiDichVuChiTietDichVuKhamBenhRepository,
            IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuKyThuat> goiDichVuChiTietDichVuKyThuatRepository,
            IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVuChiTietDichVuGiuong> goiDichVuChiTietDichVuGiuongRepository
            ) : base(repository)
        {
            _goiDichVuChiTietDichVuKhamBenhRepository = goiDichVuChiTietDichVuKhamBenhRepository;
            _goiDichVuChiTietDichVuKyThuatRepository = goiDichVuChiTietDichVuKyThuatRepository;
            _goiDichVuChiTietDichVuGiuongRepository = goiDichVuChiTietDichVuGiuongRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var query = BaseRepository.TableNoTracking.Where(cc => cc.LoaiGoiDichVu == Core.Domain.Enums.EnumLoaiGoiDichVu.TrongPhongBacSy)
                .Select(s => new NhomDichVuThuongDungGridVo
                {
                    Id = s.Id,
                    TenNhom = s.Ten,
                    MoTa = s.MoTa,
                    TrangThai = s.IsDisabled
                }).ApplyLike(queryInfo.SearchTerms, g => g.TenNhom, g => g.MoTa);

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
            var result = BaseRepository.TableNoTracking.Where(cc => cc.LoaiGoiDichVu == Core.Domain.Enums.EnumLoaiGoiDichVu.TrongPhongBacSy)
                .Select(s => new NhomDichVuThuongDungGridVo
                {
                    Id = s.Id,
                    TenNhom = s.Ten,
                    MoTa = s.MoTa,
                    TrangThai = s.IsDisabled
                }).ApplyLike(queryInfo.SearchTerms, g => g.TenNhom, g => g.MoTa);

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

    }
}
