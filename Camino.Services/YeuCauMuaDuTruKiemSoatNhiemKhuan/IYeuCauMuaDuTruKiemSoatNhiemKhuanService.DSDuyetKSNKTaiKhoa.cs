using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    public partial interface IYeuCauMuaDuTruKiemSoatNhiemKhuanService
    {
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDuTruMuaKSNKTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);

        Task<bool> TuChoiDuyetTaiKhoa(ThongTinLyDoHuyDuyetKSNKTaiKhoa thongTinLyDoHuyNhapKhoVatTu);
        DuTruMuaKSNKViewModel GetThongTinDuTruKSNKTaiKhoa(long duTruVatTuId);
        Task<bool> DuyetTaiKhoa(DuyetDuTruMuaKSNKViewModel model);
        Task<bool> HuyDuyetTaiKhoa(long id);
        GetThongTinGoiKSNKTaiKhoa GetThongTinGoiTaiKhoa(long phongBenhVienId);

        Task<GridDataSource> GetDataDuTruMuaKSNKTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaKSNKTuChoiTaiKhoaForGridAsync(QueryInfo queryInfo);

        GridDataSource GetDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalDanhTruMuaTaiKhoaForGridAsync(QueryInfo queryInfo);
        GridDataSource GetDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        long GoiThongTinTaiKhoa(DuyetDuTruMuaKSNKViewModel model);

        string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa);
        string InPhieuDuTruMuaTaiKhoaDuocPham(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa);

        DuTruMuaKSNKViewModel GetThongTinDuTruKSNKTaiKhoaDaXuLy(long duTruVatTuId);


        #region DS THDT đã xử lý
        Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(QueryInfo queryInfo);
        #endregion

        #region DS THDT đã xử lý chi tiết

        Task<GridDataSource> GetDuTruMuaKSNKTHDTChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaKSNKTHDTChiTietForGridAsync(QueryInfo queryInfo);

        #endregion

        #region DS THDT đã xử lý chi tiết child
        Task<GridDataSource> GetDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild(QueryInfo queryInfo);
        #endregion

        #region DS THDT Chi tiết phiếu mua vật tư dự trù

        Task<GridDataSource> GetDuTruMuaKSNKChiTietForGridAsyncChild(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaKSNKChiTietForGridAsyncChild(QueryInfo queryInfo);

        #endregion

        #region  DS THDT Từ Chối Chi tiết
        Task<GridDataSource> GetDSTHDTTuChoiChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDSTHDTTuChoiChildForGridAsync(QueryInfo queryInfo);
        #endregion



        Task<GridDataSource> GetDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync(QueryInfo queryInfo);
    }
}
