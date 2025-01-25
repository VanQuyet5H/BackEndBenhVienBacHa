 using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Services.DichVuGiuong;
using Microsoft.EntityFrameworkCore;
using Camino.Api.Models.DichVuGiuong;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class DichVuGiuongController : CaminoBaseController
    {
        private readonly IDichVuGiuongService _dichVuGiuongService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DichVuGiuongController(IDichVuGiuongService dichVuGiuongService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _dichVuGiuongService = dichVuGiuongService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuGiuong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportDichVuGiuong")]
        public async Task<ActionResult> ExportDichVuGiuong(QueryInfo queryInfo)
        {
            var gridData = await _dichVuGiuongService.GetDataForGridAsync(queryInfo, true);
            var dichVuGiuongData = gridData.Data.Select(p => (DichVuGiuongGridVo)p).ToList();
            var excelData = dichVuGiuongData.Map<List<DichVuGiuongExportExcel>>();

            foreach (var item in excelData)
            {
                var gridChildData = await _dichVuGiuongService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var childData = gridChildData.Data.Select(p => (DichVuGiuongThongTinGiaGridVo)p).ToList();
                var childExcelData = childData.Map<List<DichVuGiuongExportExcelChild>>();
                item.DichVuGiuongExportExcelChild.AddRange(childExcelData);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.MaTT37), "Mã TT37"));
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.Khoa), "Khoa"));
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.HangBenhVienDisplay), "Hạng bệnh viện"));
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(DichVuGiuongExportExcel.DichVuGiuongExportExcelChild), ""));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dịch vụ giường");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuGiuong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}