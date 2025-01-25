using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [HttpPost("GetDataForYeuCauNhanSuKhamSucKhoeGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetDataForYeuCauNhanSuKhamSucKhoeGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForYeuCauNhanSuKhamSucKhoeGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForYeuCauNhanSuKhamSucKhoeGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForYeuCauNhanSuKhamSucKhoeGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForYeuCauNhanSuKhamSucKhoeGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetDataForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpDelete("XuLyXoaYeuCauNhanSuKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanYeuCauNhanSuKhamSucKhoe)]
        public async Task<ActionResult> XuLyXoaYeuCauNhanSuKhamSucKhoeAsync(long id)
        {
            var khamDoan = await _khamDoanService.GetByYeuCauNhanSuKhamSucKhoeIdAsync(id, w => w.Include(q => q.YeuCauNhanSuKhamSucKhoeChiTiets));
            if (khamDoan.DuocNhanSuDuyet == true || khamDoan.DuocKHTHDuyet == true || khamDoan.DuocGiamDocDuyet == true)
            {
                throw new ApiException(_localizationService.GetResource("YeuCauNhanSuKhamSucKhoe.KhongDuocXoaNhanSuKhamSucKhoe"));
            }
            else
            {
                await _khamDoanService.DeleteByYcNhanSuKhamSucKhoeIdAsync(id);
            }          
            return NoContent();
        }

        [HttpPost("ExportDanhSachYeuCauNhanSuKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanHopDongKham)]
        public async Task<ActionResult> ExportDanhSachYeuCauNhanSuKhamSucKhoe(QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForYeuCauNhanSuKhamSucKhoeGridAsync(queryInfo, true);
            var congTyDatas = gridData.Data.Select(p => (KhamDoanYeuCauNhanSuKhamSucKhoeGridVo)p).ToList();

            var excelData = congTyDatas.Map<List<KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.HopDong), "Hợp đồng"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.TenCongTy), "Tên công ty"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.SoLuongBn), "SL BN"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.SoLuongBs), "SL BS"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.SoLuongDd), "SL ĐD"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NhanVienKhac), "NV Khác"),

                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NgayBatDauKhamDisplay), "Ngày BĐ khám"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NgayKetThucKhamDisplay), "Ngày KT khám"),

                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NguoiGui), "Người gửi"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NgayGuiDisplay), "Ngày gửi"),

                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.KhthDuyet), "KHTH duyệt"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NgayKhthDuyetDisplay), "Ngày KHTH duyệt"),

                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NsDuyet), "NS duyệt"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NgayNsDuyetDisplay), "Ngày NS duyệt"),

                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.GdDuyet), "GĐ duyệt"),
                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.NgayGdDuyetDisplay), "Ngày GĐ duyệt"),

                (nameof(KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel.TenTinhTrang), "Trạng thái"),
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Danh Sách Yêu Cầu Nhân Sự Khám Sức Khỏe", 2, "Danh Sách Yêu Cầu Nhân Sự Khám Sức Khỏe");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSHopDongKham" + DateTime.Now.Year + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}
