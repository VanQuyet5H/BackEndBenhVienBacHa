using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class QuayThuocController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChoCapThuocBHYTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDonThuocChoCapThuocBHYT)]
        public ActionResult<GridDataSource> GetDataChoCapThuocBHYTForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetDanhSachchoCapThuocBHYT(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalChoCapThuocBHYTPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDonThuocChoCapThuocBHYT)]
        public ActionResult<GridDataSource> GetTotalChoCapThuocBHYTPageForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetTotalDanhSachchoCapThuocBHYT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportDonThuocChoCapThuocBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachDonThuocChoCapThuocBHYT)]
        public async Task<ActionResult> ExportDonThuocChoCapThuocBHYT(QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetDanhSachchoCapThuocBHYT(queryInfo, true);
            var donThuocTrongNgay = gridData.Data.Select(p => (DonThuocThanhToanGridVo)p).ToList();
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DonThuocThanhToanGridVo>().ToList();
            foreach (var data in datas)
            {
                if (data.DateStart != null || data.DateEnd != null)
                {
                    queryInfo.AdditionalSearchString = data.Id.ToString() + '-' + data.DateStart + '-' + data.DateEnd;
                }
                else
                {
                    queryInfo.AdditionalSearchString = data.Id.ToString();
                }

                var gridDataChildNhaThuoc = await _quayThuocService.GetDanhSachThuocBenhNhanChild(queryInfo);
                data.ListChilDonThuocTrongNgay = gridDataChildNhaThuoc.Data.Cast<DonThuocCuaBenhNhanGridVo>().ToList();
            }
            var bytes = _quayThuocService.ExportDanhSachChoCapThuocBHYT(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDonThuocChoCapThuocBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        #region Danh sách cấp thuốc bhyt  

        [HttpPost("GetDanhSachLichSuCapThuocBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuXuatThuocCapThuocBHYT)]
        public ActionResult<GridDataSource> GetDanhSachLichSuCapThuocBHYT([FromBody]QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetDanhSachLichSuCapThuocBHYT(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalSachLichSuCapThuocBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuXuatThuocCapThuocBHYT)]
        public ActionResult<GridDataSource> GetTotalSachLichSuCapThuocBHYT([FromBody]QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetTotalSachLichSuCapThuocBHYT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportLichSuCapThuocBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuXuatThuocCapThuocBHYT)]
        public ActionResult ExportLichSuCapThuocBHYT(QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetDanhSachLichSuCapThuocBHYT(queryInfo, true);
            var lsBanThuoc = gridData.Data.Select(p => (DanhSachLichSuXuatThuocGridVo)p).ToList();
            var dataExcel = lsBanThuoc.Map<List<LichSuCapThuocBHYTExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.SoChungTu), "SỐ CHỨNG TỪ"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.MaTN), "MÃ TIẾP NHẬN"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.MaBN), "MÃ NGƯỜI BỆNH"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.HoTen), "TÊN NGƯỜI BỆNH"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.NamSinh), "NĂM SINH"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.GioiTinhHienThi), "GIỚI TÍNH"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.DiaChi), "ĐỊA CHỈ"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.SoDienThoai), "SỐ ĐIỆN THOẠI"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.DoiTuong), "ĐỐI TƯỢNG"));
            //lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.SoTienThuString), "TỔNG GIÁ TRỊ ĐƠN THUOC"));
            lstValueObject.Add((nameof(LichSuCapThuocBHYTExportExcel.NgayXuatThuocDisplay), "THỜI ĐIỂM CẤP PHÁT THUỐC"));
            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch Sử Cấp Thuốc BHYT", 2, "Lịch Sử Cấp Thuốc BHYT");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuCapThuocBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion
    }
}


