using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("GetTatCaKhoPhieuNhapHoaChat")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaKhoPhieuNhapHoaChat(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaKhoPhieuNhapHoaChat(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTenDuocPhamTheoPhieuNhap")]
        public async Task<ActionResult<ICollection<LookupItemDuocPhamVo>>> GetTenDuocPhamTheoPhieuNhap(DropDownListRequestModel queryInfo, long? KhoId)
        {
            var result =await _baoCaoService.GetTenDuocPhamTheoPhieuNhap(queryInfo, KhoId);
            return Ok(result);
        }

        [HttpPost("GetDataPhieuNhapHoaChatForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNPhieuNhapHoaChat)]
        public async Task<ActionResult<GridDataSource>> GetDataPhieuNhapHoaChatForGrid(PhieuNhapHoaChatQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataPhieuNhapHoaChatForGrid(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetDataPhieuNhapHoaChatChiTietForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNPhieuNhapHoaChat)]
        public async Task<ActionResult<GridDataSource>> GetDataPhieuNhapHoaChatChiTietForGrid(PhieuNhapHoaChatQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataPhieuNhapHoaChatChiTietForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportPhieuNhapHoaChat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoXNPhieuNhapHoaChat)]
        public async Task<ActionResult> ExportPhieuNhapHoaChat([FromBody] PhieuNhapHoaChatQueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataPhieuNhapHoaChatChiTietForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportPhieuNhapHoaChat(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=PhieuNhapHoaChat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
