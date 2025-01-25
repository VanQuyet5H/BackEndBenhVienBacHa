using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<string> GetTemplateKhamTheoDoi();
        Task<bool> CheckPreviousKhamTheoDoi(long theoDoiSauPhauThuatThuThuatId);
        Task BenhNhanTuVongKhiTheoDoi(long? yeuCauTiepNhanId, long? theoDoiSauPhauThuatThuThuatId, long? nhanVienKetLuanId, long? phongBenhVienId, EnumThoiGianTuVongPTTTTheoNgay? thoiGianTuVong, EnumTuVongPTTTTheoNgay? tuVong);
    }
}
