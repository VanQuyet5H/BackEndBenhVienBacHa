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
        Task<BaoCaoHoatDongNoiTruChiTietVo> GetDataBaoCaoHoatDongNoiTruChiTietGrid(BaoCaoHoatDongNoiTruChiTietQueryInfoVo queryInfo);
        byte[] ExportBaoCaoHoatDongNoiTruChiTiet(BaoCaoHoatDongNoiTruChiTietVo datas, BaoCaoHoatDongNoiTruChiTietQueryInfoVo query);
        string HtmlBaoCaoHoatDongNoiTruChiTiet(BaoCaoHoatDongNoiTruChiTietVo datas, BaoCaoHoatDongNoiTruChiTietQueryInfoVo query);
    }
}