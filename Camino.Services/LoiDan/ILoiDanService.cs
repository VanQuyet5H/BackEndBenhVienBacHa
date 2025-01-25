using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoiDan;

namespace Camino.Services.LoiDan
{
    public interface ILoiDanService : IMasterFileService<ICD>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<LoiDanTemplateVo>> GetICD(DropDownListRequestModel model);
    }
}
