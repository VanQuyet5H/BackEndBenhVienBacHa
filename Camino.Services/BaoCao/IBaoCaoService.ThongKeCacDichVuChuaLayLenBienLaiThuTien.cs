using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {       
        Task<GridDataSource> GetDataThongKeCacDVChuaLayLenBienLaiThuTienForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageThongKeCacDVChuaLayLenBienLaiThuTienForGrid(QueryInfo queryInfo);

        byte[] ExportThongKeCacDichVuChuaLayLenBienLaiThuTien(GridDataSource gridDataSource, QueryInfo query);
    }
}