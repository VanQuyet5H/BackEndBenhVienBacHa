using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoDoanhThuNhaThuocs;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCaoDoanhThuNhaThuocService
{
    public interface IBaoCaoDoanhThuNhaThuocService : IMasterFileService<TaiKhoanBenhNhanThu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        string InBaoCaoDoanhThuNhaThuoc(ICollection<BaoCaoDoanhThuNhaThuocGridVo> datas, QueryInfo query);
        byte[] ExportBaoCaoDoanhThuNhaThuoc(ICollection<BaoCaoDoanhThuNhaThuocGridVo> datas, QueryInfo query);
    }
}
