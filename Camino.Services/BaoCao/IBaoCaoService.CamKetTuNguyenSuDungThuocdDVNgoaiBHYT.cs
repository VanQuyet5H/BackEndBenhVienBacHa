using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo);

        Task<GridDataSource> GetDataTotalPageBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo);

        byte[] ExportBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo(GridDataSource gridDataSource, BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo query);
    }
}