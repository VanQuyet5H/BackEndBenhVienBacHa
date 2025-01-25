using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class TonKhoController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuSapHetHanForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuSapHetHan)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuSapHetHanForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuSapHetHanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalVatTuSapHetHanPagesForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.VatTuSapHetHan)]
        public async Task<ActionResult<GridDataSource>> GetTotalVatTuSapHetHanPagesForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetTotalVatTuSapHetHanPagesForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetVatTuSapHetHanHTML")]
        public ActionResult GetVatTuSapHetHanHTML(string searchString)
        {
            var result = _tonKhoService.GetVatTuSapHetHanHTML(searchString);
            return Ok(result);
        }

        [HttpPost("ExportVatTuSapHetHan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.VatTuSapHetHan)]
        public async Task<ActionResult> ExportVatTuSapHetHan(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuSapHetHanForGridAsync(queryInfo, true);

            var vatTuSapHetHanData = gridData.Data.Select(p => (VatTuSapHetHanGridVo)p).ToList();
            var excelData = vatTuSapHetHanData.Map<List<VatTuSapHetHanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.TenKho), "Kho"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.MaVatTu), "Mã vật tư"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.TenVatTu), "Vật tư"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.DonViTinh), "Đơn vị tính"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.SoLo), "Số lô"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.ViTri), "Vị trí"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.DonGiaNhap), "Đơn giá nhập"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.SoLuongTon), "Số lượng tồn"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.ThanhTien), "thành tiền"));
            lstValueObject.Add((nameof(VatTuSapHetHanExportExcel.NgayHetHanHienThi), "Ngày hết hạn"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Vật tư sắp hết hạn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTuSapHetHan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
