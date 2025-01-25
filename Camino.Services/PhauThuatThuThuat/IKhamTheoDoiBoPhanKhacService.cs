using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.PhauThuatThuThuat
{
    public interface IKhamTheoDoiBoPhanKhacService : IMasterFileService<KhamTheoDoiBoPhanKhac>
    {
        Task DeleteKhamTheoDoiBoPhanKhacs(List<long> khamTheoDoiBoPhanKhacIds, long khamTheoDoiId);
    }
}
