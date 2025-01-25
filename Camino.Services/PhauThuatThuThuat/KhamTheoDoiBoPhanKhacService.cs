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
    [ScopedDependency(ServiceType = typeof(IKhamTheoDoiBoPhanKhacService))]
    public class KhamTheoDoiBoPhanKhacService : MasterFileService<KhamTheoDoiBoPhanKhac>, IKhamTheoDoiBoPhanKhacService
    {
        public KhamTheoDoiBoPhanKhacService(IRepository<KhamTheoDoiBoPhanKhac> repository) : base(repository)
        {
        }

        public async Task DeleteKhamTheoDoiBoPhanKhacs(List<long> khamTheoDoiBoPhanKhacIds, long khamTheoDoiId)
        {
            var khamTheoDoiBoPhanKhacs = await BaseRepository.TableNoTracking.Where(p => p.KhamTheoDoiId == khamTheoDoiId && !khamTheoDoiBoPhanKhacIds.Any(p2 => p2 == p.Id)).ToListAsync();

            foreach (var item in khamTheoDoiBoPhanKhacs)
            {
                await BaseRepository.DeleteAsync(item);
            }
        }
    }
}
