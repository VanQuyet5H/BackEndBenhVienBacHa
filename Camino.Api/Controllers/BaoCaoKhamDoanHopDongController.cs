using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BaoCaoKhamDoanHopDong;
using static Camino.Core.Domain.Enums;
using System;
using System.Linq;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using Camino.Services.Helpers;
using System.Collections.Generic;
using Camino.Services.ExportImport;
using Camino.Services.KhamDoan;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Api.Controllers
{

    public class BaoCaoKhamDoanHopDongController : CaminoBaseController
    {
        private readonly IBaoCaoKhamDoanHopDongServices _baoCaoKhamDoanHopDongService;
        private readonly IExcelService _excelService;
        private readonly IKhamDoanService _khamDoanService;

        public BaoCaoKhamDoanHopDongController(
            IBaoCaoKhamDoanHopDongServices baoCaoKhamDoanHopDongService,
            IExcelService excelService,
            IKhamDoanService khamDoanService
                  )
        {
            _baoCaoKhamDoanHopDongService = baoCaoKhamDoanHopDongService;
            _excelService = excelService;
            _khamDoanService = khamDoanService;
        }
        #region CÔNG TY THEO HỢP ĐỒNG

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncTheoHopDong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public ActionResult<GridDataSource> GetDataForGridAsyncTheoHopDong([FromBody]QueryInfo queryInfo)
        {
            //var gridData = _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoHopDong(queryInfo);
            var gridData = _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoHopDongVer2(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncTheoHopDong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsyncTheoHopDong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _baoCaoKhamDoanHopDongService.GetTotalPageForGridAsyncTheoHopDong(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region NHÂN VIÊN THEO HỢP ĐỒNG

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncTheoNhanVien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncTheoNhanVien([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVien(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncTheoNhanVien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncTheoNhanVien([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetTotalPageForGridAsyncTheoNhanVien(queryInfo);
            return Ok(gridData);
        }


        //[HttpPost("ExportBaoCaoDichVuTrongGoiKhamDoan")]
        //[ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        //public async Task<ActionResult> ExportBaoCaoDichVuTrongGoiKhamDoan(QueryInfo queryInfo)
        //{
        //    var baoCaoKhamDoanHopDongTheoNhanVienVo = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVien(queryInfo, true);
        //    var yeuCauTraThuocTuBenhNhanGridVos = baoCaoKhamDoanHopDongTheoNhanVienVo.Data.Cast<BaoCaoKhamDoanHopDongTheoNhanVienVo>().ToList();
        //    var bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoDichVuTrongGoiKhamDoan(yeuCauTraThuocTuBenhNhanGridVos);
        //    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSPhieuTraThuocTuBenhNhanNoiTru" + DateTime.Now.Year + ".xls");
        //    Response.ContentType = "application/vnd.ms-excel";
        //    return new FileContentResult(bytes, "application/vnd.ms-excel");

        //}


        [HttpPost("ExportBaoCaoDichVuTrongGoiKhamDoanChuaKham")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult> ExportBaoCaoDichVuTrongGoiKhamDoanChuaKham(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVien(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (BaoCaoKhamDoanHopDongTheoNhanVienVo)p).ToList();
            var excelData = chucVuData.Map<List<BaoCaoKhamDoanHopDongTheoNhanVienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaBN), "Mã Người Bệnh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaTN), "Mã Tiếp Nhận"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.HoTen), "Họ tên"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.NamSinh), "Năm sinh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.GioiTinhDisplay), "Giới tính"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.DichVuChuaKham), "Chuyên khoa không khám"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.TenCongTyKhamSucKhoe), "Tên công ty")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG", 2, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDichVuTrongGoiKhamDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportBaoCaoDichVuTrongGoiKhamDoanDangKham")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult> ExportBaoCaoDichVuTrongGoiKhamDoanDangKham(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVien(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (BaoCaoKhamDoanHopDongTheoNhanVienVo)p).ToList();
            var excelData = chucVuData.Map<List<BaoCaoKhamDoanHopDongTheoNhanVienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaBN), "Mã Người Bệnh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaTN), "Mã Tiếp Nhận"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.HoTen), "Họ tên"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.NamSinh), "Năm sinh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.GioiTinhDisplay), "Giới tính"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.DichVuChuaKham), "Chuyên khoa chưa khám"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.TenCongTyKhamSucKhoe), "Tên công ty")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG", 2, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDichVuTrongGoiKhamDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportBaoCaoDichVuTrongGoiKhamDoanDaKham")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult> ExportBaoCaoDichVuTrongGoiKhamDoanDaKham(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVien(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (BaoCaoKhamDoanHopDongTheoNhanVienVo)p).ToList();
            var excelData = chucVuData.Map<List<BaoCaoKhamDoanHopDongTheoNhanVienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaBN), "Mã Người Bệnh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaTN), "Mã Tiếp Nhận"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.HoTen), "Họ tên"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.NamSinh), "Năm sinh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.GioiTinhDisplay), "Giới tính"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.DichVuDaKham), "Chuyên khoa đã khám"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.TenCongTyKhamSucKhoe), "Tên công ty")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG", 2, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDichVuTrongGoiKhamDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportBaoCaoDichVuTrongGoiKhamDoanChiTiet")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult> ExportBaoCaoDichVuTrongGoiKhamDoanChuaKhamChiTiet(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVien(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (BaoCaoKhamDoanHopDongTheoNhanVienVo)p).ToList();
            var excelData = chucVuData.Map<List<BaoCaoKhamDoanHopDongTheoNhanVienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaNV), "Mã nhân viên"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaBN), "Mã Người Bệnh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.MaTN), "Mã Tiếp Nhận"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.HoTen), "Họ tên"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.NamSinh), "Năm sinh"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.GioiTinhDisplay), "Giới tính"),
                (nameof(BaoCaoKhamDoanHopDongTheoNhanVienExportExcel.ThoiDiemThucHienDisplay), "Ngày hoàn thành khám")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG", 2, "DANH SÁCH BỆNH NHÂN KHÁM SỨC KHỎE THEO HỢP ĐỒNG");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDichVuTrongGoiKhamDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion


        #region DỊCH VỤ NGOÀI GÓI

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDichVuNgoai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDichVuNgoai([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncDichVuNgoai(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDichVuNgoai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDichVuNgoai([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetTotalPageForGridAsyncDichVuNgoai(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoDichVuTrongGoiKhamDoan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoDichVuTrongGoiKhamDoan)]
        public async Task<ActionResult> ExportBaoCaoDichVuTrongGoiKhamDoan(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncDichVuNgoai(queryInfo, true);
            var data = gridData.Data.Select(p => (BaoCaoKhamDoanHopDongDichVuNgoaiVo)p).ToList();            
            var bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoHoatDongDichVuNgoai(queryInfo, data);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDichVuTrongGoiKhamDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion


        [HttpPost("GetLoaiNhanVienHoacHopDong")]
        public async Task<ActionResult> GetLoaiNhanVienHoacHopDong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _baoCaoKhamDoanHopDongService.GetLoaiNhanVienHoacHopDong(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("InDanhSachNhanVien")]
        public ActionResult InDanhSachNhanVien(InDanhSachNhanVien inDanhSachNhanVien)//InDanhSachNhanVien
        {
            var result = _baoCaoKhamDoanHopDongService.InDanhSachNhanVien(inDanhSachNhanVien);
            return Ok(result);
        }


        #region NGƯỜI BỆNH KHÁM DỊCH VỤ THEO PHÒNG KHÁM ĐOÀN

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoHoatDongKhamDoan)]
        public ActionResult<GridDataSource> GetDataForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            var gridData = _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(queryInfo);
            return Ok(gridData.Result.Data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.BaoCaoHoatDongKhamDoan)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            var gridData = _baoCaoKhamDoanHopDongService.GetTotalPageForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpPost("GetCongTyBaoCaos")]
        public async Task<ActionResult> GetCongTy([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetCongTy(queryInfo);
            lookup.Add(new LookupItemTemplateVo { DisplayName = "Tất cả", KeyId = 0 });
            lookup.Reverse();
            return Ok(lookup);
        }

        [HttpPost("GetHopDongKhamSucKhoeBaoCaos")]
        public async Task<ActionResult> GetHopDongKhamSucKhoe([FromBody]DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lookup = await _baoCaoKhamDoanHopDongService.GetHopDongKhamSucKhoe(queryInfo);
            lookup.Add(new LookupItemHopDingKhamSucKhoeTemplateVo { DisplayName = "Tất cả", KeyId = 0 });
            lookup.Reverse();
            return Ok(lookup);
        }

        [HttpPost("ExportBaoCaoHoatDongKhamDoan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.BaoCaoHoatDongKhamDoan)]
        public async Task<ActionResult> ExportBaoCaoHoatDongKhamDoan(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetDataForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(queryInfo);
            var data = gridData.Data.Select(p => (NguoiBenhKhamDichVuTheoPhong)p).ToList();
            var bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoHoatDongKhamDoan(queryInfo, data);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongKhamDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}