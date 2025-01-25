using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {

        [HttpPost("GetTatCaKhoaThongKeSoLuongThuThuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCaKhoaThongKeSoLuongThuThuat([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTatCaKhoaThongKeSoLuongThuThuat(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTatCaPhongTheoKhoaThongKeSoLuongThuThuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCaPhongTheoKhoaThongKeSoLuongThuThuat([FromBody] DropDownListRequestModel queryInfo)
        {
            var lstKhoaIdDaChon = JsonConvert.DeserializeObject<KhoaDaChon>(queryInfo.ParameterDependencies);
            var result = await _baoCaoService.GetTatCaPhongTheoKhoaThongKeSoLuongThuThuat(queryInfo, lstKhoaIdDaChon.KhoaId);
            return Ok(result);
        }


        [HttpPost("GetDataThongKeSoLuongThuThuatForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KHTHBaoCaoThongKeSLThuThuat)]
        public async Task<ActionResult> GetDataThongKeSoLuongThuThuatForGridAsync(KHTHBaoCaoThongKeSLThuThuatQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataKHTHBaoCaoThongKeSLThuThuatForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportThongKeSoLuongThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KHTHBaoCaoThongKeSLThuThuat)]
        public async Task<ActionResult> ExportThongKeSoLuongThuThuat(KHTHBaoCaoThongKeSLThuThuatQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataKHTHBaoCaoThongKeSLThuThuatForGridAsync(queryInfo);
            byte[] bytes = null;

            if (gridData != null)
            {
                bytes = _baoCaoService.ExportKHTHBaoCaoThongKeSLThuThuatGridVo(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThongKeSLThuThuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
