using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.ChucVu;
using Camino.Api.Models.ChucVu;
using Camino.Core.Domain.Entities.ChucVus;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Core.Domain.ValueObject.ChucVu;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class ChucVuController : CaminoBaseController
    {
        private readonly IChucVuService _chucVuService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public ChucVuController(IChucVuService chucVuService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _chucVuService = chucVuService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chucVuService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chucVuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult> Post([FromBody] ChucVuViewModel chucVuViewModel)
        {
            //chucVuViewModel.TenVietTat = chucVuViewModel.TenVietTat.RemoveCharacters();
            //chucVuViewModel.Ten = chucVuViewModel.Ten.RemoveCharacters();
            var chucvu = chucVuViewModel.ToEntity<ChucVu>();
            await _chucVuService.AddAsync(chucvu);
            return CreatedAtAction(nameof(Get), new { id = chucvu.Id }, chucvu.ToModel<ChucVuViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucVu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChucVuViewModel>> Get(long id)
        {
            var chucvu = await _chucVuService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            return Ok(chucvu.ToModel<ChucVuViewModel>());
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult> Put([FromBody] ChucVuViewModel chucVuViewModel)
        {
            var chucvu = await _chucVuService.GetByIdAsync(chucVuViewModel.Id);
            if (chucvu == null)
            {
                return NotFound();
            }
            //chucVuViewModel.TenVietTat = chucVuViewModel.TenVietTat.RemoveCharacters();
            //chucVuViewModel.Ten = chucVuViewModel.Ten.RemoveCharacters();
            chucVuViewModel.ToEntity(chucvu);

            await _chucVuService.UpdateAsync(chucvu);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _chucVuService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            await _chucVuService.DeleteByIdAsync(id);
            return NoContent();
        }
        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var chucVus = await _chucVuService.GetByIdsAsync(model.Ids);
            if (chucVus == null)
            {
                return NotFound();
            }
            if (chucVus.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _chucVuService.DeleteAsync(chucVus);
            return NoContent();
        }
        #endregion
        [HttpPost("KichHoatChucVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult> KichHoatChucVu(long id)
        {
            var entity = await _chucVuService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _chucVuService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportChucVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucChucVu)]
        public async Task<ActionResult> ExportChucVu(QueryInfo queryInfo)
        {
            var gridData = await _chucVuService.GetDataForGridAsync(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (ChucVuGridVo)p).ToList();
            var excelData = chucVuData.Map<List<ChucVuExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChucVuExportExcel.TenVietTat), "Tên viết tắt"));
            lstValueObject.Add((nameof(ChucVuExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(ChucVuExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Chức vụ");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChucVu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}