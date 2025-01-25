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
        Task<List<LookupItemVo>> GetKhoVTYTBaoCaoChiTietXuatNoiTruLookupAsync(LookupQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoChiTietXuatNoiBoForGridAsync(BaoCaoChiTietXuatNoiBoQueryInfo queryInfo);
        byte[] ExportBaoCaoChiTietXuatNoiBo(GridDataSource gridDataSource, BaoCaoChiTietXuatNoiBoQueryInfo query);
    }
}
