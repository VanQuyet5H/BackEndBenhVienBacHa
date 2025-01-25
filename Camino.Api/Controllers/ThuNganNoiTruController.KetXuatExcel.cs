using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class ThuNganNoiTruController : CaminoBaseController
    {
        #region Bảng kê chi phí xem trước

        [HttpPost("XuatBangKeNoiTruCoBHYTExcel")]
        public ActionResult XuatBangKeNoiTruCoBHYTExcel(long yeuCauTiepNhanId)
        {
            var xuatBangKeNoiTruCoBHYTExcel = _thuNganNoiTruService.XuatBangKeCoBHYTChuaQuyetToan(yeuCauTiepNhanId);
            if (xuatBangKeNoiTruCoBHYTExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeCoBHYTChuaQuyetToan" + yeuCauTiepNhanId + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruCoBHYTExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNoiTruExcel")]
        public ActionResult XuatBangKeNoiTruExcel(long yeuCauTiepNhanId)
        {
            var xuatBangKeNoiTruCoBHYTExcel = _thuNganNoiTruService.XuatBangKeChuaQuyetToan(yeuCauTiepNhanId);
            if (xuatBangKeNoiTruCoBHYTExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeChuaQuyetToan" + yeuCauTiepNhanId + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruCoBHYTExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeChuaQuyetToanTrongGoiDvExcel")]
        public ActionResult XuatBangKeChuaQuyetToanTrongGoiDvExcel(long yeuCauTiepNhanId)
        {
            var xuatBangKeNoiTruCoBHYTExcel = _thuNganNoiTruService.XuatBangKeChuaQuyetToanTrongGoiDv(yeuCauTiepNhanId);
            if (xuatBangKeNoiTruCoBHYTExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeChuaQuyetToanTrongGoiDv" + yeuCauTiepNhanId + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruCoBHYTExcel, "application/vnd.ms-excel");

        }

        #endregion

        #region Bảng kê chi phí chờ thu

        [HttpPost("XuatBangKeNoiTruChoThuExcel")]
        public ActionResult XuatBangKeNoiTruChoThuExcel(ThuPhiKhamChuaBenhNoiTruVo model)
        {
            var xuatBangKeNoiTruCoBHYTExcel = _thuNganNoiTruService.XuatBangKeNoiTruChoThu(model);
            if (xuatBangKeNoiTruCoBHYTExcel == null)
                return Ok(null);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNoiTruChoThu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruCoBHYTExcel, "application/vnd.ms-excel");
        }

        [HttpPost("XuatBangKeNoiTruTrongGoiChoThuExcel")]
        public ActionResult XuatBangKeNoiTruTrongGoiChoThuExcel(QuyetToanDichVuTrongGoiVo thuPhiKhamChuaBenhVo)
        {
            var xuatBangKeNoiTruExcel = _thuNganNoiTruService.XuatBangKeNoiTruChoThuTrongGoi(thuPhiKhamChuaBenhVo);
            if (xuatBangKeNoiTruExcel == null)
                return Ok(null);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNoiTruTrongGoiChoThuExcel" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruExcel, "application/vnd.ms-excel");
        }

        #endregion

        #region Bảng kê chi phí đã thu

        [HttpPost("XuatBangKeNoiTruCoBHYTExcelDaThu")]
        public ActionResult XuatBangKeNoiTruCoBHYTExcelDaThu(long taiKhoanThuId)
        {
            var xuatBangKeNoiTruCoBHYTExcel = _thuNganNoiTruService.XuatBangKeCoBHYT(taiKhoanThuId);
            if (xuatBangKeNoiTruCoBHYTExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKeCoBHYT"+ DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruCoBHYTExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNoiTruExcelDaThu")]
        public ActionResult XuatBangKeNoiTruExcelDaThu(long taiKhoanThuId)
        {
            var xuatBangKeNoiTruExcel = _thuNganNoiTruService.XuatBangKe(taiKhoanThuId);
            if (xuatBangKeNoiTruExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKe" +  DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNoiTruTrongGoiDvDaThu")]
        public ActionResult XuatBangKeNoiTruTrongGoiDvDaThu(long taiKhoanThuId)
        {
            var xuatBangKeNoiTruTrongGoiDv = _thuNganNoiTruService.XuatBangKeTrongGoiDv(taiKhoanThuId);
            if (xuatBangKeNoiTruTrongGoiDv == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNoiTruTrongGoiDv" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNoiTruTrongGoiDv, "application/vnd.ms-excel");

        }

        #endregion
    }
}
