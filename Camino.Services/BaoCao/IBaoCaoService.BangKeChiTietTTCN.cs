using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoBangKeChiTietTTCNForGridAsync(BaoCaoBangKeChiTietTTCNQueryInfo queryInfo);
        byte[] ExportBaoCaoBangKeChiTietTTCN(GridDataSource datas, BaoCaoBangKeChiTietTTCNQueryInfo query);
    }
}
