using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IXacNhanBHYTNoiTruService : IYeuCauTiepNhanBaseService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForDaXacNhanAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForDaXacNhanAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForBothBhyt(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForBothBhyt(QueryInfo queryInfo);
    }
}
