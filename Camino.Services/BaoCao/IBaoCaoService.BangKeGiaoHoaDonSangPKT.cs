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
        Task<GridDataSource> GetDataBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo);
        Task<GridDataSource> GetDataTotalPageBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo);
        byte[] ExportBaoCaoBangKeGiaoHoaDonSangPKTGridVo(GridDataSource gridDataSource, BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo query);
    }
}
