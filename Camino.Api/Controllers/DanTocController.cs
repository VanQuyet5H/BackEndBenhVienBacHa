using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DanToc;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DanTocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanToc;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DanToc;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DanTocController : CaminoBaseController
    {
        private readonly IDanTocService _danTocService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DanTocController(IDanTocService danTocService, ILocalizationService localizationService, IExcelService excelService)
        {
            _danTocService = danTocService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanToc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _danTocService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanToc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _danTocService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("KichHoatChucDanh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanToc)]
        public async Task<ActionResult> KichHoatChucDanh(long id)
        {
            var entity = await _danTocService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _danTocService.UpdateAsync(entity);
            return NoContent();
        }
        [HttpPost("GetListQuocGia")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListQuocGia([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _danTocService.GetListQuocGia(queryInfo);
            return Ok(lookup);
        }
        #region CRUD

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanToc)]
        public async Task<ActionResult<DanTocViewModel>> Post([FromBody] DanTocViewModel danTocViewModel)
        {
            var user = danTocViewModel.ToEntity<DanToc>();
            _danTocService.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<DanTocViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanToc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DanTocViewModel>> Get(long id)
        {
            var result = await _danTocService.GetByIdAsync(id, s => s.Include(k => k.QuocGia));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<DanTocViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanToc)]
        public async Task<ActionResult> Put([FromBody] DanTocViewModel danTocViewModel)
        {
            var dantoc = await _danTocService.GetByIdAsync(danTocViewModel.Id);
            if (dantoc == null)
            {
                return NotFound();
            }
            danTocViewModel.ToEntity(dantoc);
            await _danTocService.UpdateAsync(dantoc);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanToc)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucdanh = await _danTocService.GetByIdAsync(id);
            if (chucdanh == null)
            {
                return NotFound();
            }
            await _danTocService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanToc)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var dantocs = await _danTocService.GetByIdsAsync(model.Ids);
            if (dantocs == null)
            {
                return NotFound();
            }
            if (dantocs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _danTocService.DeleteAsync(dantocs);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportDanToc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanToc)]
        public async Task<ActionResult> ExportDanToc(QueryInfo queryInfo)
        {
            var gridData = await _danTocService.GetDataForGridAsync(queryInfo, true);
            var danTocData = gridData.Data.Select(p => (DanTocGridVo)p).ToList();
            var excelData = danTocData.Map<List<DanTocExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DanTocExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DanTocExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(DanTocExportExcel.TenQuocGia), "Tên quốc gia"));
            lstValueObject.Add((nameof(DanTocExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dân tộc");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanToc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
