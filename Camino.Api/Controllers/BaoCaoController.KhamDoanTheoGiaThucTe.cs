using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;


namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {

        [HttpPost("GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BCDTKhamDoanTheoKhoaPhongDGThucTe)]
        public ActionResult<GridDataSource> GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeForGrid(QueryInfo queryInfo)
        {
            var grid = _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeForGrid(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BCDTKhamDoanTheoKhoaPhongDGThucTe)]
        public ActionResult ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTe([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeForGrid(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTeGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuKhamDoanTheoKhoaPhongThucTe" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
