using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Services.DichVuKhamBenh
{
    public interface IDichVuKhamBenhService : IMasterFileService<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuKhamBenhId = null, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoas(DropDownListRequestModel model);
        //Task<bool> IsTenExists(string tenDVKhamBenh = null, long dVKhamBenhId = 0);
        Task<bool> KiemTraTrungMaHoacTen(string maHoacTen = null, long dichVuKhamBenhId = 0, bool flag = false);
    }
}
