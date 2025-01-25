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
        [HttpPost("GetTatCaKhoNhapChiTietKeToans")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCaKhoNhapChiTietKeToans([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTatCaKhoNhapChiTietKeToans(queryInfo);
            return Ok(result);
        }


        [HttpPost("GetDataBaoCaoTinhHinhNhapNCCChiTietKeToanDuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapNhaCungCapChiTiet)]
        public async Task<ActionResult> GetDataBaoCaoTinhHinhNhapNCCChiTietKeToanDuocForGridAsync(NhapNhaCungCapChiTietKeToanDuocQueryInfoVo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataNhapNhaCungCapChiTietKeToanForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoTinhHinhNhapNCCChiTietKeToanDuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapNhaCungCapChiTiet)]
        public async Task<ActionResult> ExportBaoCaoTinhHinhNhapNCCChiTietKeToanDuoc(NhapNhaCungCapChiTietKeToanDuocQueryInfoVo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataNhapNhaCungCapChiTietKeToanForGrid(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoNhapCungCapChiTietKeToan(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTinhHinhNhapNCCChiTietKeToan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
