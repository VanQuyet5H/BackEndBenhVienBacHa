using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BenhVien.CapQuanLyBenhVien
{
    public interface ICapQuanLyBenhVienService
        : IMasterFileService<Core.Domain.Entities.BenhVien.CapQuanLyBenhViens
            .CapQuanLyBenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListCapQuanLyBenhVien(DropDownListRequestModel model);

        Task<bool> IsTenExists(string ten = null, long id = 0);
    }
}
