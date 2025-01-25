using Camino.Api.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDieuTriNoiTruForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDieuTriNoiTruForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachDieuTriNoiTruForGrid(queryInfo, false);          
            return Ok(gridData);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachDieuTriNoiTruForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachDieuTriNoiTruForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPagesDanhSachDieuTriNoiTruForGrid(queryInfo);
            return Ok(gridData);

        }

        [HttpPost("ExportDanhSachNoiTru")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ExportDanhSachNoiTru(QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachDieuTriNoiTruForGrid(queryInfo, true);
            var dataNoiTrus = gridData.Data.Select(p => (DanhSachNoiTruBenhAnGridVo)p).ToList();
            var dataExcel = dataNoiTrus.Map<List<DanhSachNoiTruExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachNoiTruExportExcel.Phong), "Phòng"),
                (nameof(DanhSachNoiTruExportExcel.MaTiepNhan), "Mã TN"),
                (nameof(DanhSachNoiTruExportExcel.SoBenhAn), "Số BA"),
                (nameof(DanhSachNoiTruExportExcel.HoTen), "Họ tên"),
                (nameof(DanhSachNoiTruExportExcel.NamSinhDisplay), "Năm sinh"),
                (nameof(DanhSachNoiTruExportExcel.GioiTinhDisplay), "GT"),
                (nameof(DanhSachNoiTruExportExcel.DoiTuong), "Đối tượng"),
                (nameof(DanhSachNoiTruExportExcel.CapCuu), "CC"),
                (nameof(DanhSachNoiTruExportExcel.ChanDoan), "Chẩn đoán"),
                (nameof(DanhSachNoiTruExportExcel.ThoiGianNhapVienDisplay), "Thời gian nhập viện"),
                (nameof(DanhSachNoiTruExportExcel.ThoiGianRaVienDisplay), "Thời gian ra viện"),
                (nameof(DanhSachNoiTruExportExcel.TenTrangThai), "Trạng thái"),
                (nameof(DanhSachNoiTruExportExcel.MaBenhNhan), "Mã NB"),
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Danh Sách Điều Trị Nội Trú", 2, "Danh Sách Điều Trị Nội Trú");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDieuTriNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
