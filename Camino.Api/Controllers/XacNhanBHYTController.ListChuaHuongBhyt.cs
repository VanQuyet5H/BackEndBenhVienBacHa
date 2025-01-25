using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

// include all APIs connected to the bhyt-chua-duoc-huongs behavior
namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        #region LoadGridChuaDuocHuongBhyt
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForDvChuaHuongBhytAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataForDvChuaHuongBhytAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _bhytDvChuaHuongService.GetDataForDvChuaHuongBhytAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
    }
}
