using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.VatTu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraVatTuController
    {
        [HttpPost("ExportYeuCauTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.YeuCauHoanTraVatTu)]
        public async Task<ActionResult> ExportYeuCauTraVatTu(QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraVatTuService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (DanhSachYeuCauHoanTraVatTuGridVo)p).ToList();
            var excelData = data.Map<List<DanhSachYeuCauHoanTraVatTuExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort
            {
                Field = "VatTu",
                Dir = "asc"
            });

            foreach (var item in excelData)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _ycHoanTraVatTuService.GetDataForGridChildAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (DanhSachYeuCauHoanTraVatTuChiTietGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DanhSachYeuCauHoanTraVatTuChiTietExportExcelChild>>();
                item.DanhSachYeuCauHoanTraVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.Ma), "Số chứng từ"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.NguoiYeuCau), "Người yêu cầu"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.KhoHoanTraTu), "Hoàn trả từ kho"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.KhoHoanTraVe), "Hoàn trả về kho"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.NgayYeuCauText), "Ngày yêu cầu"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.TinhTrangDisplay), "Tình trạng"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.NguoiDuyet), "Người duyệt"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.NgayDuyetText), "Ngày duyệt"),
                (nameof(DanhSachYeuCauHoanTraVatTuExportExcel.DanhSachYeuCauHoanTraVatTuExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Yêu cầu hoàn trả vật tư", 2, "Danh sách yêu cầu hoàn trả vật tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YcHoanTraVtu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
