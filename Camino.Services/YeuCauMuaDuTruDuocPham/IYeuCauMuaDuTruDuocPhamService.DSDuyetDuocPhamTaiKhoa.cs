using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    public partial interface IYeuCauMuaDuTruDuocPhamService
    {
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaForGridAsync(QueryInfo queryInfo);

        #region Danh sách đang chờ xử lý chi tiết

        Task<GridDataSource> GetDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);

        #endregion

        #region DS THDT đã xử lý chi tiết
        Task<GridDataSource> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        #endregion

        #region DS THDT đã xử lý chi tiết child
        Task<GridDataSource> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo);
        #endregion

        #region DS THDT Chi tiết phiếu mua dược phẩm dự trù
        Task<GridDataSource> GetDuTruMuaDuocPhamChiTietForGridAsyncChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamChiTietForGridAsyncChild(QueryInfo queryInfo);
        #endregion

        Task<bool> TuChoiDuyetTaiKhoa(ThongTinLyDoHuyDuyetTaiKhoa thongTinLyDoHuyNhapKhoDuocPham);
        DuTruMuaDuocPhamViewModel GetThongTinDuTruDuocPhamTaiKhoa(long duTruDuocPhamId);
        DuTruMuaDuocPhamViewModel GetThongTinDuTruDuocPhamTaiKhoaDaXuLy(long duTruDuocPhamId);

        Task<bool> DuyetTaiKhoa(DuyetDuTruMuaDuocPhamViewModel model);
        Task<bool> HuyDuyetTaiKhoa(long id);
        GetThongTinGoiTaiKhoa GetThongTinGoiTaiKhoa(long phongBenhVienId);

        #region DS THDT Từ Chối
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo);
        #endregion

        #region  DS THDT Từ Chối Chi tiết
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync(QueryInfo queryInfo);
        #endregion

        Task<GridDataSource> GetDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        long GoiThongTinTaiKhoa(DuyetDuTruMuaDuocPhamViewModel model);
        string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa);

        Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo);

    }
}
    