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
        Task<GridDataSource> GetDataBaoCaoBangKePhieuXuatKhoForGridAsync(BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo, bool exportExcel = false);
        //Task<GridDataSource> GetDataTotalPageBaoCaoBangKePhieuXuatKhoForGridAsync(BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo);
        byte[] ExportBaoCaoBangKePhieuXuatKhoGridVo(GridDataSource gridDataSource, BaoCaoBangKePhieuXuatKhoQueryInfo query);
    }
}
