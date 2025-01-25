using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauTraThuocTuBenhNhan
{
    public interface IYeuCauTraThuocTuBenhNhanService : IMasterFileService<YeuCauTraDuocPhamTuBenhNhan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDuocPhamChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamChild(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDuocPhamTheoKho(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamTheoKho(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncBenhNhanChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanChild(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncBenhNhanTheoKhoChild(QueryInfo queryInfo);
        Task<List<YeuCauTraDuocPhamTuBenhNhanChiTiet>> YeuCauTraDuocPhamTuBenhNhanChiTiet(long yeuCauDuocPhamBenhVienId, long duocPhamBenhVienId);
        Task<ThongTinKhoaHoanTra> ThongTinKhoaHoanTra();
        Task<List<LookupItemVo>> GetKhoDuocPhamCap2(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetKhoaPhong(DropDownListRequestModel queryInfo);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuTraDuocPham(long phieuTraId);
        Task XoaPhieuTraThuoc(YeuCauTraDuocPhamTuBenhNhan entity);
        string InPhieuYeuCauTraThuocTuBenhNhan(PhieuTraThuoc phieuTraThuoc);

    }
}
