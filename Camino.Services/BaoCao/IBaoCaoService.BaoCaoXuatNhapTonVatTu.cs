using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        #region Báo cáo xuất nhập tồn vật tư

        Task<GridDataSource> GetDataBaoCaoXuatNhapTonVTForGridAsync(BaoCaoXuatNhapTonVTQueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoXuatNhapTonVTForGridAsync(BaoCaoXuatNhapTonVTQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoXuatNhapTonVTForGridAsyncChild(BaoCaoXuatNhapTonVTQueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalBaoCaoXuatNhapTonVTForGridAsyncChild(BaoCaoXuatNhapTonVTQueryInfo queryInfo);
        string InBaoCaoXuatNhapTonVT(InBaoCaoXuatNhapTonVTVo inBaoCaoXuatNhapTon);
        byte[] ExportBaoCaoXuatNhapTonVT(GridDataSource datas, BaoCaoXuatNhapTonVTQueryInfo query);
        #endregion
    }
}
