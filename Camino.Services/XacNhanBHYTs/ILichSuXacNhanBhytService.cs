using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.XacNhanBHYTs
{
    public interface ILichSuXacNhanBhytService : IMasterFileService<DuyetBaoHiem>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        GridDataSource GetDataForGridXacNhanAsync(QueryInfo queryInfo);
    }
}