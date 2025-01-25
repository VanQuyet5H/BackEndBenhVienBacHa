using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BenhVien.LoaiBenhVien
{
    public interface ILoaiBenhVienService
        : IMasterFileService<Core.Domain.Entities.BenhVien.LoaiBenhViens
            .LoaiBenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListLoaiBenhVien(DropDownListRequestModel model);

        Task<bool> IsTenExists(string ten = null, long id = 0);
    }
}
