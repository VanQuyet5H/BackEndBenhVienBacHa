using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    public partial interface IYeuCauHoanTraDuocPhamService
    {
        Task<GridDataSource> GetDataForGridAsyncDuocPhamTuTrucDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamTuTrucDaChon(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo);

        Task<TrangThaiDuyetVo> GetTrangThaiYeuCauHoanTraDuocPham(long phieuLinhId);
        Task XuLyThemHoacCapNhatHoanTraThuocAsync(YeuCauTraDuocPham yeuCauTraDuocPham, List<YeuCauTraDuocPhamTuTrucChiTietVo> yeuCauTraDuocPhamTuTrucChiTiets, bool isCreate = true);
        //Task XuLyCapNhatHoanTraThuocAsync(YeuCauTraDuocPham yeuCauTraDuocPham, List<YeuCauTraDuocPhamTuTrucChiTietVo> yeuCauTraDuocPhamTuTrucChiTiets);

        Task<List<YeuCauTraDuocPhamGridVo>> YeuCauTraDuocPhamTuTrucChiTiets(long yeuCauTraDuocPhamId);

    }
}
