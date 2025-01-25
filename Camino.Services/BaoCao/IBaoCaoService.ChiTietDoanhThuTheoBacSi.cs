using Camino.Core.Domain.ValueObject.BaoCaos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<BaoCaoChiTietDoanhThuTheoBacSiGridVo> GetDataForBaoCaoChiTietDoanhThuTheoBacSi(DateTimeFilterVo dateTimeFilter);
        Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(BaoCaoDoanhThuTheoBacSiQueryInfo queryInfo);
        Task<GridItem> GetTotalBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(BaoCaoDoanhThuTheoBacSiQueryInfo queryInfo);
        Task<List<LookupItemVo>> GetListBacSy(DropDownListRequestModel queryInfo);
        Task<string> GetNameBacSy(long bacSiId);
    }
}
