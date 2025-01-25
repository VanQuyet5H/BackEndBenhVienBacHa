using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region BVHD-3916
        [HttpGet("GetGhiChuCanLamSangTheoPhieuDieuTri")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> GetGhiChuCanLamSangTheoPhieuDieuTri(long noiTruPhieuDieuTriId)
        {
            var ghiChu = await _dieuTriNoiTruService.GetGhiChuCanLamSangTheoPhieuDieuTri(noiTruPhieuDieuTriId);
            return ghiChu;
        }

        [HttpPut("CapNhatGhiChuCanLamSang")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatGhiChuCanLamSangAsync(GhiChuCanLamSangVo updateVo)
        {
            await _dieuTriNoiTruService.CapNhatGhiChuCanLamSangAsync(updateVo);
            return Ok();
        }

        #endregion
    }
}
