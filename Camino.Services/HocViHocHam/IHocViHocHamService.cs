using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Camino.Services.HocViHocHam
{
    public interface IHocViHocHamService : IMasterFileService<Core.Domain.Entities.HocViHocHams.HocViHocHam>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        bool CheckMaSoExits(string ma, long id);
        bool CheckTenExits(string ten, long id);
        Task<List<LookupItemVo>> GetListHocViHocHam();
        Task<List<LookupItemTemplateVo>> GetListHocViHocHam(DropDownListRequestModel model);
    }
}
