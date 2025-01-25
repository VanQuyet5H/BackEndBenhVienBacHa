using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.NhaThau
{
    public interface INhaThauService : IMasterFileService<Core.Domain.Entities.NhaThaus.NhaThau>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(string ten = null, long id = 0);
    }
}