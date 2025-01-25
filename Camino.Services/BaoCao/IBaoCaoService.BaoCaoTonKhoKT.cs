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
        #region báo cáo tồn kho kế toán
        Task<GridDataSource> GetDataBaoCaoTonKhoKTForGridAsync(BaoCaoTonKhoKTQueryInfo queryInfo);
        Task<GridDataSource> GetDataTotalPageBaoCaoTonKhoKTForGridAsync(BaoCaoTonKhoKTQueryInfo queryInfo);
        byte[] ExportBaoCaoTonKhoKTGridVo(GridDataSource gridDataSource, BaoCaoTonKhoKTQueryInfo query);
        #endregion
    }
}
