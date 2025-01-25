using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService
    {
        Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetKhoLinh(DropDownListRequestModel queryInfo);
        Task<NhanVienYeuCauVo> GetCurrentUser();
        Task<NhanVienDuyetVo> GetNhanVienDuyet(long? id);
        Task<List<DuocPhamLookupVo>> GetDuocPham(DropDownListRequestModel queryInfo);
        //Task<List<DuocPhamLookupVo>> GetDuocPhamOld(DropDownListRequestModel queryInfo);
        LinhThuongDuocPhamVo LinhThuongDuocPhamGridVo(LinhThuongDuocPhamVo model);
        Task<bool> CheckSoLuongTonDuocPham(long duocPhamBenhVienId, bool laDuocPhamBHYT, long khoXuatId, double? soLuongYeuCau, bool? duocDuyet, double? soLuongCoTheXuat, bool? isValidator);
        Task<bool> CheckSoLuongTonDuocPhamLinhBu(long? duocPhamBenhVienId, bool? laDuocPhamBHYT, long? khoXuatId, double? soLuongYeuCau);
        Task<bool> CheckSoLuongTonDuocPhamDuocPhamGridVo(long? duocPhamBenhVienId, double? soLuongYeuCau, long khoXuatId, bool laDuocPhamBHYT);
        Task<bool> CheckDuocPhamExists(long? duocPhamBenhVienId, bool? laDuocPhamBHYT, List<DuocPhamGridViewModelValidator> duocPhamBenhVienIds);
        string InPhieuLinhThuongDuocPham(PhieuLinhThuongDuocPham phieuLinhThuongDuoc);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long phieuLinhId);
        Task CheckPhieuLinhDaDuyetHoacDaHuy(long phieuLinhId);
        Task<bool> CheckKhoNhanVienQuanLy(long khoNhapId);
        double GetSoLuongTonDuocPhamGridVo(long duocPhamBenhVienId, long khoXuatId, bool laDuocPhamBHYT);
        string InPhieuLinhBuDuocPhamXemTruoc(XemPhieuLinhBuDuocPham phieuLinhThuongDuoc);
        List<LookupItemTextVo> GetAllMayXetNghiemYeuCauLinh(DropDownListRequestModel queryInfo, DuocPhamBenhVienMayXetNghiemJson duocPhamBenhVienMayXetNghiemJson);
        string GetTrangThaiPhieuLinh(string danhSachMayXetNghiemIdStrs);
    }
}
