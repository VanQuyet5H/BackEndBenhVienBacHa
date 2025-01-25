using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.HDPP
{
    public interface IHDPPService : IMasterFileService<Core.Domain.Entities.HDPP.HDPP>
    {
        Task<bool> KiemTraTenTrung(long id, string ten);
        Task<List<LookupItemVo>> GetListHDPPAsync(DropDownListRequestModel model);
        Task<GridDataSource> GetDataHDPPForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetHDPPTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
