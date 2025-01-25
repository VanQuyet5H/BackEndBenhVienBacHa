using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    public interface IThuocHoacHoatChatService : IMasterFileService<ThuocHoacHoatChat>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsMaAtcExists(string maAtc = null, long thuocHoacHoatChatId = 0);
        Task<List<MaHoatChatHoatChatDuongDungTemplateVo>> LookupThuocHoacHoatChat(DropDownListRequestModel model);
    }
}
