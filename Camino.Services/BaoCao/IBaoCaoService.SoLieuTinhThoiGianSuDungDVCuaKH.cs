using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo);
        Task<GridDataSource> GetDataTotalPageBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo);
        byte[] ExportBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo(GridDataSource gridDataSource, BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo query);
    }
}
