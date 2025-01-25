using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoBenhNhanLamXetNghiemForGridAsync(BaoCaoBenhNhanLamXetNghiemQueryInfo queryInfo, bool exportExcel = false);
        byte[] ExportBaoCaoBenhNhanLamXetNghiem(GridDataSource datas, BaoCaoBenhNhanLamXetNghiemQueryInfo query);
    }
}
