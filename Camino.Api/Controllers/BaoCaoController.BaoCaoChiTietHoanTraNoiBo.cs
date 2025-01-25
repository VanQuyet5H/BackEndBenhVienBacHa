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
        [HttpPost("GetKhoVTYTBaoCaoChiTietHoanTraNoiTruLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoVTYTBaoCaoChiTietHoanTraNoiTruLookupAsync([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoVTYTBaoCaoChiTietHoanTraNoiTruLookupAsync(queryInfo);
            return Ok(result);
        }
        
        [HttpPost("GetDataBaoCaoChiTietHoanTraNoiBoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VTYTBaoCaoChiTietHoanTraNoiBo)]
        public async Task<ActionResult> GetDataBaoCaoChiTietHoanTraNoiBoForGridAsync(BaoCaoChiTietHoanTraNoiBoQueryInfo queryInfo)
        {       
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietHoanTraNoiBoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoChiTietHoanTraNoiBo")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VTYTBaoCaoChiTietHoanTraNoiBo)]
        public async Task<ActionResult> ExportBaoCaoChiTietHoanTraNoiBo(BaoCaoChiTietHoanTraNoiBoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietHoanTraNoiBoForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoChiTietHoanTraNoiBo(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietHoanTraNoiBo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
