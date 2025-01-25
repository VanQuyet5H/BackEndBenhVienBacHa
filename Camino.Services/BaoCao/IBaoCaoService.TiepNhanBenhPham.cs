using Camino.Core.Domain.ValueObject;
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
        List<LookupItemVo> GetListDoan(DropDownListRequestModel model);
        Task<GridDataSource> GetDataBaoCaoTiepNhanBenhPhamForGridAsync(BaoCaoTiepNhanBenhPhamQueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoTiepNhanBenhPhamForGridAsync(BaoCaoTiepNhanBenhPhamQueryInfo queryInfo);
        byte[] ExportBaoCaoTiepNhanBenhPhamGridVo(GridDataSource datas, BaoCaoTiepNhanBenhPhamQueryInfo queryInfo);
    }
}
