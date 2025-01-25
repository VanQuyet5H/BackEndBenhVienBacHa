using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.XacNhanBhytDaHoanThanh
{
    public interface IXacNhanBhytDaHoanThanhDetailedService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<GridDataSource> GetDataForGridDaXacNhanAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForDuyetBaoHiemAsync(QueryInfo queryInfo);

        GridDataSource GetDataForGridDuyetBaoHiemChiTietAsync(QueryInfo queryInfo);
    }
}
