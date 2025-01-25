using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauHoanTraDuocPham;
using Camino.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraThuocController
    {
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauHoanTraDuocPhamViewModel>> Get(long id)
        {
            var ycHoanTraDpEntity = await _ycHoanTraDuocPhamService.GetByIdAsync(id,
                w => w.Include(e => e.KhoXuat)
                    .Include(e => e.KhoNhap)
                    .Include(e => e.NhanVienDuyet).ThenInclude(e => e.User)
                    .Include(e => e.NhanVienYeuCau).ThenInclude(e => e.User));

            if (ycHoanTraDpEntity == null)
            {
                return NotFound();
            }

            return Ok(ycHoanTraDpEntity.ToModel<YeuCauHoanTraDuocPhamViewModel>());
        }
    }
}
