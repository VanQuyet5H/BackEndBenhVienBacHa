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
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.HeThong;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class NhatKyHeThongController:CaminoBaseController
    {
        readonly INhatKyHeThongService _nhatKyHeThongService;
        private readonly IExcelService _excelService;

        public NhatKyHeThongController(INhatKyHeThongService iNhatKyHeThongService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _nhatKyHeThongService = iNhatKyHeThongService;
            _excelService = excelService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNhatKyHeThong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfoLichSuHoatDong queryInfo)
        {
            var gridData = await _nhatKyHeThongService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNhatKyHeThong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfoLichSuHoatDong queryInfo)
        {
            var gridData = await _nhatKyHeThongService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetHoatDong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetHoatDong()
        {
            var lookup = await _nhatKyHeThongService.GetHoatDongAsync();
            return Ok(lookup);
        }

        [HttpPost("GetNguoiTao")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNguoiTao([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _nhatKyHeThongService.GetNguoiTaoAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("ExportLichSuHoatDong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyNhatKyHeThong)]
        public async Task<ActionResult> ExportLichSuHoatDong(QueryInfo queryInfo)
        {
            var gridData = await _nhatKyHeThongService.GetDataForGridAsync(queryInfo, true);
            var lichSuHoatDongData = gridData.Data.Select(p => (NhatKyHeThongGridVo)p).ToList();
            var excelData = lichSuHoatDongData.Map<List<NhatKyHeThongExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NhatKyHeThongExportExcel.TenHoatDong), "Hoạt động"));
            lstValueObject.Add((nameof(NhatKyHeThongExportExcel.NoiDung), "Nội dung"));
            lstValueObject.Add((nameof(NhatKyHeThongExportExcel.NgayTaoFormat), "Ngày tạo"));
            lstValueObject.Add((nameof(NhatKyHeThongExportExcel.NguoiTao), "Người tạo"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch sử hoạt động");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuHoatDong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}