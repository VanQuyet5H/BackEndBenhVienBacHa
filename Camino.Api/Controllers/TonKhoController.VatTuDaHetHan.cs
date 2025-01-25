using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.ValueObject.TonKhos.VatTuDaHetHan;

namespace Camino.Api.Controllers
{
    public partial class TonKhoController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuDaHetHanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.VatTuDaHetHan)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuDaHetHanForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuDaHetHanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachVatTuDaHetHanTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.VatTuDaHetHan)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachVatTuDaHetHanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuDaHetHanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetKhoTatCaVatTu")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoTatCaVatTu([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _tonKhoService.GetKhoVatTu(queryInfo);
            return Ok(result);
        }

        [HttpPost("XemVatTuDaHetHan")]
        public ActionResult XemVatTuDaHetHan(InVatTuDaHetHan inVatTuDaHetHan)//InXemVatTuDaHetHan
        {
            var result = _tonKhoService.XemVatTuDaHetHan(inVatTuDaHetHan);
            return Ok(result);
        }

        [HttpPost("ExportVatTuDaHetHan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.VatTuDaHetHan)]
        public async Task<ActionResult> ExportVatTuDaHetHan(QueryInfo queryInfo)
        {
            var gridData = await _tonKhoService.GetDanhSachVatTuDaHetHanForGridAsync(queryInfo, true);
            var vatTuDaHetHanData = gridData.Data.Select(p => (VatTuDaHetHanGridVo)p).ToList();
            var excelData = vatTuDaHetHanData.Map<List<VatTuDaHetHanExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(VatTuDaHetHanExportExcel.TenKho), "Kho"),
                (nameof(VatTuDaHetHanExportExcel.MaVatTu), "Mã Vật Tư"),
                (nameof(VatTuDaHetHanExportExcel.TenVatTu), "Vật Tư"),
                (nameof(VatTuDaHetHanExportExcel.DonViTinh), "Đơn Vị Tính"),
                (nameof(VatTuDaHetHanExportExcel.SoLo), "Sô Lô"),
                (nameof(VatTuDaHetHanExportExcel.ViTri), "Vị Trí"),
                (nameof(VatTuDaHetHanExportExcel.DonGiaNhap), "Đơn Giá Nhập"),
                (nameof(VatTuDaHetHanExportExcel.SoLuongTon), "Số Lượng Tồn"),
                (nameof(VatTuDaHetHanExportExcel.ThanhTien), "Thành Tiền"),
                (nameof(VatTuDaHetHanExportExcel.NgayHetHanHienThi), "Ngày Hết Hạn")

            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Vật Tư Đã Hết Hạn", 2, "Vật Tư Đã Hết Hạn");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTuDaHetHan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
