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
        [HttpPost("GetDataTinhHinhBenhTatTuVongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTinhHinhBenhTatTuVong)]
        public async Task<ActionResult<GridDataSource>> GetDataTinhHinhBenhTatTuVongForGrid(TinhHinhBenhTatTuVongQueryInfoVo queryInfo)
        {
            var grid = await _baoCaoService.GetDataTinhHinhBenhTatTuVongForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTinhHinhBenhTatTuVong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhBenhTatTuVong)]
        public async Task<ActionResult> ExportBaoCaoTinhHinhBenhTatTuVong([FromBody] TinhHinhBenhTatTuVongQueryInfoVo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTinhHinhBenhTatTuVongForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportTinhHinhBenhTatTuVong(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
