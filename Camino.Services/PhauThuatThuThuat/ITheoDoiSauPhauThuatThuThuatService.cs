using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using System.Threading.Tasks;

namespace Camino.Services.PhauThuatThuThuat
{
    public interface ITheoDoiSauPhauThuatThuThuatService : IMasterFileService<TheoDoiSauPhauThuatThuThuat>
    {
        Task<TheoDoiSauPhauThuatThuThuat> GetTheoDoiSauPhauThuatThuThuatByYeuCauTiepNhan(long yeuCauTiepNhanId, bool? isTuongTrinhLai);
    }
}
