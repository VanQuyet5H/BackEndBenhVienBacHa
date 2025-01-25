using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    [ScopedDependency(ServiceType = typeof(ITheoDoiSauPhauThuatThuThuatService))]
    public class TheoDoiSauPhauThuatThuThuatService : MasterFileService<TheoDoiSauPhauThuatThuThuat>, ITheoDoiSauPhauThuatThuThuatService
    {
        public TheoDoiSauPhauThuatThuThuatService(IRepository<TheoDoiSauPhauThuatThuThuat> repository) : base(repository)
        {
        }

        public async Task<TheoDoiSauPhauThuatThuThuat> GetTheoDoiSauPhauThuatThuThuatByYeuCauTiepNhan(long yeuCauTiepNhanId, bool? isTuongTrinhLai)
        {
            return await BaseRepository.TableNoTracking.Include(p => p.BacSiPhuTrachTheoDoi).ThenInclude(p => p.User)
                                                       .Include(p => p.DieuDuongPhuTrachTheoDoi).ThenInclude(p => p.User)
                                                       .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                   (isTuongTrinhLai == true ? p.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.KetThucTheoDoi : p.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi))
                                                       .OrderByDescending(p => p.Id)
                                                       .FirstOrDefaultAsync();
        }
    }
}