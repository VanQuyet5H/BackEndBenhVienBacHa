using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ThietBiXetNghiems;

namespace Camino.Services.ThietBiXetNghiems
{
    public interface IThietBiXetNghiemService : IMasterFileService<MayXetNghiem>
    {
        Task<List<MayXetNghiem>> GetTatCaMayXetNghiemAsync();

        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<MauMayXetNghiem>> GetListNhomThietBi(long id, DropDownListRequestModel model = null);

        Task<List<LookupItemTemplateVo>> GetNhomXetNghiem(DropDownListRequestModel model);

        Task<bool> IsMaExists(long nhomThietBiId, string ma, long nhomXetNghiemId, long id = 0, bool isCopy = false);

        Task<bool> IsTenExists(long nhomThietBiId, string ten, long nhomXetNghiemId, long id = 0, bool isCopy = false);

        #region loookup

        Task<List<LookupItemTemplateVo>> GetListMayXetNghiemAsync(DropDownListRequestModel model);

        #endregion
    }
}