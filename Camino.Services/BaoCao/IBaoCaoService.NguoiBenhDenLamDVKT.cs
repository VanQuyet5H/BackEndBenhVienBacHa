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
        Task<GridDataSource> GetDataBaoCaoNguoiBenhDenLamDVKTForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoNguoiBenhDenLamDVKTForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoNguoiBenhDenLamDVKTAsync(GridDataSource gridDataSource, QueryInfo query);
    }
}
