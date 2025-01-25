using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.Grid;
using static Camino.Core.Domain.Enums;
using System;
using System.Linq;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using Camino.Services.ExportImport;
using Camino.Services.KhamDoan;
using Camino.Services.BaoCaoKSKChuyenKhoa;

namespace Camino.Api.Controllers
{
    public class BaoCaoKSKChuyenKhoaController : CaminoBaseController
    {
        private readonly IBaoCaoKSKChuyenKhoaService _baoCaoKSKChuyenKhoaService;
        private readonly IExcelService _excelService;
        private readonly IKhamDoanService _khamDoanService;

        public BaoCaoKSKChuyenKhoaController(
            IBaoCaoKSKChuyenKhoaService baoCaoKSKChuyenKhoaService,
            IExcelService excelService,
            IKhamDoanService khamDoanService
                  )
        {
            _baoCaoKSKChuyenKhoaService = baoCaoKSKChuyenKhoaService;
            _excelService = excelService;
            _khamDoanService = khamDoanService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncKSKChuyenKhoa")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoKSKChuyenKhoa)]
        public ActionResult<GridDataSource> GetDataForGridAsyncKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            var gridData = _baoCaoKSKChuyenKhoaService.GetDataForGridAsyncKSKChuyenKhoa(queryInfo);
            return Ok(gridData.Result.Data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncKSKChuyenKhoa")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoKSKChuyenKhoa)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsyncKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            var gridData = _baoCaoKSKChuyenKhoaService.GetTotalPageForGridAsyncKSKChuyenKhoa(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoKSKChuyenKhoa")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoKSKChuyenKhoa)]
        public async Task<ActionResult> ExportBaoCaoKSKChuyenKhoa(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            var gridData = await _baoCaoKSKChuyenKhoaService.GetDataForGridAsyncKSKChuyenKhoa(queryInfo);
            var data = gridData.Data.Select(p => (NguoiBenhKhamDichVuTheoChuyenKhoa)p).ToList();
            var bytes = _baoCaoKSKChuyenKhoaService.ExportBaoCaoKSKChuyenKhoa(queryInfo, data);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoKSKChuyenKhoa" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}