using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.ChiSoXetNghiems
{
    public interface IChiSoXetNghiemService : IMasterFileService<ChiSoXetNghiem>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        bool CheckMaSoExits(string ma, long id);
        bool CheckTenExits(string ten, long id);
    }
}
