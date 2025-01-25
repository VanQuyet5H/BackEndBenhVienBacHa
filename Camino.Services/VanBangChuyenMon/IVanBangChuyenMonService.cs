using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.VanBangChuyenMon
{
    public interface IVanBangChuyenMonService : IMasterFileService<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        bool CheckTrinhDoChuyenMonExits(string ten, long id, bool isTen = true);

        Task<bool> KiemTraMaTonTai(long id, string ma);

        Task<List<LookupItemVo>> GetListVanBangChuyenMon();
        Task<List<LookupItemTemplateVo>> GetListVanBangChuyenMon(DropDownListRequestModel model);
    }
}
