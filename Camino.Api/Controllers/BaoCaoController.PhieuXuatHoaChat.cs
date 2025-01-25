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
        [HttpPost("GetTatCaKhoPhieuXuatHoaChat")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaKhoPhieuXuatHoaChat(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaKhoPhieuXuatHoaChat(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTenMayXetNghiem")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTenMayXetNghiem(DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTenMayXetNghiem(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTenDuocPhamTheoPhieuXuat")]
        public async Task<ActionResult<ICollection<LookupItemDuocPhamVo>>> GetTenDuocPhamTheoPhieuXuat(DropDownListRequestModel queryInfo, long? KhoId)
        {
            var result = await _baoCaoService.GetTenDuocPhamTheoKhoXuat(queryInfo, KhoId);
            return Ok(result);
        }

        [HttpPost("GetDataPhieuXuatHoaChatForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNPhieuNhapHoaChat)]
        public async Task<ActionResult<GridDataSource>> GetDataPhieuXuatHoaChatForGrid(PhieuXuatHoaChatQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataPhieuXuatHoaChatForGrid(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetDataPhieuXuatHoaChatChiTietForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNPhieuXuatHoaChat)]
        public async Task<ActionResult<GridDataSource>> GetDataPhieuXuatHoaChatChiTietForGrid(PhieuXuatHoaChatQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataPhieuXuatHoaChatChiTietForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportPhieuXuatHoaChat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoXNPhieuXuatHoaChat)]
        public async Task<ActionResult> ExportPhieuXuatHoaChat([FromBody] PhieuXuatHoaChatQueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataPhieuXuatHoaChatChiTietForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportPhieuXuatHoaChat(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=PhieuXuatHoaChat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
