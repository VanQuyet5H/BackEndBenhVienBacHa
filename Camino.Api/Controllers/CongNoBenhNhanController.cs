using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Auth;
using Camino.Api.Models.CongNoBenhNhan;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CongNoBenhNhans;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongNoBenhNhanController : CaminoBaseController
    {
        private readonly ICongNoBenhNhanService _congNoBenhNhanService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IExcelService _excelService;
        private IMapper _mapper;
        private readonly IPdfService _pdfService;

        public CongNoBenhNhanController(ICongNoBenhNhanService congNoBenhNhanService, ILocalizationService localizationService,
            IUserAgentHelper userAgentHelper, IExcelService excelService, IMapper mapper, IPdfService pdfService)
        {
            _congNoBenhNhanService = congNoBenhNhanService;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _excelService = excelService;
            _mapper = mapper;
            _pdfService = pdfService;
        }

        [HttpPost("GetCongNos")]
        public List<LookupItemVo> GetCongNos()
        {
            var congNoBenhNhans = EnumHelper.GetListEnum<CongNoBenhNhan>();
            var res = congNoBenhNhans.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return res;
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        [HttpPost("GetDanhSachBenhNhanConNo")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachBenhNhanConNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetDanhSachBenhNhanConNoAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPagesBenhNhanConNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesBenhNhanConNo([FromBody] QueryInfo queryInfo)
        {
            //đã set lazyLoadPage=false
            var gridData = await _congNoBenhNhanService.GetDanhSachBenhNhanConNoAsync(queryInfo);
            return Ok(gridData);
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        [HttpPost("GetDanhSachBenhNhanHetNo")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachBenhNhanHetNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetDanhSachBenhNhanHetNoAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPagesBenhNhanHetNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesBenhNhanHetNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetTotalPagesBenhNhanHetNoAsync(queryInfo);
            return Ok(gridData);
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        [HttpGet("GetCongNoBenhNhanTTHC")]
        public async Task<ActionResult<CongNoBenhNhanTTHCVo>> GetCongNoBenhNhanTTHC(long benhNhanId)
        {
            return await _congNoBenhNhanService.GetCongNoBenhNhanTTHCAsync(benhNhanId);
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        [HttpPost("GetDanhSachThongTinThuNo")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachThongTinThuNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetDanhSachThongTinThuNoAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPagesThongTinThuNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesThongTinThuNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetTotalPagesThongTinThuNoAsync(queryInfo);
            return Ok(gridData);
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        [HttpPost("GetDanhSachThongTinChuaThuNo")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachThongTinChuaThuNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetDanhSachThongTinChuaThuNoAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPagesThongTinChuaThuNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesThongTinChuaThuNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetTotalPagesThongTinChuaThuNoAsync(queryInfo);
            return Ok(gridData);
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        [HttpPost("GetDanhSachThongTinDaThuNo")]
        public async Task<ActionResult<GridDataSource>> GetDanhSachThongTinDaThuNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetDanhSachThongTinDaThuNoAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPagesThongTinDaThuNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesThongTinDaThuNo([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _congNoBenhNhanService.GetTotalPagesThongTinDaThuNoAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetThongTinTraNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.CongNoBenhNhan)]
        public ActionResult<ThongTinTraNoVo> GetThongTinTraNo(long taiKhoanBenhNhanThuId)
        {
            return _congNoBenhNhanService.GetThongTinTraNo(taiKhoanBenhNhanThuId);
        }

        [HttpPost("ThuTienTraNo")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.CongNoBenhNhan)]
        public ActionResult ThuTienTraNo([FromBody] CongNoBenhNhanViewModel congNoBenhNhanViewModel)
        {
            if ((congNoBenhNhanViewModel.TienMat == null || congNoBenhNhanViewModel.TienMat == 0) && (congNoBenhNhanViewModel.ChuyenKhoan == null || congNoBenhNhanViewModel.ChuyenKhoan == 0) && (congNoBenhNhanViewModel.POS == null || congNoBenhNhanViewModel.POS == 0))
            {
                throw new ApiException(_localizationService.GetResource("CongNoBenhNhan.TienTraNo.Required"));
            }

            var soTienChuaThu = _congNoBenhNhanService.GetSoTienChuaThu(congNoBenhNhanViewModel.Id);

            if (congNoBenhNhanViewModel.TienMat.GetValueOrDefault() + congNoBenhNhanViewModel.ChuyenKhoan.GetValueOrDefault() + congNoBenhNhanViewModel.POS.GetValueOrDefault() > soTienChuaThu)
            {
                throw new ApiException(_localizationService.GetResource("CongNoBenhNhan.TienTraNo.NotGreaterThan.TienChuaThu"));
            }

            var thuTienTraNoVo = _mapper.Map<ThuTienTraNoVo>(congNoBenhNhanViewModel);
            thuTienTraNoVo.SoTienChuaThu = soTienChuaThu;
            thuTienTraNoVo.SoTienThu = thuTienTraNoVo.TienMat.GetValueOrDefault() + thuTienTraNoVo.ChuyenKhoan.GetValueOrDefault() + thuTienTraNoVo.POS.GetValueOrDefault();

            var taiKhoanBenhNhanThuId = _congNoBenhNhanService.ThuTienTraNo(thuTienTraNoVo);
            return Ok(taiKhoanBenhNhanThuId);
        }

        [HttpPost("GetHTMLTatCaPhieuThuNoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.CongNoBenhNhan)]
        public ActionResult<List<HtmlToPdfVo>> GetHTMLTatCaPhieuThuNoBenhNhan(long taiKhoanBenhNhanThuChinhId, string hostingName)
        {
            var htmlPhieuThu = _congNoBenhNhanService.GetHTMLTatCaPhieuThuNoBenhNhan(taiKhoanBenhNhanThuChinhId, hostingName);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlPhieuThu);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("GetHTMLPhieuThuNoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.CongNoBenhNhan)]
        public ActionResult<List<HtmlToPdfVo>> GetHTMLPhieuThuNoBenhNhan(long taiKhoanBenhNhanThuId, string hostingName)
        {
            var htmlPhieuThu = _congNoBenhNhanService.GetHTMLPhieuThuNoBenhNhan(taiKhoanBenhNhanThuId, hostingName);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(htmlPhieuThu);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("ExportCongNoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.CongNoBenhNhan)]
        public async Task<ActionResult> ExportCongNoBenhNhan(QueryInfo queryInfo, bool isConNo = false)
        {
            var gridData = new GridDataSource();

            if (isConNo)
            {
                //BVHD-3956
                byte[] byteDSConNoBenhNhans = _congNoBenhNhanService.ExportDanhSachConNoBenhNhan(queryInfo);

                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CongNoBenhNhan-ConNo" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                return new FileContentResult(byteDSConNoBenhNhans, "application/vnd.ms-excel");
            }
            else
            {
                gridData = await _congNoBenhNhanService.GetDanhSachBenhNhanHetNoAsync(queryInfo, true);

                var congNoData = gridData.Data.Select(p => (CongNoBenhNhanGridVo)p).ToList();
                var excelData = congNoData.Map<List<CongNoBenhNhanExportExcel>>();

                var lstValueObject = new List<(string, string)>();
                lstValueObject.Add((nameof(CongNoBenhNhanExportExcel.MaBenhNhan), "Mã người bệnh"));
                lstValueObject.Add((nameof(CongNoBenhNhanExportExcel.HoTen), "Họ tên"));
                lstValueObject.Add((nameof(CongNoBenhNhanExportExcel.GioiTinh), "Giới tính"));
                lstValueObject.Add((nameof(CongNoBenhNhanExportExcel.NamSinh), "Năm sinh"));
                lstValueObject.Add((nameof(CongNoBenhNhanExportExcel.SoDienThoai), "Số điện thoại"));
                lstValueObject.Add((nameof(CongNoBenhNhanExportExcel.DiaChi), "Địa chỉ"));

                var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, isConNo ? "Công nợ người bệnh - Còn nợ" : "Công nợ người bệnh - Hết nợ");
                var fileName = isConNo ? "CongNoBenhNhan-ConNo" : "CongNoBenhNhan-HetNo";

                HttpContext.Response.Headers.Add("content-disposition", $"attachment; filename={fileName}" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
        }
    }
}
