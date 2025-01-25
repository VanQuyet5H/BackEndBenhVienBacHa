using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync(QueryInfo queryInfo);

        Task DuyetGiamDocAsync(long id);

        Task TuChoiDuyetGiamDocAsync(TuChoiDuyetKhthRequest tuChoiDuyet);
    }
}
