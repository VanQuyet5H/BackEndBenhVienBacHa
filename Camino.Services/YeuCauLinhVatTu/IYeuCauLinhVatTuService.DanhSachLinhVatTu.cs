using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial interface IYeuCauLinhVatTuService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo,bool print);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        #region In lĩnh duoc pham
        Task<string> InLinhVatTu(XacNhanInLinhVatTu xacnhanInLinhDuocPham);
        #endregion
        #region child
        //Task<GridDataSource> GetDataForGridAsyncChild(QueryInfo queryInfo);
        //Task<GridDataSource> GetTotalPageForGridAsyncChild(QueryInfo queryInfo);
        #endregion
        #region ds duyệt linh DP
        Task<GridDataSource> GetDataDSDuyetVatTuForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDSDuyetVatTuTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSDuyetLinhVatTuChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSLinhVatTuChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSDuyetLinhVatTuChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSLinhVatTuChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
        byte[] ExportDanhSachLayDuTruLinh(ICollection<DSLinhVatTuGridVo> datalinhs);
        byte[] ExportDanhSachDuyetLayDuTruLinh(ICollection<DSLinhVatTuGridVo> datalinhs);
        #region ds yc linh DP
        Task<GridDataSource> GetDataDSYeuCauLinhVatTuChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSYeuCauLinhVatTuChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
    }
}
