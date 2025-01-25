using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        #region báo cáo sổ phúc trình phẩu thuật thủ thuật
        Task<GridDataSource> GetBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoSoPhucTrinhPhauThuatThuThuatGridVo(GridDataSource gridDataSource, QueryInfo query);
        #endregion
    }
}
