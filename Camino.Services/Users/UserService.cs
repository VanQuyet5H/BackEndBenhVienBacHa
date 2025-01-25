using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Users;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.Users
{
    [ScopedDependency(ServiceType = typeof(IUserService))]
    public partial class UserService : MasterFileService<User>, IUserService
    {
        private readonly ILocalizationService _localizationService;
        readonly IUserAgentHelper _userAgentHelper;
        public IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;
        public IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;

        public UserService(IRepository<User> repository, ILocalizationService localizationService,
             IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository,
             IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository,
            IUserAgentHelper userAgentHelper) : base(repository)
        {
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;
        }

        public async Task<User> GetCurrentUser()
        {
            return await BaseRepository.Table.FirstOrDefaultAsync(o => o.Id == _userAgentHelper.GetCurrentUserId());
        }

        public async Task<bool> IsExistPhoneNumberOrEmail(string soDienThoai, string email)
        {
            return await BaseRepository.TableNoTracking.AnyAsync(p => (p.SoDienThoai == soDienThoai || p.Email == email) && p.Region == Enums.Region.External);
        }
        public async Task<User> GetUserByPhoneNumberOrEmail(string phoneNumberOrEmail, Enums.Region region)
        {
            var user = await BaseRepository.Table.FirstOrDefaultAsync(o => (o.SoDienThoai == phoneNumberOrEmail || o.Email == phoneNumberOrEmail) && (o.Region == Enums.Region.All || o.Region == region));

            return user;
        }
        public async Task<User> GetUserByPassCode(string phoneNumberOrEmail, string passCode, Enums.Region region)
        {
            var user = await BaseRepository.Table.FirstOrDefaultAsync(o => (o.SoDienThoai == phoneNumberOrEmail || o.Email == phoneNumberOrEmail) && o.PassCode == passCode && (o.Region == Enums.Region.All || o.Region == region));

            return user;
        }
        public async Task<User> GetUserByPhoneNumberOrEmailAndPassword(string phoneNumberOrEmail, string password, Enums.Region region)
        {
            var user = await BaseRepository.Table.FirstOrDefaultAsync(o => (o.SoDienThoai == phoneNumberOrEmail || o.Email == phoneNumberOrEmail) && (o.Region == Enums.Region.All || o.Region == region));

            if (user != null && (string.IsNullOrEmpty(user.Password) || PasswordHasher.VerifyHashedPassword(user.Password, password)))
            {
                //BaseRepository.Context.Entry(user).Reference(b => b.NhanVien).Query().Include(o=>o.NhanVienRoles).ThenInclude(o=>o.Role).Load();
                //BaseRepository.Context.Entry(user).Collection(b => b.Quays).Load();
                return user;
            }
            return null;
        }

        public async Task<string> IsExistsEmailExternalUser(string email)
        {
            var isExists = await BaseRepository.TableNoTracking.AnyAsync(p => p.Email == email && p.Region == Enums.Region.External);
            return isExists ? _localizationService.GetResource("UserError.EmailUsed") : string.Empty;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            //TODO: User need update
            return null;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //TODO: User need update
            return null;
        }

        public async Task<User> GetInternalUserByPhone(string phone)
        {
            return await BaseRepository.Table.AsNoTracking().Where(o => o.Region == Enums.Region.Internal && o.SoDienThoai.Equals(phone)).Include(o => o.NhanVien).ThenInclude(n => n.NhanVienRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByPhoneAndPassCode(string phone, string passCode)
        {
            var user = await BaseRepository.Table.FirstOrDefaultAsync(o => o.SoDienThoai == phone);

            if (user != null && passCode == "1111")
            {
                return user;
            }
            return null;
        }
        public async Task<ICollection<User>> GetUserAfter(long after, int limit, string searchString)
        {
            return await BaseRepository.TableNoTracking.OrderByDescending(o => o.Id).Where(o => after == 0 || o.Id < after)
                .Take(limit).ToArrayAsync();
        }

        public async Task<bool> CheckIsExistPhone(string sdt, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoDienThoai.Equals(sdt) && (p.Region == Enums.Region.External));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.SoDienThoai.Equals(sdt) && p.Id != id && (p.Region == Enums.Region.External));
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> CheckIsExistEmail(string email, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Email.Equals(email) && p.Email != "" && p.Email != null && (p.Region == Enums.Region.External));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Email.Equals(email) && p.Id != id && p.Email != "" && p.Email != null && (p.Region == Enums.Region.External));
            }
            if (result)
                return false;
            return true;
        }

        public List<int> GetUserTypesByUserId(long userId)
        {
            //TODO: User need update
            return null;
        }

        public async Task<GridDataSource> GetDataInternalForGridAsync(QueryInfo queryInfo)
        {
            //TODO: User need update
            //  return null;
            BuildDefaultSortExpression(queryInfo);


            var list = BaseRepository.Table.Where(o => o.IsDefault != true && o.Region == Enums.Region.Internal).AsNoTracking();


            list = list.Where(o => o.NhanVien.NhanVienRoles.Any(k => k.Role.UserType == Enums.UserType.NhanVien));


            var queryGetUserRoleStr = list.Include(u => u.NhanVien).ThenInclude(u => u.NhanVienRoles).ThenInclude(x => (x as NhanVienRole).Role).AsParallel().ToList().Select(x => new
            {

                UserId = x.Id,
                UserRolesStr = string.Join(", ", x.NhanVien.NhanVienRoles.Select(o => o.Role.Name).Distinct().ToList()),

            });



            var query = (from entity in list
                         join userRole in queryGetUserRoleStr.AsParallel() on entity.Id equals userRole.UserId
                         select new { entity, userRole }).Select(s => new UserGridVo
                         {
                             Id = s.entity.Id,

                             FullName = s.entity.HoTen,
                             Email = s.entity.Email,
                             Phone = s.entity.SoDienThoai.ApplyFormatPhone(),
                             UserRole = s.userRole.UserRolesStr,
                             IsActive = s.entity.IsActive,
                             //   UserTypes = s.userRole.UserTypeStr,
                             IsDefault = s.entity.IsDefault,

                             // NhanVienId = s.entity.NhanVien != null ? s.entity.NhanVien.Id : 0,
                             Region = s.entity.Region,

                         }).ApplyLike(queryInfo.SearchTerms, g => g.Email, g => g.Phone, g => g.FullName, g => g.UserRole, g => g.Phone);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageInternalForGridAsync(QueryInfo queryInfo)
        {
            //TODO: User need update
            // return null;

            var list = BaseRepository.Table.Where(o => o.IsDefault != true && o.Region == Enums.Region.Internal).AsNoTracking();


            list = list.Where(o => o.NhanVien.NhanVienRoles.Any(k => k.Role.UserType == Enums.UserType.NhanVien));


            var queryGetUserRoleStr = list.Include(u => u.NhanVien).ThenInclude(u => u.NhanVienRoles).ThenInclude(x => x.Role).AsParallel().ToList().Select(x => new
            {

                UserId = x.Id,
                UserRolesStr = string.Join(", ", x.NhanVien.NhanVienRoles.Select(o => o.Role.Name).Distinct().ToList()),

            });


            var query = (from entity in list
                         join userRole in queryGetUserRoleStr.AsParallel() on entity.Id equals userRole.UserId
                         select new { entity, userRole }).Select(s => new UserGridVo
                         {
                             Id = s.entity.Id,

                             FullName = s.entity.HoTen,
                             Email = s.entity.Email,
                             Phone = s.entity.SoDienThoai.ApplyFormatPhone(),
                             UserRole = s.userRole.UserRolesStr,
                             IsActive = s.entity.IsActive,
                             //   UserTypes = s.userRole.UserTypeStr,
                             IsDefault = s.entity.IsDefault,

                             //     NhanVienId = s.entity.NhanVien != null ? s.entity.NhanVien.Id : 0,
                             Region = s.entity.Region,
                         }).ApplyLike(queryInfo.SearchTerms, g => g.Email, g => g.Phone, g => g.FullName, g => g.UserRole, g => g.Phone);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<User> GetUserByPhoneNumberAndEmail(string phoneNumber, string email, Enums.Region region = Enums.Region.External)
        {
            var entity =
                await BaseRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                    p.SoDienThoai == phoneNumber && p.Email == email && p.Region == region);
            return entity;
        }
        public async Task CapNhatTrangThaiDaXemNotification(long id, Enums.MessagingType loaiThongBaoXuLy)
        {
            //var currentUserId = _userAgentHelper.GetCurrentUserId();
            //var allNotificationAndTaskByUser = await GetAllNotificationAndTaskByUserAsync(currentUserId);
            //if (loaiThongBaoXuLy == Enums.MessagingType.Notification)
            //{
            //    if (allNotificationAndTaskByUser.UserNotifications.All(x => x.NotificationId != id))
            //    {
            //        allNotificationAndTaskByUser.UserNotifications.Add(new UserNotification()
            //        {
            //            UserId = currentUserId,
            //            NotificationId = id,
            //            IsRead = true

            //        });
            //    }
            //    else
            //    {
            //        allNotificationAndTaskByUser.UserNotifications.First(x => x.NotificationId == id).IsRead = true;
            //    }
            //}
            //else if (loaiThongBaoXuLy == Enums.MessagingType.Task)
            //{
            //    if (allNotificationAndTaskByUser.UserTasks.All(x => x.TaskId != id))
            //    {
            //        allNotificationAndTaskByUser.UserTasks.Add(new UserTask()
            //        {
            //            UserId = currentUserId,
            //            TaskId = id,
            //            IsRead = true
            //        });
            //    }
            //    else
            //    {
            //        allNotificationAndTaskByUser.UserTasks.First(x => x.TaskId == id).IsRead = true;
            //    }
            //}
            //await BaseRepository.UpdateAsync(allNotificationAndTaskByUser);
        }

        public async Task<long> PhongBenhVienByUserId(long userId)
        {
            var result = 0;
            var res = await BaseRepository.TableNoTracking.Where(us => us.Id == userId).Include(cc => cc.NhanVien)
                                         .ThenInclude(hd => hd.HoatDongNhanViens)
                                         .Include(cc => cc.NhanVien).ThenInclude(cc => cc.LichSuHoatDongNhanViens)
                                         .Include(cc => cc.NhanVien)
                                         .FirstOrDefaultAsync();
            //lây theo lich sử hoạt động nhân viên và  coi nhân viên đó nó có phòng chính.
            if (res != null && res.NhanVien.LichSuHoatDongNhanViens.Count > 0)
            {
                return res.NhanVien.LichSuHoatDongNhanViens.LastOrDefault().PhongBenhVienId;
            }
            else
            {
                var phongChinhId = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == userId && cc.LaPhongLamViecChinh == true).Select(cc => cc.PhongBenhVienId).FirstOrDefault();
                if (phongChinhId != null)
                {
                    return (long)phongChinhId;
                }
            }

            return result;
        }

        public void XoaHoatDongPhongKhiLogout(long PhongBenhVienId)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nhanVienHoatDongPhong = _hoatDongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId && cc.PhongBenhVienId == PhongBenhVienId).FirstOrDefault();
            if (nhanVienHoatDongPhong != null)
            {
                _hoatDongNhanVienRepository.Delete(nhanVienHoatDongPhong);
            }
        }

    }
}
