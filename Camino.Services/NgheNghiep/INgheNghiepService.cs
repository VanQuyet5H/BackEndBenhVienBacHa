using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.NgheNghiep
{
    public interface INgheNghiepService : IMasterFileService<Core.Domain.Entities.NgheNghieps.NgheNghiep>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenNgheNghiepExists(string tenNgheNghiep = null, long ngheNghiepId = 0);

        Task<bool> IsTenVietTatExists(string tenVietTat = null, long ngheNghiepId = 0);
    }
}
