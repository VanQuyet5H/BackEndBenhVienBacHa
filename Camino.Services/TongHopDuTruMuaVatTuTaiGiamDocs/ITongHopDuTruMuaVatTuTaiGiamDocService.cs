using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaVatTuTaiGiamDocs;

namespace Camino.Services.TongHopDuTruMuaVatTuTaiGiamDocs
{
    public interface ITongHopDuTruMuaVatTuTaiGiamDocService : IMasterFileService<DuTruMuaVatTuKhoDuoc>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildChuaDuyetAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildChuaDuyetTaiGiamDocAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildDuyetAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDuyetDetail(QueryInfo queryInfo);

        Task Approve(long id);

        Task ApproveForm(DuTruGiamDocVatTuApproveGridVo duTruGiamDoc);

        Task Decline(DuTruGiamDocVatTuApproveGridVo duTruGiamDoc);
    }
}
