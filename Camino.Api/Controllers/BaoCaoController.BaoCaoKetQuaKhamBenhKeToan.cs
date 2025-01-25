using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetDataBaoCaoKetQuaKhamChuaBenhKTForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoKetQuaKhamChuaBenhKT)]
        public async Task<ActionResult<string>> GetDataBaoCaoKetQuaKhamChuaBenhKTForGrid(BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKetQuaKhamChuaBenhKTForGrid(queryInfo);
            var content = string.Empty;
            if (gridData != null)
            {
                content = _baoCaoService.GetHTMLBaoCaoKetQuaKhamChuaBenhKT(gridData, queryInfo);
            }          
            return content;       
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoKetQuaKhamChuaBenhKT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKetQuaKhamChuaBenhKT)]
        public async Task<ActionResult> ExportBaoCaoKetQuaKhamChuaBenhKT([FromBody] BaoCaoKetQuaKhamChuaBenhKTQueryInfoVo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKetQuaKhamChuaBenhKTForGrid(queryInfo);
            byte[] bytes = null;

            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoKetQuaKhamChuaBenhKT(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoKetQuaKhamChuaBenhKT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
