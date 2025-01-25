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
    public partial class BaoCaoController
    {
        [HttpPost("GetKhoVTYTBaoCaoChiTietXuatNoiTruLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoVTYTBaoCaoChiTietXuatNoiTruLookupAsync
            ([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoVTYTBaoCaoChiTietXuatNoiTruLookupAsync(queryInfo);
            return Ok(result);
        }
        
        [HttpPost("GetDataBaoCaoChiTietXuatNoiBoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VTYTBaoCaoChiTietXuatNoiBo)]
        public async Task<ActionResult> GetDataBaoCaoChiTietXuatNoiBoForGridAsync(BaoCaoChiTietXuatNoiBoQueryInfo queryInfo)
        {       
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietXuatNoiBoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoChiTietXuatNoiBo")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VTYTBaoCaoChiTietXuatNoiBo)]
        public async Task<ActionResult> ExportBaoCaoChiTietXuatNoiBo(BaoCaoChiTietXuatNoiBoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietXuatNoiBoForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoChiTietXuatNoiBo(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietXuatNoiBo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
