using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBhytDaHoanThanhController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDaXacNhanAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDaXacNhanAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xacNhanBhytDaHoanThanhDetailedService.GetDataForGridDaXacNhanAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForDuyetBaoHiemAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult<GridDataSource>> GetDataForDuyetBaoHiemAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xacNhanBhytDaHoanThanhDetailedService.GetDataForDuyetBaoHiemAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDuyetBaoHiemChiTietAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public ActionResult<GridDataSource> GetDataForGridDuyetBaoHiemChiTietAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _xacNhanBhytDaHoanThanhDetailedService.GetDataForGridDuyetBaoHiemChiTietAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
