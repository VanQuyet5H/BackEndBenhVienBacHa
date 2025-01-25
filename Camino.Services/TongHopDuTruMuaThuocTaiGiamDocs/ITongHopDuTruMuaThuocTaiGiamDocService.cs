using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs;

namespace Camino.Services.TongHopDuTruMuaThuocTaiGiamDocs
{
    public interface ITongHopDuTruMuaThuocTaiGiamDocService : IMasterFileService<DuTruMuaDuocPhamKhoDuoc>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildChuaDuyetAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildChuaDuyetTaiGiamDocAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildDuyetAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDuyetDetail(QueryInfo queryInfo);

        Task Approve(long id);

        Task ApproveForm(DuTruGiamDocApproveGridVo duTruGiamDoc);

        Task Decline(DuTruGiamDocApproveGridVo duTruGiamDoc);
    }
}
