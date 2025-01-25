using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [HttpPost("GetDataForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForYeuCauNhanSuKhamSucKhoeGiamDocGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("DuyetGiamDoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc)]
        public async Task<ActionResult> DuyetGiamDocAsync(long id)
        {
            await _khamDoanService.DuyetGiamDocAsync(id);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetGiamDoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc)]
        public async Task<ActionResult> TuChoiDuyetGiamDocAsync([FromBody]TuChoiDuyetKhthRequest tuChoiDuyet)
        {
            await _khamDoanService.TuChoiDuyetGiamDocAsync(tuChoiDuyet);
            return Ok();
        }
    }
}
