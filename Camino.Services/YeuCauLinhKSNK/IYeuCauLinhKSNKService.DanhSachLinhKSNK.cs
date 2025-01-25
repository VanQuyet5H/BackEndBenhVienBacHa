using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial interface IYeuCauLinhKSNKService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool print);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        #region In lĩnh KSNK
        Task<string> InLinhKSNK(XacNhanInLinhKSNK xacnhanInLinhKSNK);
        #endregion

        #region child
        #endregion
        #region ds duyệt linh DP
        Task<GridDataSource> GetDataDSDuyetKSNKForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDSDuyetKSNKTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSDuyetLinhKSNKChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSLinhKSNKChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSDuyetLinhKSNKChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSLinhKSNKChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
        byte[] ExportDanhSachLayDuTruLinh(ICollection<DSLinhKSNKGridVo> datalinhs);
        byte[] ExportDanhSachDuyetLayDuTruLinh(ICollection<DSLinhKSNKGridVo> datalinhs);
        #region ds yc linh DP
        Task<GridDataSource> GetDataDSYeuCauLinhKSNKChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSYeuCauLinhKSNKChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
        string TenNoiNhanPhieuLinhTrucTiep(long noiYeuCauId);
    }
}
