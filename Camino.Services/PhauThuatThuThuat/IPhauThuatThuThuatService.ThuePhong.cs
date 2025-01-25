using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<bool> KiemTraThoiGianBatDauThuePhongAsync(long yeuCauDichVuKyThuatId, DateTime? batDau);
        bool KiemTraThoiGianThuePhongVoiNgayHienTai(DateTime? thoiGianThue);
        Task<bool> KiemTraThoiGianBatDauThuePhongTheoTiepNhanAsync(long yeuCauTiepNhanId, DateTime? batDau);

        Task XuLyLuuThongTinThuePhongAsync(ThongTinThuePhongVo thongTinThuePhong);
        Task<ThuePhong> GetThongTinThuePhongTheoDichVuKyThuatAsync(long yeuCauDichVuKyThuatId);
        Task<ThuePhong> GetThongTinThuePhongTheoDichVuKyThuatFoUpdateAsync(long yeuCauDichVuKyThuatId);

        #region Grid lịch sử thuê phòng
        Task<GridDataSource> GetDataForGridLichSuThuePhongAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridLichSuThuePhongAsync(QueryInfo queryInfo);

        #endregion

        #region Chi tiết lịch sử thuê phòng
        Task<LichSuThuePhongThongTinHanhChinhVo> GetThongTinHanhChinh(long yeuCauTiepNhanId);

        Task<List<LookupYeuCauCoThuePhongVo>> GetListDichVuCoThuePhongTheoTiepNhan(DropDownListRequestModel queryInfo);
        #endregion
    }
}
