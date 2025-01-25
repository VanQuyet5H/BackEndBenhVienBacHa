using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.QuayThuoc
{
    public interface IQuayThuocLichSuHuyBanThuocService : IMasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>
    {
        Task<GridDataSource> GetDataForGridLichSuHuyBanThuocAsync(QueryInfo queryInfo, bool isPrint);
        Task<GridDataSource> GetTotalPageForGridLichSuHuyBanThuocAsync(QueryInfo queryInfo);
        List<ThongTinDuocPhamQuayThuocVo> GetDanhSachThuocDaHuyThuocKhongBHYTLS(long tiepNhanId, long? idTaiKhoanBenhNhanThu);
        Task<string> InBaoCaoToaThuocHuyBanAsync(long id, bool bangKe, bool thuTien, string hostingName);
    }
}
