using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDuTruSoLuongNguoiThucHienDvLSCLS;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using Camino.Core.Domain.ValueObject.DSDichVuNgoaiGoiKeToan;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCaoKhamDoanHopDong
{
    public interface IBaoCaoKhamDoanTheoGiaThucTeServices : IMasterFileService<HopDongKhamSucKhoeNhanVien>
    {
        GridDataSource BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTe(BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeQueryInfo queryInfo);
        byte[] ExportBaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTe(IList<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo> bc,
                                                                BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeQueryInfo queryInfo);
    }
}
