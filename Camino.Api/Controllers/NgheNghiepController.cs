using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.NgheNghiep;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NgheNghieps;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NgheNghiep;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NgheNghiep;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public class NgheNghiepController : CaminoBaseController
    {
        private readonly INgheNghiepService _ngheNghiepService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;


        public NgheNghiepController(INgheNghiepService ngheNghiepService, ILocalizationService localizationService, IExcelService excelService)
        {
            _ngheNghiepService = ngheNghiepService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ngheNghiepService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _ngheNghiepService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult<NgheNghiepViewModel>> Post
            ([FromBody]NgheNghiepViewModel ngheNghiepViewModel)
        {
            var ngheNghiep = ngheNghiepViewModel.ToEntity<NgheNghiep>();
            await _ngheNghiepService.AddAsync(ngheNghiep);
            var ngheNghiepId = await _ngheNghiepService.GetByIdAsync(ngheNghiep.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = ngheNghiep.Id },
                ngheNghiepId.ToModel<NgheNghiepViewModel>()
            );
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNgheNghiep)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NgheNghiepViewModel>> Get(long id)
        {
            var ngheNghiep = await _ngheNghiepService.GetByIdAsync(id);
            if (ngheNghiep == null)
            {
                return NotFound();
            }

            return Ok(ngheNghiep.ToModel<NgheNghiepViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNgheNghiep)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatNgheNghiep([FromBody]NgheNghiepViewModel ngheNghiepViewModel)
        {
            var ngheNghiep = await _ngheNghiepService.GetByIdAsync(ngheNghiepViewModel.Id);
            ngheNghiepViewModel.ToEntity(ngheNghiep);
            await _ngheNghiepService.UpdateAsync(ngheNghiep);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult> Delete(long id)
        {
            var ngheNghiep = await _ngheNghiepService.GetByIdAsync(id);
            if (ngheNghiep == null)
            {
                return NotFound();
            }

            await _ngheNghiepService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var ngheNghieps = await _ngheNghiepService.GetByIdsAsync(model.Ids);
            if (ngheNghieps == null)
            {
                return NotFound();
            }
            if (ngheNghieps.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _ngheNghiepService.DeleteAsync(ngheNghieps);
            return NoContent();
        }
        #endregion

        [HttpPost("KichHoatNgheNghiep")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult> KichHoatNgheNghiep(long id)
        {
            var entity = await _ngheNghiepService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _ngheNghiepService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportNgheNghiep")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult> ExportNgheNghiep(QueryInfo queryInfo)
        {
            var gridData = await _ngheNghiepService.GetDataForGridAsync(queryInfo, true);
            var ngheNghiepData = gridData.Data.Select(p => (NgheNghiepGridVo)p).ToList();
            var excelData = ngheNghiepData.Map<List<NgheNghiepExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NgheNghiepExportExcel.TenVietTat), "Tên viết tắt"));
            lstValueObject.Add((nameof(NgheNghiepExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(NgheNghiepExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(NgheNghiepExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nghề nghiệp");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NgheNghiep" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}