using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IBhytDvHuongService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<GridDataSource> GetDataForDvHuongBhytAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForDvHuongBhytNoiTruAsync(long yeuCauTiepNhanId);

        Task<ActionResult<LichSuVo>> GetHistoryLog(LichSuXacNhanVo lichSuXacNhanVo);
    }
}
