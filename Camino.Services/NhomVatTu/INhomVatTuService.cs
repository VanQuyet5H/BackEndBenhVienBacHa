using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhomVatTu;

namespace Camino.Services.NhomVatTu
{
    public interface INhomVatTuService : IMasterFileService<Core.Domain.Entities.NhomVatTus.NhomVatTu>
    {
        Task<List<NhomVatTuGridVo>> GetDataTreeView(QueryInfo queryInfo);

        Task<List<LookupTreeItemVo>> GetTreeTemp(DropDownListRequestModel model); //todo: cần xóa
    }
}
