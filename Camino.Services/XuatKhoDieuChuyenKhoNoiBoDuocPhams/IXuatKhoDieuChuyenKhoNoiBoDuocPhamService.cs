using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;

namespace Camino.Services.XuatKhoDieuChuyenKhoNoiBoDuocPhams
{
    public interface IXuatKhoDieuChuyenKhoNoiBoDuocPhamService : IMasterFileService<YeuCauDieuChuyenDuocPham>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoTongCap2VaNhaThuocs(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetNguoiNhap(DropDownListRequestModel model);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauDieuChuyenDuocPhamId);
        Task<GridDataSource> GetDataForGridAsyncDuocPhamDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDaTaoYeuCau(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDaTaoYeuCau(QueryInfo queryInfo);
        Task<List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>> YeuCauDieuChuyenDuocPhamChiTietHienThis(long yeuCauDieuChuyenDuocPhamId);

        Task XuLyThemDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTiets);
        Task CheckPhieuYeuCauDieuChuyenDaDuyetHoacDaHuy(long yeuCauDieuChuyenDuocPhamId);
        Task XuLyCapNhatDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, List<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTiets);
        Task XuLyXoaDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, bool isUpdate = true);

        Task XuLyDuyetDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham);
        Task XuLyKhongDuyetDieuChuyenThuocAsync(YeuCauDieuChuyenDuocPham yeuCauDieuChuyenDuocPham, string lyDoKhongDuyet);

        string InPhieuXuatKhoDuocPhamDieuChuyen(YeuCauDieuChuyenDuocPhamDataVo yeuCauDieuChuyenDuocPhamDataVo);

    }
}
