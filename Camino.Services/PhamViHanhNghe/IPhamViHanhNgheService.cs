using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.PhamViHanhNghe
{
    public interface IPhamViHanhNgheService
        : IMasterFileService<Core.Domain.Entities.PhamViHanhNghes.PhamViHanhNghe>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(string ten = null, long id = 0);

        Task<bool> IsMaExists(string ma = null, long id = 0);

        Task<List<LookupItemTemplateVo>> GetListPhamViHanhNghe(DropDownListRequestModel queryInfo);

    }
}
