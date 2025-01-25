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
        [HttpPost("GetDataBaoCaoHoatDongNoiTruChiTietForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongNoiTruChiTiet)]
        public async Task<ActionResult<string>> GetDataBaoCaoHoatDongNoiTruChiTietForGrid(BaoCaoHoatDongNoiTruChiTietQueryInfoVo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongNoiTruChiTietGrid(queryInfo);
            string htmlContent = string.Empty;
            if (gridData != null)
            {
                htmlContent = _baoCaoService.HtmlBaoCaoHoatDongNoiTruChiTiet(gridData, queryInfo);
            }

            return Ok(htmlContent);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoHoatDongNoiTruChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongNoiTruChiTiet)]
        public async Task<ActionResult> ExportBaoCaoHoatDongNoiTruChiTiet([FromBody] BaoCaoHoatDongNoiTruChiTietQueryInfoVo queryInfo)
        {            
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongNoiTruChiTietGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongNoiTruChiTiet(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongNoiTruChiTiet" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
