using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.TonKhos
{
    public partial interface ITonKhoService
    {
        Task<GridDataSource> GetDanhSachVatTuSapHetHanForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalVatTuSapHetHanPagesForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        string GetVatTuSapHetHanHTML(string searchString);
    }
}