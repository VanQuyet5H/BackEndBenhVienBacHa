using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDanhSachChoKhams")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachChoKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncDanhSachChoKhams(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachChoKhams")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachChoKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncDanhSachChoKhams(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("DoiTuongPhongChos")]
        public async Task<ActionResult<ICollection<DoiTuongPhongChoTemplateVo>>> GetDoiTuongs([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetDoiTuongPhongCho(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetBenhViens")]
        public Core.Domain.Entities.BenhVien.BenhVien GetBenhViens(long? benhVienId)
        {
            var lookup = _yeuCauKhamBenhService.GetBenhViens(benhVienId);
            return lookup;
        }
    }
}
