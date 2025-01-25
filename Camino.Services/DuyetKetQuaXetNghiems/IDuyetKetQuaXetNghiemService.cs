using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.DuyetKetQuaXetNghiems
{
    public interface IDuyetKetQuaXetNghiemService : IMasterFileService<PhienXetNghiem>
    {
        PhienXetNghiem GetChiTietById(long id);
        PhienXetNghiemDataVo GetPhienXetNghiemData(long id);
        List<LookupItemVo> GetTenMayXetNghiems();
        List<LookupItemVo> GetTenNhanViens();
        List<LookupItemVo> GetTenDichVuXetNghiems();
        List<CauHinhNguoiDuyetTheoNhomDichVu> GetCauHinhNguoiDuyetTheoNhomDichVu();
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataChildrenAsync(QueryInfo queryInfo, long? phienXetNghiemId = null, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageChildrenAsync(QueryInfo queryInfo);

        Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiem(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn);

        Task<List<PhieuInXetNghiemModel>> InKetQuaXetNghiem(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn);
        Task<List<PhieuInXetNghiemModel>> InKetQuaXetNghiemNew(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn);

        Task<PhienXetNghiem> DuyetOnGrid(DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid, PhienXetNghiem entity, bool currentCheck, long? idParent);

        PhienXetNghiem DuyetOnLevelOne(DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid, PhienXetNghiem entity, long idCurrent, bool currentCheck);

        Task<PhienXetNghiem> ApproveOnGroup(DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid,
            PhienXetNghiem entity, bool currentCheck, long currentNhomId);

        Task<List<NhomDichVuXetNghiemDuyetVo>> GetNhomDichVuDuyets(long phienXetNghiemId);

        Task<long?> TimKiemPhienXetNghiemGanNhat(PhienXNGanNhatVo phienXNGanNhatVo);

        List<PhieuInXetNghiemModel> InPhieuXetNghiemTheoYeuCauKyThuatVaNhom(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo ketQuaXetNghiemPhieuIn);
        List<PhieuInXetNghiemModel> InPhieuXetNghiemTheoYeuCauKyThuatVaNhomNew(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo ketQuaXetNghiemPhieuIn);
        #region update phiếu in (15072021): chỉ in những dịch vụ được check trên grid
        Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiemManHinhDuyet(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn);
        #endregion chỉ in những dịch vụ được check trên grid
        #region BVHD-3761
        List<PhieuInXetNghiemModel> InDiChXetNghiemTestNhanhKhangNguyenSarsCoV2(List<long> yeuCauDichVuKyThuatIds, long phienXetNghiemId,string hostingName);
        Task<List<long>> GetListYeuCauTrongNhomSars(List<DuyetKqXetNghiemChiTietModel> listIn);
        DichVuKyThuatBenhVienThuocXNGridVo YeuCauDichVuKyThuatIdTheoPhienXetNghiemSars(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo ketQuaXetNghiemPhieuIn);

        #endregion

        Task<List<CauHinhDichVuTestSarsCovid>> CauHinhDichVuTestSarsCovids();
        Task<List<LookupItemVo>> DichVuTestSarsCovids(DropDownListRequestModel queryInfo);
        Task<string> GetNgayDuyetKetQuaCu(KetQuaXetNghiemVo ketQuaXetNghiemVo);
        List<long> GetListPhienXetNghiemIdChoIn(long yeuCauTiepNhanId, long? phienXetNghiemId);

    }
}
