using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachGoiKhamChungSucKhoe(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachGoiKhamChungSucKhoe(QueryInfo queryInfo);

        Task CheckGoiKhamDaKetThucHoacDaXoaChung(long GoiKhamChungId);
        Task<bool> IsTenGoiKhamExistsChung(string tenGoiKhamChung = null, long GoiKhamChungId = 0);
        Task<bool> IsMaGoiKhamExistsChung(string maGoiKhamChung = null, long GoiKhamChungId = 0);
        Task<bool> CheckGoiDichVuExistsChung(long? dichVuKyThuatBenhVienId, List<long> dichVuKhamBenhVaKyThuatIds);
    }
}
