using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Services.KhamBenhs
{
    public interface IYeuCauDichVuKyThuatTuongTrinhPTTTService : IMasterFileService<YeuCauDichVuKyThuatTuongTrinhPTTT>
    {
        Task<TuongTrinhTuVongResultVo> GetTuVong(long idDvkt);

        Task KetThucTuongTrinh(long ycdvktId);

        Task<GridDataSource> LoadChiSoSinhHieu(long yctnId);
    }
}
