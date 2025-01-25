using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Api.Models.ThongTinMienGiamThuNgan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class ThuNganController : CaminoBaseController
    {

        #region Bảng kê chi phí xem trước

        [HttpPost("XuatBangKeNgoaiTruCoBHYTExcel")]
        public ActionResult XuatBangKeNgoaiTruCoBHYTExcel(long yeuCauTiepNhanId)
        {
            var xuatBangKeNgoaiTruCoBHYTExcel = _yeuCauTiepNhanService.XuatBangKeNgoaiTruCoBHYTExcel(yeuCauTiepNhanId, true);
            if (xuatBangKeNgoaiTruCoBHYTExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKeCoBHYT"  + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruCoBHYTExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNgoaiTruExcel")]
        public ActionResult XuatBangKeNgoaiTruExcel(long yeuCauTiepNhanId)
        {
            var xuatBangKeNgoaiTruExcel = _yeuCauTiepNhanService.XuatBangKeNgoaiTruExcel(yeuCauTiepNhanId, true);
            if (xuatBangKeNgoaiTruExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKe"+ DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNgoaiTruTrongGoiDv")]
        public ActionResult XuatBangKeNgoaiTruTrongGoiDv(long yeuCauTiepNhanId)
        {
            var xuatBangKeNgoaiTruTrongGoiDv = _yeuCauTiepNhanService.XuatBangKeNgoaiTruTrongGoiDv(yeuCauTiepNhanId, true);
            if (xuatBangKeNgoaiTruTrongGoiDv == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNgoaiTruTrongGoiDv" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruTrongGoiDv, "application/vnd.ms-excel");

        }

        #endregion

        #region Bảng kê chi phí chờ thu

        [HttpPost("XuatBangKeNgoaiTruChoThuExcel")]
        public ActionResult XuatBangKeNgoaiTruChoThuExcel(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo)
        {
            var xuatBangKeNgoaiTruCoBHYTExcel = _yeuCauTiepNhanService.XuatBangKeNgoaiTruChoThuExcel(thuPhiKhamChuaBenhVo);
            if (xuatBangKeNgoaiTruCoBHYTExcel == null)
                return Ok(null);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNgoaiTruChoThuExcel" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruCoBHYTExcel, "application/vnd.ms-excel");
        }

        [HttpPost("XuatBangKeNgoaiTruTrongGoiChoThuExcel")]
        public ActionResult XuatBangKeNgoaiTruTrongGoiChoThuExcel(QuyetToanDichVuTrongGoiVo thuPhiKhamChuaBenhVo)
        {
            var xuatBangKeNgoaiTruExcel = _yeuCauTiepNhanService.XuatBangKeNgoaiTruTrongGoiChoThuExcel(thuPhiKhamChuaBenhVo);
            if (xuatBangKeNgoaiTruExcel == null)
                return Ok(null);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNgoaiTruTrongGoiChoThuExcel" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruExcel, "application/vnd.ms-excel");
        }

        #endregion

        #region Bảng kê chi phí đã thu

        [HttpPost("XuatBangKeNgoaiTruCoBHYTExcelDaThu")]
        public ActionResult XuatBangKeNgoaiTruCoBHYTExcelDaThu(long yeuCauTiepNhanId)
        {
            var xuatBangKeNgoaiTruCoBHYTExcel = _yeuCauTiepNhanService.XuatBangKeNgoaiTruCoBHYTExcel(yeuCauTiepNhanId);
            if (xuatBangKeNgoaiTruCoBHYTExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKeCoBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruCoBHYTExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNgoaiTruExcelDaThu")]
        public ActionResult XuatBangKeNgoaiTruExcelDaThu(long yeuCauTiepNhanId)
        {
            var xuatBangKeNgoaiTruExcel = _yeuCauTiepNhanService.XuatBangKeNgoaiTruExcel(yeuCauTiepNhanId);
            if (xuatBangKeNgoaiTruExcel == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKe"  + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruExcel, "application/vnd.ms-excel");

        }

        [HttpPost("XuatBangKeNgoaiTruTrongGoiDvDaThu")]
        public ActionResult XuatBangKeNgoaiTruTrongGoiDvDaThu(long yeuCauTiepNhanId)
        {
            var xuatBangKeNgoaiTruTrongGoiDv = _yeuCauTiepNhanService.XuatBangKeNgoaiTruTrongGoiDv(yeuCauTiepNhanId);
            if (xuatBangKeNgoaiTruTrongGoiDv == null)
                return Ok(null);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XuatBangKeNgoaiTruTrongGoiDv" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(xuatBangKeNgoaiTruTrongGoiDv, "application/vnd.ms-excel");

        }

        #endregion
    }
}