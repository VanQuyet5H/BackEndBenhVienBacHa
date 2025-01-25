using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Core.Helpers;
using Camino.Services.ToaThuocMau;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using Camino.Api.Models.ToaThuocMau;
using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class ToaThuocMauController : CaminoBaseController
    {
        private readonly IToaThuocMauService _toaThuocMauService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public ToaThuocMauController(IToaThuocMauService toaThuocMauService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _toaThuocMauService = toaThuocMauService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        #region gridToaThuocMau
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _toaThuocMauService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _toaThuocMauService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region gridToaThuocMauChiTiet
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridToaThuocMauChiTietChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridToaThuocMauChiTietChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _toaThuocMauService.GetDataForGridToaThuocMauChiTietChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridToaThuocMauChiTietChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridToaThuocMauChiTietChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _toaThuocMauService.GetTotalPageForGridToaThuocMauChiTietChildAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion


        [HttpPost("BacSiKeToa")]
        public async Task<ActionResult<ICollection<NhanVienTemplateVos>>> BacSiKeToa([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _toaThuocMauService.GetBacSiKeToas(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("TrieuChung")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> TrieuChung([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _toaThuocMauService.GetTrieuChungs(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("DuocPhams")]
        public async Task<ActionResult<ICollection<DuocPhamTemplateGridVo>>> DuocPham(DropDownListRequestModel queryInfo)
        {
            var lookup = await _toaThuocMauService.GetDuocPhams(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("ChuanDoan")]
        public async Task<ActionResult<ICollection<ChuanDoanTemplateVo>>> ChuanDoan(DropDownListRequestModel queryInfo)
        {
            var lookup = await _toaThuocMauService.GetChuanDoans(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("ICD")]
        public async Task<ActionResult<ICollection<ICDTemplateVos>>> ICD(DropDownListRequestModel queryInfo)
        {
            var lookup = await _toaThuocMauService.GetICDs(queryInfo);
            return Ok(lookup);
        }

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult> Post([FromBody] ToaThuocMauViewModel viewModel)
        {
            foreach (var item in viewModel.ToaThuocMauChiTiets)
            {
                item.DungSang = item.DungSangDisplay.ToFloatFromFraction();
                item.DungTrua = item.DungTruaDisplay.ToFloatFromFraction();
                item.DungChieu = item.DungChieuDisplay.ToFloatFromFraction();
                item.DungToi = item.DungToiDisplay.ToFloatFromFraction();
            }
            var toaThuoc = viewModel.ToEntity<ToaThuocMau>();
            await _toaThuocMauService.AddAsync(toaThuoc);

            return CreatedAtAction(nameof(Get), new { id = toaThuoc.Id }, toaThuoc.ToModel<ToaThuocMauViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ToaThuocMau)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ToaThuocMauViewModel>> Get(long id)
        {
            var toaThuoc = await _toaThuocMauService.GetByIdAsync(id, s => s.Include(o => o.ToaThuocMauChiTiets).ThenInclude(ct => ct.DuocPham)
                                                                            .Include(o => o.BacSiKeToa.User)
                                                                            .Include(o => o.TrieuChung)
                                                                            .Include(o => o.ICD)
                                                                            .Include(o => o.ChuanDoan));
            if (toaThuoc == null)
            {
                return NotFound();
            }
            var result = toaThuoc.ToModel<ToaThuocMauViewModel>();
            foreach (var item in result.ToaThuocMauChiTiets)
            {
                //item.TenDuocPham = item.DuocPham?.MaHoatChat + " - " + item.DuocPham?.Ten;
                item.TenDuocPham = item.DuocPham?.Ten;
            }
            return Ok(result);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult> Put([FromBody] ToaThuocMauViewModel viewModel)
        {
            var toaThuoc = await _toaThuocMauService.GetByIdAsync(viewModel.Id, s => s.Include(o => o.ToaThuocMauChiTiets).ThenInclude(ct => ct.DuocPham)
                                                                            .Include(o => o.BacSiKeToa.User)
                                                                            .Include(o => o.TrieuChung)
                                                                            .Include(o => o.ICD)
                                                                            .Include(o => o.ChuanDoan));
            if (toaThuoc == null)
            {
                return NotFound();
            }
            foreach (var item in viewModel.ToaThuocMauChiTiets)
            {
                item.DungSang = item.DungSangDisplay.ToFloatFromFraction();
                item.DungTrua = item.DungTruaDisplay.ToFloatFromFraction();
                item.DungChieu = item.DungChieuDisplay.ToFloatFromFraction();
                item.DungToi = item.DungToiDisplay.ToFloatFromFraction();
            }
            viewModel.ToEntity(toaThuoc);
            await _toaThuocMauService.UpdateAsync(toaThuoc);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult> Delete(long id)
        {
            var toaThuoc = await _toaThuocMauService.GetByIdAsync(id, p => p.Include(x => x.ToaThuocMauChiTiets));
            if (toaThuoc == null)
            {
                return NotFound();
            }
            await _toaThuocMauService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion

        [HttpPost("KichHoatToaThuocMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ToaThuocMau)]
        public async Task<ActionResult> KichHoatToaThuocMau(long id)
        {
            var entity = await _toaThuocMauService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _toaThuocMauService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportToaThuocMau")]
        public async Task<ActionResult> ExportToaThuocMau(QueryInfo queryInfo)
        {
            var gridData = await _toaThuocMauService.GetDataForGridAsync(queryInfo, true);
            var toaThuocMauData = gridData.Data.Select(p => (ToaThuocMauGridVo)p).ToList();
            var excelData = toaThuocMauData.Map<List<ToaThuocMauExportExcel>>();

            foreach (var item in excelData)
            {
                var gridChildData = await _toaThuocMauService.GetDataForGridToaThuocMauChiTietChildAsync(queryInfo, item.Id, true);
                var childData = gridChildData.Data.Select(p => (ToaThuocMauChiTietGridVo)p).ToList();
                var childExcelData = childData.Map<List<ToaThuocMauExportExcelChild>>();
                item.ToaThuocMauExportExcelChild.AddRange(childExcelData);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.Ten), "Toa thuốc mẫu"));
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.TenICD), "Tên ICD"));
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.TenTrieuChung), "Triệu chứng"));
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.ChuanDoan), "Chẩn đoán"));
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.TenBacSiKeToa), "Bác sĩ kê toa"));
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.GhiChu), "Ghi chú"));
            lstValueObject.Add((nameof(ToaThuocMauExportExcel.ToaThuocMauExportExcelChild), ""));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Toa thuốc mẫu");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ToaThuocMau" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}