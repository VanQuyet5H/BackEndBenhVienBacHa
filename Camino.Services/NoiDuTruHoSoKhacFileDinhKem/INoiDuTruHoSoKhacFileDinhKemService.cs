using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.NoiDuTruHoSoKhacFileDinhKem
{
    public interface INoiDuTruHoSoKhacFileDinhKemService : IMasterFileService<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem>
    {
        Task<List<ThongTinFileDinhKem>> GetListFileDinhKem(long idNoiTruHoSoKhac);
    }
}
