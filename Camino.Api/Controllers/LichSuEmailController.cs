using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Messages;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class LichSuEmailController:CaminoBaseController
    {
        readonly ILichSuEmailService _lichSuEmailService;
        private readonly IExcelService _excelService;

        public LichSuEmailController(ILichSuEmailService lichSuEmailService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _lichSuEmailService = lichSuEmailService;
            _excelService = excelService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichSuEmail)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuEmailService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichSuEmail)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuEmailService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportLichSuEmail")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyLichSuEmail)]
        public async Task<ActionResult> ExportLichSuEmail(QueryInfo queryInfo)
        {
            var gridData = await _lichSuEmailService.GetDataForGridAsync(queryInfo, true);
            var lichSuEmailData = gridData.Data.Select(p => (LichSuEmailGrid)p).ToList();
            var excelData = lichSuEmailData.Map<List<LichSuEmailExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuEmailExportExcel.GoiDen), "Gửi đến"));
            lstValueObject.Add((nameof(LichSuEmailExportExcel.TieuDe), "Tiêu đề"));
            lstValueObject.Add((nameof(LichSuEmailExportExcel.NoiDung), "Nội dung"));
            lstValueObject.Add((nameof(LichSuEmailExportExcel.TapTinDinhKem), "Tập tin đính kèm"));
            lstValueObject.Add((nameof(LichSuEmailExportExcel.TenTrangThai), "Trạng thái"));
            lstValueObject.Add((nameof(LichSuEmailExportExcel.NgayGuiFormat), "Ngày gửi"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch sử Email");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuEmail" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
