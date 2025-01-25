using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraThuocController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult> Delete(long id)
        {
            var ycHoanTraDuocPham = await _ycHoanTraDuocPhamService.GetByIdAsync(id, s =>
                s.Include(w => w.YeuCauTraDuocPhamChiTiets).ThenInclude(w => w.XuatKhoDuocPhamChiTietViTri).ThenInclude(w => w.XuatKhoDuocPhamChiTiet)
                    .Include(w => w.YeuCauTraDuocPhamChiTiets).ThenInclude(w => w.XuatKhoDuocPhamChiTietViTri).ThenInclude(w => w.NhapKhoDuocPhamChiTiet));
            if (ycHoanTraDuocPham == null || ycHoanTraDuocPham.DuocDuyet != null)
            {
                return NotFound();
            }

            foreach (var chiTiet in ycHoanTraDuocPham.YeuCauTraDuocPhamChiTiets)
            {
                chiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat =
                                                            chiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - chiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                chiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                chiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                chiTiet.WillDelete = true;
            }
            await _ycHoanTraDuocPhamService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
