using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Services.Thuocs;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Services.ExportImport;
using System.Collections.Generic;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class DuongDungController : CaminoBaseController
    {
        private readonly IDuongDungService _duongDungService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DuongDungController(IDuongDungService duongDungService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _duongDungService = duongDungService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duongDungService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duongDungService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult> Post([FromBody] DuongDungViewModel duongDungViewModel)
        {
            var duongDung = duongDungViewModel.ToEntity<DuongDung>();
            await _duongDungService.AddAsync(duongDung);
            return CreatedAtAction(nameof(Get), new { id = duongDung.Id }, duongDung.ToModel<DuongDungViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuongDung)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuongDungViewModel>> Get(long id)
        {
            var duongDung = await _duongDungService.GetByIdAsync(id);
            if (duongDung == null)
            {
                return NotFound();
            }
            return Ok(duongDung.ToModel<DuongDungViewModel>());
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult> Put([FromBody] DuongDungViewModel duongDungViewModel)
        {
            var duongDung = await _duongDungService.GetByIdAsync(duongDungViewModel.Id);
            if (duongDung == null)
            {
                return NotFound();
            }
            duongDungViewModel.ToEntity(duongDung);

            await _duongDungService.UpdateAsync(duongDung);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult> Delete(long id)
        {
            var duongDung = await _duongDungService.GetByIdAsync(id);
            if (duongDung == null)
            {
                return NotFound();
            }
            await _duongDungService.DeleteByIdAsync(id);
            return NoContent();
        }
        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var duongDung = await _duongDungService.GetByIdsAsync(model.Ids);
            if (duongDung == null)
            {
                return NotFound();
            }
            if (duongDung.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _duongDungService.DeleteAsync(duongDung);
            return NoContent();
        }
        #endregion
        [HttpPost("KichHoatDuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult> KichHoatDuongDung(long id)
        {
            var entity = await _duongDungService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _duongDungService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportDuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDuongDung)]
        public async Task<ActionResult> ExportDuongDung(QueryInfo queryInfo)
        {
            var gridData = await _duongDungService.GetDataForGridAsync(queryInfo, true);
            var duongDungData = gridData.Data.Select(p => (DuongDungGridVo)p).ToList();
            var excelData = duongDungData.Map<List<DuongDungExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DuongDungExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DuongDungExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(DuongDungExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(DuongDungExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Đường dùng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuongDung" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}