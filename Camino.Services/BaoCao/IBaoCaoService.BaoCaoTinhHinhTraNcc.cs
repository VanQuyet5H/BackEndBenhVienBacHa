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
        Task<GridDataSource> GetDataBaoCaoTinhHinhTraNCCForGridAsync(BaoCaoTinhHinhTraNCCQueryInfo queryInfo);
        byte[] ExportBaoCaoTinhHinhTraNCC(GridDataSource datas, BaoCaoTinhHinhTraNCCQueryInfo query);

        Task<List<LookupItemVo>> GetKhoDuocPhamLookupAsync(LookupQueryInfo queryInfo);
    }
}
