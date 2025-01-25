using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.GoiDichVuMarketings
{
    public interface IGoiDvMarketingService : IMasterFileService<GoiDichVu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(Enums.EnumLoaiGoiDichVu loaiGoiDv,string ten = null, long id = 0);
    }
}
