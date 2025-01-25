using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoThucHienCls;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BaoCaoThucHienCls;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class BaoCaoThucHienClsController : CaminoBaseController
    {
        private readonly IBaoCaoThucHienClsService _baoCaoThucHienClsService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public BaoCaoThucHienClsController(IBaoCaoThucHienClsService baoCaoThucHienClsService, ILocalizationService localizationService, IExcelService excelService)
        {
            _baoCaoThucHienClsService = baoCaoThucHienClsService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
      
        #region BẢNG KÊ BÁC SĨ THỰC HIỆN CẬN LÂM SÀNG

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThucHienCls)]
        public ActionResult<GridDataSource> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _baoCaoThucHienClsService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }      

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoThucHienCls)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoThucHienClsService.GetDataForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }
               
        //[HttpPost("ExportBaoCaoThucHienCls")]
        //[ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThucHienCls)]
        //public async Task<ActionResult> ExportBaoCaoThucHienCls(QueryInfo queryInfo)
        //{
        //    var gridData = _baoCaoThucHienClsService.GetDataForGridAsync(queryInfo, true);
        //    var baoCaoThucHienClsData = gridData.Data.Select(p => (BaoCaoThucHienClsVo)p).ToList();
        //    var excelData = baoCaoThucHienClsData.Map<List<BaoCaoThucHienClsExportExcel>>();

        //    var lstValueObject = new List<(string, string)>
        //    {
        //        (nameof(BaoCaoThucHienClsExportExcel.ThoiGianChiDinhDisplay), "Thời gian chỉ định"),
        //        (nameof(BaoCaoThucHienClsExportExcel.MaTn), "Mã TN"),
        //        (nameof(BaoCaoThucHienClsExportExcel.HoTenBn), "Họ Tên BN"),
        //        (nameof(BaoCaoThucHienClsExportExcel.NgaySinhDisplay), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.GioiTinh), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.KhoaChiDinh), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.MaDv), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.DichVu), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.PhongThucHien), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.BsKetLuan), "Trạng thái"),
        //        (nameof(BaoCaoThucHienClsExportExcel.ThoiGianCoKqDisplay), "Trạng thái")
        //    };

        //    var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Báo Cáo Thực Hiện Cận Lâm Sàng");

        //    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThucHienCls" + DateTime.Now.Year + ".xls");
        //    Response.ContentType = "application/vnd.ms-excel";

        //    return new FileContentResult(bytes, "application/vnd.ms-excel");
        //}

        [HttpPost("GetBaoCaoBangKeBacSiCLSForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThucHienCls)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoBaoCaoBangKeBacSiCLSForGridAsync(QueryInfo queryInfo)
        {
            var gridData =  _baoCaoThucHienClsService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoBangKeBacSiCLS")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThucHienCls)]
        public async Task<ActionResult> ExportBaoCaoBangKeBacSiCLS(BaoCaoThucHienCLSVo queryInfo)
        {
            byte[] bytes = _baoCaoThucHienClsService.ExportBaoCaoBangKeBacSiCLS(queryInfo); ;
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThucHienCls" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region HOẠT ĐỘNG CẬN LÂM SÀNG THEO KHOA

        [HttpPost("GetDataForGridAsyncHoatDongCLS")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoHoatDongClsTheoKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncHoatDongCLS([FromBody]BaoCaoHoatDongCLSVo queryInfo)
        {
            var gridData = await _baoCaoThucHienClsService.GetDataForGridAsyncHoatDongCLS(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncHoatDongCLS")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoHoatDongClsTheoKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncHoatDongCLS([FromBody]BaoCaoHoatDongCLSVo queryInfo)
        {
            var gridData = await _baoCaoThucHienClsService.GetTotalPageForGridAsyncHoatDongCLS(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoHoatDongClsTheoKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongClsTheoKhoa)]
        public async Task<ActionResult> ExportBaoCaoHoatDongClsTheoKhoa(BaoCaoHoatDongCLSVo queryInfo)
        {
            byte[] bytes = _baoCaoThucHienClsService.ExportBaoCaoHoatDongClsTheoKhoa(queryInfo); ;
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongClsTheoKhoa" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region SỔ THỐNG KÊ CẬN LÂM SÀNG

        [HttpPost("GetDataSoThongKeCLSForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoSoThongKeCls)]
        public async Task<ActionResult<GridDataSource>> GetDataSoThongKeCLSForGridAsync([FromBody] BaoCaoSoThongKeCLSChiTietVo queryInfo)
        {
            var gridData = await _baoCaoThucHienClsService.GetDataSoThongKeCLSForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageSoThongKeCLSForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoSoThongKeCls)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageSoThongKeCLSForGridAsyn([FromBody] BaoCaoSoThongKeCLSChiTietVo queryInfo)
        {
            //bo lazy load
            var gridData = await _baoCaoThucHienClsService.GetDataSoThongKeCLSForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("ExportBaoCaoSoThongKeCls")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoThongKeCls)]
        public async Task<ActionResult> ExportBaoCaoSoThongKeCls(BaoCaoSoThongKeCLSChiTietVo queryInfo)
        {
            byte[] bytes = _baoCaoThucHienClsService.ExportBaoCaoSoThongKeCls(queryInfo); ;
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoSoThongKeCLS" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        [HttpPost("KhoaPhongs")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> KhoaPhongs(DropDownListRequestModel model)
        {
            var lookup = await _baoCaoThucHienClsService.KhoaPhongs(model);
            return Ok(lookup);
        }
    }
}
