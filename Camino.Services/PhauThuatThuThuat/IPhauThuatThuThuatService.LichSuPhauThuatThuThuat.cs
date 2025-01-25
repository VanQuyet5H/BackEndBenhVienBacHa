using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<GridDataSource> GetDataForGridAsyncLichSuPhauThuatThuThuat(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuPhauThuatThuThuat(QueryInfo queryInfo);
        Task<ThongTinBenhNhanPTTTVo> GetThongTinBenhNhan(long yeuCauDichVuKyThuatId);
        Task<LichSuKetLuanPTTTVo> GetThongTinLichSuKetLuanPTTT(long yeuCauDichVuKyThuatId);
        Task<GridDataSource> GetDataForGridAsyncChiSoSinhHieuPTTT(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncChiSoSinhHieuPTTT(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncLichSuKhamCacCoQuan(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamCacCoQuan(QueryInfo queryInfo);
        Task<LichSuEkipPTTTVo> GetThongTinLichSuEkipPTTT(long yeuCauDichVuKyThuatId);
        Task<bool> KiemTraCoDichVuHuy(long yeuCauTiepNhanId);
        Task<GridDataSource> GetDataForGridAsyncLichSuEkipBacSi(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuEkipBacSi(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncLichSuCLS(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuCLS(QueryInfo queryInfo);
        Task<List<DichVuPTTTsLookupItemVo>> GetDichVuPTTTs(DropDownListRequestModel model);
        Task<GridDataSource> GetDataForGridAsyncLichSuDVPTTTKhongThucHien(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuDVPTTTKhongThucHien(QueryInfo queryInfo);
        Task<LichSuDichVuKyThuatDaTuongTrinhPTTT> GetDichVuDaTuongTrinh(LichSuDichVuKyThuatDaTuongTrinhVo lichSuDichVuKyThuatDaTuongTrinhVo);

    }
}
