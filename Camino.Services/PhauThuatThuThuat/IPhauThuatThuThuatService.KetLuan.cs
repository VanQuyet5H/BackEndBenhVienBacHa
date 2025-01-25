using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using System;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<bool> IsExistDoubleKetLuanVaTheoDoi(long yeuCauTiepNhanId, long phongBenhVienId);
        Task<bool> KiemTraTatCaYeuCauDichVuKyThuatPTTT(long yeuCauTiepNhanId, long phongBenhVienId);
        Task<bool> CheckHasPhauThuat(long noiThucHienId, long yctnId, bool isExistTheoDoi);
        Task ChuyenGiaoSauPhauThuatThuThuat(long yeuCauTiepNhanId, long theoDoiSauPhauThuatThuThuatId, long? nhanVienKetLuanId, long phongBenhVienId, bool IsChuyenGiaoTuTuongTrinh, DateTime? thoiDiemKetThucTheoDoi);
        Task CapNhatTheoDoiPhauThuatThuThuatChoYeuCauDichVuKyThuat(long yeuCauTiepNhanId, long theoDoiSauPhauThuatThuThuatId, EnumTrangThaiYeuCauDichVuKyThuat? enumTrangThaiYeuCauDichVuKyThuat, long? nhanVienKetLuanId, long phongBenhVienId, bool IsChuyenGiaoTuTuongTrinh, DateTime? thoiDiemKetThucTheoDoi);
        bool IsPhauThuat(long dichVuKyThuatBenhVienId);
        Task KhongTuongTrinh(long yeuCauTiepNhanId, long yeuCauDichVuKyThuatId, long phongBenhVienId);
        Task HoanThanhTuongTrinhLai(long phongBenhVienId, long yeuCauTiepNhanId);
    }
}