using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<ICollection<LookupItemVo>> GetKhoBaoCaoTinhHinhNhapTuNCCLookupAsync(LookupQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoTinhHinhNhapTuNhaCungCapForGridAsync(BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo queryInfo);        
        byte[] ExportBaoCaoBangTinhHinhNhapTuNhaCungCapGridVo(GridDataSource gridDataSource, BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo query);
    }
}
