using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;

namespace Camino.Services.YeuCauMuaDuTruVatTu
{
    public partial interface IYeuCauMuaDuTruVatTuService
    {
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalDuTruMuaVatTuTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDuTruMuaVatTuTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaVatTuTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<bool> TuChoiDuyetTaiKhoa(ThongTinLyDoHuyDuyetVatTuTaiKhoa thongTinLyDoHuyNhapKhoVatTu);
        DuTruMuaVatTuViewModel GetThongTinDuTruVatTuTaiKhoa(long duTruVatTuId);
        Task<bool> DuyetTaiKhoa(DuyetDuTruMuaVatTuViewModel model);
        Task<bool> HuyDuyetTaiKhoa(long id);
        GetThongTinGoiVatTuTaiKhoa GetThongTinGoiTaiKhoa(long phongBenhVienId);
        Task<GridDataSource> GetDataDuTruMuaVatTuTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaVatTuTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo);

        GridDataSource GetDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo);
        GridDataSource GetDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        long GoiThongTinTaiKhoa(DuyetDuTruMuaVatTuViewModel model);
        string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaVatTuTaiKhoa phieuInDuTruMuaTaiKhoa);
        DuTruMuaVatTuViewModel GetThongTinDuTruVatTuTaiKhoaDaXuLy(long duTruVatTuId);


        #region DS THDT đã xử lý
        Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo);
        #endregion

        #region DS THDT đã xử lý chi tiết

        Task<GridDataSource> GetDuTruMuaVatTuTHDTChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaVatTuTHDTChiTietForGridAsync(QueryInfo queryInfo);

        #endregion

        #region DS THDT đã xử lý chi tiết child
        Task<GridDataSource> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo);
        #endregion

        #region DS THDT Chi tiết phiếu mua vật tư dự trù
        Task<GridDataSource> GetDuTruMuaVatTuChiTietForGridAsyncChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaVatTuChiTietForGridAsyncChild(QueryInfo queryInfo);
        #endregion

        #region  DS THDT Từ Chối Chi tiết
        Task<GridDataSource> GetDSTHDTTuChoiChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDSTHDTTuChoiChildForGridAsync(QueryInfo queryInfo);
        #endregion



        Task<GridDataSource> GetDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
    }
}
