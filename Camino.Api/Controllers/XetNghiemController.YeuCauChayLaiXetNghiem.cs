using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class XetNghiemController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachYeuCauChayLaiXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachYeuCauChayLaiXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachYeuCauChayLaiXetNghiemForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachYeuCauChayLaiXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachYeuCauChayLaiXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalDanhSachYeuCauChayLaiXetNghiemForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachYeuCauChayLaiXetNghiemChiTietForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachYeuCauChayLaiXetNghiemChiTietForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachYeuCauChayLaiXetNghiemChiTietForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachYeuCauChayLaiXetNghiemChiTietForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachKQChiTietXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public ActionResult<GridDataSource> GetDanhSachKQChiTietXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = _xetNghiemService.GetDanhSachKQChiTietXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachKQChiTietXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public ActionResult<GridDataSource> GetTotalDanhSachKQChiTietXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = _xetNghiemService.GetTotalDanhSachKQChiTietXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ThongTinHanhChinhXN/{phienXetNghiemId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public ThongTinHanhChinhXN ThongTinHanhChinhXN(long phienXetNghiemId)
        {
            var ThongTinHanhChinhXN = _xetNghiemService.ThongTinHanhChinhXN(phienXetNghiemId);
            return ThongTinHanhChinhXN;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiXetNghiemLai")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public ActionResult<bool> TuChoiXetNghiem(TuChoiYeuCauGoiLaiXetNghiem tuChoiYeuCauGoiLaiXetNghiem)
        {
            var tuchoi = _xetNghiemService.TuChoiXetNghiem(tuChoiYeuCauGoiLaiXetNghiem);
            return Ok(tuchoi);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetXetNghiemLai")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> DuyetXetNghiem(DanhSachGoiXetNghiemLai duyetYeuCauGoiLaiXetNghiem)
        {
            await _xetNghiemService.DuyetXetNghiem(duyetYeuCauGoiLaiXetNghiem);
            return Ok(null);
        }

        [HttpPost("ExportYeuCauXetNghiemChayLai")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DuyetYeuCauChayLaiXetNghiem)]
        public async Task<ActionResult> ExportYeuCauXetNghiemChayLai(QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachYeuCauChayLaiXetNghiemForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (YeuCauGoiLaiXetNghiemGridVo)p).ToList();
            var dataExcel = data.Map<List<YeuCauXetNghiemChayLaiExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.PhienXetNghiemId.ToString();
                var gridChildData = await _xetNghiemService.GetDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (YeuCauGoiLaiXetNghiemChiTietGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<YeuCauXetNghiemChayLaiExportExcelChild>>();
                item.YeuCauXetNghiemChayLaiExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(YeuCauXetNghiemChayLaiExportExcel.MaBarCode), "BarCode"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.MaTN), "Mã TN"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.MaBN), "Mã BN"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.HoTen), "Họ tên"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.GioiTinh), "Giới tính"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.NamSinh), "Năm sinh"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.DiaChi), "Địa chỉ"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.TenTrangThai), "Trạng thái"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.NguoiDuyetKetQua), "Người duyệt KQ"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.NgayDuyetKetQuaDisplay), "Ngày duyệt KQ"),
                (nameof(YeuCauXetNghiemChayLaiExportExcel.YeuCauXetNghiemChayLaiExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Yêu cầu xét nghiệm chạy lại", 2, "Yêu cầu xét nghiệm chạy lại");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YeuCauXNChayLai" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
