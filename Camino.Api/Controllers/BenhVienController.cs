using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject;
using Camino.Services.BenhVien;
using Camino.Api.Extensions;
using System.Collections.Generic;
using Camino.Services.BenhVien.LoaiBenhVien;
using Camino.Services.BenhVien.CapQuanLyBenhVien;
using Camino.Services.DonViHanhChinh;
using Camino.Api.Models.BenhVien;
using Camino.Core.Domain.Entities.BenhVien;
using Camino.Api.Models.General;
using System.Linq;
using Camino.Services.Localization;
using System;
using Camino.Api.Models.Error;
using System.Net;
using Camino.Core.Domain.ValueObject.BenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class BenhVienController : CaminoBaseController
    {
        private readonly IBenhVienService _benhvienService;
        private readonly ILoaiBenhVienService _loaiBenhVienService;
        private readonly ICapQuanLyBenhVienService _capQuanLyBenhVienService;
        private readonly IExcelService _excelService;
        private readonly IDonViHanhChinhService _donViHanhChinhService;
        private readonly ILocalizationService _localizationService;

        public BenhVienController(IBenhVienService benhvienService,
                                  ILoaiBenhVienService loaiBenhVienService,
                                  ICapQuanLyBenhVienService capQuanLyBenhVienService,
                                  IDonViHanhChinhService donViHanhChinhService,
                                  IExcelService excelService,
                                  ILocalizationService localizationService)
        {
            _benhvienService = benhvienService;
            _loaiBenhVienService = loaiBenhVienService;
            _capQuanLyBenhVienService = capQuanLyBenhVienService;
            _excelService = excelService;
            _donViHanhChinhService = donViHanhChinhService;
            _localizationService = localizationService;
        }


        #region GetDataForGrid
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _benhvienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _benhvienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region GetListLookupItemVo
        [HttpPost("GetListLoaiBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLoaiBenhVien(DropDownListRequestModel model)
        {
            var lookup = await _loaiBenhVienService.GetListLoaiBenhVien(model);
            return Ok(lookup);
        }

        [HttpPost("GetListCapQuanLyBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListCapQuanLyBenhVien(DropDownListRequestModel model)
        {
            var lookup = await _capQuanLyBenhVienService.GetListCapQuanLyBenhVien(model);
            return Ok(lookup);
        }

        [HttpPost("GetListDonViHanhChinh")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListDonViHanhChinh([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _donViHanhChinhService.GetListDonViHanhChinh(model);
            return Ok(lookup);
        }

        [HttpPost("GetHangBenhVien")]
        public ActionResult<ICollection<LookupItemVo>> GetHangBenhVien(DropDownListRequestModel model)
        {
            var lookup = _benhvienService.GetHangBenhVienDescription(model);
            return Ok(lookup);
        }

        [HttpPost("GetTuyenChuyenMonKyThuat")]
        public ActionResult<ICollection<LookupItemVo>> GetTuyenChuyenMonKyThuat(DropDownListRequestModel model)
        {
            var lookup = _benhvienService.GetTuyenChuyenMonKyThuatDescription(model);
            return Ok(lookup);
        }
        #endregion

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<BienVienViewModel>> Get(long id)
        {
            var result = await _benhvienService.GetBenhVienById(id);
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<BienVienViewModel>();
            return Ok(resultData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult<BienVienViewModel>> Post([FromBody] BienVienViewModel viewModel)
        {
            if (!await _benhvienService.CheckLoaiBenhVienAsync(viewModel.LoaiBenhVienId ?? 0))
                throw new ApiException(_localizationService.GetResource("BenhVien.LoaiBenhVien.NotExists"), (int)HttpStatusCode.BadRequest);
            var entity = viewModel.ToEntity<BenhVien>();
            await _benhvienService.AddAsync(entity);
            return NoContent();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult> Put([FromBody] BienVienViewModel viewModel)
        {
            if (!await _benhvienService.CheckLoaiBenhVienAsync(viewModel.LoaiBenhVienId ?? 0))
                throw new ApiException(_localizationService.GetResource("BenhVien.LoaiBenhVien.NotExists"), (int)HttpStatusCode.BadRequest);
            var entity = await _benhvienService.GetByIdAsync(viewModel.Id);
            if (entity == null)
                return NotFound();

            viewModel.ToEntity(entity);
            await _benhvienService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _benhvienService.GetByIdAsync(id);
            if (chucvu == null)
                return NotFound();

            await _benhvienService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entitys = await _benhvienService.GetByIdsAsync(model.Ids);
            if (entitys == null)
            {
                return NotFound();
            }

            var benhViens = entitys.ToList();
            if (benhViens.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }
            await _benhvienService.DeleteAsync(benhViens);
            return NoContent();
        }

        [HttpPost("KichHoatTrangThai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult> KichHoatTrangThai(long id)
        {
            var entity = await _benhvienService.GetByIdAsync(id);
            entity.HieuLuc = entity.HieuLuc == null ? true : !entity.HieuLuc;
            await _benhvienService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucBenhVien)]
        public async Task<ActionResult> ExportBenhVien(QueryInfo queryInfo)
        {
            var gridData = await _benhvienService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (BenhVienGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<BenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(BenhVienExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(BenhVienExportExcel.TenVietTat), "Tên Viết Tắt"));
            lstValueObject.Add((nameof(BenhVienExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(BenhVienExportExcel.TenDonViHanhChinh), "Tên Đơn Vị Hành Chính"));
            lstValueObject.Add((nameof(BenhVienExportExcel.TenLoaiBenhVien), "Tên Loại Bệnh Viện"));
            lstValueObject.Add((nameof(BenhVienExportExcel.HangBenhVienDisplay), "Hạng Bệnh Viện"));
            lstValueObject.Add((nameof(BenhVienExportExcel.TuyenChuyenMonKyThuatDisplay), "Tuyến Chuyên Môn Kỹ Thuật"));
            lstValueObject.Add((nameof(BenhVienExportExcel.SoDienThoaiLanhDao), "Số Điện Thoại Lãnh Đạo"));
            lstValueObject.Add((nameof(BenhVienExportExcel.HieuLucDisplay), "Hiệu Lực"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Bệnh Viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}