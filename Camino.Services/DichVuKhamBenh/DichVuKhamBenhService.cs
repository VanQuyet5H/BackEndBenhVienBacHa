using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.DichVuKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.DichVuKhamBenh
{
    [ScopedDependency(ServiceType = typeof(IDichVuKhamBenhService))]

    public class DichVuKhamBenhService : MasterFileService<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh>, IDichVuKhamBenhService
    {
        private readonly IRepository<DichVuKhamBenhThongTinGia> _dichVuKhamBenhThongTinGiaRepository;
        private readonly IRepository<Khoa> _khoaRepository;

        public DichVuKhamBenhService(IRepository<
            Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> repository,
            IRepository<DichVuKhamBenhThongTinGia> dichVuKhamBenhThongTinGiaRepository,
            IRepository<Khoa> khoaRepository
            ) : base(repository)
        {
            _dichVuKhamBenhThongTinGiaRepository = dichVuKhamBenhThongTinGiaRepository;
            _khoaRepository = khoaRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Include(p => p.Khoa)
                .Include(p => p.HangBenhVien).Select(s => new DichVuKhamBenhGridVo
                {
                    Id = s.Id,
                    Ma = s.MaChung,
                    MaTT37 = s.MaTT37,
                    Ten = s.TenChung,
                    TenKhoa = s.Khoa.Ten,
                    TenHangBenhVien = s.HangBenhVien.GetDescription(),
                    MoTa = s.MoTa,
                    Khoa = s.Khoa.Ten
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.Ma, g => g.MaTT37, g => g.MoTa, g => g.Khoa);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Include(p => p.Khoa)
                .Select(s => new DichVuKhamBenhGridVo
                {
                    Id = s.Id,
                    Ma = s.MaChung,
                    MaTT37 = s.MaTT37,
                    Ten = s.TenChung,
                    TenKhoa = s.Khoa.Ten,
                    TenHangBenhVien = s.HangBenhVien.GetDescription(),
                    MoTa = s.MoTa,
                    Khoa = s.Khoa.Ten
                }); ;
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten, g => g.MaTT37, g => g.MoTa, g => g.Khoa);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuKhamBenhId, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par = 0;
            if (dichVuKhamBenhId != null && dichVuKhamBenhId != 0)
            {
                par = dichVuKhamBenhId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var query = _dichVuKhamBenhThongTinGiaRepository.TableNoTracking
                //.Where(p=>p.DichVuKhamBenhId == long.Parse(queryInfo.SearchTerms))
                .Where(x => x.DichVuKhamBenhId == par)
                .OrderByDescending(x => x.TuNgay)
                .Select(s => new DichVuKhamBenhThongTinGiaGridVo()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaFormat = Convert.ToDouble(s.Gia).ApplyNumber(),
                    TuNgay = s.TuNgay,
                    TuNgayFormat = s.TuNgay.ApplyFormatDate(),
                    DenNgay = s.DenNgay,
                    DenNgayFormat = s.DenNgay == null ? null : s.DenNgay.Value.ApplyFormatDate(),
                    MoTa = s.MoTa,
                    ActiveName = s.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _dichVuKhamBenhThongTinGiaRepository.TableNoTracking.Where(p => p.DichVuKhamBenhId == long.Parse(queryInfo.SearchTerms))
                .OrderByDescending(x => x.TuNgay)
                .Select(s => new DichVuKhamBenhThongTinGiaGridVo()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaFormat = Convert.ToDouble(s.Gia).ApplyNumber(),
                    TuNgay = s.TuNgay,
                    TuNgayFormat = s.TuNgay.ApplyFormatDate(),
                    DenNgay = s.DenNgay,
                    DenNgayFormat = s.DenNgay == null ? null : s.DenNgay.Value.ApplyFormatDate(),
                    MoTa = s.MoTa,
                    ActiveName = s.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetKhoas(DropDownListRequestModel model)
        {
            //if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            //{
            //    var khoas = _khoaRepository.TableNoTracking
            //        .Select(item => new LookupItemVo
            //        {
            //            KeyId = item.Id,
            //            DisplayName = item.Ten
            //        })
            //        .ApplyLike(model.Query, x => x.DisplayName)
            //        .Take(model.Take);

            //    return await khoas.ToListAsync();
            //}
            //else
            //{
            //    var lstColumnNameSearch = new List<string>
            //    {
            //        nameof(Khoa.Ma),
            //        nameof(Khoa.Ten),
            //    };

            //    var khoas = _khoaRepository
            //        .ApplyFulltext(model.Query, nameof(Khoa), lstColumnNameSearch)
            //        .Take(model.Take)
            //        .Select(item => new LookupItemVo
            //        {
            //            KeyId = item.Id,
            //            DisplayName = item.Ten
            //        });
            //    return await khoas.ToListAsync();
            //}
            var khoas = _khoaRepository.TableNoTracking
                    .Select(item => new LookupItemVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.Ten
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .Take(model.Take);

            return await khoas.ToListAsync();
        }

        public async Task<bool> KiemTraTrungMaHoacTen(string maHoacTen = null, long dichVuKhamBenhId = 0, bool flag = false)
        {
            var result = false;
            if (dichVuKhamBenhId == 0)
            {

                result = !flag ? await BaseRepository.TableNoTracking.AnyAsync(p => p.MaChung.Equals(maHoacTen)) :
                                 await BaseRepository.TableNoTracking.AnyAsync(p => p.TenChung.Equals(maHoacTen));
            }
            else
            {
                result = !flag ? await BaseRepository.TableNoTracking.AnyAsync(p => p.MaChung.Equals(maHoacTen) && p.Id != dichVuKhamBenhId) :
                                 await BaseRepository.TableNoTracking.AnyAsync(p => p.TenChung.Equals(maHoacTen) && p.Id != dichVuKhamBenhId);
            }
            if (result)
                return false;
            return true;
        }


    }
}
