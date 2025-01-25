using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauTraVatTuTuBenhNhan
{
    public interface IYeuCauTraVatTuTuBenhNhanService : IMasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        //Grid
        Task<GridDataSource> GetDataForGridAsyncVatTuChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncVatTuChild(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncBenhNhanChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanChild(QueryInfo queryInfo);
        //Create and Update
        Task<GridDataSource> GetDataForGridAsyncVatTuTheoKho(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncVatTuTheoKho(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo);

        //Thong Tin
        Task<List<LookupItemVo>> GetKhoVatTuCap2(DropDownListRequestModel queryInfo);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuTraVatTu(long phieuTraId);
        Task<List<YeuCauTraVatTuTuBenhNhanChiTiet>> YeuCauTraVatTuTuBenhNhanChiTiets(long yeuCauVatTuBenhVienId, long vatTuBenhVienId);
        Task XoaPhieuTraVatTu(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan entity);
        string InPhieuYeuCauTraVatTuTuBenhNhan(PhieuTraVatTu phieuTraVatTu);

    }
}
