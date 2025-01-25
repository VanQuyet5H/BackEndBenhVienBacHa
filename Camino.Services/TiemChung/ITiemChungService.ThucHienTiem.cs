using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.TiemChungs;

namespace Camino.Services.TiemChung
{
    public partial interface ITiemChungService
    {
        #region Kiểm tra data
        Task<bool> KiemTraTatCaVacxinDaThucHienAsync(long yeuCauKhamSangLocId, bool isKhacPhongHienTai = true);
        #endregion

        #region get data
        Task<YeuCauDichVuKyThuatKhamSangLocTiemChung> GetThongTinTiemChungTheoPhongThucHienAsync(long yeuCauKhamSangLocId, bool isTheoPhongHienTai = true);
        #endregion

        #region Xử lý lưu data
        Task XuLyLuuThongTinThucHienTiemAsync(YeuCauDichVuKyThuatKhamSangLocTiemChung thongTinTiemChung, bool? IsHoanThanhTiem = false);

        Task CapNhatKhamLaiKhamSangLocTiemChungAsync(KhamTiemChungMoLaiVo thongTinKham);
        Task CapNhatKhamLaiThucHienTiemTheoPhongAsync(KhamTiemChungMoLaiVo thongTinKham);
        Task XuLyHuyTiemVacxinAsync(long yeuCauVacxinId);
        #endregion
    }
}
