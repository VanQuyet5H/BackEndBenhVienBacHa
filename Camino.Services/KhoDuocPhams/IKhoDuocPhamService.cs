using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.KhoDuocPhams
{
    public interface IKhoDuocPhamService : IMasterFileService<Kho>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListKhoDuocPham(DropDownListRequestModel model);
        Task<bool> CheckExistKhoTongAsync(EnumLoaiKhoDuocPham loaiKhoDuocPham);
        Task<bool> CheckIsExistTenKho(string strTen, long id);

        Task<List<LookupItemVo>> GetLoaiKhos(DropDownListRequestModel model);

    }
}
