using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.Messages
{
    public interface ILichSuSMSService : IMasterFileService<Core.Domain.Entities.Messages.LichSuSMS>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        List<LookupItemVo> GetTrangThai();
    }
}
