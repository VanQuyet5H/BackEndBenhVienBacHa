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
        GridDataSource GetDanhSachVatTuCanBuForGrid(QueryInfo queryInfo);

        Task<GridDataSource> GetDanhSachChiTietVatTuCanBuForGrid(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageDanhSachChiTietVatTuCanBuForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietYeuCauTheoVatTuCanBuForGrid(
            QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanBuForGrid(
            QueryInfo queryInfo);
        List<LookupItemVo> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model);
        List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model);

        Task UpdateYeuCauVatTuBenhVien(string yeuCauLinhVatTuIdstring);

    }
}
