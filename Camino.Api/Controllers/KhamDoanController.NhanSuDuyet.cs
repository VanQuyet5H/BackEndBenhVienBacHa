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
        [HttpPost("DuyetPhongNhanSu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoePhongNhanSu)]
        public async Task<ActionResult> DuyetPhongNhanSuAsync([FromBody]YeuCauDuyetPhongNhanSu ycDuyetPhongNhanSu)
        {
            await _khamDoanService.DuyetPhongNhanSuAsync(ycDuyetPhongNhanSu);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetPhongNhanSu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanDuyetYeuCauNhanSuKhamSucKhoePhongNhanSu)]
        public async Task<ActionResult> TuChoiDuyetPhongNhanSuAsync([FromBody]TuChoiDuyetKhthRequest tuChoiDuyet)
        {
            await _khamDoanService.TuChoiDuyetPhongNhanSuAsync(tuChoiDuyet);
            return Ok();
        }

    }
}
