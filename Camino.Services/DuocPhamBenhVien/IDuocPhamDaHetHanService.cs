using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DuocPhamBenhVien
{
    public interface IDuocPhamDaHetHanService : IMasterFileService<NhapKhoDuocPham>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);

        string GetHtml(string search);
    }
}