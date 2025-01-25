using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.XuatKhoQuaTangMarketing
{
    public partial interface IXuatKhoQuaTangService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
