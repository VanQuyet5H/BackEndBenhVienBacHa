using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BenhVien.Khoa;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BenhVien.Khoa;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class KhoaController : CaminoBaseController
    {
        private readonly IExcelService _excelService;
        private readonly IKhoaService _khoaService;

        public KhoaController(
            IExcelService excelService,
            IKhoaService khoaService
            )
        {
            _excelService = excelService;
            _khoaService = khoaService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoaService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuyenKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoaService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("KichHoatKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChuyenKhoa)]
        public async Task<ActionResult> KichHoatKhoa(long id)
        {
            var entity = await _khoaService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _khoaService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportChuyenKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucChuyenKhoa)]
        public async Task<ActionResult> ExportChuyenKhoa(QueryInfo queryInfo)
        {
            var gridData = await _khoaService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (KhoaGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<ChuyenKhoaExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChuyenKhoaExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ChuyenKhoaExportExcel.Ten), "Tên Chuyên Khoa"));
            lstValueObject.Add((nameof(ChuyenKhoaExportExcel.MoTa), "Mô Tả"));
            lstValueObject.Add((nameof(ChuyenKhoaExportExcel.HieuLuc), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Chuyên Khoa");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChuyenKhoa" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
