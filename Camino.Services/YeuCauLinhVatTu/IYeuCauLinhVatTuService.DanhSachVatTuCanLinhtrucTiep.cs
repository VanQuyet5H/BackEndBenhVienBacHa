using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DanhSachVatTuCanBu;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial interface IYeuCauLinhVatTuService
    {
        GridDataSource GetDanhSachVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        List<LookupItemVo> GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model);
        List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model);
    }
}
