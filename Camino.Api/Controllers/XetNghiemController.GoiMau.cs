using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class XetNghiemController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachGoiMauXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachGoiMauXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachGoiMauXetNghiemForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachGoiMauXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachGoiMauXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPagesDanhSachGoiMauXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachGoiMauNhomXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachGoiMauNhomXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachGoiMauNhomXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachGoiMauNhomXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachGoiMauNhomXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPagesDanhSachGoiMauNhomXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachGoiMauDichVuXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachGoiMauDichVuXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDanhSachGoiMauDichVuXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachGoiMauDichVuXetNghiemForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachGoiMauDichVuXetNghiemForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPagesDanhSachGoiMauDichVuXetNghiemForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("XoaPhieuGoiMauXetNghiem")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult> XoaPhieuGoiMauXetNghiem(long id)
        {
            await _xetNghiemService.XoaPhieuGoiMauXetNghiem(id);
            return NoContent();
        }

        [HttpPost("ExportGoiMauXetNghiem")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult> ExportGoiMauXetNghiem(QueryInfo queryInfo)
        {
            var gridGoiMauXetNghiemData = await _xetNghiemService.GetDanhSachGoiMauXetNghiemForGrid(queryInfo, true);
            if (gridGoiMauXetNghiemData == null || gridGoiMauXetNghiemData.Data.Count == 0)
            {
                return NoContent();
            }

            var goiMauXetNghiemData = gridGoiMauXetNghiemData.Data.Cast<GoiMauDanhSachXetNghiemGridVo>().ToList();
            foreach (var goiMauXetNghiem in goiMauXetNghiemData)
            {
                var queryNhomXetNghiem = new QueryInfo();
                queryNhomXetNghiem.AdditionalSearchString = queryInfo.AdditionalSearchString.Replace("}", "," + "\"PhieuGoiMauXetNghiemId\":\"" + goiMauXetNghiem.Id.ToString() + "\" }" );

                var dataNhoms = await _xetNghiemService.GetDanhSachGoiMauNhomXetNghiemForGrid(queryNhomXetNghiem, true);
                goiMauXetNghiem.GoiMauDanhSachNhomXetNghiems = dataNhoms.Data.Cast<GoiMauDanhSachNhomXetNghiemGridVo>().ToList();

                foreach (var nhomXetNghiem in goiMauXetNghiem.GoiMauDanhSachNhomXetNghiems)
                {
                    var queryDichVuXetNghiem = new QueryInfo();
                    queryDichVuXetNghiem.AdditionalSearchString = "{ PhienXetNghiemId: " + nhomXetNghiem.PhienXetNghiemId + ", NhomDichVuBenhVienId: " + nhomXetNghiem.NhomDichVuBenhVienId + " }";

                    var dataDichVus = await _xetNghiemService.GetDanhSachGoiMauDichVuXetNghiemForGrid(queryDichVuXetNghiem, true);
                    nhomXetNghiem.GoiMauDanhSachDichVuXetNghiems = dataDichVus.Data.Cast<GoiMauDanhSachDichVuXetNghiemGridVo>().ToList();
                }
            }

            var bytes = _excelService.ExportDanhSachGoiMauXetNghiem(goiMauXetNghiemData, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GoiMauXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        //[HttpPost("ExportGoiMauXetNghiem")]
        //[ClaimRequirement(SecurityOperation.Process, DocumentType.GoiMauXetNghiem)]
        //public async Task<ActionResult> ExportGoiMauXetNghiem(QueryInfo queryInfo)
        //{
        //    var gridGoiMauXetNghiemData = await _xetNghiemService.GetDanhSachGoiMauXetNghiemForGrid(queryInfo, true);
        //    var goiMauXetNghiemData = gridGoiMauXetNghiemData.Data.Select(p => (GoiMauDanhSachXetNghiemGridVo)p).ToList();
        //    var goiMauXetNghiemExcel = goiMauXetNghiemData.Map<List<GoiMauDanhSachXetNghiemExportExcel>>();

        //    var goiMauNhomXetNghiemExcel = new List<GoiMauDanhSachNhomXetNghiemExportExcelChild>();
        //    foreach (var item in goiMauXetNghiemExcel)
        //    {
        //        queryInfo.AdditionalSearchString = item.Id.ToString();
        //        var gridGoiMauNhomXetNghiemData = await _xetNghiemService.GetDanhSachGoiMauNhomXetNghiemForGrid(queryInfo, true);
        //        var goiMauNhomXetNghiemChild = gridGoiMauNhomXetNghiemData.Data.Select(p => (GoiMauDanhSachNhomXetNghiemGridVo)p).ToList();
        //        goiMauNhomXetNghiemExcel = goiMauNhomXetNghiemChild.Map<List<GoiMauDanhSachNhomXetNghiemExportExcelChild>>();
        //        item.GoiMauDanhSachXetNghiemExportExcelChild.AddRange(goiMauNhomXetNghiemExcel);
        //    }

        //    foreach (var item in goiMauNhomXetNghiemExcel)
        //    {
        //        queryInfo.AdditionalSearchString = "{ PhienXetNghiemId: " + item.PhienXetNghiemId + ", NhomDichVuBenhVienId: " + item.NhomDichVuBenhVienDisplay + " }";
        //        var gridGoiMauDichVuXetNghiemData = await _xetNghiemService.GetDanhSachGoiMauNhomXetNghiemForGrid(queryInfo, true);
        //        var goiMauDichVuXetNghiemChild = gridGoiMauDichVuXetNghiemData.Data.Select(p => (GoiMauDanhSachDichVuXetNghiemGridVo)p).ToList();
        //        var goiMauDichVuXetNghiemExcel = goiMauDichVuXetNghiemChild.Map<List<GoiMauDanhSachDichVuXetNghiemExportExcelChild>>();
        //        item.GoiMauDanhSachNhomXetNghiemExportExcelChildChild.AddRange(goiMauDichVuXetNghiemExcel);
        //    }

        //    var lstValueObject = new List<(string, string)>
        //    {
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.SoPhieu), "Số Phiếu"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.NguoiGoiMauDisplay), "Người Gởi Mẫu"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.NgayGoiMauDisplay), "Ngày Gởi Mẫu"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.SoLuongMau), "SL Mẫu (KQ/Tổng)"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.TinhTrangDisplay), "Tình Trạng"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.NoiTiepNhan), "Nơi Tiếp Nhận"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.NguoiNhanMauDisplay), "Người Nhận Mẫu"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.NgayNhanMauDisplay), "Ngày Nhận Mẫu"),
        //        (nameof(GoiMauDanhSachXetNghiemExportExcel.GoiMauDanhSachXetNghiemExportExcelChild), "")
        //    };

        //    var bytes = _excelService.ExportManagermentView(goiMauXetNghiemExcel, lstValueObject, "Gởi Mẫu Xét Nghiệm", 3, "Gởi Mẫu Xét Nghiệm");

        //    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GoiMauXetNghiem" + DateTime.Now.Year + ".xls");
        //    Response.ContentType = "application/vnd.ms-excel";

        //    return new FileContentResult(bytes, "application/vnd.ms-excel");
        //}

        [HttpGet("GetPhieuGoiMauXetNghiem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<PhieuGoiMauXetNghiemViewModel>> GetPhieuGoiMauXetNghiem(long id)
        {
            var phieuGoiMauXetNghiem = await _xetNghiemService.GetPhieuGoiMauXetNghiem(id);

            PhieuGoiMauXetNghiemViewModel phieuGoiMauXetNghiemVM = phieuGoiMauXetNghiem.ToModel<PhieuGoiMauXetNghiemViewModel>();

            return Ok(phieuGoiMauXetNghiemVM);
        }

        [HttpGet("GetTongSoLuongMauGoiMauXetNghiem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.GoiMauXetNghiem, DocumentType.NhanMauXetNghiem)]
        public async Task<ActionResult<int>> GetTongSoLuongMauGoiMauXetNghiem(long id)
        {
            QueryInfo queryInfo = new QueryInfo();

            return Ok(await _xetNghiemService.TongSoLuongMauGoiMauXetNghiem(queryInfo, id));
        }
        #region search grid popup  in 
        [HttpPost("TimKiemGridPopUpXetNghiem")]
        public async Task<ActionResult> TimKiemGridPopUpXetNghiem(TimKiemPopupInXetNghiemVo model)
        {
            var grid = await _xetNghiemService.GetDanhSachSearchPopupForGrid(model);
            return Ok(grid);
        }
        #endregion
    }
}