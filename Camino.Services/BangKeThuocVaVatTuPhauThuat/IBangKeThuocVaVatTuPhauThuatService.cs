using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BangKeThuocVaVatTuPhauThuat
{
    public interface IBangKeThuocVaVatTuPhauThuatService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<ICollection<LookupItemVo>> GetPhongPhauThuats();

        Task<ICollection<ThongTinBenhNhanLookupItemVo>> GetBenhNhanPhongPhauThuats(
            ThongTinBenhNhanPhauThuatQueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsync(BaoCaoThuocVaVatTuPhauThuatQueryInfoVo queryInfo);
        byte[] ExportBangKeThuocVatTuPT(GridDataSource gridDataSource, BaoCaoThuocVaVatTuPhauThuatQueryInfoVo query);
    }
}
