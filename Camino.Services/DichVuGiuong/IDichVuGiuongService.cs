using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Services.DichVuGiuong
{
    public interface IDichVuGiuongService : IMasterFileService<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuGiuongId = null, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        //Task<ICollection<LookupItemVo>> GetLookupAsync();
        //Task<bool> IsTenExists(string tenChucVu = null, long chucVuId = 0);
        //Task<bool> IsTenVietTatExists(string tenChucVuVietTat = null, long chucVuId = 0);
    }
}
