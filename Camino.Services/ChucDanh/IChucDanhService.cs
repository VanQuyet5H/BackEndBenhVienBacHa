using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.ChucDanh
{
    public interface IChucDanhService : IMasterFileService<Core.Domain.Entities.ChucDanhs.ChucDanh>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<ICollection<LookupItemVo>> GetLookupAsync();
        Task<bool> IsTenExists(string ten,long id = 0);
        Task<List<LookupItemVo>> GetListChucDanh();
        Task<bool> IsMaExists(string ma, long id = 0);
        Task<List<ChucDanhItemVo>> GetListNhomChucDanh(DropDownListRequestModel model);
        Task<List<LookupItemTemplateVo>> GetListChucDanh(DropDownListRequestModel model);

    }
}
