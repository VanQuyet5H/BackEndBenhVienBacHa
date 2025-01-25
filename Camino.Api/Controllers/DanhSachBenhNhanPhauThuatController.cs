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
using Camino.Services.BaoCaoDanhSachBenhNhanPhauThuatService;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhSachBenhNhanPhauThuatController : ControllerBase
    {
        private readonly IDanhSachBenhNhanPhauThuatService _danhSachBnPhauThuatService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DanhSachBenhNhanPhauThuatController(IDanhSachBenhNhanPhauThuatService danhSachBnPhauThuatService, ILocalizationService localizationService, IExcelService excelService)
        {
            _danhSachBnPhauThuatService = danhSachBnPhauThuatService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachBenhNhanPhauThuat)]
        public ActionResult<GridDataSource> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _danhSachBnPhauThuatService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachBenhNhanPhauThuat)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _danhSachBnPhauThuatService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoThucHienCls")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThucHienCls)]
        public async Task<ActionResult> ExportBaoCaoThucHienCls(QueryInfo queryInfo)
        {
            var gridData = _danhSachBnPhauThuatService.GetDataForGridAsync(queryInfo, true);
            var baoCaoThucHienClsData = gridData.Data.Select(p => (BaoCaoThucHienClsVo)p).ToList();
            var excelData = baoCaoThucHienClsData.Map<List<BaoCaoThucHienClsExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.ThoiGianChiDinhDisplay), "Thời gian chỉ định"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.MaTn), "Mã TN"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.HoTenBn), "Họ Tên BN"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.NgaySinhDisplay), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.GioiTinh), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.KhoaChiDinh), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.MaDv), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.DichVu), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.PhongThucHien), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.BsKetLuan), "Trạng thái"));
            lstValueObject.Add((nameof(BaoCaoThucHienClsExportExcel.ThoiGianCoKqDisplay), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Báo Cáo Thực Hiện Cận Lâm Sàng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThucHienCls" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
