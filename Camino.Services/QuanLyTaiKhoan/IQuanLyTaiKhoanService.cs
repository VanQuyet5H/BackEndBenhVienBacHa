using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuanLyTaiKhoan;

namespace Camino.Services.QuanLyTaiKhoan
{
    public interface IQuanLyTaiKhoanService : IMasterFileService<Core.Domain.Entities.NhanViens.NhanVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridTimNhanVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTimNhanVienAsync(QueryInfo queryInfo);

        Task<Core.Domain.Entities.NhanViens.NhanVien> CreateEmployeeAccount(long nhanVienId, string password);

        Task<Core.Domain.Entities.NhanViens.NhanVien> ChangeRoleEmployeeAccount(long nhanVienId, List<long> roleNewId);

        Task<List<LookupItemVo>> GetListRoleForEmployee(long nhanVienId);

        Task<List<LookupItemVo>> GetListRole(DropDownListRequestModel model);

        Task<Core.Domain.Entities.NhanViens.NhanVien> ChangeActiveEmployeeAccount(long nhanVienId);

        Task<Core.Domain.Entities.NhanViens.NhanVien> RemoveEmployeeAccount(long nhanVienId);
    }
}