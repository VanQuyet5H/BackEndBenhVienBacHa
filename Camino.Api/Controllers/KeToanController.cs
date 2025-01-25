using AutoMapper;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.NhapKhoVatTus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.Error;

namespace Camino.Api.Controllers
{
    public class KeToanController : CaminoBaseController
    {
        private readonly INhapKhoDuocPhamService _nhapKhoDuocPhamService;
        private readonly INhapKhoVatTuService _nhapKhoVatTuService;
        private readonly IExcelService _excelService;
        private IMapper _mapper;
        public KeToanController(IExcelService excelService, INhapKhoDuocPhamService nhapKhoDuocPhamService, INhapKhoVatTuService nhapKhoVatTuService, IMapper mapper)
        {
            _excelService = excelService;
            _nhapKhoDuocPhamService = nhapKhoDuocPhamService;
            _nhapKhoVatTuService = nhapKhoVatTuService;
            _mapper = mapper;
        }

        #region Thông tin chung get thông tin dược phẩm

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuyetKhoDuocPham/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<ThongTinDuyetKhoDuocPham>> GetThongTinDuyetKhoDuocPham(long id)
        {
            var thongTinDuyetKhoDuocPham = await _nhapKhoDuocPhamService.GetThongTinDuyetKhoDuocPham(id);
            return Ok(thongTinDuyetKhoDuocPham);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetDuocPhamNhapKho")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> TuChoiDuyetDuocPhamNhapKho(ThongTinLyDoHuyNhapKhoDuocPham thongTinLyDoHuyNhapKhoDuocPham)
        {
            var tuChoiDuyetKho = await _nhapKhoDuocPhamService.TuChoiDuyetDuocPhamNhapKho(thongTinLyDoHuyNhapKhoDuocPham);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetDuocPhamNhapKho/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> DuyetDuocPhamNhapKho(long id)
        {
            var tinhTrangDuyet = await _nhapKhoDuocPhamService.DuyetDuocPhamNhapKho(id);
            return Ok(tinhTrangDuyet);
        }

        #endregion
        #region Thông tin duyệt kho dược phẩm

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetKhoDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetKhoDuocPhamForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetKhoDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoDuocPhamForGridAsync([FromBody]QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportDuyetDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> ExportHopDongThauDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachDuyetKhoDuocPhamVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetDuocPhamExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachDuyetKhoDuocPhamChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetDuocPhamExportExcelChild>>();
                item.DuyetDuocPhamExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetDuocPhamExportExcel.NgayNhapDisplay), "Ngày Nhập"),
                (nameof(DuyetDuocPhamExportExcel.TenKho), "Kho Nhập"),
                (nameof(DuyetDuocPhamExportExcel.SoChungTu), "Số Hóa Đơn"),
                (nameof(DuyetDuocPhamExportExcel.NgayHoaDonDisplay), "Ngày Hóa Đơn"),
                (nameof(DuyetDuocPhamExportExcel.TenNguoiNhap), "Người Nhập"),
                (nameof(DuyetDuocPhamExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetDuocPhamExportExcel.NguoiDuyet), "Người Duyệt"),
                (nameof(DuyetDuocPhamExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetDuocPhamExportExcel.DuyetDuocPhamExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Nhập Kho Dược Phẩm", 2, "Duyệt Nhập Kho Dược Phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetNhapKhoDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Thông tin chung get thông tin vật tư

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuyetKhoVatTu/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<ThongTinDuyetKhoDuocPham>> GetThongTinDuyetKhoVatTu(long id)
        {
            var thongTinDuyetKhoVatTu = await _nhapKhoVatTuService.GetThongTinDuyetKhoVatTu(id);
            return Ok(thongTinDuyetKhoVatTu);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetVatTuNhapKho")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> TuChoiDuyetVatTuNhapKho(ThongTinLyDoHuyNhapKhoVatTu thongTinLyDoHuyNhapKhoVatTu)
        {
            var tuChoiDuyetKho = await _nhapKhoVatTuService.TuChoiDuyetVatTuNhapKho(thongTinLyDoHuyNhapKhoVatTu);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetDuocVatTuKho/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> DuyetDuocVatTuKho(long id)
        {
            var tinhTrangDuyet = await _nhapKhoVatTuService.DuyetVatTuNhapKho(id);
            return Ok(tinhTrangDuyet);
        }

        #endregion
        #region Thông tin duyệt kho vật tư

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetKhoVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetKhoVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetKhoVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetTotalDanhSachDuyetKhoVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetKhoVatTuChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetKhoVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportDuyetVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> ExportDuyetVatTu(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachDuyetKhoVatTuVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetVatTuExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachDuyetKhoVatTuChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetVatTuExportExcelChild>>();
                item.DuyetVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetVatTuExportExcel.NgayNhapDisplay), "Ngày Nhập"),
                (nameof(DuyetVatTuExportExcel.TenKho), "Kho Nhập"),
                (nameof(DuyetVatTuExportExcel.SoChungTu), "Số Hóa Đơn"),
                (nameof(DuyetVatTuExportExcel.NgayHoaDonDisplay), "Ngày Hóa Đơn"),
                (nameof(DuyetVatTuExportExcel.TenNguoiNhap), "Người Nhập"),
                (nameof(DuyetVatTuExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetVatTuExportExcel.NguoiDuyet), "Người Duyệt"),
                (nameof(DuyetVatTuExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetVatTuExportExcel.DuyetVatTuExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Nhập Kho Vật Tư", 2, "Duyệt Nhập Kho Vật Tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetNhapKhoVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Thông tin chung get thông tin hoàn trả dược phẩm

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuyetHoanTraDuocPham/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<ThongTinDuyetKhoDuocPham>> GetThongTinDuyetHoanTraDuocPham(long id)
        {
            var thongTinDuyetKhoDuocPham = await _nhapKhoDuocPhamService.GetThongTinDuyetKhoDuocPham(id);
            return Ok(thongTinDuyetKhoDuocPham);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> TuChoiDuyetHoanTraDuocPham(ThongTinLyDoHuyNhapKhoDuocPham thongTinLyDoHuyNhapKhoDuocPham)
        {
            var tuChoiDuyetKho = await _nhapKhoDuocPhamService.TuChoiDuyetDuocPhamNhapKho(thongTinLyDoHuyNhapKhoDuocPham);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetHoanTraDuocPham/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> DuyetHoanTraDuocPham(long id)
        {
            var tinhTrangDuyet = await _nhapKhoDuocPhamService.DuyetDuocPhamNhapKho(id);
            return Ok(tinhTrangDuyet);
        }

        #endregion
        #region Thông tin duyệt hoàn trả dược phẩm

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetHoanTraDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetHoanTraDuocPhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportDuyetHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> ExportDuyetHoanTraDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachDuyetKhoDuocPhamVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetDuocPhamExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _nhapKhoDuocPhamService.GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachDuyetKhoDuocPhamChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetDuocPhamExportExcelChild>>();
                item.DuyetDuocPhamExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetDuocPhamExportExcel.SoChungTu), "Số Chứng Từ"),
                (nameof(DuyetDuocPhamExportExcel.TenNguoiNhap), "Người Nhập"),
                (nameof(DuyetDuocPhamExportExcel.NgayNhap), "Ngày Nhập"),
                (nameof(DuyetDuocPhamExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetDuocPhamExportExcel.NguoiDuyet), "Người Duyệt"),
                (nameof(DuyetDuocPhamExportExcel.NgayDuyet), "Ngày Duyệt"),
                (nameof(DuyetDuocPhamExportExcel.DuyetDuocPhamExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Nhập Kho Dược Phẩm", 2, "Duyệt Nhập Kho Dược Phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetNhapKhoDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Thông tin chung get hoàn trả vật tư

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoanTraVatTu/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<ThongTinDuyetKhoDuocPham>> GetThongTinDuyetHoanTraVatTu(long id)
        {
            var thongTinDuyetKhoVatTu = await _nhapKhoVatTuService.GetThongTinDuyetKhoVatTu(id);
            return Ok(thongTinDuyetKhoVatTu);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> TuChoiHoanTraVatTuNhapKho(ThongTinLyDoHuyNhapKhoVatTu thongTinLyDoHuyNhapKhoVatTu)
        {
            var tuChoiDuyetKho = await _nhapKhoVatTuService.TuChoiDuyetVatTuNhapKho(thongTinLyDoHuyNhapKhoVatTu);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HoanTraVatTuKho/{id}/{nguoiNhanId}/")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> HoanTraVatTuKho(long id)
        {
            var tinhTrangDuyet = await _nhapKhoVatTuService.DuyetVatTuNhapKho(id);
            return Ok(tinhTrangDuyet);
        }

        #region Thông tin hoàn trả vật tư

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachHoanTraVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHoanTraVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHoanTraVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHoanTraVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetTotalDanhSachDuyetKhoVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHoanTraVatTuChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHoanTraVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHoanTraVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> ExportHoanTraVatTu(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachDuyetKhoVatTuVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetVatTuExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _nhapKhoVatTuService.GetDanhSachDuyetKhoVatTuChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachDuyetKhoVatTuChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetVatTuExportExcelChild>>();
                item.DuyetVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetVatTuExportExcel.TenNguoiNhap), "Người Nhập"),
                (nameof(DuyetVatTuExportExcel.NgayNhapDisplay), "Ngày Nhập"),
                (nameof(DuyetVatTuExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetVatTuExportExcel.NguoiDuyet), "Người Duyệt"),
                (nameof(DuyetVatTuExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetVatTuExportExcel.DuyetVatTuExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Nhập Kho Vật Tư", 2, "Duyệt Nhập Kho Vật Tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetNhapKhoVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #endregion


        [HttpPost("HuyDuyetNhapKhoDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoDuocPham)]
        public async Task<ActionResult> HuyDuyetNhapKhoDuocPham(long yeuCauNhapKhoDuocPhamId)
        {
            var error = await _nhapKhoDuocPhamService.HuyDuyetNhapKhoDuocPham(yeuCauNhapKhoDuocPhamId);
            if(!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return Ok(error);
        }

        [HttpPost("HuyDuyetNhapKhoVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetNhapKhoVatTu)]
        public async Task<ActionResult> HuyDuyetNhapKhoVatTu(long yeuCauNhapKhoVatTuId)
        {
            var error = await _nhapKhoDuocPhamService.HuyDuyetNhapKhoVatTu(yeuCauNhapKhoVatTuId);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return Ok(error);
        }
    }
}