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
        [HttpPost("GetNhomThuVienPhiChuaHoan")]
        public async Task<ActionResult<ICollection<LookupItemNhomThuVienPhiVo>>> GetNhomThuVienPhiChuaHoanAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetNhomThuVienPhiChuaHoanAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoThuVienPhiChuaHoanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThuVienPhiChuaHoan)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoThuVienPhiChuaHoanForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoThuVienPhiChuaHoanForGridAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetTotalPageBaoCaoThuVienPhiChuaHoanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThuVienPhiChuaHoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoThuVienPhiChuaHoanForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalPageBaoCaoThuVienPhiChuaHoanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoThuVienPhiChuaHoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThuVienPhiChuaHoan)]
        public async Task<ActionResult> ExportBaoCaoThuVienPhiChuaHoan([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoThuVienPhiChuaHoanForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoThuVienPhiChuaHoan(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThuVienPhiChuaHoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTongCongThuVienPhiChuaHoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThuVienPhiChuaHoan)]
        public async Task<ActionResult<BaoCaoThuVienPhiChuaHoanTongCongVo>> GetTongCongThuVienPhiChuaHoanAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoThuVienPhiChuaHoanTongCongVo();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoThuVienPhiChuaHoanForGridAsync(queryInfo);
            var datas = (ICollection<BaoCaoThuVienPhiChuaHoanGridVo>)gridData.Data;

            tongCong.TongTienTamUng = datas.Sum(x => x.SoTienTamUng ?? 0);
            tongCong.TongTienDaHoan = datas.Sum(x => x.SoTienDaHoan ?? 0);
            return Ok(tongCong);
        }
    }
}
