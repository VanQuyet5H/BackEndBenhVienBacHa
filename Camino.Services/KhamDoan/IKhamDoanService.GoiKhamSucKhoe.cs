using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachGoiKhamSucKhoe(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachGoiKhamSucKhoe(QueryInfo queryInfo);
        Task<string> GetNoiThucHienString(long phongBenhVienId);
        Task CheckGoiKhamDaKetThucHoacDaXoa(long goiKhamId);
        Task<bool> IsTenGoiKhamExists(string tenGoiKham = null, long goiKhamId = 0, long? hopDongKhamSucKhoeId = 0);
        Task<bool> IsMaGoiKhamExists(string maGoiKham = null, long goiKhamId = 0, long? hopDongKhamSucKhoeId = 0);
        Task<bool> CheckDichVuExists(long? dichVuKyThuatBenhVienId, bool laDichVuKham, List<long> dichVuKhamBenhIds, List<long> dichVuKyThuatIds);
        bool CapNhatSoLuongNhanVienKhamTrongHopDong(long hopDongKhamSucKhoeId);
    }
}
