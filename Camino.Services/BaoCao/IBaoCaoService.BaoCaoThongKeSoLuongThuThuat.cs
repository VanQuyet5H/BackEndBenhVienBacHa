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
        Task<GridDataSource> GetDataBaoCaoThongKeSoLuongThuThuatForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoThongKeSoLuongThuThuatAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoThongKeSoLuongThuThuat(GridDataSource datas, QueryInfo query);
    }
}
