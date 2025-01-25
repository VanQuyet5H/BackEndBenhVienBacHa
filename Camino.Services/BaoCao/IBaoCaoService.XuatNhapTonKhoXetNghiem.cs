using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<LookupItemVo> GetTatCaTuHoaChat(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetDataXuatNhapTonKhoXetNghiemForGrid(XuatNhapTonKhoXetNghiemQueryInfo queryInfo);
        byte[] ExportXuatNhapTonKhoXetNghiem(GridDataSource gridDataSource, XuatNhapTonKhoXetNghiemQueryInfo query);
    }
}