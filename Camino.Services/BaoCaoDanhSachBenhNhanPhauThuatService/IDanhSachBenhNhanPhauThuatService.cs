using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCaoDanhSachBenhNhanPhauThuatService
{
    public interface IDanhSachBenhNhanPhauThuatService : IMasterFileService<Core.Domain.Entities.NgheNghieps.NgheNghiep>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
