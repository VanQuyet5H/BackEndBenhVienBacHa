using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDataForDvChuaHuongBhytNoiTruAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForDvChuaHuongBhytNoiTruAsync
            (long yeuCauTiepNhanId)
        {
            var gridData = await _bhytDvChuaHuongService.GetDataForDvChuaHuongBhytNoiTruAsync(yeuCauTiepNhanId);
            return Ok(gridData);
        }
    }
}
