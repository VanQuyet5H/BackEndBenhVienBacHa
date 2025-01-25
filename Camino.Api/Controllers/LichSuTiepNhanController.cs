using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Services.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Helpers;
using Camino.Services.ExportImport;

namespace Camino.Api.Controllers
{
    public class LichSuTiepNhanController : CaminoBaseController
    {
        private readonly ILichSuTiepNhanService _lichSuTiepNhanService;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public LichSuTiepNhanController(ILichSuTiepNhanService lichSuTiepNhanService, IDanhSachChoKhamService danhSachChoKhamService, IExcelService excelService, ILocalizationService localizationService, IJwtFactory iJwtFactory)
        {
            _lichSuTiepNhanService = lichSuTiepNhanService;
            _localizationService = localizationService;
            _danhSachChoKhamService = danhSachChoKhamService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncLichSuTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuTiepNhan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuTiepNhanService.GetDataForGridAsyncLichSuTiepNhan(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncLichSuTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuTiepNhan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuTiepNhanService.GetTotalPageForGridAsyncLichSuTiepNhan(queryInfo);
            return Ok(gridData);
        }

        #region InPhieuLichSuCacDichVuKhamBenh
        [HttpGet("InPhieuLichSuCacDichVuKhamBenh")]
        public ActionResult InPhieuLichSuCacDichVuKhamBenh(long yeuCauTiepNhanId, string hostingName, bool header, bool laPhieuKhamBenh)//InPhieuLichSuCacDichVuKhamBenh
        {
            var result = _danhSachChoKhamService.InPhieuCacDichVuKhamBenh(yeuCauTiepNhanId, hostingName, header, laPhieuKhamBenh);
            return Ok(result);
        }
        #endregion

        #region Excel
        [HttpPost("ExportLichSuTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuTiepNhan)]
        public async Task<ActionResult> ExportLichSuTiepNhan(QueryInfo queryInfo)
        {
            var gridData = await _lichSuTiepNhanService.GetDataForGridAsyncLichSuTiepNhan(queryInfo, true);
            var lichSuTiepNhanData = gridData.Data.Select(p => (DanhSachChoKhamGridVo)p).ToList();
            var excelData = lichSuTiepNhanData.Map<List<DanhSachTiepNhanExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachTiepNhanExportExcel.MaYeuCauTiepNhan), "Mã TN"),
                (nameof(DanhSachTiepNhanExportExcel.MaBenhNhan), "Mã BN"),
                (nameof(DanhSachTiepNhanExportExcel.HoTen), "Tên người bệnh"),
                (nameof(DanhSachTiepNhanExportExcel.NamSinh), "Năm sinh"),
                (nameof(DanhSachTiepNhanExportExcel.DiaChi), "Địa chỉ"),
                (nameof(DanhSachTiepNhanExportExcel.TenNhanVienTiepNhan), "Người tiếp nhận"),
                (nameof(DanhSachTiepNhanExportExcel.ThoiDiemTiepNhanDisplay), "Tiếp nhận lúc"),
                (nameof(DanhSachTiepNhanExportExcel.TrieuChungTiepNhan), "Lý do khám bệnh"),
                (nameof(DanhSachTiepNhanExportExcel.DoiTuong), "Đối tượng"),
                (nameof(DanhSachTiepNhanExportExcel.CoBaoHiemTuNhan), "Bảo hiểm tư nhân"),
                (nameof(DanhSachTiepNhanExportExcel.TrangThaiYeuCauTiepNhanSearch), "Trạng thái")

            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch Sử Tiếp Nhận", 2, "Lịch Sử Tiếp Nhận");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuTiepNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}