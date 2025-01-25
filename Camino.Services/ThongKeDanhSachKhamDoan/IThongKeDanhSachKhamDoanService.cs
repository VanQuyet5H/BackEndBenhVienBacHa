using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ThongKeDanhSachKhamDoan;

namespace Camino.Services.BaoCaoVatTus
{
    public interface IThongKeDanhSachKhamDoan : IMasterFileService<HopDongKhamSucKhoeNhanVien>
    {
        List<LookupItemVo> GetCongTyKhamSucKhoe(LookupQueryInfo queryInfo);
        List<LookupItemVo> GetSoHopDongTheoCongTy(LookupQueryInfo queryInfo, long? congTyKhamSucKhoeId);
        byte[] ExportBaoCaoThongKeDichVuKhamSucKhoe(QueryInfo queryInfo, List<ThongKeDanhSachKhamDoanVo> datas);
        Task<GridDataSource> GetDataThongKeDichVuKhamSucKhoe(QueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetDataTotalThongKeDichVuKhamSucKhoe(QueryInfo queryInfo);
    }
}
