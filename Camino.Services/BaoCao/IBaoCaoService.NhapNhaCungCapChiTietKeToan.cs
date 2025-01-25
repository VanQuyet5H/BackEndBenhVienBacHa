using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemVo>> GetTatCaKhoNhapChiTietKeToans(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetDataNhapNhaCungCapChiTietKeToanForGrid(NhapNhaCungCapChiTietKeToanDuocQueryInfoVo queryInfo);
        byte[] ExportBaoCaoNhapCungCapChiTietKeToan(GridDataSource gridDataSource, NhapNhaCungCapChiTietKeToanDuocQueryInfoVo query);
    }
}
