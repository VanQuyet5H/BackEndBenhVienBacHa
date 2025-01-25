using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBhytDaHoanThanhController
    {
        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xacNhanBhytDaHoanThanhListService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xacNhanBhytDaHoanThanhListService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportXacNhanBhytDaHoanThanh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult<GridDataSource>> ExportXacNhanBhytDaHoanThanh
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _xacNhanBhytDaHoanThanhListService.GetDataForGridAsync(queryInfo, true);
            var xacNhanBhytDaHoanThanhData = gridData.Data.Select(p => (ListXacNhanBhytDaHoanThanhGridVo)p).ToList();
            var dataExcel = xacNhanBhytDaHoanThanhData.Map<List<ListXacNhanBhytDaHoanThanhExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.MaTn), "Mã TN"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.MaBn), "Mã BN"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.HoTen), "Họ Tên"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.NamSinh), "Năm Sinh"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.GioiTinh), "Giới Tính"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.DiaChi), "Địa Chỉ"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.SoDienThoai), "Số Điện Thoại"),
                (nameof(ListXacNhanBhytDaHoanThanhExportExcel.SoTienDaXacNhan), "Số Tiền Đã XN")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Xác Nhận Đã Hoàn Thành", 2, "Xác Nhận Đã Hoàn Thành");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XacNhanDaHoanThanh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
