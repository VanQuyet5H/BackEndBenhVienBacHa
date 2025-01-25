using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.LoaiPhongBenh.LoaiPhongBenhNoiTru
{
    public interface ILoaiPhongBenhNoiTruService
        : IMasterFileService<Core.Domain.Entities.LoaiPhongBenh.LoaiPhongBenhNoiTrus
            .LoaiPhongBenhNoiTru>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(string tenNgheNghiep = null, long id = 0);
    }
}
