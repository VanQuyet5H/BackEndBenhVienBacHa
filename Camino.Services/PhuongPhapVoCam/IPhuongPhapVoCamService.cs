using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhuongPhapVoCams;

namespace Camino.Services.PhuongPhapVoCam
{
    public interface IPhuongPhapVoCamService : IMasterFileService<Core.Domain.Entities.PhuongPhapVoCams.PhuongPhapVoCam>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<PhuongPhapVoCamDataResult> DeletePhuongPhapVoCamMultiAsync(long[] modelsDeletesViewModel);

        Task<bool> IsMaExists(string ma, long id);
    }
}
