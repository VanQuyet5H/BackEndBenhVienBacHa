using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoTongHopSoLuongXetNghiemTheoThoiGianForGridAsync(BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo queryInfo);
        byte[] ExportBaoCaoTongHopSoLuongXetNghiemTheoThoiGian(GridDataSource datas, BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo query);
    }
}
