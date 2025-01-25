using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.NoiDuTruHoSoKhacFileDinhKem
{
    [ScopedDependency(ServiceType = typeof(INoiDuTruHoSoKhacFileDinhKemService))]
    public class NoiDuTruHoSoKhacFileDinhKemService : MasterFileService<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem>, INoiDuTruHoSoKhacFileDinhKemService
    {
        public NoiDuTruHoSoKhacFileDinhKemService(IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhacFileDinhKem> repository) : base(repository)
        { }

        public async Task<List<ThongTinFileDinhKem>> GetListFileDinhKem(long idNoiTruHoSoKhac)
        {
            var noiTruHoSoKhacQueries = BaseRepository.TableNoTracking
                .Where(e => e.NoiTruHoSoKhacId == idNoiTruHoSoKhac)
                .Select(q => new ThongTinFileDinhKem
                {
                    Id = q.Id,
                    TenGuid = q.TenGuid,
                    Uid = q.Ma,
                    DuongDanTmp = q.DuongDan
                });
            return await noiTruHoSoKhacQueries.ToListAsync();
        }
    }
}
