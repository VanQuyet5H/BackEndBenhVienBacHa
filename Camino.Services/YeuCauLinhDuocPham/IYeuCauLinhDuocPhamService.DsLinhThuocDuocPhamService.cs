using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool isPrint);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        #region In lĩnh duoc pham
        Task<string> InLinhDuocPham(XacNhanInLinhDuocPham xacnhanInLinhDuocPham);
        #endregion
        #region ds yeu cau
        Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
        #region ds duyệt linh DP
        Task<GridDataSource> GetDataDSDuyetDuocPhamForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDSDuyetDuocPhamTotalPageForGridAsync(QueryInfo queryInfo);
        #endregion
        byte[] ExportDanhSachLayDuTruLinh(ICollection<DsLinhDuocPhamGridVo> datalinhs);
        byte[] ExportDanhSachDuyetLayDuTruLinh(ICollection<DsLinhDuocPhamGridVo> datalinhs);
        #region ds duyệt
        Task<GridDataSource> GetDataDSDuyetLinhDuocPhamChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSLinhDuocPhamChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSDuyetLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSLinhDuocPhamChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
    }
}
