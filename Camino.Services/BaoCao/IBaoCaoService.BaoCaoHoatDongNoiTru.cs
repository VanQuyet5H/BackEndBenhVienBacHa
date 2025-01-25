using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoHoatDongNoiTruForGridAsync(BaoCaoHoatDongNoiTruQueryInfo queryInfo);

        byte[] ExportBaoCaoHoatDongNoiTruGridVo(GridDataSource gridDataSource, BaoCaoHoatDongNoiTruQueryInfo query);
    }
}