using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.BaoCaoTheKhos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;

namespace Camino.Services.BaoCaoTheKho
{
    public interface IBaoCaoTheKhoService : IMasterFileService<NhapKhoDuocPham>
    {
        Task<GridDataSource> GetDataForGridAsync(BaoCaoTheKhoQueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        byte[] ExportTheKho(BaoCaoTheKhoQueryInfo queryInfo);

        Task<List<LookupItemVo>> GetTatCaKho(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamTheoKhoBaoCaoLookup>> GetDuocPhamTheoKho(DropDownListRequestModel queryInfo);

    }
}
