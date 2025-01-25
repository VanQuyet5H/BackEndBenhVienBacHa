using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemVo>> GetKhoHangHoa(LookupQueryInfo queryInfo);
        Task<List<LookupItemDuocPhamHoacVatTuVo>> GetKhoDuocPhamVatTuTheoKhoHangHoa(DropDownListRequestModel queryInfo, long khoId);
        Task<GridDataSource> GetDataBaoCaoTheKhoSoChiTietVatTuHangHoaForGridAsync(BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo queryInfo);
        byte[] ExportBaoCaoBangTheKhoSoChiTietVatTuHangHoaGridVo(GridDataSource gridDataSource, BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo query);
    }
}
