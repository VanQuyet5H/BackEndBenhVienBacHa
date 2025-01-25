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
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class LichSuSMSController : CaminoBaseController
    {
        readonly ILichSuSMSService _lichsuSMSService;
        private readonly IExcelService _excelService;

        public LichSuSMSController(ILichSuSMSService lichsuSMSService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _lichsuSMSService = lichsuSMSService;
            _excelService = excelService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichSuSMS)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichsuSMSService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichSuSMS)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichsuSMSService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTrangThaiSMS")]
        public ActionResult<ICollection<LookupItemVo>> GetTrangThaiSMS()
        {
            var lookup = _lichsuSMSService.GetTrangThai();
            return Ok(lookup);
        }

        [HttpPost("ExportLichSuSMS")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyLichSuSMS)]
        public async Task<ActionResult> ExportLichSuSMS(QueryInfo queryInfo)
        {
            var gridData = await _lichsuSMSService.GetDataForGridAsync(queryInfo, true);
            var lichSuSMSData = gridData.Data.Select(p => (LichSuSMSGrid)p).ToList();
            var excelData = lichSuSMSData.Map<List<LichSuSMSExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuSMSExportExcel.GoiDen), "Gửi đến"));
            lstValueObject.Add((nameof(LichSuSMSExportExcel.NoiDung), "Nội dung"));
            lstValueObject.Add((nameof(LichSuSMSExportExcel.TenTrangThai), "Trạng thái"));
            lstValueObject.Add((nameof(LichSuSMSExportExcel.NgayGui), "Ngày gửi"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch sử SMS");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuSMS" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}