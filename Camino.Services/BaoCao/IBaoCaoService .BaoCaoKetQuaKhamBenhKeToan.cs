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
        Task<BaoCaoKetQuaKhamChuaBenhKTVo> GetDataBaoCaoKetQuaKhamChuaBenhKTForGrid(BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo queryInfo);
        byte[] ExportBaoCaoKetQuaKhamChuaBenhKT(BaoCaoKetQuaKhamChuaBenhKTVo datas, BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo query);
        string GetHTMLBaoCaoKetQuaKhamChuaBenhKT(BaoCaoKetQuaKhamChuaBenhKTVo datas, BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo query);
    }
}