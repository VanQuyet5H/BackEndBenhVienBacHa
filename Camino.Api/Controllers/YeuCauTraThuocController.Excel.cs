using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.DuocPham;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraThuocController
    {
        [HttpPost("ExportYeuCauTraThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.YeuCauHoanTraDuocPham)]
        public async Task<ActionResult> ExportYeuCauTraThuoc(QueryInfo queryInfo)
        {
            var gridData = await _ycHoanTraDuocPhamService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (DanhSachYeuCauHoanTraDuocPhamGridVo)p).ToList();
            var excelData = data.Map<List<DanhSachYeuCauHoanTraDuocPhamExportExcel>>();

            queryInfo.Sort = new List<Sort>();
            queryInfo.Sort.Add(new Sort
            {
                Field = "DuocPham",
                Dir = "asc"
            });

            foreach (var item in excelData)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _ycHoanTraDuocPhamService.GetDataForGridChildAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (DanhSachYeuCauHoanTraDuocPhamChiTietGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DanhSachYeuCauHoanTraDuocPhamChiTietExportExcelChild>>();
                item.DanhSachYeuCauHoanTraDuocPhamExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.Ma), "Số chứng từ"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.NguoiYeuCau), "Người yêu cầu"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.KhoHoanTraTu), "Hoàn trả từ kho"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.KhoHoanTraVe), "Hoàn trả về kho"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.NgayYeuCauText), "Ngày yêu cầu"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.TinhTrangDisplay), "Tình trạng"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.NguoiDuyet), "Người duyệt"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.NgayDuyetText), "Ngày duyệt"),
                (nameof(DanhSachYeuCauHoanTraDuocPhamExportExcel.DanhSachYeuCauHoanTraDuocPhamExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Yêu cầu hoàn trả dược phẩm", 2, "Danh sách yêu cầu hoàn trả dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=YcHoanTraDp" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
