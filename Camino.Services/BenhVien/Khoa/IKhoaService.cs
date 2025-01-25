using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BenhVien.Khoa;

namespace Camino.Services.BenhVien.Khoa
{
    public interface IKhoaService
        : IMasterFileService<Core.Domain.Entities.BenhVien.Khoas
            .Khoa>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<KhoaTemplateVo>> GetListKhoa(DropDownListRequestModel queryInfo);
    }
}
