using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class QuayThuocController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLichSuHuyBanThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachLichSuHuyBanThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuHuyBanThuocAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuHuyBanThuocService.GetDataForGridLichSuHuyBanThuocAsync(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLichSuHuyBanThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachLichSuHuyBanThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuHuyBanThuocAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuHuyBanThuocService.GetTotalPageForGridLichSuHuyBanThuocAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDanhSachHuyThuocKhongBHYTLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachLichSuHuyBanThuoc)]
        public ActionResult GetDanhSachHuyThuocKhongBHYTLS(long tiepNhanId, long idTaiKhoanThu)
        {
            var result = _quayThuocLichSuHuyBanThuocService.GetDanhSachThuocDaHuyThuocKhongBHYTLS(tiepNhanId, idTaiKhoanThu);
            return Ok(result);
        }

        [HttpPost("XacNhanHuyBanThuocIn")]       
        public async Task<ActionResult<string>> XacNhanHuyBanThuocIn([FromBody] XacNhanIn xacNhanIn)
        {           
            var list = new List<HtmlToPdfVo>();
            var hostingName = xacNhanIn.Hosting;
            var tenfile = "";

            if (xacNhanIn.InBangKe)
            {
                var htmlInBangKe = await _quayThuocLichSuHuyBanThuocService.InBaoCaoToaThuocHuyBanAsync(xacNhanIn.TaiKhoanBenhNhanThuId, xacNhanIn.InBangKe, false, hostingName);
                list.Add(obj(htmlInBangKe, "A4", "Portrait"));
                tenfile = "BangKe";
            }           
           
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + tenfile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");

        }
    }
}
