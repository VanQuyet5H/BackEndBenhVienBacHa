using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemVo>> GetKhoDuocPhamBaoCaoTinhHinhNhapNCCChiTietLookupAsync(LookupQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoTinhHinhNhapNCCChiTietForGridAsync(BaoCaoTinhHinhNhapNCCChiTietQueryInfo queryInfo);
        byte[] ExportBaoCaoTinhHinhNhapNCCChiTiet(GridDataSource datas, BaoCaoTinhHinhNhapNCCChiTietQueryInfo query);
    }
}
