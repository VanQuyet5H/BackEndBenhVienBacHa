using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DuocPhamBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class DuocPhamDaHetHanController : CaminoBaseController
    {
        private readonly IDuocPhamDaHetHanService _duocPhamDaHetHanService;
        private readonly IExcelService _excelService;

        public DuocPhamDaHetHanController(IDuocPhamDaHetHanService duocPhamDaHetHanService, IExcelService excelService)
        {
            _duocPhamDaHetHanService = duocPhamDaHetHanService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamDaHetHan)]
        public ActionResult<GridDataSource> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _duocPhamDaHetHanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamDaHetHan)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _duocPhamDaHetHanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("InDanhMucDaHetHan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamDaHetHan)]
        public ActionResult InDanhMucDaHetHan(string search)
        {
            var result = _duocPhamDaHetHanService.GetHtml(search);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetHTML")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuocPhamDaHetHan)]
        public ActionResult GetHtml(string search)
        {
            var result = _duocPhamDaHetHanService.GetHtml(search);
            return Ok(result);
        }

        [HttpPost("ExportDuocPhamDaHetHan")]
        [ClaimRequirement(
            Enums.SecurityOperation.Process,
            Enums.DocumentType.DuocPhamDaHetHan
        )]
        public ActionResult ExportDuocPhamDaHetHan(QueryInfo queryInfo)
        {
            var gridData = _duocPhamDaHetHanService.GetDataForGridAsync(queryInfo, true);
            var duocPhamDaHetHanData = gridData.Data.Select(p => (DuocPhamDaHetHanGridVo)p).ToList();
            var dataExcel = duocPhamDaHetHanData.Map<List<DuocPhamDaHetHanExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuocPhamDaHetHanExportExcel.Kho), "Kho"),
                (nameof(DuocPhamDaHetHanExportExcel.MaDuocPham), "Mã Dược"),
                (nameof(DuocPhamDaHetHanExportExcel.DuocPham), "Dược Phẩm"),
                (nameof(DuocPhamDaHetHanExportExcel.HamLuong), "Hàm Lượng"),
                (nameof(DuocPhamDaHetHanExportExcel.HoatChat), "Hoạt Chất"),
                (nameof(DuocPhamDaHetHanExportExcel.DonViTinh), "Đơn Vị Tính"),
                (nameof(DuocPhamDaHetHanExportExcel.SoLo), "Số Lô"),
                (nameof(DuocPhamDaHetHanExportExcel.ViTri), "Vị Trí"),
                (nameof(DuocPhamDaHetHanExportExcel.DonGiaNhap), "Đơn Giá Nhập"),
                (nameof(DuocPhamDaHetHanExportExcel.SoLuongTon), "Số Lượng Tồn"),
                (nameof(DuocPhamDaHetHanExportExcel.ThanhTien), "Thành Tiền"),
                (nameof(DuocPhamDaHetHanExportExcel.NgayHetHanDisplay), "Ngày Hết Hạn")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Dược Phẩm Đã Hết Hạn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuocPhamDaHethan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}