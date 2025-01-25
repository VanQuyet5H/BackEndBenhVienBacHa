using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class TiepNhanBenhNhanController : CaminoBaseController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncTaiKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncTaiKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetDataForGridAsyncTaiKham(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncTaiKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncTaiKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetTotalForGridAsyncTaiKham(queryInfo);
            return Ok(gridData);
        }
    }
}