using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.BaoCaoKSKChuyenKhoa
{
    public interface IBaoCaoKSKChuyenKhoaService : IMasterFileService<HopDongKhamSucKhoeNhanVien>
    {
        Task<GridDataSource> GetDataForGridAsyncKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo);
        byte[] ExportBaoCaoKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, List<NguoiBenhKhamDichVuTheoChuyenKhoa> nguoiBenhKhamDichVuTheoPhongs);


    }
}
