using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TongHopDuTruMuaKSNKTaiGiamDocs;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    public partial interface IYeuCauMuaDuTruKiemSoatNhiemKhuanService
    {
        Task<GridDataSource> GetDataGiamDocForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetGiamDocTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildChuaDuyetAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildDuyetAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDuyetDetail(QueryInfo queryInfo);

        Task Approve(long id);

        Task ApproveForm(DuTruGiamDocKSNKApproveGridVo duTruGiamDoc);

        Task Decline(DuTruGiamDocKSNKApproveGridVo duTruGiamDoc);

        Task<DuTruMuaVatTuKhoDuoc> GetDuTruMuaVatTuKhoDuocByIdAsync(long duTruMuaVatTuKhoDuocId);
        
    }
}
