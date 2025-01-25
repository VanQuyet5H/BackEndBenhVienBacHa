using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.ChuanDoanHinhAnhs
{
    public interface IChuanDoanHinhAnhService : IMasterFileService<ChuanDoanHinhAnh>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsMaExists(string ma = null, long id = 0);

        List<LookupItemVo> GetListLoaiChuanDoanHinhAnh(LookupQueryInfo queryInfo);
    }
}
