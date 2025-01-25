using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuanLyTaiKhoan;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.QuanLyTaiKhoan
{
    [ScopedDependency(ServiceType = typeof(IQuanLyTaiKhoanService))]
    public class QuanLyTaiKhoanService : MasterFileService<Core.Domain.Entities.NhanViens.NhanVien>, IQuanLyTaiKhoanService
    {
        private IRepository<Role> _roleRepository;
        public QuanLyTaiKhoanService(IRepository<Core.Domain.Entities.NhanViens.NhanVien> repository, IRepository<Role> roleRepository) : base(repository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            //ReplaceDisplayValueSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Include(x => x.User)
                .Include(x => x.NhanVienRoles)
                .Where(p => !string.IsNullOrEmpty(p.User.Password))
                .Select(s => new QuanLyTaiKhoanGridVo
                {
                    Id = s.Id,
                    SoDienThoai = s.User.SoDienThoai,
                    DiaChi = s.User.DiaChi,
                    HoTen = s.User.HoTen,
                    Email = s.User.Email,
                    IsActive = s.User.IsActive,
                    IsActiveDisplay = s.User.IsActive ? "Kích hoạt" : "Chưa kích hoạt"
                });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.SoDienThoai, g => g.DiaChi, g => g.HoTen, g => g.Email, g => g.IsActiveDisplay);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            //ReplaceDisplayValueSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(x => x.User)
                .Include(x => x.NhanVienRoles)
                .Where(p => !string.IsNullOrEmpty(p.User.Password))
                .Select(s => new QuanLyTaiKhoanGridVo
                {
                    Id = s.Id,
                    SoDienThoai = s.User.SoDienThoai,
                    DiaChi = s.User.DiaChi,
                    HoTen = s.User.HoTen,
                    Email = s.User.Email,
                    IsActive = s.User.IsActive,
                    IsActiveDisplay = s.User.IsActive ? "Kích hoạt" : "Chưa kích hoạt"
                });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.SoDienThoai, g => g.DiaChi, g => g.HoTen, g => g.Email, g => g.IsActiveDisplay);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridTimNhanVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(x => x.User)
                .Where(p => string.IsNullOrEmpty(p.User.Password))
                .Select(s => new TimNhanVienGridVo
                {
                    Id = s.Id,
                    SoDienThoai = s.User.SoDienThoai,
                    DiaChi = s.User.DiaChi,
                    HoTen = s.User.HoTen,
                    Email = s.User.Email,
                    GioiTinh = s.User.GioiTinh,
                    GioiTinhDisplay = s.User.GioiTinh.GetValueOrDefault().GetDescription(),
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SearchTimNhanVien>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.HoTen))
                {
                    query = query.Where(p => p.HoTen.Contains(queryString.HoTen.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.SoDienThoai))
                {
                    query = query.Where(p => p.SoDienThoai.Contains(queryString.SoDienThoai.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.Email))
                {
                    query = query.Where(p => p.Email.Contains(queryString.Email.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.DiaChi))
                {
                    query = query.Where(p => p.DiaChi.Contains(queryString.DiaChi.Trim()));
                }
                if (queryString.GioiTinh != null)
                {
                    query = query.Where(p => p.GioiTinh == queryString.GioiTinh);
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridTimNhanVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(x => x.User)
                .Where(p => string.IsNullOrEmpty(p.User.Password))
                .Select(s => new TimNhanVienGridVo
                {
                    Id = s.Id,
                    SoDienThoai = s.User.SoDienThoai,
                    DiaChi = s.User.DiaChi,
                    HoTen = s.User.HoTen,
                    Email = s.User.Email,
                    GioiTinh = s.User.GioiTinh,
                    GioiTinhDisplay = s.User.GioiTinh.GetValueOrDefault().GetDescription(),
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SearchTimNhanVien>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.HoTen))
                {
                    query = query.Where(p => p.HoTen.Contains(queryString.HoTen.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.SoDienThoai))
                {
                    query = query.Where(p => p.SoDienThoai.Contains(queryString.SoDienThoai.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.Email))
                {
                    query = query.Where(p => p.Email.Contains(queryString.Email.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.DiaChi))
                {
                    query = query.Where(p => p.DiaChi.Contains(queryString.DiaChi.Trim()));
                }
                if (queryString.GioiTinh != null)
                {
                    query = query.Where(p => p.GioiTinh == queryString.GioiTinh);
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<Core.Domain.Entities.NhanViens.NhanVien> CreateEmployeeAccount(long nhanVienId, string password)
        {
            var entity = await BaseRepository.Table.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == nhanVienId);

            if (entity == null) return null;

            entity.User.Password = PasswordHasher.HashPassword(password);
            await BaseRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<Core.Domain.Entities.NhanViens.NhanVien> ChangeRoleEmployeeAccount(long nhanVienId, List<long> roleNewId)
        {
            var entity = await BaseRepository.Table.Include(p => p.User).Include(p => p.NhanVienRoles).ThenInclude(p => p.Role).FirstOrDefaultAsync(p => p.Id == nhanVienId);

            if (entity == null) return null;

            foreach (var item in entity.NhanVienRoles)
            {
                if (!roleNewId.Any(p => p == item.RoleId))
                {
                    item.WillDelete = true;
                }
            }
            foreach (var item in roleNewId)
            {
                if (!entity.NhanVienRoles.Any(p => p.RoleId == item))
                {
                    var nhanVienRole = new NhanVienRole
                    {
                        NhanVien = entity,
                        RoleId = item,
                    };
                    entity.NhanVienRoles.Add(nhanVienRole);
                }
            }

            await BaseRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<List<LookupItemVo>> GetListRoleForEmployee(long nhanVienId)
        {
            var entity = await BaseRepository.Table
                .Include(p => p.User)
                .Include(p => p.NhanVienRoles).ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(p => p.Id == nhanVienId);

            if (!entity.NhanVienRoles.Any())
            {
                return new List<LookupItemVo>();
            }

            var lstRole = entity.NhanVienRoles.Select(p => new LookupItemVo
            {
                KeyId = p.Role.Id,
                DisplayName = p.Role.Name,
            }).ToList();

            return lstRole;
        }

        public async Task<List<LookupItemVo>> GetListRole(DropDownListRequestModel model)
        {
            if (!string.IsNullOrEmpty(model.Query))
            {
                var res =  _roleRepository.TableNoTracking.Select(p => new LookupItemVo
                {
                    KeyId = p.Id,
                    DisplayName = p.Name,
                });


                var kq = await res.Where(cc => cc.DisplayName.ToUpper().Contains(model.Query.ToUpper())).ToListAsync();
                return kq;
            }
            else
            {
                return await _roleRepository.TableNoTracking.Select(p => new LookupItemVo
                {
                    KeyId = p.Id,
                    DisplayName = p.Name,
                }).ToListAsync();

            }
        }

        public async Task<Core.Domain.Entities.NhanViens.NhanVien> ChangeActiveEmployeeAccount(long nhanVienId)
        {
            var entity = await BaseRepository.Table.Include(p => p.User).Include(p => p.NhanVienRoles).ThenInclude(p => p.Role).FirstOrDefaultAsync(p => p.Id == nhanVienId);

            if (entity == null) return null;

            entity.User.IsActive = !entity.User.IsActive;
            await BaseRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<Core.Domain.Entities.NhanViens.NhanVien> RemoveEmployeeAccount(long nhanVienId)
        {
            var entity = await BaseRepository.Table.Include(p => p.User).Include(p => p.NhanVienRoles).ThenInclude(p => p.Role).FirstOrDefaultAsync(p => p.Id == nhanVienId);

            if (entity == null) return null;

            entity.User.Password = null;
            await BaseRepository.UpdateAsync(entity);

            return entity;
        }
    }
}