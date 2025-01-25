using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(QueryInfo queryInfo, bool laTinhTong = false);
        Task<GridDataSource> GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoDoanhThuKhamDoanTheoNhomDVGridVo(GridDataSource gridDataSource, QueryInfo query);
    }
}
