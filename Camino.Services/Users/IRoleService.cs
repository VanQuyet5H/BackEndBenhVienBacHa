using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace Camino.Services.Users
{
    public interface IRoleService : IMasterFileService<Role>
    {
        string Test();
        bool VerifyAccess(long[] roleIds, Enums.DocumentType[] documentTypes, Enums.SecurityOperation securityOperation);
        MenuInfo GetMenuInfo(long[] roleIds);
        PortalMenuInfo GetPortalMenuInfo(long[] roleIds);
        ICollection<CaminoPermission> GetPermissions(long[] roleIds);
        Task<ICollection<LookupItemVo>> GetLookupAsync();

        #region CRUD

        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task UpdateRoleFunctionForRole(List<RoleFunction> roleFunctions, long roleId);

        Task AddPermissionForRole(List<RoleFunction> roleFunctions, long roleId);

        #endregion CRUD
        Task<ICollection<LookupItemVo>> GetRoleTypeNhanVienNoiBoAsync();
      
        long GetRoleTypeKhachVanLaiBoAsync();
        Task<Role> GetRoleWithUserType(Enums.UserType userType);
        Task<ICollection<LookupItemTextVo>> GetRoleQuyenHanNhanVienAsync();

        #region tiep nhan benh nhan

        bool IsHavePermissionForUpdateInformationTNBN();

        #endregion tiep nhan benh nhan
    }
}
