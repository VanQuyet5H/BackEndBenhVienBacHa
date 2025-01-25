using AutoMapper;
using Camino.Api.Auth;
using Camino.Api.Models.HoanTra;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.NhapKhoVatTus;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public class HoanTraController : CaminoBaseController
    {
        private readonly INhapKhoDuocPhamService _nhapKhoDuocPhamService;
        private readonly INhapKhoVatTuService _nhapKhoVatTuService;
        private readonly IUserService _userService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;
        private IMapper _mapper;
        public HoanTraController(IExcelService excelService, INhapKhoDuocPhamService nhapKhoDuocPhamService, INhapKhoVatTuService nhapKhoVatTuService, IUserService userService, ILocalizationService localizationService, IMapper mapper)
        {
            _excelService = excelService;
            _nhapKhoDuocPhamService = nhapKhoDuocPhamService;
            _nhapKhoVatTuService = nhapKhoVatTuService;
            _userService = userService;
            _localizationService = localizationService;
            _mapper = mapper;
        }

        #region Thông tin chung get thông tin hoàn trả dược phẩm

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuyetHoanTraDuocPham/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<ThongTinDuyetKhoDuocPham>> GetThongTinDuyetHoanTraDuocPham(long id)
        {
            var thongTinDuyetHoanTraDuocPham = await _nhapKhoDuocPhamService.GetThongTinDuyetHoanTraDuocPham(id);
            return Ok(thongTinDuyetHoanTraDuocPham);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult> TuChoiDuyetHoanTraDuocPham(TuChoiDuyetHoanTraDuocPhamViewModel tuChoiDuyetHoanTraDuocPhamViewModel)
        {
            await _nhapKhoDuocPhamService.TuChoiDuyetHoanTraDuocPham(tuChoiDuyetHoanTraDuocPhamViewModel.Id, tuChoiDuyetHoanTraDuocPhamViewModel.LyDoHuy);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<ThongTinDuyetKhoDuocPham>> DuyetHoanTraDuocPham(DuyetHoanTraDuocPhamViewModel duyetHoanTraDuocPhamViewModel)
        {
            duyetHoanTraDuocPhamViewModel.TenNhanVienNhan = (await _userService.GetByIdAsync(duyetHoanTraDuocPhamViewModel.NhanVienNhanId)).HoTen;
            duyetHoanTraDuocPhamViewModel.TenNhanVienTra = (await _userService.GetByIdAsync(duyetHoanTraDuocPhamViewModel.NhanVienTraId)).HoTen;

            await _nhapKhoDuocPhamService.DuyetHoanTraDuocPham(duyetHoanTraDuocPhamViewModel.Id, duyetHoanTraDuocPhamViewModel.NhanVienTraId, duyetHoanTraDuocPhamViewModel.NhanVienNhanId, duyetHoanTraDuocPhamViewModel.TenNhanVienNhan, duyetHoanTraDuocPhamViewModel.TenNhanVienTra); ;

            //var thongTinDuyetHoanTraDuocPham = await _nhapKhoDuocPhamService.GetThongTinDuyetHoanTraDuocPham(duyetHoanTraDuocPhamViewModel.Id);
            //return Ok(thongTinDuyetHoanTraDuocPham);
            return NoContent();
        }

        #endregion
        #region Thông tin duyệt hoàn trả dược phẩm

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetHoanTraDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetHoanTraDuocPhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetHoanTraDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportDuyetHoanTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetYeuCauHoanTraDuocPham)]
        public async Task<ActionResult> ExportDuyetHoanTraDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoDuocPhamService.GetDanhSachDuyetHoanTraDuocPhamForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachDuyetHoanTraDuocPhamVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetHoanTraDuocPhamExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _nhapKhoDuocPhamService.GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachDuyetHoanTraDuocPhamChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetHoanTraDuocPhamExportExcelChild>>();
                item.DuyetHoanTraDuocPhamExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetHoanTraDuocPhamExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.TenNhanVienYeuCau), "Người yêu cầu"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.TenKhoCanHoanTra), "Hoàn Trả Từ Kho"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.TenKhoNhanHoanTra), "Hoàn Trả Về Kho"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.NgayYeuCauDisplay), "Ngày Yêu Cầu"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.TenNhanVienDuyet), "Người Duyệt"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetHoanTraDuocPhamExportExcel.DuyetHoanTraDuocPhamExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Hoàn Trả Dược Phẩm", 2, "Duyệt Hoàn Trả Dược Phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetHoanTraDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Thông tin chung get hoàn trả vật tư

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoanTraVatTu/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult<ThongTinHoanTraVatTu>> GetThongTinDuyetHoanTraVatTu(long id)
        {
            var thongTinDuyetKhoVatTu = await _nhapKhoVatTuService.GetThongTinHoanTraVatTu(id);
            return Ok(thongTinDuyetKhoVatTu);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult> TuChoiHoanTraVatTuNhapKho(TuChoiDuyetHoanTraVatTuViewModel tuChoiDuyetHoanTraVatTuViewModel)
        {
            await _nhapKhoVatTuService.TuChoiDuyetHoanTraVatTu(tuChoiDuyetHoanTraVatTuViewModel.Id, tuChoiDuyetHoanTraVatTuViewModel.LyDoHuy);
            return Ok();
            //var tuChoiDuyetKho = await _nhapKhoVatTuService.TuChoiDuyetHoanTraVatTu(thongTinLyDoHuyNhapKhoVatTu);
            //return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HoanTraVatTuKho")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult<ThongTinHoanTraVatTu>> HoanTraVatTuKho(DuyetHoanTraVatTuViewModel duyetHoanTraVatTuViewModel)
        {
            await _nhapKhoVatTuService.DuyetHoanTraNhapKho(duyetHoanTraVatTuViewModel.Id, duyetHoanTraVatTuViewModel.NguoiNhanId, duyetHoanTraVatTuViewModel.NguoiTraId);

            var thongTinDuyetKhoVatTu = await _nhapKhoVatTuService.GetThongTinHoanTraVatTu(duyetHoanTraVatTuViewModel.Id);
            return Ok(thongTinDuyetKhoVatTu);
        }

        #endregion
        #region Thông tin hoàn trả vật tư

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachHoanTraVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHoanTraVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachHoanTraVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHoanTraVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHoanTraVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetTotalDanhSachHoanTraVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachHoanTraVatTuChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachHoanTraVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachHoanTraVatTuChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachHoanTraVatTuChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachHoanTraVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetTotalDanhSachHoanTraVatTuChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportHoanTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetYeuCauHoanTraVatTu)]
        public async Task<ActionResult> ExportHoanTraVatTu(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoVatTuService.GetDanhSachHoanTraVatTuForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DanhSachHoanTraVatTuVo)p).ToList();
            var dataExcel = duocPhamData.Map<List<DuyetHoanTraVatTuExportExcel>>();

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id.ToString();
                var gridChildData = await _nhapKhoVatTuService.GetDanhSachHoanTraVatTuChiTietForGridAsync(queryInfo, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachHoanTraVatTuChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetHoanTraVatTuExportExcelChild>>();
                item.DuyetHoanTraVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetHoanTraVatTuExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(DuyetHoanTraVatTuExportExcel.TenNhanVienYeuCau), "Người yêu cầu"),
                (nameof(DuyetHoanTraVatTuExportExcel.TenKhoCanHoanTra), "Hoàn Trả Từ Kho"),
                (nameof(DuyetHoanTraVatTuExportExcel.TenKhoNhanHoanTra), "Hoàn Trả Về Kho"),
                (nameof(DuyetHoanTraVatTuExportExcel.NgayYeuCauDisplay), "Ngày Yêu Cầu"),
                (nameof(DuyetHoanTraVatTuExportExcel.TinhTrangDuyet), "Tình Trạng"),
                (nameof(DuyetHoanTraVatTuExportExcel.TenNhanVienDuyet), "Người Duyệt"),
                (nameof(DuyetHoanTraVatTuExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetHoanTraVatTuExportExcel.DuyetHoanTraVatTuExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Hoàn Trả Vật Tư", 2, "Duyệt Hoàn Trả Vật Tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetHoanTraVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion


        #region Phiếu In Dược Phẩm Vật Tư

        [HttpGet("GetHtmlPhieuInHoanTraDuocPham")]
        public ActionResult GetHtmlPhieuInHoanTraDuocPham(long yeuCauHoanTraDuocPhamId, string hostingName)
        {
            var htmlPhieuInHoanTra = _nhapKhoDuocPhamService.GetHtmlPhieuInHoanTraDuocPham(yeuCauHoanTraDuocPhamId, hostingName);
            return Ok(htmlPhieuInHoanTra);
        }

        [HttpGet("GetHtmlPhieuInHoanTraVatTu")]
        public ActionResult GetHtmlPhieuInHoanTraVatTu(long yeuCauHoanTraVatTuId, string hostingName)
        {
            var htmlPhieuInHoanTra = _nhapKhoVatTuService.GetHtmlPhieuInHoanTraVatTu(yeuCauHoanTraVatTuId, hostingName);
            return Ok(htmlPhieuInHoanTra);
        }

        [HttpPost("InPhieuHoanTraDuocPhamVatTu")]
        public string InPhieuHoanTraDuocPhamVatTu(PhieuHoanTraDuocPhamVatTu phieuHoanTraDuocPhamVatTu)
        {
            var result = _nhapKhoDuocPhamService.InPhieuHoanTraDuocPhamVatTuUpdate(phieuHoanTraDuocPhamVatTu);
            return result;
        }
        #endregion


    }
}