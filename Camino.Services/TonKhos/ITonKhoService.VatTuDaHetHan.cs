using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.ValueObject.TonKhos.VatTuDaHetHan;

namespace Camino.Services.TonKhos
{
    public partial interface ITonKhoService
    {
        Task<GridDataSource> GetDanhSachVatTuDaHetHanForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDanhSachVatTuDaHetHanTotalPageForGridAsync(QueryInfo queryInfo);
        string XemVatTuDaHetHan(InVatTuDaHetHan inVatTuDaHetHan);
        Task<List<LookupItemVo>> GetKhoVatTu(LookupQueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoVatTusWithoutTatCa(LookupQueryInfo queryInfo);
        Task<LookupItemVo> GetFirstKhoVatTu();
    }
}
