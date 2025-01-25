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
        Task<GridDataSource> GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(QueryInfo queryInfo, bool? isToTal = false);
        byte[] ExportBaoCaoKeToanBangKeChiTietNguoiBenh(GridDataSource gridDataSource, QueryInfo query);
    }
}
