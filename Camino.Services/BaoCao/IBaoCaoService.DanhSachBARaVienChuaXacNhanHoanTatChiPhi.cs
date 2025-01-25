using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<LookupItemVo> GetTatKhoaChoDanhSachBARaVienChuaXacNhanHoanTatChiPhi(DropDownListRequestModel queryInfo);

        Task<GridDataSource> GetDataDanhSachBARaVienChuaXacNhanHoanTatChiPhiForGrid(DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo queryInfo);

        byte[] ExportDanhSachBARaVienChuaXacNhanHoanTatChiPhi(GridDataSource gridDataSource, DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo query);
    }
}