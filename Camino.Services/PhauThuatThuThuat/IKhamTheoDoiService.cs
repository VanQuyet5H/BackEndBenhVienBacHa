using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.PhauThuatThuThuat
{
    public interface IKhamTheoDoiService : IMasterFileService<KhamTheoDoi>
    {
        Task<List<KhamTheoDoi>> GetKhamTheoDoisByTheoDoiSauPhauThuatThuThuat(long theoDoiSauPhauThuatThuThuatId);
    }
}
