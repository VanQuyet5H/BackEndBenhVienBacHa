using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetDataBaoCaoNguoiBenhDenLamDVKTForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoNguoiBenhDenLamDVKT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoNguoiBenhDenLamDVKTForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoNguoiBenhDenLamDVKTForGridAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetTotalBaoCaoNguoiBenhDenLamDVKTForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoNguoiBenhDenLamDVKT)]
        public async Task<ActionResult<GridDataSource>> GetTotalBaoCaoNguoiBenhDenLamDVKTForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoNguoiBenhDenLamDVKTForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoNguoiBenhDenLamDVKT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoNguoiBenhDenLamDVKT)]
        public async Task<ActionResult> ExportBaoCaoNguoiBenhDenLamDVKTAsync([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoNguoiBenhDenLamDVKTForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoNguoiBenhDenLamDVKTAsync(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoNguoiBenhDenKham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
