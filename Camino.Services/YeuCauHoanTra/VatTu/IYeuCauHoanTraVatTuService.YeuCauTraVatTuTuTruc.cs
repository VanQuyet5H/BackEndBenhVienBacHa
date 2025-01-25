using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Camino.Services.YeuCauHoanTra.VatTu
{

    public partial interface IYeuCauHoanTraVatTuService
    {
        Task<GridDataSource> GetDataForGridAsyncVatTuTuTrucDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncVatTuTuTrucDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsyncDaDuyetVatTu(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyetVatTu(QueryInfo queryInfo);
        Task<TrangThaiDuyetVo> GetTrangThaiYeuCauHoanTraVT(long phieuLinhId);
        Task<List<YeuCauTraVatTuGridVo>> YeuCauHoanTraVatTuChiTiets(long yeuCauTraVatTuId);
        Task XuLyThemHoacCapNhatHoanTraVTAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraVatTuTuTrucChiTietVo> yeuCauTraVatTuTuTrucChiTietVos, bool isCreate = true);
        //Task XuLyCapNhatHoanTraVTAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraVatTuTuTrucChiTietVo> yeuCauTraVatTuTuTrucChiTietVos);
    }
}
