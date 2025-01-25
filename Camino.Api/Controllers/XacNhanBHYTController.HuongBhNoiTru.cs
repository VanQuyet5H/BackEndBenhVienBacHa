using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDataForDvHuongBhytNoiTruAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForDvHuongBhytNoiTruAsync
            (long yeuCauTiepNhanId)
        {
            var gridData = await _bhytDvHuongService.GetDataForDvHuongBhytNoiTruAsync(yeuCauTiepNhanId);
            return Ok(gridData);
        }
    }
}
