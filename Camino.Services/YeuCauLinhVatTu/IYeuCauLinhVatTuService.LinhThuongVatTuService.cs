using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Camino.Services.YeuCauLinhVatTu
{
    public partial interface IYeuCauLinhVatTuService
    {
        Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetKhoLinh(DropDownListRequestModel queryInfo);
        Task<NhanVienYeuCauVo> GetCurrentUser();
        Task<List<VatTuLookupVo>> GetVatTu(DropDownListRequestModel queryInfo); 
        //Task<List<VatTuLookupVo>> GetVatTuOld(DropDownListRequestModel queryInfo);
        Task<bool> CheckVatTuExists(long? vatTuBenhVienId, bool? laVatTuBHYT, List<VatTuGridViewModelValidator> vatTuBenhVienIds);
        Task<bool> CheckSoLuongTonVatTuGridVo(long? vatTuBenhVienId, double? soLuongYeuCau, long khoXuatId, bool laVatTuBHYT);
        Task<bool> CheckSoLuongTonVatTu(long? vatTuBenhVienId, double? soLuongYeuCau, bool? duocDuyet, double? soLuongCoTheXuat, long? khoXuatId, bool? laVatTuBHYT, bool? isValidator);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long phieuLinhId);
        string InPhieuLinhThuongVatTu(PhieuLinhThuongVatTu phieuLinhThuongVatTu);
        Task CheckPhieuLinhDaDuyetHoacDaHuy(long phieuLinhId);

        LinhThuongVatTuGridVo LinhThuongVatTuGridVo(LinhThuongVatTuGridVo model);
        double GetSoLuongTonVatTuGridVo(long vatTuBenhVienId, long khoXuatId, bool laVatTuBHYT);
        #region in bu xem truoc
        string InPhieuLinhBuVatTuXemTruoc(PhieuLinhThuongVatTuXemTruoc phieuLinhThuongVatTuXemTruoc);
        #endregion
    }
}
