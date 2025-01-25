using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoDoanhThuTheoNhomDichVuAsync(BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoDoanhThuTheoNhomDichVu3961Async(BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo);
    }
}
