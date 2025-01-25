using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.PhauThuatThuThuat
{
    [ScopedDependency(ServiceType = typeof(IKhamTheoDoiService))]
    public class KhamTheoDoiService : MasterFileService<KhamTheoDoi>, IKhamTheoDoiService
    {
        public KhamTheoDoiService(IRepository<KhamTheoDoi> repository) : base(repository)
        {
        }

        public async Task<List<KhamTheoDoi>> GetKhamTheoDoisByTheoDoiSauPhauThuatThuThuat(long theoDoiSauPhauThuatThuThuatId)
        {
            return await BaseRepository.TableNoTracking.Where(p => p.TheoDoiSauPhauThuatThuThuatId == theoDoiSauPhauThuatThuThuatId)
                                                       .Include(p => p.KhamTheoDoiBoPhanKhacs)
                                                       .OrderByDescending(p => p.Id)
                                                       .ToListAsync();
        }
    }
}
