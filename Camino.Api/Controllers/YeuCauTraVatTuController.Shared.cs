using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauHoanTraVatTu;
using Camino.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraVatTuController
    {
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauHoanTraVatTuViewModel>> Get(long id)
        {
            var ycHoanTraDpEntity = await _ycHoanTraVatTuService.GetByIdAsync(id,
                w => w.Include(e => e.KhoXuat)
                    .Include(e => e.KhoNhap)
                    .Include(e => e.NhanVienDuyet).ThenInclude(e => e.User)
                    .Include(e => e.NhanVienYeuCau).ThenInclude(e => e.User));

            if (ycHoanTraDpEntity == null)
            {
                return NotFound();
            }

            var ycHoanTraVm = ycHoanTraDpEntity.ToModel<YeuCauHoanTraVatTuViewModel>();

            return Ok(ycHoanTraVm);
        }
    }
}
