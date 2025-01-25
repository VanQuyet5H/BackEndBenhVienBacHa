using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauTiepNhans
{
    public interface ILichSuTiepNhanService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<GridDataSource> GetDataForGridAsyncLichSuTiepNhan(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuTiepNhan(QueryInfo queryInfo);
    }
}
