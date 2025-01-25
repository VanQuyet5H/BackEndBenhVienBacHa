using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Services.CheDoAn;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Api.Models.CheDoAn;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Core.Domain.Entities.CheDoAns;
using Camino.Core.Domain.ValueObject.CheDoAn;
using Camino.Services.Localization;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{

    public class CheDoAnController : CaminoBaseController
    {
        private readonly ICheDoAnService _CheDoAnService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public CheDoAnController(ICheDoAnService CheDoAnService, ILocalizationService localizationService, IExcelService excelService)
        {
            _CheDoAnService = CheDoAnService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _CheDoAnService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _CheDoAnService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region CRUD
        /// <summary>
        ///     Add can ho
        /// </summary>
        /// <param name="CheDoAnViewModel"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult<CheDoAnViewModel>> Post([FromBody] CheDoAnViewModel CheDoAnViewModel)
        {
            var user = CheDoAnViewModel.ToEntity<CheDoAn>();
            _CheDoAnService.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<CheDoAnViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCheDoAn)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CheDoAnViewModel>> Get(long id)
        {
            var result = await _CheDoAnService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<CheDoAnViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult> Put([FromBody] CheDoAnViewModel CheDoAnViewModel)
        {
            var CheDoAn = await _CheDoAnService.GetByIdAsync(CheDoAnViewModel.Id);
            if (CheDoAn == null)
            {
                return NotFound();
            }
            CheDoAnViewModel.ToEntity(CheDoAn);
            await _CheDoAnService.UpdateAsync(CheDoAn);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult> Delete(long id)
        {
            var CheDoAn = await _CheDoAnService.GetByIdAsync(id);
            if (CheDoAn == null)
            {
                return NotFound();
            }
            await _CheDoAnService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var CheDoAns = await _CheDoAnService.GetByIdsAsync(model.Ids);
            if (CheDoAns == null)
            {
                return NotFound();
            }
            if (CheDoAns.Count()!= model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _CheDoAnService.DeleteAsync(CheDoAns);
            return NoContent();
        }
        #endregion
        #region GetListLookupItemVo

        [HttpPost("GetLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLookupAsync(DropDownListRequestModel model)
        {
            var lookup = await _CheDoAnService.GetLookupAsync(model);
            return Ok(lookup);

        }
        #endregion

        [HttpPost("KichHoatCheDoAn")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult> KichHoatCheDoAn(long id)
        {
            var entity = await _CheDoAnService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _CheDoAnService.UpdateAsync(entity);
            return NoContent();
        }
        [HttpPost("ExportCheDoAn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCheDoAn)]
        public async Task<ActionResult> ExportCheDoAn(QueryInfo queryInfo)
        {
            var gridData = await _CheDoAnService.GetDataForGridAsync(queryInfo, true);
            var CheDoAnData = gridData.Data.Select(p => (CheDoAnGridVo)p).ToList();
            var excelData = CheDoAnData.Map<List<CheDoAnExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CheDoAnExportExcel.KyHieu), "Ký hiệu"));
            lstValueObject.Add((nameof(CheDoAnExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(CheDoAnExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(CheDoAnExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Chế độ ăn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CheDoAn" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
