using Camino.Core.Domain.ValueObject.BaoCaos;
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
        Task<GridDataSource> GetDataBaoCaoChiTietMienPhiTronVienForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoChiTietMienPhiTronVienForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoChiTietMienPhiTronVien(GridDataSource gridDataSource, QueryInfo query);
    }
}
