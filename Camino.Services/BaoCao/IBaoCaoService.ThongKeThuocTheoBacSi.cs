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
        List<LookupItemBacSiTemplateVo> GetTatCaBacSi(DropDownListRequestModel queryInfo);
        GridDataSource GetDataThongKeThuocTheoBacSiForGrid(ThongKeThuocTheoBacSiQueryInfo queryInfo);
        byte[] ExportThongKeThuocTheoBacSi(GridDataSource gridDataSource, ThongKeThuocTheoBacSiQueryInfo query);
        Task<string> ThongKeThuocTheoBacSiHTML(List<DanhSachThongKeThuocTheoBacSi> datas);
    }
}