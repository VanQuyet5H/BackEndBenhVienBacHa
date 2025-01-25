using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IBhytListNoiTruService : IYeuCauTiepNhanBaseService
    {
        Task<GridDataSource> GetDataForXacNhanBhytNoiTruAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForXacNhanBhytNoiTruAsync(QueryInfo queryInfo);
    }
}
