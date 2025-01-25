using System.Threading.Tasks;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IXacNhanNoiTruVaNgoaiTruBHYTService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<GridDataSource> GetDataXacNhanNoiTruVaNgoaiTruHoanThanh(FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo queryInfo, bool forExportExcel);        
        byte[] ExportXacNhanNoiTruVaNgoaiTruHoanThanh(GridDataSource gridDataSource, FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo query);
    }
}