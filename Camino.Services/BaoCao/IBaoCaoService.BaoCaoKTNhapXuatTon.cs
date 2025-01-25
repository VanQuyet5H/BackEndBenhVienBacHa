using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemVo>> GetTatCakhoaNhapXuatTonLookupAsync(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetDataBaoCaoKTNhapXuatTonForGridAsync(BaoCaoKTNhapXuatTonQueryInfo queryInfo);
        byte[] ExportBaoCaoKTNhapXuatTon(GridDataSource datas, BaoCaoKTNhapXuatTonQueryInfo query);        
    }
}
