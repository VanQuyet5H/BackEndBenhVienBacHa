using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial interface IYeuCauLinhKSNKService
    {
        Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetKhoLinh(DropDownListRequestModel queryInfo);
        Task<Core.Domain.ValueObject.YeuCauKSNK.NhanVienYeuCauVo> GetCurrentUser();
        Task<List<KSNKLookupVo>> GetKSNK(DropDownListRequestModel queryInfo);
        Task<bool> CheckKSNKExists(long? KSNKBenhVienId, bool? laKSNKBHYT, List<KSNKGridViewModelValidators> KSNKBenhVienIds, bool laDpHayVt);
        Task<bool> CheckSoLuongTonKSNKGridVo(long? KSNKBenhVienId, double? soLuongYeuCau, long khoXuatId, bool laKSNKBHYT,bool laDpHayVt);
        Task<bool> CheckSoLuongTonKSNK(long? KSNKBenhVienId, double? soLuongYeuCau, bool? duocDuyet, double? soLuongCoTheXuat, long? khoXuatId, bool? laKSNKBHYT, bool? isValidator, bool loaiDuocPhamHayVatTu);
        Task<Core.Domain.ValueObject.YeuCauKSNK.TrangThaiDuyetVos> GetTrangThaiPhieuLinh(long phieuLinhId, bool loai);
        string InPhieuLinhThuongKSNK(PhieuLinhThuongDPVTModel phieuLinhThuongKSNK);
        Task CheckPhieuLinhDaDuyetHoacDaHuy(long phieuLinhId);

        LinhThuongKSNKGridVo LinhThuongKSNKGridVo(LinhThuongKSNKGridVo model);
        double GetSoLuongTonKSNKGridVo(long KSNKBenhVienId, long khoXuatId, bool laKSNKBHYT);
        #region in bu xem truoc
        string InPhieuLinhBuKSNKXemTruoc(PhieuLinhThuongKSNKXemTruoc phieuLinhThuongKSNKXemTruoc);
        #endregion
        List<long> GetIdsYeuCauVT(long KhoLinhTuId, long KhoLinhVeId, long vatTuBenhVienId);
        DateTime GetDateTimeVatTu(long YeuCauVatTuBenhVienId);
        string GetNamKhoLinh(long khoLinhId);
    }
}
