using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    public interface IADRService : IMasterFileService<ADR>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<HoatChatTemplateVo>> GetListHoatChatAsync(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetHoatChatAsync(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetChuYKhiChiDinhDescription();
        List<LookupItemVo> GetTuongTacDescription();
        Task<bool> IsTenHoatChatExists(long hoatChatId1 = 0, long hoatChatId2 = 0, long Id = 0);
        Task<bool> CheckHoatChatDeleted(long Id);


    }
}
