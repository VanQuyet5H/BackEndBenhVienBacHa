using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("DuyetKhth")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeKhth)]
        public async Task<ActionResult> DuyetKhthAsync(long id)
        {
            await _khamDoanService.DuyetKhthAsync(id);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetKhth")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoeKhth)]
        public async Task<ActionResult> TuChoiDuyetKhthAsync([FromBody]TuChoiDuyetKhthRequest tuChoiDuyet)
        {
            await _khamDoanService.TuChoiDuyetKhthAsync(tuChoiDuyet);
            return Ok();
        }
    }
}
