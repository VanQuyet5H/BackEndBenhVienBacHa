using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    public interface INhomThuocService : IMasterFileService<NhomThuoc>
    {
        Task<List<NhomThuocGridVo>> GetDataTreeView(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetListNhomThuoc(LookupQueryInfo queryInfo);
    }
}
