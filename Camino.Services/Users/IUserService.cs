using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.Users
{
    public interface IUserService : IMasterFileService<User>
    {
        Task<User> GetUserByPhoneNumberOrEmail(string phoneNumberOrEmail, Enums.Region region);
        Task<User> GetUserByPassCode(string phoneNumberOrEmail, string passCode, Enums.Region region);
        Task<User> GetUserByPhoneNumberOrEmailAndPassword(string phoneNumberOrEmail, string password, Enums.Region region);
        Task<User> GetUserByPhoneAndPassCode(string phone, string passCode);
        Task<User> GetInternalUserByPhone(string phone);
        Task<string> IsExistsEmailExternalUser(string email);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<ICollection<User>> GetUserAfter(long after, int limit, string searchString);
        Task<User> GetCurrentUser();
        Task<bool> IsExistPhoneNumberOrEmail(string soDienThoai, string email);
        Task<bool> CheckIsExistEmail(string email, long id = 0);
        Task<bool> CheckIsExistPhone(string sdt, long id = 0);
        List<int> GetUserTypesByUserId(long userId);
        Task<GridDataSource> GetDataInternalForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageInternalForGridAsync(QueryInfo queryInfo);
        Task<User> GetUserByPhoneNumberAndEmail(string phoneNumber, string email, Enums.Region region = Enums.Region.External);
        Task<long> PhongBenhVienByUserId(long userId);
        void XoaHoatDongPhongKhiLogout(long PhongBenhVienId);
    }
}
