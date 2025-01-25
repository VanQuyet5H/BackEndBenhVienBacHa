using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoHieuQuaCongViecForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoHieuQuaCongViec(GridDataSource gridDataSource, QueryInfo query);
    }
}
