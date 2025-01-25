using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.NguoiGioiThieu;

namespace Camino.Services.NguoiGioiThieu
{
    [ScopedDependency(ServiceType = typeof(INguoiGioiThieuService))]

    public class NguoiGioiThieuService : MasterFileService<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu>, INguoiGioiThieuService
    {
        public IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        public IRepository<Core.Domain.Entities.Users.User> _userRepository;

        public NguoiGioiThieuService(IRepository<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu> repository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository) : base(repository)
        {
            _nhanVienRepository = nhanVienRepository;
            _userRepository = userRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Select(s => new NguoiGioiThieuGridVo
                {
                    Id = s.Id,
                    HoTen = s.HoTen,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                    HoTenNhanVienQuanLy = s.NhanVien.User.HoTen
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.HoTen, g => g.SoDienThoaiDisplay, g => g.HoTenNhanVienQuanLy);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
               .Select(s => new NguoiGioiThieuGridVo
               {
                   Id = s.Id,
                   HoTen = s.HoTen,
                   SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                   HoTenNhanVienQuanLy = s.NhanVien.User.HoTen
               });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.HoTen, g => g.SoDienThoaiDisplay, g => g.HoTenNhanVienQuanLy);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<NguoiQuanLyTemplateVo>> GetNguoiQuanLyListAsync(DropDownListRequestModel queryInfo)
        {
            var result = await _nhanVienRepository.TableNoTracking
                .Select(item => new NguoiQuanLyTemplateVo
                {
                    DisplayName = item.User.HoTen + "  -  " + item.User.SoDienThoaiDisplay,
                    KeyId = item.Id,
                    HoTen = item.User.HoTen,
                    SoDienThoai = item.User.SoDienThoaiDisplay,
                }).ApplyLike(queryInfo.Query, o => o.HoTen, o => o.SoDienThoai).Take(queryInfo.Take).ToListAsync();
            return result;
        }

        public async Task<bool> IsTenExists(string hoTen = null, long? nhanVienQuanLyId = null, long Id = 0)
        {
            var result = false;
            if (hoTen == null && nhanVienQuanLyId == null || nhanVienQuanLyId == 0)
            {
                return result;
            }
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.HoTen.Equals(hoTen) && p.NhanVienQuanLyId == nhanVienQuanLyId);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.HoTen.Equals(hoTen) && p.Id != Id && p.NhanVienQuanLyId == nhanVienQuanLyId);
            }
            return result;
        }

        public async Task<bool> IsPhoneNumberExists(string hoTen = null, string soDienThoai = null, long Id = 0)
        {
            var result = false;
            if (hoTen != null && soDienThoai == null || soDienThoai == "")
            {
                return result;
            }
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoDienThoai == soDienThoai);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Id != Id && p.SoDienThoai == soDienThoai);
            }
            return result;
        }
    }
}
