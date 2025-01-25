using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCaoHoatDongKhoaKhamBenhService
{
    public interface IBaoCaoHoatDongKhoaKhamBenhService : IMasterFileService<Core.Domain.Entities.NgheNghieps.NgheNghiep>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
