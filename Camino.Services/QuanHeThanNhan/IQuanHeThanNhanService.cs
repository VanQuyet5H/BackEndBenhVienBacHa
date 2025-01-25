using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.QuanHeThanNhan
{
    public interface IQuanHeThanNhanService : IMasterFileService<Core.Domain.Entities
        .QuanHeThanNhans.QuanHeThanNhan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
