using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
// include all APIs connected to the screen that confirms bhyts for a day
namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        #region Connected To A List Of Bhyt Data
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var filter = new FilterDanhSachBHYTGridVo();

            if (queryInfo.AdditionalSearchString != null)
            {
                filter = JsonConvert.DeserializeObject<FilterDanhSachBHYTGridVo>(queryInfo.AdditionalSearchString);
            }

            if (filter.DaXacNhan == null)
            {
                filter = new FilterDanhSachBHYTGridVo
                {
                    DaXacNhan = true,
                    ChuaXacNhan = true
                };
            }

            if (filter.DaXacNhan == true && filter.ChuaXacNhan == false)
            {
                var gridData = await _bhytConfirmByDayService.GetDataForDaXacNhanAsync(queryInfo);
                return Ok(gridData);
            }

            if (filter.DaXacNhan == false && filter.ChuaXacNhan == true)
            {
                var gridData = await _bhytConfirmByDayService.GetDataForGridAsync(queryInfo);
                return Ok(gridData);
            }

            var gridDataAll = await _bhytConfirmByDayService.GetDataForBothBhyt(queryInfo);
            return Ok(gridDataAll);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var filter = new FilterDanhSachBHYTGridVo();

            if (queryInfo.AdditionalSearchString != null)
            {
                filter = JsonConvert.DeserializeObject<FilterDanhSachBHYTGridVo>(queryInfo.AdditionalSearchString);
            }

            if (filter.DaXacNhan == null)
            {
                filter = new FilterDanhSachBHYTGridVo
                {
                    DaXacNhan = true,
                    ChuaXacNhan = true
                };
            }

            if (filter.DaXacNhan == true && filter.ChuaXacNhan == false)
            {
                var gridData = await _bhytConfirmByDayService.GetTotalPageForDaXacNhanAsync(queryInfo);
                return Ok(gridData);
            }

            if (filter.DaXacNhan == false && filter.ChuaXacNhan == true)
            {
                var gridData = await _bhytConfirmByDayService.GetTotalPageForGridAsync(queryInfo);
                return Ok(gridData);
            }

            var gridDataAll = await _bhytConfirmByDayService.GetTotalPageForBothBhyt(queryInfo);
            return Ok(gridDataAll);
        }

        [HttpPost("ExportXacNhanBhyt")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult> ExportXacNhanBhyt([FromBody]QueryInfo queryInfo)
        {
            var filter = new FilterDanhSachBHYTGridVo();

            if (queryInfo.AdditionalSearchString != null)
            {
                filter = JsonConvert.DeserializeObject<FilterDanhSachBHYTGridVo>(queryInfo.AdditionalSearchString);
            }

            if (filter.DaXacNhan == null)
            {
                filter = new FilterDanhSachBHYTGridVo
                {
                    DaXacNhan = true,
                    ChuaXacNhan = true
                };
            }

            GridDataSource gridData;

            if (filter.DaXacNhan == true && filter.ChuaXacNhan == false)
            {
                gridData = await _bhytConfirmByDayService.GetDataForDaXacNhanAsync(queryInfo, true);
            }
            else if (filter.DaXacNhan == false && filter.ChuaXacNhan == true)
            {
                gridData = await _bhytConfirmByDayService.GetDataForGridAsync(queryInfo, true);
            }
            else
            {
                gridData = await _bhytConfirmByDayService.GetDataForBothBhyt(queryInfo, true);
            }

            var xacNhanBhytData = gridData.Data.Select(p => (DanhSachChoGridVo)p).ToList();
            var dataExcel = xacNhanBhytData.Map<List<XacNhanBhytExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.MaTN), "Mã TN"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.MaBN), "Mã BN"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.HoTen), "Họ Tên"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.NamSinh), "Năm Sinh"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.TenGioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.SoDienThoai), "Số Điện Thoại"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.SoTienDaXacNhan), "Số Tiền Đã XN"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.SoTienChoXacNhan), "Số Tiền Chờ XN"));
            lstValueObject.Add((nameof(XacNhanBhytExportExcel.Status), "Trạng Thái"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "DS Xác Nhận BHYT");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSXacNhanBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Connected To Scanning Barcode For Bhyt
        [HttpPost("GetXacNhanBHYTByMaBNVaMaTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DanhSachChoGridVo[]>> GetXacNhanBhytByMaBnVaMaTt(TimKiemThongTinBenhNhan timKiemThongTinBenhNhan)
        {
            var ketQuaTimKiem = await _bhytConfirmByDayService.GetXacNhanBhytByMaBnVaMaTt(timKiemThongTinBenhNhan);
            return Ok(ketQuaTimKiem);
        }
        #endregion
    }
}
