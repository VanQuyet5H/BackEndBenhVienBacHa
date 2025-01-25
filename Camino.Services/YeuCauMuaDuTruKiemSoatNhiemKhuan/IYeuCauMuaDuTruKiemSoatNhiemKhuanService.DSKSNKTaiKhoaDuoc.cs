using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoaDuoc;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    public partial interface IYeuCauMuaDuTruKiemSoatNhiemKhuanService
    {

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocToTalPageChildForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocToTalPageChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataChildKhoaForGridAsync(QueryInfo queryInfo);

        List<DuTruMuaKSNKChiTietGridVo> GetDuTruMuaKSNKChiTiet(long idDuTruMua);
        List<DuTruMuaKSNKKhoaChiTietGridVo> GetDuTruMuaKSNKKhoaChiTiet(long idDuTruMuaKhoa);
        Task<DuTruMuaVatTuTheoKhoa> GetDuTruKSNKTheoKhoaByIdAsync(long duTruMuaVatTuKhoaId);

        bool? GetTrangThaiDuyet(long Id, long? khoaId, long? khoId);
        DuTruMuaKSNKChiTietViewGridVo GetDataUpdate(long Id, bool typeLoaiKho);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocDaXuLyForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocDaXuLyToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocChildDaXuLyChildChildChildTotalPageForGridAsync(QueryInfo queryInfo);

        DuTruMuaKSNKChiTietGoiViewGridVo GetDuTruMuaKSNKChiTietGoiView(long idDuTruMuaKhoaDuoc, long tinhTrang);
        Task<GridDataSource> GetDuTruMuaKSNKChiTietGoiViewChild(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDuTruMuaKSNKTaiKhoaDuocTuChoiToTalPageChildChildForGridAsync(QueryInfo queryInfo);

        DuTruMuaKSNKChiTietGoiViewGridVo GetDuTruMuaKSNKChiTietGoi(long idDuTruMua);

        string InPhieuDuTruMuaTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa);
        string InPhieuDuTruMuaDuocPhamTaiKhoaDuoc(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa);

        Task<long> GuiDuTruMuaKSNKTaiKhoaDuoc(DuTruMuaKSNKChiTietGoiViewGridVo duTruMuaVatTuTaiKhoaDuoc);

    }
}
