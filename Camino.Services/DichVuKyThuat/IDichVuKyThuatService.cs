using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DichVuKyThuat
{
    public interface IDichVuKyThuatService : IMasterFileService<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat>
    {
        #region Danh sách
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuKyThuatId = null, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        #endregion
        List<DichVuKyThuatExportExcel> DataGridForExcel(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetListNhomDichVuKyThuat(DropDownListRequestModel model);
        Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat GetDVKTByMa(string ma);
        Task<List<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo>> NhomDichVuKyThuatBenhVienPhanNhomTreeViews(DropDownListRequestModel model);
        List<LookupItemVo> GetNhomDVKTs(DropDownListRequestModel model);
        Task<bool> KiemTraTrungMaHoacTen(string maHoacTen = null, long dichVuKyThuatId = 0, bool flag = false);
    }
}
