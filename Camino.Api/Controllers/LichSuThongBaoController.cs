using Camino.Api.Auth;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Camino.Services.Messages;
using Camino.Services.ExportImport;
using System.Linq;
using Camino.Core.Domain.ValueObject.Messages;
using Camino.Services.Helpers;
using System;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{

    public class LichSuThongBaoController : CaminoBaseController
    {
        readonly ILichSuThongBaoService _lichSuThongBaoService;
        private readonly IExcelService _excelService;

        public LichSuThongBaoController(ILichSuThongBaoService lichSuThongBaoService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _lichSuThongBaoService = lichSuThongBaoService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichSuThongBao)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuThongBaoService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyLichSuThongBao)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuThongBaoService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        /// <summary>
        ///     Get enums trang thai
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetTrangThaiLichSu")]
        public  ActionResult<ICollection<LookupItemVo>> GetTrangThaiLichSu()
        {
            var lookup =  _lichSuThongBaoService.GetTrangThai();
            return Ok(lookup);
        }

        [HttpPost("ExportLichSuThongBao")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyLichSuThongBao)]
        public async Task<ActionResult> ExportLichSuThongBao(QueryInfo queryInfo)
        {
            var gridData = await _lichSuThongBaoService.GetDataForGridAsync(queryInfo, true);
            var lichSuThongBaoData = gridData.Data.Select(p => (LichSuThongBaoGripVo)p).ToList();
            var excelData = lichSuThongBaoData.Map<List<LichSuThongBaoExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuThongBaoExportExcel.GoiDen), "Gửi đến"));
            lstValueObject.Add((nameof(LichSuThongBaoExportExcel.NoiDung), "Nội dung"));
            lstValueObject.Add((nameof(LichSuThongBaoExportExcel.TenTrangThai), "Tên trạng thái"));
            lstValueObject.Add((nameof(LichSuThongBaoExportExcel.NgayGui), "Ngày gửi"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch sử thông báo");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuThongBao" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}