using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.GoiBaoHiemYTe
{
    partial interface IGoiBaoHiemYTeService : IMasterFileService<YeuCauTiepNhan>
    {
        #region ĐẨY FILE EXCEL 79a 80a

        Task<GridDataSource> GetDataDanhSach79AForGrid(ExcelFile7980AQueryInfo queryInfo);      
        byte[] ExportDanhSachDayFile7980(ExcelFile7980AQueryInfo queryInfo);     

        #endregion

    }
}
