using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.PhanQuyenNguoiDungs
{
    public interface IPhanQuyenNguoiDungService : IMasterFileService<Role>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        List<LookupItemVo> GetListDocumentType();

        List<LookupItemVo> GetListUserType(LookupQueryInfo model);

        Task<bool> IsTenExists(string ten = null, long id = 0);

        Task<bool> AddRoleAsync(Role phanQuyenEntity);
    }
}
