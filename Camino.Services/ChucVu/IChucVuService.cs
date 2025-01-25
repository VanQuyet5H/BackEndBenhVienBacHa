using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ChucVu;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.ChucVu
{
    public interface IChucVuService : IMasterFileService<Core.Domain.Entities.ChucVus.ChucVu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        //Task<ICollection<LookupItemVo>> GetLookupAsync();
        Task<bool> IsTenExists(string tenChucVu = null, long chucVuId = 0);
        Task<bool> IsTenVietTatExists(string tenChucVuVietTat = null, long chucVuId = 0);
    }
}
