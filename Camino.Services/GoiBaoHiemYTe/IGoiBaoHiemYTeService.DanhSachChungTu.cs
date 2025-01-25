
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.GoiBaoHiemYTe
{
    public partial interface IGoiBaoHiemYTeService : IMasterFileService<YeuCauTiepNhan>
    {
        GridDataSource GetDataDanhSachXuatChungTuExcelForGrid(QueryInfo queryInfo);
        
        #region Lấy loại phiếu in

        long GetIdPhieuNoiTruHoSoKhac(long id, Enums.LoaiHoSoDieuTriNoiTru loai);

        #endregion
    }
}
