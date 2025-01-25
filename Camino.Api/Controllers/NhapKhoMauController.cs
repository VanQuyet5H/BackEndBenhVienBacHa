using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.NhapKhoMau;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhapKhoMaus;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhapKhoMau;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    public partial class NhapKhoMauController : CaminoBaseController
    {
        private INhapKhoMauService _nhapKhoMauService;
        private readonly IExcelService _excelService;
        private IUserAgentHelper _userAgentHelper;
        private ILocalizationService _localizationService;
        public NhapKhoMauController(INhapKhoMauService nhapKhoMauService,
            IExcelService excelService,
            IUserAgentHelper userAgentHelper,
            ILocalizationService localizationService)
        {
            _nhapKhoMauService = nhapKhoMauService;
            _excelService = excelService;
            _userAgentHelper = userAgentHelper;
            _localizationService = localizationService;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhapKhoMauAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetDataForGridNhapKhoMauAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhapKhoMauAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetTotalPageForGridNhapKhoMauAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhapKhoMauChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhapKhoMauChiTietAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetDataForGridNhapKhoMauChiTietAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhapKhoMauChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhapKhoMauChiTietAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoMauService.GetTotalPageForGridNhapKhoMauChiTietAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Export excel
        [HttpPost("ExportDanhSachNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.NhapKhoMau, Enums.DocumentType.DuyetNhapKhoMau)]
        public async Task<ActionResult> ExportDanhSachNhapKhoMauAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _nhapKhoMauService.GetDataForGridNhapKhoMauAsync(queryInfo);
            var phieuNhapKhoMauData = gridData.Data.Select(p => (PhieuNhapKhoMauGridVo)p).ToList();
            var dataExcel = phieuNhapKhoMauData.Map<List<NhapKhoMauExportExcel>>();

            foreach (var item in dataExcel)
            {
                var queryInfoChild = new QueryInfo()
                {
                    AdditionalSearchString = item.Id + ""
                };
                var gridChildData = await _nhapKhoMauService.GetDataForGridNhapKhoMauChiTietAsync(queryInfoChild);
                var dataChild = gridChildData.Data.Select(p => (PhieuNhapKhoMauGridChiTietVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<NhapKhoMauExportExcelChild>>();
                item.NhapKhoMauExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.SoPhieu), "Số Phiếu"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.SoHoaDon), "Số Hóa Đơn"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.NhaCungCap), "Nhà Cung Cấp"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.GhiChu), "Ghi Chú"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.TenTinhTrang), "Tình Trạng"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.NguoiNhap), "Người Nhập"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.NgayNhapDisplay), "Ngày Nhập"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.NguoiDuyet), "Người Duyệt"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.NgayDuyetDisplay), "Ngày Duyệt"));
            lstValueObject.Add((nameof(NhapKhoMauExportExcel.NhapKhoMauExportExcelChild), ""));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Nhập Kho Máu");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhapKhoMau" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region lookup
        [HttpPost("GetListYeuCauTruyenMau")]
        public async Task<ActionResult<ICollection<LookupItemYeuCauTruyenMauVo>>> GetListYeuCauTruyenMauAsync(DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoMauService.GetListYeuCauTruyenMauAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetListNhomMau")]
        public async Task<ActionResult<List<LookupItemVo>>> GetListNhomMauAsync(DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoMauService.GetListNhomMauAsync(model);
            return lookup;
        }

        [HttpPost("GetListYeuToRh")]
        public async Task<ActionResult<List<LookupItemVo>>> GetListYeuToRhAsync(DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoMauService.GetListYeuToRhAsync(model);
            return lookup;
        }

        [HttpPost("GetListLoaiXetNghiemMauNhapThem")]
        public async Task<ActionResult<List<LookupItemVo>>> GetListLoaiXetNghiemMauNhapThemAsync(DropDownListRequestModel model)
        {
            var lookup = await _nhapKhoMauService.GetListLoaiXetNghiemMauNhapThemAsync(model);
            return lookup;
        }

        #endregion

        #region thêm/xóa/sửa
        [HttpGet("GetPhieuNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMau)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhieuNhapKhoMauViewModel>> GetPhieuNhapKhoMau(long id)
        {
            var phieuNhapKho = new PhieuNhapKhoMauViewModel();
            if (id != 0)
            {
                //var result = await _nhapKhoMauService
                //    .GetByIdAsync(id, s => s.Include(a => a.NhapKhoMauChiTiets).ThenInclude(b => b.YeuCauTruyenMau).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(d => d.BenhNhan)
                //                                    .Include(a => a.NhapKhoMauChiTiets).ThenInclude(b => b.YeuCauTruyenMau).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(d => d.NoiTruBenhAn));
                var result = await _nhapKhoMauService.GetPhieuNhapKhoMauAsync(id);
                phieuNhapKho = result.ToModel<PhieuNhapKhoMauViewModel>();
                //update BVHD-3320: bỏ kế toán nhập giá máu
                phieuNhapKho.DuocKeToanDuyet = null;
            }
            else
            {
                phieuNhapKho.NguoiNhapId = _userAgentHelper.GetCurrentUserId();
                phieuNhapKho.NgayNhap = DateTime.Now;
                phieuNhapKho.TrangThai = Enums.TrangThaiNhapKhoMau.ChoNhapGia;
                phieuNhapKho.LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong;
            }

            return Ok(phieuNhapKho);
        }

        [HttpPost("KiemTraValidationThemMauVaChePham")]
        public async Task<ActionResult<PhieuNhapKhoMauChiTietViewModel>> KiemTraValidationThemMauVaChePhamAsync(PhieuNhapKhoMauChiTietViewModel chiTietNhapMau)
        {
            if (chiTietNhapMau.KetQuaXetNghiemKhacs.Any())
            {
                var ketQuaXetNghiemKhacsDistinctCount =
                    chiTietNhapMau.KetQuaXetNghiemKhacs.GroupBy(x => x.LoaiXetNghiem).Select(x => x.Key).Count();
                if (ketQuaXetNghiemKhacsDistinctCount < chiTietNhapMau.KetQuaXetNghiemKhacs.Count)
                {
                    throw new ApiException(_localizationService.GetResource("NhapKhoMau.KetQuaXetNghiemKhacs.IsExists"));
                }
            }
            return Ok(chiTietNhapMau);
        }

        [HttpPost("XuLyTaoPhieuNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<string>> XuLyTaoPhieuNhapKhoMauAsync(PhieuNhapKhoMauViewModel phieuNhapMau)
        {
            if (!phieuNhapMau.NhapKhoMauChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("NhapKhoMau.NhapKhoMauChiTiets.Required"));
            }

            var newPhieuNhapMau = phieuNhapMau.ToEntity<NhapKhoMau>();
            await _nhapKhoMauService.XuLyTaoPhieuNhapKhoMauAsync(newPhieuNhapMau);

            var phieuIn = string.Empty;
            if (phieuNhapMau.InPhieu)
            {
                phieuIn = await _nhapKhoMauService.XuLyInPhieuTruyenMauAsync(newPhieuNhapMau.Id);
            }
            return Ok(phieuIn);
        }

        [HttpPut("XuLyCapNhatPhieuNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<string>> XuLyCapNhatPhieuNhapKhoMauAsync(PhieuNhapKhoMauViewModel phieuNhapMau)
        {
            var phieuNhapKhoMau =
                await _nhapKhoMauService.GetByIdAsync(phieuNhapMau.Id, x => x.Include(a => a.NhapKhoMauChiTiets));

            //if (phieuNhapKhoMau.DuocKeToanDuyet != null)
            //{
            //    throw new ApiException(_localizationService.GetResource("NhapKhoMau.DaDuyet"));
            //}

            if (!phieuNhapMau.NhapKhoMauChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("NhapKhoMau.NhapKhoMauChiTiets.Required"));
            }

            phieuNhapMau.ToEntity(phieuNhapKhoMau);
            await _nhapKhoMauService.XuLyCapNhatPhieuNhapKhoMauAsync(phieuNhapKhoMau);

            var phieuIn = string.Empty;
            if (phieuNhapMau.InPhieu)
            {
                phieuIn = await _nhapKhoMauService.XuLyInPhieuTruyenMauAsync(phieuNhapKhoMau.Id);
            }
            return Ok(phieuIn);
        }

        [HttpDelete("XuLyXoaPhieuNhapKhoMau")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult> XuLyXoaPhieuNhapKhoMauAsync(long id)
        {
            //var phieuNhapKhoMau =
            //    await _nhapKhoMauService.GetByIdAsync(id, x => x.Include(a => a.NhapKhoMauChiTiets).ThenInclude(o=>o.XuatKhoMauChiTiets).ThenInclude(o=>o.YeuCauTruyenMaus));
            //if (phieuNhapKhoMau == null)
            //{
            //    return NotFound();
            //}

            //if (phieuNhapKhoMau.DuocKeToanDuyet == true)
            //{
            //    throw new ApiException(_localizationService.GetResource("NhapKhoMau.DaDuyet"));
            //}

            //await _nhapKhoMauService.DeleteByIdAsync(id);
            await _nhapKhoMauService.XuLyXoaPhieuNhapKhoMauAsync(id);
            return NoContent();
        }
        #endregion

        #region In phiếu

        [HttpGet("XuLyInPhieuTruyenMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMau)]
        public async Task<ActionResult<string>> XuLyInPhieuTruyenMauAsync(long phieuTruyenMauId)
        {
            var phieuIn = await _nhapKhoMauService.XuLyInPhieuTruyenMauAsync(phieuTruyenMauId);
            return phieuIn;
        }

        #endregion
    }
}
