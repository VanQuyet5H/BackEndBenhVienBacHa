using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.PhieuInXetNghiem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomDichVuCLSPTTT(DropDownListRequestModel model);
        Task XuLyThemYeuCauDichVuKyThuatMultiselectAsync(ChiDinhDichVuKyThuatMultiselectVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet);
        Task<int> GetSoLuongDichVuKyThuatDaHoanThanh(long yeuCauTiepNhanId);
        Task<int> GetSoLuongDichVuKyThuat(long yeuCauTiepNhanId);
        Task<(int, int)> GetTienTrinhHoanThanhDichVuKyThuat(long yeuCauTiepNhanId);
        Task<List<PhauThuatThuThuatCanLamSanGridVo>> GetDichVuKyThuatsByYeuCauTiepNhan(long yeuCauTiepNhanId, long? phieuDieuTriId = null);
        Task<List<PhauThuatThuThuatCanLamSanGridVo>> GetDichVuKyThuatsByYeuCauTiepNhanVer2(long yeuCauTiepNhanId, long? phieuDieuTriId = null);
        void ApDungThoiGianDienBienDichVuKhamVaKyThuat(List<long> yeuCauKhamBenhIds, List<long> yeuCauDichVuKyThuatIds, DateTime? thoiGianDienBien);
        string InBaoCaoChiDinh(long yeuCauTiepNhanId, string hostingName, List<ListDichVuChiDinhCLSPTTT> lst
            , long inChungChiDinh, bool KieuInChung, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId,List<long> listChonLoaiPhieuIn);

        Task<int> GetSoLuongDichVuKyThuatDaHoanThanhCanLamSang(long yeuCauTiepNhanId);
        Task<int> GetSoLuongDichVuKyThuatCanLamSang(long yeuCauTiepNhanId);
        bool HuyYeuCauChayLaiXetNghiemTheoNhomDichVu(long phienXetNghiemId, long nhomDichVuBenhVienId);
        bool IsGoiChayLaiXetNghiem(long phienXetNghiemId, long nhomDichVuBenhVienId);
        Task<List<LichSuYeuCauChayLai>> LichSuYeuCauChayLaiXetNghiem(LichSuChayLaiXetNghiemVo lichSuChayLaiXetNghiem);

        #region BVHD-3860
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId);


        #endregion
    }
}
