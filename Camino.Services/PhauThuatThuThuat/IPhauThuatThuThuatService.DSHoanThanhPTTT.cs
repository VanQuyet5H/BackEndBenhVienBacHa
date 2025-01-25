using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<GridDataSource> GetDataForGridAsyncDSHTPhauThuatThuThuat(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDSHTPhauThuatThuThuat(QueryInfo queryInfo);
        Task TuongTrinhLai(TuongTrinhLai tuongTrinhLai);
        Task<GridDataSource> GetDataCanLamSangForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalCanLamSangPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupTrangThaiPtttVo>> GetDichVuHoanThanh(LookupQueryInfo queryInfo, long noiThucHienId, long yctnId, long soLan);
        Task<LichSuKetLuanPTTTVo> GetThongKetLuanDaHoanThanh(long yeuCauDichVuKyThuatId);
        Task<ThongTinBenhNhanPTTTVo> GetThongTinBenhNhanPTTTHoanThanh(long yeuCauDichVuKyThuatId);
        Task<LichSuDichVuKyThuatDaTuongTrinhPTTT> GetDichVuDaTuongTrinhPTTT(LichSuDichVuKyThuatDaTuongTrinhVo lichSuDichVuKyThuatDaTuongTrinhVo);
    }
}
