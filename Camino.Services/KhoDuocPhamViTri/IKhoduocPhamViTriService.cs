using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhoDuocPhamViTris;

namespace Camino.Services.KhoDuocPhamViTri
{
    public interface IKhoduocPhamViTriService : IMasterFileService<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenExists(string ten, long id = 0);

        Task<List<LookupItemVo>> GetListTenKhoDuocPham(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetListViTriKhoDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemViTriVo>> GetListDataViTri();
    }
}
