using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Microsoft.AspNetCore.Mvc;

// include all APIs connected to the bhyt-da-huongs behavior
namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        #region LoadGridHuongBhyt
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForDvHuongBhytAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataForDvHuongBhytAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _bhytDvHuongService.GetDataForDvHuongBhytAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region LoadLichSu

        [HttpPost("GetHistoryLog")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult> GetHistoryLog([FromBody]LichSuXacNhanVo lichSuXacNhanVo)
        {
            var gridLichSu = await _bhytDvHuongService.GetHistoryLog(lichSuXacNhanVo);
            return Ok(gridLichSu);
        }
        #endregion
    }
}
