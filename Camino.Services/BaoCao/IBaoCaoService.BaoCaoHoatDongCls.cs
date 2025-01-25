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
        Task<GridDataSource> GetDataBaoCaoHoatDongCLSMauThucTeForGridAsync(BaoCaoHoatDongClsQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoHoatDongCLSMauCucQuanLyForGridAsync(BaoCaoHoatDongClsQueryInfo queryInfo);
        byte[] ExportBaoCaoHoatDongCLSMauThucTe(GridDataSource data, BaoCaoHoatDongClsQueryInfo query);
        byte[] ExportBaoCaoHoatDongCLSMauCucQuanLy(GridDataSource data, BaoCaoHoatDongClsQueryInfo query);
    }
}
