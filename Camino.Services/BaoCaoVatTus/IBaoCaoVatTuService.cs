using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCaoVatTus
{
    public interface IBaoCaoVatTuService : IMasterFileService<NhapKhoVatTu>
    {
        Task<List<LookupItemVo>> GetKhoChoBaoCaoVatTu(LookupQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoTonKhoVatTuForGridAsync(BaoCaoTonKhoVatTuQueryInfo queryInfo);
        byte[] ExportBaoCaoTonKhoVatTu(GridDataSource gridDataSource, BaoCaoTonKhoVatTuQueryInfo query);

        Task<List<DuocPhamTheoKhoBaoCaoLookup>> GetKhoDuocPhamVatTuTheoKhoHangHoa(DropDownListRequestModel queryInfo, long khoId);
        Task<GridDataSource> GetDataTheKhoVatTuForGridAsync(BaoCaoTheKhoQueryInfo queryInfo);
        Task<GridDataSource> GetDataTheKhoVatTuForGridChildAsync(QueryInfo queryInfo);
        byte[] ExportTheKhoVatTu(BaoCaoTheKhoQueryInfo queryInfo);
    }
}
