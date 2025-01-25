using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoHoatDongKhamBenhTheoDichVu(GridDataSource gridDataSource, QueryInfo query);

        Task<GridDataSource> GetDataBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoHoatDongKhamBenhTheoKhoaPhong(GridDataSource gridDataSource, QueryInfo query);
    }
}
