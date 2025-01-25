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
        List<LookupItemVo> GetTatKhoaChoTiepNhanNoiTru(DropDownListRequestModel queryInfo);

        Task<GridDataSource> GetDataBaoCaoBenhNhanRaVienNoiTruForGrid(BaoCaoRaVienNoiTruQueryInfoVo queryInfo);

        byte[] ExportBaoCaoBenhNhanRaVienNoiTru(GridDataSource gridDataSource, BaoCaoRaVienNoiTruQueryInfoVo query);
    }
}