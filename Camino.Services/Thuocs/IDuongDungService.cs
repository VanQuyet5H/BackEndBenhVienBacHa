using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    public interface IDuongDungService : IMasterFileService<DuongDung>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenExists(string tenDuongDung = null, long Id = 0);
        Task<bool> IsMaExists(string maDuongDung = null, long Id = 0);

        Task<List<DuongDungTemplateVo>> GetListDuongDung(DropDownListRequestModel queryInfo);
    }
}
