using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    public partial interface IYeuCauMuaDuTruDuocPhamService
    {
        #region chờ xử lý
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        #region child cho xu lý
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageChildForGridAsync(QueryInfo queryInfo);
        #endregion
        #region child child cho xu lý
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocToTalPageChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChildKhoaForGridAsync(QueryInfo queryInfo);
      
            #endregion
            #region get list chi tiet 
            List<DuTruMuaDuocPhamChiTietGridVo> GetDuTruMuaDuocPhamChiTiet(long idDuTruMua);
        List<DuTruMuaDuocPhamKhoaChiTietGridVo> GetDuTruMuaDuocPhamKhoaChiTiet(long idDuTruMuaKhoa);
        Task<DuTruMuaDuocPhamTheoKhoa> GetDuTruDuocPhamTheoKhoaByIdAsync(long duTruMuaDuocPhamKhoaId);
        #endregion
        #region GetDataUpdate
        bool? GetTrangThaiDuyet(long Id, long? khoaId, long? khoId);
        DuTruMuaDuocPhamChiTietViewGridVo GetDataUpdate(long Id, bool typeLoaiKho);
        #endregion
        #endregion
        #region đã xử lý 
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocDaXuLyToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(QueryInfo queryInfo);
        DuTruMuaDuocPhamChiTietGoiViewGridVo GetDuTruMuaDuocPhamChiTietGoiView(long idDuTruMuaKhoaDuoc, long tinhTrang);
        Task<GridDataSource> GetDuTruMuaDuocPhamChiTietGoiViewChild(QueryInfo queryInfo);

        #endregion
        #region Từ chối
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaDuocPhamTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
        #region gởi
        DuTruMuaDuocPhamChiTietGoiViewGridVo GetDuTruMuaDuocPhamChiTietGoi(long idDuTruMua);
        #endregion
        #region In
        string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa);
        #endregion
    }
}
