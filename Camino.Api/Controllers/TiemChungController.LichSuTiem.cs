using Camino.Api.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class TiemChungController
    {
        [HttpPost("GetDataForGridLichSuTiemChung")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungLichSuTiem, DocumentType.TiemChungKhamSangLoc, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuTiemChung([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiemChungService.GetDataForGridLichSuTiemChungAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridLichSuTiemChung")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungLichSuTiem, DocumentType.TiemChungKhamSangLoc, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuTiemChung([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiemChungService.GetTotalPageForGridLichSuTiemChungAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
