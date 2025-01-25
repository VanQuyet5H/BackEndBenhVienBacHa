using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    partial class BaoCaoController
    {
        [HttpPost("GetKhoVatTuLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoVatTuLookupAsync([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoVatTuTraNCCLookupAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoTinhHinhTraVTYTNCCForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VTYTTinhHinhTraNCC)]
        public async Task<ActionResult> GetDataBaoCaoTinhHinhTraVTYTNCCForGridAsync(BaoCaoTinhHinhTraVTYTNCCQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhTraVTYTNCCForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoTinhHinhTraVTYTNCC")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VTYTTinhHinhTraNCC)]
        public async Task<ActionResult> ExportBaoCaoTinhHinhTraVTYTNCC(BaoCaoTinhHinhTraVTYTNCCQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhTraVTYTNCCForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTinhHinhTraVTYTNCC(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTinhHinhTraVTYTNCC" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
