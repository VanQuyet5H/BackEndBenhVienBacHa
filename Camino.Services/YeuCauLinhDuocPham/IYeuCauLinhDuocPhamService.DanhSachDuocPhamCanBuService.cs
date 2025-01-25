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

        GridDataSource GetDanhSachDuocPhamCanBuForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietDuocPhamCanBuForGrid(
            QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageDanhSachChiTietDuocPhamCanBuForGrid(
            QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid(
            QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid(
            QueryInfo queryInfo);
        List<LookupItemVo> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model);
        List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model);
        Task UpdateYeuCauDuocPhamBenhVien(string yeuCauLinhDuocPhamIdstring);

    }
}
