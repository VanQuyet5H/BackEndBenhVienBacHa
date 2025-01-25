using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<GridDataSource> GetDataForGridAsyncLuuTruHoSo(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncLuuTruHoSo(QueryInfo queryInfo);
        ThongTiLuuTruBenhAnNoiTru ThongTiLuuTruBenhAnNoiTru(long noiTruBenhAnId);
        Task<bool> KiemTraThuTuSapXepLuuTruBATrung(long noiTruBenhAnId, string thuTuSapXepLuuTruBA);
        #region cập nhật 3648
        byte[] ExportBaoCaoSoLuuTruBenhAn(IList<LuuTruHoSoGridVo> luuTruHoSoGridVos, QueryInfo queryInfo);
        #endregion
    }
}
