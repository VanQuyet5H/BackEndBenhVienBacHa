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
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.TonKhos;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class DuocPhamSapHetHanController : CaminoBaseController
    {
        private ITonKhoService _tonKhoService;
        private readonly IExcelService _excelService;

        public DuocPhamSapHetHanController(IJwtFactory iJwtFactory, ITonKhoService tonKhoService, IExcelService excelService)
        {
            _tonKhoService = tonKhoService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamSapHetHan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForDuocPhamSapHetHanGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamSapHetHan)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _tonKhoService.GetTotalPageDuocPhamSapHetHanForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDuocPhamSapHetHan")]
        public ActionResult GetDuocPhamSapHetHan(string search)
        {
            var result = _tonKhoService.GetDuocPhamSapHetHan(search);
            return Ok(result);
        }
        [HttpPost("GetHTML")]
        public ActionResult GetHTML(string search)
        {
            var result = _tonKhoService.GeHTML(search);
            return Ok(result);
        }
        [HttpGet("InBaoCaoDuocPhamSapHetHan")]
        public async Task<ActionResult> InBaoCaoDuocPhamSapHetHan(string search)
        {

            var result = _tonKhoService.GeHTML(search);
            return Ok(result);
        }
        [HttpPost("CheckCaoDuocPhamSapHetHan")]
        public async Task<ActionResult> CheckCaoDuocPhamSapHetHan(string search)
        {

            var result = _tonKhoService.GetDuocPhamSapHetHan(search);
            return Ok(result);
        }

        [HttpPost("ExportDuocPhamSapHetHan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuocPhamSapHetHan)]
        public async Task<ActionResult> ExportDuocPhamSapHetHan(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDataForDuocPhamSapHetHanGridAsync(queryInfo, true);
            var duocPhamSapHetHanData = gridData.Data.Select(p => (DuocPhamSapHetHanGridVo)p).ToList();
            var excelData = duocPhamSapHetHanData.Map<List<DuocPhamSapHetHanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.TenKho), "Kho"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.MaDuocPham), "Mã dược"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.TenDuocPham), "Dược phẩm"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.HamLuong), "Hàm lượng"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.TenHoatChat), "Hoạt chất"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.DonViTinh), "Đơn vị tính"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.SoLo), "Số lô"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.ViTri), "Vị trí"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.DonGiaNhap), "Đơn giá nhập"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.SoLuongTon), "Số lượng tồn"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.ThanhTien), "Thành tiền"));
            lstValueObject.Add((nameof(DuocPhamSapHetHanExportExcel.NgayHetHanHienThi), "Ngày hết hạn"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dược phẩm sắp hết hạn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuocPhamSapHetHan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}