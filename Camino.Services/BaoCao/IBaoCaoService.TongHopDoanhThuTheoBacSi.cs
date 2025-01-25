using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.BaoCao;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<BaoCaoTongHopDoanhThuTheoBacSiGridVo> GetDataForBaoCaoTongHopDoanhThuTheoBacSi(DateTimeFilterVo dateTimeFilter);
        Task<GridDataSource> GetBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(BaoCaoTongHopDoanhThuTheoBacSiQueryInfo queryInfo);
        Task<GridItem> GetTotalBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(BaoCaoTongHopDoanhThuTheoBacSiQueryInfo queryInfo);
    }
}
