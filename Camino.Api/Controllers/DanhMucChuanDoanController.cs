using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.Localization;
using Camino.Services.ICDs;
using Camino.Api.Models.ICDs;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using System;
using System.Linq;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.ICDs;
using System.Collections.Generic;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class DanhMucChuanDoanController : CaminoBaseController
    {
        private readonly IDanhMucChuanDoanService _danhMucChuanDoanService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DanhMucChuanDoanController(IDanhMucChuanDoanService danhMucChuanDoanService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _danhMucChuanDoanService = danhMucChuanDoanService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _danhMucChuanDoanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _danhMucChuanDoanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region Add/Get/Update/Delete
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult> Post([FromBody] DanhMucChuanDoanViewModel danhMucChuanDoanVM)
        {
            //danhMucChuanDoanVM.TenTiengViet = danhMucChuanDoanVM.TenTiengViet.RemoveCharacters();
            //danhMucChuanDoanVM.TenTiengAnh = danhMucChuanDoanVM.TenTiengAnh.RemoveDiacritics();
            var danhMucChuanDoan = danhMucChuanDoanVM.ToEntity<DanhMucChuanDoan>();
            await _danhMucChuanDoanService.AddAsync(danhMucChuanDoan);
            return CreatedAtAction(nameof(Get), new { id = danhMucChuanDoan.Id }, danhMucChuanDoan.ToModel<DanhMucChuanDoanViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomChanDoan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DanhMucChuanDoanViewModel>> Get(long id)
        {
            var danhMucChuanDoan = await _danhMucChuanDoanService.GetByIdAsync(id);
            if (danhMucChuanDoan == null)
            {
                return NotFound();
            }
            return Ok(danhMucChuanDoan.ToModel<DanhMucChuanDoanViewModel>());
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult> Put([FromBody] DanhMucChuanDoanViewModel danhMucChuanDoanVM)
        {
            var danhMucChuanDoan = await _danhMucChuanDoanService.GetByIdAsync(danhMucChuanDoanVM.Id);
            if (danhMucChuanDoan == null)
            {
                return NotFound();
            }
            //danhMucChuanDoanVM.TenTiengViet = danhMucChuanDoanVM.TenTiengViet.RemoveCharacters();
            //danhMucChuanDoanVM.TenTiengAnh = danhMucChuanDoanVM.TenTiengAnh.RemoveDiacritics();
            danhMucChuanDoanVM.ToEntity(danhMucChuanDoan);

            await _danhMucChuanDoanService.UpdateAsync(danhMucChuanDoan);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult> Delete(long id)
        {
            var danhMucChuanDoan = await _danhMucChuanDoanService.GetByIdAsync(id);
            if (danhMucChuanDoan == null)
            {
                return NotFound();
            }
            await _danhMucChuanDoanService.DeleteByIdAsync(id);
            return NoContent();
        }
        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var danhMucChuanDoans = await _danhMucChuanDoanService.GetByIdsAsync(model.Ids);
            if (danhMucChuanDoans == null)
            {
                return NotFound();
            }
            if (danhMucChuanDoans.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _danhMucChuanDoanService.DeleteAsync(danhMucChuanDoans);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportDanhMucChanDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNhomChanDoan)]
        public async Task<ActionResult> ExportDanhMucChanDoan(QueryInfo queryInfo)
        {
            var gridData = await _danhMucChuanDoanService.GetDataForGridAsync(queryInfo, true);
            var danhMucChanDoanData = gridData.Data.Select(p => (DanhMucChuanDoanGridVo)p).ToList();
            var excelData = danhMucChanDoanData.Map<List<DanhMucChanDoanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DanhMucChanDoanExportExcel.TenTiengViet), "Tên tiếng Việt"));
            lstValueObject.Add((nameof(DanhMucChanDoanExportExcel.TenTiengAnh), "Tên tiếng Anh"));
            lstValueObject.Add((nameof(DanhMucChanDoanExportExcel.GhiChu), "Ghi chú"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Danh mục chẩn đoán");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhMucChanDoan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}