using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ToaThuocMau;

namespace Camino.Services.ToaThuocMau
{
    public interface IToaThuocMauService : IMasterFileService<Core.Domain.Entities.Thuocs.ToaThuocMau>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridToaThuocMauChiTietChildAsync(QueryInfo queryInfo, long? toaThuocMauId = null, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridToaThuocMauChiTietChildAsync(QueryInfo queryInfo);
        Task<List<ICDTemplateVos>> GetICDs(DropDownListRequestModel queryInfo);
        Task<List<NhanVienTemplateVos>> GetBacSiKeToas(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetTrieuChungs(DropDownListRequestModel queryInfo);
        Task<List<ChuanDoanTemplateVo>> GetChuanDoans(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamTemplateGridVo>> GetDuocPhams(DropDownListRequestModel queryInfo);
    }
}
