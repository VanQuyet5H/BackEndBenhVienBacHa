using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetTatCaTuHoaChat")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaTuHoaChat(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaTuHoaChat(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataXuatNhapTonKhoXetNghiemForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNXuatNhapTonKhoXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataXuatNhapTonKhoXetNghiemForGrid(XuatNhapTonKhoXetNghiemQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataXuatNhapTonKhoXetNghiemForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportXuatNhapTonKhoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoXNXuatNhapTonKhoXetNghiem)]
        public async Task<ActionResult> ExportXuatNhapTonKhoXetNghiem([FromBody] XuatNhapTonKhoXetNghiemQueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataXuatNhapTonKhoXetNghiemForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportXuatNhapTonKhoXetNghiem(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatNhapTonKhoXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
