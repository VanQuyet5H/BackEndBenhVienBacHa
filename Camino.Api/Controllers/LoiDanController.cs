using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.General;
using Camino.Api.Models.LoiDan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoiDan;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.LoiDan;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class LoiDanController : CaminoBaseController
    {
        private readonly IExcelService _excelService;
        private readonly ILoiDanService _loiDanService;
        private readonly ILocalizationService _localizationService;

        public LoiDanController(ILoiDanService loiDanService,
            IExcelService excelService,
            ILocalizationService localizationService)
        {
            _excelService = excelService;
            _loiDanService = loiDanService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LoiDan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loiDanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LoiDan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loiDanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetICD")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetICD([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _loiDanService.GetICD(model);
            return Ok(lookup);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LoiDan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LoiDanViewModel>> Get(long id)
        {
            var loiDan = await _loiDanService.GetByIdAsync(id);

            if (loiDan == null)
            {
                return NotFound();
            }

            var loiDanViewModel = new LoiDanViewModel
            {
                Id = loiDan.Id,
                IcdId = loiDan.Id,
                LoiDanCuaBacSi = loiDan.LoiDanCuaBacSi,
                ICD = loiDan.Ma + " - " + loiDan.TenTiengViet
            };

            return Ok(loiDanViewModel);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.LoiDan)]
        public async Task<ActionResult> Post
            ([FromBody]LoiDanViewModel loiDanViewModel)
        {
            var loiDan = await _loiDanService.GetByIdAsync(loiDanViewModel.IcdId);
            loiDan.LoiDanCuaBacSi = loiDanViewModel.LoiDanCuaBacSi;
            await _loiDanService.UpdateAsync(loiDan);
            return Ok();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LoiDan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatLoiDan([FromBody]LoiDanViewModel loiDanViewModel)
        {
            var loiDan = await _loiDanService.GetByIdAsync(loiDanViewModel.Id);
            loiDan.LoiDanCuaBacSi = loiDanViewModel.LoiDanCuaBacSi;
            await _loiDanService.UpdateAsync(loiDan);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.LoiDan)]
        public async Task<ActionResult> Delete(long id)
        {
            var loiDan = await _loiDanService.GetByIdAsync(id);

            if (loiDan == null)
            {
                return NotFound();
            }

            loiDan.LoiDanCuaBacSi = null;

            await _loiDanService.UpdateAsync(loiDan);
            return Ok();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.LoiDan)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var loiDans = await _loiDanService.GetByIdsAsync(model.Ids);
            if (loiDans == null)
            {
                return NotFound();
            }

            var loiDanList = loiDans.ToList();
            if (loiDanList.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            foreach (var loiDan in loiDanList)
            {
                loiDan.LoiDanCuaBacSi = null;
                await _loiDanService.UpdateAsync(loiDan);
            }

            return Ok();
        }

        [HttpPost("ExportLoiDan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LoiDan)]
        public async Task<ActionResult> ExportLoiDan(QueryInfo queryInfo)
        {
            var gridData = await _loiDanService.GetDataForGridAsync(queryInfo, true);
            var loaiBenhVienData = gridData.Data.Select(p => (LoiDanGridVo)p).ToList();
            var dataExcel = loaiBenhVienData.Map<List<LoiDanExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(LoiDanExportExcel.ICD), "ICD"),
                (nameof(LoiDanExportExcel.LoiDanCuaBacSi), "Lời dặn của bác sỹ")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lời Dặn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LoiDan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
