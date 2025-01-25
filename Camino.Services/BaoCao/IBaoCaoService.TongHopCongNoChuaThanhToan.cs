using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoTongHopCongNoChuaThanhToanForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBaoCaoTongHopCongNoChuaThanhToanForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoTongHopCongNoChuaThanhToan(GridDataSource gridDataSource, QueryInfo query);
    }
}
