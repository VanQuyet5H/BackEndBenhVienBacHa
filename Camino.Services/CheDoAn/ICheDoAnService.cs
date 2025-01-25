using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.CheDoAn
{
    public interface ICheDoAnService : IMasterFileService<Core.Domain.Entities.CheDoAns.CheDoAn>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<ICollection<LookupItemTemplateVo>> GetLookupAsync(DropDownListRequestModel model);
        Task<bool> IsTenExists(string ten, long id = 0);
        Task<bool> IsMaExists(string ma, long id = 0);

    }
}
