using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        #region Grid
        Task<GridDataSource> GetDataForGridTongHopYLenhAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTongHopYLenhAsync(QueryInfo queryInfo);


        #endregion

        #region Get data

        Task<TongHopYLenhThongTinHanhChinhVo> GetTongHopYLenhThongTinHanhChinhAsync(ChiTietYLenhQueryInfoVo queryInfo);
        Task<TongHopYLenhPhieuDieuTriVo> GetThongTinChiTietYLenhPhieuDieuTriAsync(ChiTietYLenhQueryInfoVo queryInfo);
        #endregion

        #region Kiểm tra data

        Task KiemTraPhieuDieuTriNoiTruByNgayDieuTriAsync(long? noiTruBenhAnId, string ngayDieuTri, long? yeuCauTiepNhanId);


        #endregion

        #region Thêm/xóa/sửa

        Task XuLyLuuDienBienYLenhAsync(TongHopYLenhPhieuDieuTriVo phieuDieuTri);


        #endregion

        #region In phiếu/ xuất excel
        Task<string> InPhieuChamSocAsync(InPhieuChamSocVo detail);
        Task<string> InPhieuChamSocAsyncVer2(InPhieuChamSocVo detail);
        #endregion

        #region Tổng hợp y lệnh từ tất cả các nguồn
        void XuLyTongHopYLenh(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null);
        void XuLyTongHopYLenhDichVuKyThuat(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null);
        void XuLyTongHopYLenhTruyenMau(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null);
        void XuLyTongHopYLenhVatTu(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null);
        void XuLyTongHopYLenhDuocPham(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null);

        #region BVHD-3575
        void XuLyTongHopYLenhKhamBenh(long? noiTruBenhAnId = null, DateTime? ngayDieuTri = null);

        #endregion
        #endregion
    }
}
