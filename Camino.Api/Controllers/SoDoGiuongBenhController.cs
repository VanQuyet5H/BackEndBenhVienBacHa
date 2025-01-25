using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GiuongBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.ExportImport;
using Camino.Services.GiuongBenhs;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Camino.Services.PhongBenhVien;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class SoDoGiuongBenhController : CaminoBaseController
    {
        private readonly IGiuongBenhService _giuongBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly IExcelService _excelService;

        public SoDoGiuongBenhController(IGiuongBenhService giuongBenhService, ILocalizationService localizationService
            , IKhoaPhongService khoaPhongService, IPhongBenhVienService phongBenhVienService, IExcelService excelService)
        {
            _giuongBenhService = giuongBenhService;
            _localizationService = localizationService;
            _khoaPhongService = khoaPhongService;
            _phongBenhVienService = phongBenhVienService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _giuongBenhService.GetDataForGridSoDoGiuongBenhKhoaAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _giuongBenhService.GetTotalPageForGridSoDoGiuongBenhKhoaAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridSoDoGiuongBenhPhongAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridSoDoGiuongBenhPhongAsync([FromBody]QueryInfo queryInfo, long khoaId)
        {
            var gridData = await _giuongBenhService.GetDataForGridSoDoGiuongBenhPhongAsync(queryInfo, khoaId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridSoDoGiuongBenhPhongAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridSoDoGiuongBenhPhongAsync([FromBody]QueryInfo queryInfo, long khoaId)
        {
            var gridData = await _giuongBenhService.GetTotalPageForGridSoDoGiuongBenhPhongAsync(queryInfo, khoaId);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridSoDoGiuongBenhKhoaPhongAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridSoDoGiuongBenhKhoaPhongAsync([FromBody]QueryInfo queryInfo, long khoaId, long phongId)
        {
            var gridData = await _giuongBenhService.GetDataForGridSoDoGiuongBenhKhoaPhongAsync(queryInfo, khoaId, phongId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridSoDoGiuongBenhKhoaPhongAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridSoDoGiuongBenhKhoaPhongAsync([FromBody]QueryInfo queryInfo, long khoaId, long phongId)
        {
            var gridData = await _giuongBenhService.GetTotalPageForGridSoDoGiuongBenhKhoaPhongAsync(queryInfo, khoaId, phongId);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListPhongForPopup")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhongForPopup([FromBody]DropDownListRequestModel model, long? khoaId)
        {

            var lstPhongBenhVienWithKhoaPhong = await _phongBenhVienService.GetListPhongBenhVienByKhoaPhongId(khoaId ?? 0, model);
            var lookup = lstPhongBenhVienWithKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return Ok(lookup);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTenKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTenKhoa(long khoaId)
        {
            var result = await _giuongBenhService.GetTenKhoa(khoaId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetMaTenPhong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetMaTenPhong(long phongId)
        {
            var result = await _giuongBenhService.GetMaTenPhong(phongId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachGiuongKhoaPopup")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachGiuongKhoaPopup([FromBody]ResultSoDoPopup model)
        {
            if (model != null && (model.PhongId == null || model.PhongId == 0))
            {
                var resultDontHavePhong = new ResultSoDoPopup();
                return Ok(resultDontHavePhong);
            }
            var result = await _giuongBenhService.getLstPhongForKhoaPopup(model);
            return Ok(result);
        }

        [HttpPost("GetListPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhong([FromBody]DropDownListRequestModel model, long? khoaId)
        {

            var lstPhongBenhVienWithKhoaPhong = await _phongBenhVienService.GetListPhongBenhVienByKhoaPhongId(khoaId ?? 0, model);
            var lookup = lstPhongBenhVienWithKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return Ok(lookup);
        }

        [HttpPost("ExportSoDoGiuongBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.TinhTrangGiuongBenh)]
        public async Task<ActionResult> ExportSoDoGiuongBenh(QueryInfo queryInfo)
        {
            var gridData = await _giuongBenhService.GetDataForGridSoDoGiuongBenhKhoaAsync(queryInfo, true);
            var soDoGiuongBenhData = gridData.Data.Select(p => (SoDoGiuongBenhKhoaGridVo)p).ToList();
            var excelData = soDoGiuongBenhData.Map<List<SoDoGiuongBenhKhoaExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(SoDoGiuongBenhKhoaExportExcel.Ten), "Khoa"));
            lstValueObject.Add((nameof(SoDoGiuongBenhKhoaExportExcel.GiuongTrong), "Giường trống"));
            lstValueObject.Add((nameof(SoDoGiuongBenhKhoaExportExcel.GiuongCoBenhNhan), "Giường có người bệnh"));
            lstValueObject.Add((nameof(SoDoGiuongBenhKhoaExportExcel.TongGiuongBenhCuaKhoa), "Tổng giường bệnh của khoa"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Sơ đồ giường bệnh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=SoDoGiuongBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}