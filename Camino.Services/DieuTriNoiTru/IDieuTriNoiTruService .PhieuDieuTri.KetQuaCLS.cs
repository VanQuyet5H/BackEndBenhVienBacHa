using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataNoiTruKetQuaCDHATDCN(QueryInfo queryInfo);
        GridDataSource GetTotalNoiTruKetQuaCDHATDCN(QueryInfo queryInfo);
        GridDataSource GetDataNoiTruKetQuaXetNghiem(long yeuCauTiepNhanId, long phieuDieuTriHienTaiId);

        #region BVHD-3575
        Task<GridDataSource> GetDataForGridLichSuKhamAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridLichSuKhamAsync(QueryInfo queryInfo);


        #endregion
    }
}
