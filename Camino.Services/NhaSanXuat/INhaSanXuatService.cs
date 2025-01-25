using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.NhaSanXuat
{
    public interface INhaSanXuatService : IMasterFileService<Core.Domain.Entities.NhaSanXuats.NhaSanXuat>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo,bool isPrint);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenExists(string ten, long id = 0);
        Task<bool> IsMaExists(string ma, long id = 0);
        Task<List<ChucDanhItemVo>> GetListTenQuocGia(DropDownListRequestModel model);
    }
}
