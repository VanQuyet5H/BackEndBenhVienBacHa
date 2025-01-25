using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Services.ICDs
{
    public interface IDanhMucChuanDoanService: IMasterFileService<DanhMucChuanDoan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenViExists(string TenVi = null, long Id = 0);
        Task<bool> IsTenEngExists(string TenEng = null, long Id = 0);
    }
}
