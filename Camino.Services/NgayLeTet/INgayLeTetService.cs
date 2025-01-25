using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NgayLeTet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NgayLeTet
{
    public interface INgayLeTetService : IMasterFileService<Camino.Core.Domain.Entities.CauHinhs.NgayLeTet>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        List<LookupItemTextVo> GetNamSreachs(DropDownListRequestModel model, NamSearch nam);
    }
}
