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
        [HttpPost("GetTatKhoaChoDanhSachBARaVienChuaXacNhanHoanTatChiPhi")]
        public ActionResult<ICollection<LookupItemVo>> GetTatKhoaChoDanhSachBARaVienChuaXacNhanHoanTatChiPhi(DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatKhoaChoDanhSachBARaVienChuaXacNhanHoanTatChiPhi(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataDanhSachBARaVienChuaXacNhanHoanTatChiPhiForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachBARaVienChuaXacNhanHoanTatChiPhi)]
        public async Task<ActionResult<GridDataSource>> GetDataDanhSachBARaVienChuaXacNhanHoanTatChiPhiForGrid(DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo queryInfo)
        {
            var grid = await _baoCaoService.GetDataDanhSachBARaVienChuaXacNhanHoanTatChiPhiForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportDanhSachBARaVienChuaXacNhanHoanTatChiPhi")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachBARaVienChuaXacNhanHoanTatChiPhi)]
        public async Task<ActionResult> ExportDanhSachBARaVienChuaXacNhanHoanTatChiPhi([FromBody] DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataDanhSachBARaVienChuaXacNhanHoanTatChiPhiForGrid(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportDanhSachBARaVienChuaXacNhanHoanTatChiPhi(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachBARaVienChuaXacNhanHoanTatChiPhi" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
