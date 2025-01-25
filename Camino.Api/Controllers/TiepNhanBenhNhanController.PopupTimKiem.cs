using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpPost("GetDataForGridAsyncPopupTimKiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncPopupTimKiem([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetDataForGridAsyncPopupTimKiem(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncPopupTimKiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncPopupTimKiem([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetTotalPageForGridAsyncPopupTimKiem(queryInfo);
            return Ok(gridData);
        }
    }
}
