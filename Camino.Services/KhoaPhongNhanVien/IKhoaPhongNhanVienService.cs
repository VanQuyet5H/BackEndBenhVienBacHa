using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.KhoaPhongNhanVien
{
    public interface IKhoaPhongNhanVienService
        : IMasterFileService<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> CheckExistAsync(long khoaPhongId, long nhanVienId, long id);

        Task<bool> CheckNhanVienExistAsync(long khoaPhongId, long[] nhanVienIds, long[] input);
    }
}
