using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhBuKSNK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial interface IYeuCauLinhKSNKService
    {
        GridDataSource GetDanhSachKSNKCanBuForGrid(QueryInfo queryInfo);

        Task<GridDataSource> GetDanhSachChiTietKSNKCanBuForGrid(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageDanhSachChiTietKSNKCanBuForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChiTietYeuCauTheoKSNKCanBuForGrid(
            QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoKSNKCanBuForGrid(
            QueryInfo queryInfo);
        List<LookupItemVo> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model);
        List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model);

        Task UpdateYeuCauKSNKBenhVien(List<KhongYeuCauLinhBuKSNKVo> yeuCauLinhKSNKIdstring);

    }
}
