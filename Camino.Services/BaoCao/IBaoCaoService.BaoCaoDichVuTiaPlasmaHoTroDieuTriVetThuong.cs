using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        byte[] ExportBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong(GridDataSource gridDataSource, QueryInfo query);
        Task<GridDataSource> GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(QueryInfo queryInfo);
        List<LookupItemVo> GetTatKhoaTiaPlasmaHoTroVetThuong(DropDownListRequestModel queryInfo);
        Task<List<LookupItemTongHopDichVuVo>> GetTongHopDichVuBaoCaoAsync(DropDownListRequestModel queryInfo);
    }
}
