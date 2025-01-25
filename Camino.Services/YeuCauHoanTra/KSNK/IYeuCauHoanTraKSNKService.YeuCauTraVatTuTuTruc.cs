using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;

namespace Camino.Services.YeuCauHoanTra.KSNK
{

    public partial interface IYeuCauHoanTraKSNKService
    {
        Task<GridDataSource> GetDataForGridAsyncDpVtKSNKTuTrucDaChon(QueryInfo queryInfo);
        //Task<GridDataSource> GetDataForGridAsyncKSNKTuTrucDaChon(QueryInfo queryInfo);
        //Task<GridDataSource> GetTotalPageForGridAsyncKSNKTuTrucDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsyncDaDuyetKSNK(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyetKSNK(QueryInfo queryInfo);
        Task<TrangThaiDuyetVo> GetTrangThaiYeuCauHoanTraKSNK(long phieuLinhId);
        Task<List<YeuCauTraKSNKGridVo>> YeuCauHoanTraKSNKChiTiets(long yeuCauTraVatTuId);
        Task<List<YeuCauTraKSNKGridVo>> YeuCauTraDuocPhamTuTrucChiTiets(long yeuCauTraDuocPhamId);

        Task<ThemHoanTraKSNKResultVo> XuLyThemHoanTraKSNKAsync(YeuCauTraVatTu yeuCauTraVatTu,YeuCauTraDuocPham yeuCauTraDuocPham, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets);

        Task XuLyCapNhatHoanTraDuocPhamKSNKAsync(YeuCauTraDuocPham yeuCauTraDuocPham,List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVTTuTrucChiTiets);
        Task XuLyCapNhatHoanTraVatTuKSNKAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVatTuTuTrucChiTietVos);
        //Task XuLyThemHoacCapNhatHoanTraKSNKAsync(YeuCauTraVatTu yeuCauTraVatTu, List<YeuCauTraKSNKTuTrucChiTietVo> yeuCauTraVatTuTuTrucChiTietVos, bool isCreate = true);  
        Task XoaYeuCauHoanTraDuocPhamAsync(long yeuCauHoanTraDuocPhamId);

        Task<GridDataSource> GetDataForGridChildDuocPhamAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsyncDaDuyet(QueryInfo queryInfo);

        Task XoaYeuCauHoanTraVatTuAsync(long yeuCauHoanTraVatTuId);
    }
}
