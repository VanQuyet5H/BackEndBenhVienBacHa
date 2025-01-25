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
        Task<GridDataSource> GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync(BaoCaoKTNhapXuatTonChiTietQueryInfo queryInfo);
        byte[] ExportBaoCaoKTNhapXuatTonChiTiet(GridDataSource datas, BaoCaoKTNhapXuatTonChiTietQueryInfo query);
        Task<List<LookupItemVo>> GetTatCaKhoas(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetTatCaKhoTheoKhoas(DropDownListRequestModel queryInfo, long khoaId);     
    }
}
