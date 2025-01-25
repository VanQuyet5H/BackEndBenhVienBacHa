using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<GridDataSource> GetDanhSachDieuTriNoiTruForGrid(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalPagesDanhSachDieuTriNoiTruForGrid(QueryInfo queryInfo);
    }
}
