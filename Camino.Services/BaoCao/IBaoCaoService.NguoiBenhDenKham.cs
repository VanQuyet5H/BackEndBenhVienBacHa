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
        Task<GridDataSource> GetDataBaoCaoNguoiBenhDenKhamForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoNguoiBenhDenKhamForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoNguoiBenhDenKhamAsync(GridDataSource gridDataSource, QueryInfo query);
    }
}
