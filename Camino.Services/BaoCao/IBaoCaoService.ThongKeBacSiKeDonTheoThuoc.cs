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
        List<LookupItemBacSiTemplateVo> GetTatCaBacSiKeDonTheoThuoc(DropDownListRequestModel queryInfo);
        List<DuocPhamBenhVienLookupItemVo> GetTatCaThuocBenhVien(DropDownListRequestModel queryInfo);
        GridDataSource GetDataThongKeBacSiKeDonTheoThuocForGrid(ThongKeBacSiKeDonTheoThuocQueryInfo queryInfo);
        byte[] ExportThongKeBacSiKeDonTheoThuoc(GridDataSource gridDataSource, ThongKeBacSiKeDonTheoThuocQueryInfo query);
    }
}