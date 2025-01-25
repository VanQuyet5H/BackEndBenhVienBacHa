using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Services.CauHinh
{
    [ScopedDependency(ServiceType = typeof(ICauHinhNguoiDuyetTheoNhomDVService))]
    public class CauHinhNguoiDuyetTheoNhomDVService : MasterFileService<Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu>, ICauHinhNguoiDuyetTheoNhomDVService
    {
        private readonly IRepository<Camino.Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<User> _userRepository;
        public CauHinhNguoiDuyetTheoNhomDVService(IRepository<Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu> repository, 
            IRepository<User> userRepository,
            IRepository<Camino.Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository) : base(repository)
        {
            _userRepository = userRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
          
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new CauHinhNguoiDuyetTheoNhomDVGridVo()
            {
                Id = s.Id,
                NhanVienDuyetId = s.NhanVienId,
                NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                TenNhanVienDuyet = s.NhanVien.User.HoTen,
                TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten
            }).ApplyLike(queryInfo.SearchTerms, g => g.TenNhanVienDuyet, g => g.TenNhomDichVuBenhVien);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new CauHinhNguoiDuyetTheoNhomDVGridVo()
            {
                Id = s.Id,
                NhanVienDuyetId = s.NhanVienId,
                NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                TenNhanVienDuyet = s.NhanVien.User.HoTen,
                TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten
            }).ApplyLike(queryInfo.SearchTerms, g => g.TenNhanVienDuyet, g => g.TenNhomDichVuBenhVien);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<LookupItemVo>> GetListNhanViens(DropDownListRequestModel model)
        {
            var nvs =
               _userRepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.HoTen)
                  .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.HoTen,
                    KeyId = i.Id
                });
            return await nvs.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetLisNhomDVXetNghiems(DropDownListRequestModel model)
        {
            var cauHinhNguoiDuyetTheoNhomDichVuDaDcTaoIds = BaseRepository.TableNoTracking.Select(d => d.NhomDichVuBenhVienId).ToList();

            var nvs =
               _nhomDichVuBenhVienRepository.TableNoTracking
               .Where(d=>d.NhomDichVuBenhVienChaId == 2 && !cauHinhNguoiDuyetTheoNhomDichVuDaDcTaoIds.Contains(d.Id)) // nhóm dịch vụ cn = 2
                 .ApplyLike(model.Query, g => g.Ten)
                  .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });
            return await nvs.ToListAsync();
        }
        public async Task<List<LookupItemVo>> GetLisNhomDVXetNghiemUpdates(DropDownListRequestModel model)
        {
            var nvs =
               _nhomDichVuBenhVienRepository.TableNoTracking
               .Where(d => d.NhomDichVuBenhVienChaId == 2) // nhóm dịch vụ cn = 2
                 .ApplyLike(model.Query, g => g.Ten)
                  .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });
            return await nvs.ToListAsync();
        }
        public bool KiemTraNhomDichVuDaSetNhanVienDuyet(long? nhomDichVuBenhVienId,long id)
        {
            var nhomDichVuBVId = BaseRepository.TableNoTracking.Where(d => d.Id == (long)id).Select(d => d.NhomDichVuBenhVienId).FirstOrDefault();
            if(nhomDichVuBVId != null && nhomDichVuBVId == nhomDichVuBenhVienId)
            {
                return false;
            }
            else
            {
                var result = BaseRepository.TableNoTracking.Where(d => d.NhomDichVuBenhVienId == (long)nhomDichVuBenhVienId).ToList();

                return result.Count() != 0 ? true : false;
            }
        }
    }
}
