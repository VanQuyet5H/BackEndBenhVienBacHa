using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachChoPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId = 0);
        Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachDangPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId = 0);
        Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachTheoDoiPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId = 0);
        Task<YeuCauTiepNhan> GetThongTinBenhNhanDangTuongTrinh(long phongKhamHienTaiId);
        Task<YeuCauTiepNhan> GetThongTinBenhNhanTheoYeuCauTiepNhan(long yeuCauTiepNhanId);
        Task<EnumTrangThaiPhauThuatThuThuat> GetTrangThaiPhauThuatThuThuat(long yeuCauTiepNhanId, long phongKhamHienTaiId);
        Task<List<PhongBenhVienHangDoi>> GetPhongBenhVienHangDoiTuongTrinhLai(long yeuCauTiepNhanId, long phongKhamHienTaiId);
        Task<YeuCauTiepNhan> GetThongTinBenhNhanTiepTheo(long phongKhamHienTaiId, long yeuCauTiepNhanHienTaiId);
        Task<bool> KiemTraConYeuCauDichVuKyThuatTaiPhong(long phongBenhVienId, long yeuCauTiepNhanId);
        Task<bool> KiemTraCoBenhNhanKhacDangKhamTrongPhong(long currentUserId, long phongBenhVienId);
        Task BatDauKhamBenhNhanPTTT(long yeuCauTiepNhanDangKhamId, long yeuCauTiepNhanBatDauKhamId, long phongBenhVienId);
        Task HuyKhamBenhNhanPTTT(long phongBenhVienId);
        Task<bool> CoDuocHuongBHYT(long yeuCauDichVuKyThuatId);
    }
}
