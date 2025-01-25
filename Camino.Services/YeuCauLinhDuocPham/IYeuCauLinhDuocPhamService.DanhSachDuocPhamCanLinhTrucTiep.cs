using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanBu;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService
    {
        GridDataSource GetDanhSachDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo);
        List<LookupItemVo> GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model);
        List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model);
    }
}
