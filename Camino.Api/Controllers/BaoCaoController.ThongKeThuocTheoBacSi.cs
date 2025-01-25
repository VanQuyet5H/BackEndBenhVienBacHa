using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpPost("GetTatCaBacSi")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaBacSi(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaBacSi(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataThongKeThuocTheoBacSiForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeThuocTheoBacSi)]
        public ActionResult<GridDataSource> GetDataThongKeThuocTheoBacSiForGrid(ThongKeThuocTheoBacSiQueryInfo queryInfo)
        {
            var grid = _baoCaoService.GetDataThongKeThuocTheoBacSiForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportThongKeThuocTheoBacSi")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.ThongKeThuocTheoBacSi)]
        public ActionResult ExportThongKeThuocTheoBacSi([FromBody] ThongKeThuocTheoBacSiQueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = _baoCaoService.GetDataThongKeThuocTheoBacSiForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportThongKeThuocTheoBacSi(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThongKeThuocTheoBacSi" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        
        [HttpPost("ThongKeThuocTheoBacSiHTML")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeThuocTheoBacSi)]
        public async Task<string> ThongKeThuocTheoBacSiHTML(ThongKeThuocTheoBacSiQueryInfo queryInfo)
        {
            var grid = _baoCaoService.GetDataThongKeThuocTheoBacSiForGrid(queryInfo);
            var gridCastVo = grid.Data.Cast<DanhSachThongKeThuocTheoBacSi>().ToList();
            var html = await _baoCaoService.ThongKeThuocTheoBacSiHTML(gridCastVo);
            return html;
        }

    }
}
