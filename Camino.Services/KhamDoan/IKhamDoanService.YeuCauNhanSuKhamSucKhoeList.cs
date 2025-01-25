using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.KhamDoan
{
    // partial interface for Yeu Cau Nhan Su Kham Suc Khoe List
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataForYeuCauNhanSuKhamSucKhoeGridAsync(QueryInfo queryInfo, bool exportExcel);

        Task<GridDataSource> GetTotalPageForYeuCauNhanSuKhamSucKhoeGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync(QueryInfo queryInfo);
    }
}
