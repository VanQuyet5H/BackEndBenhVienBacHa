using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruVatTu
{
    public partial interface IYeuCauMuaDuTruVatTuService 
    {
        #region chờ xử lý
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        #region child cho xu lý
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageChildForGridAsync(QueryInfo queryInfo);
        #endregion
        #region child child cho xu lý
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocToTalPageChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChildKhoaForGridAsync(QueryInfo queryInfo);

        #endregion
        #region get list chi tiet 
        List<DuTruMuaVatTuChiTietGridVo> GetDuTruMuaVatTuChiTiet(long idDuTruMua);
        List<DuTruMuaVatTuKhoaChiTietGridVo> GetDuTruMuaVatTuKhoaChiTiet(long idDuTruMuaKhoa);
        Task<DuTruMuaVatTuTheoKhoa> GetDuTruVatTuTheoKhoaByIdAsync(long duTruMuaVatTuKhoaId);
        #endregion
        #region GetDataUpdate
        bool? GetTrangThaiDuyet(long Id, long? khoaId, long? khoId);
        DuTruMuaVatTuChiTietViewGridVo GetDataUpdate(long Id, bool typeLoaiKho);
        #endregion
        #endregion
        #region đã xử lý 
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocDaXuLyToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(QueryInfo queryInfo);
        DuTruMuaVatTuChiTietGoiViewGridVo GetDuTruMuaVatTuChiTietGoiView(long idDuTruMuaKhoaDuoc, long tinhTrang);
        Task<GridDataSource> GetDuTruMuaVatTuChiTietGoiViewChild(QueryInfo queryInfo);

        #endregion
        #region Từ chối
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaVatTuTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(QueryInfo queryInfo);
        #endregion
        #region gởi
        DuTruMuaVatTuChiTietGoiViewGridVo GetDuTruMuaVatTuChiTietGoi(long idDuTruMua);
        #endregion
        #region In
        string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa);
        #endregion
    }
}
