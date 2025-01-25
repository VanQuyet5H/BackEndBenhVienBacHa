using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemNhomThuVienPhiVo>> GetNhomThuVienPhiChuaHoanAsync(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetDataBaoCaoThuVienPhiChuaHoanForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBaoCaoThuVienPhiChuaHoanForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoThuVienPhiChuaHoan(GridDataSource gridDataSource, QueryInfo query);
    }
}
