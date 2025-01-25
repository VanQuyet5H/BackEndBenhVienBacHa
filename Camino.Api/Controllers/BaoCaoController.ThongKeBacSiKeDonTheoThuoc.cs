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
        [HttpPost("GetTatCaBacSiKeDonTheoThuoc")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaBacSiKeDonTheoThuoc(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaBacSiKeDonTheoThuoc(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTatCaThuocBenhVien")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaThuocBenhVien(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaThuocBenhVien(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataThongKeBacSiKeDonTheoThuocForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeBSKeDonTheoThuoc)]
        public ActionResult<GridDataSource> GetDataThongKeBacSiKeDonTheoThuocForGrid(ThongKeBacSiKeDonTheoThuocQueryInfo queryInfo)
        {
            var grid = _baoCaoService.GetDataThongKeBacSiKeDonTheoThuocForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportThongKeBacSiKeDonTheoThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.ThongKeBSKeDonTheoThuoc)]
        public ActionResult ExportThongKeBacSiKeDonTheoThuoc([FromBody] ThongKeBacSiKeDonTheoThuocQueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = _baoCaoService.GetDataThongKeBacSiKeDonTheoThuocForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportThongKeBacSiKeDonTheoThuoc(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThongKeBacSiKeDonTheoThuoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
