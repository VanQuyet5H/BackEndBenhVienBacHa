using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IBhytDvChuaHuongService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<GridDataSource> GetDataForDvChuaHuongBhytAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForDvChuaHuongBhytNoiTruAsync(long yeuCauTiepNhanId);
    }
}