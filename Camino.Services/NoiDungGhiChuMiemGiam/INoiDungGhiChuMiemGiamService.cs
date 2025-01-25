using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.NoiDungGhiChuMiemGiam
{
    public interface INoiDungGhiChuMiemGiamService : IMasterFileService<Core.Domain.Entities.NoiDungGhiChuMiemGiams.NoiDungGhiChuMiemGiam>
    {
        Task<bool> KiemTraTrungMa(long id, string ma);
        Task<List<NoiDungGhiChuMiemGiamLookupItemVo>> GetListNoiDungMauAsync(DropDownListRequestModel model);
        Task<GridDataSource> GetDataForGridNoiDungGhiChuMiemGiamAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNoiDungGhiChuMiemGiamAsync(QueryInfo queryInfo);
    }
}
