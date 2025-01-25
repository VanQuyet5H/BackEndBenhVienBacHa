using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.InputStringStored
{
    public interface IInputStringStoredService : IMasterFileService<Core.Domain.Entities.InputStringStoreds.InputStringStored>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        //Task<bool> IsTenExists(string tenChucVu = null, long chucVuId = 0);
        //Task<bool> IsTenVietTatExists(string tenChucVuVietTat = null, long chucVuId = 0);
    }
}
