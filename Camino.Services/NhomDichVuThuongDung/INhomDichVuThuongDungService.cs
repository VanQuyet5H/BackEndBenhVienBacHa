using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhomVatTu;

namespace Camino.Services.NhomDichVuThuongDung
{
    public interface INhomDichVuThuongDungService : IMasterFileService<Core.Domain.Entities.GoiDichVus.GoiDichVu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
