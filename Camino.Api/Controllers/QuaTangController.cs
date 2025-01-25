using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.Marketing;
using Camino.Core.Domain.Entities.QuaTangs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Localization;
using Camino.Services.QuaTang;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Marketing;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuaTangController : CaminoBaseController
    {
        private readonly IQuaTangService _quaTangService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public QuaTangController(IQuaTangService quaTangService, ILocalizationService localizationService, IExcelService excelService)
        {
            _quaTangService = quaTangService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ClaimRequirement(SecurityOperation.View, DocumentType.QuaTangMarketing)]
        [HttpPost("GetDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _quaTangService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _quaTangService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult<QuaTangMarketingViewModel>> Get(long id)
        {
            var quaTang = await _quaTangService.GetByIdAsync(id);

            if (quaTang == null)
            {
                return NotFound();
            }

            QuaTangMarketingViewModel quaTangVM = quaTang.ToModel<QuaTangMarketingViewModel>();

            return Ok(quaTangVM);
        }

        [HttpPost]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult<QuaTangMarketingViewModel>> Post([FromBody] QuaTangMarketingViewModel quaTangViewModel)
        {
            var quaTang = quaTangViewModel.ToEntity<QuaTang>();

            await _quaTangService.AddAsync(quaTang);

            var quaTangResponse = await _quaTangService.GetByIdAsync(quaTang.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = quaTang.Id },
                quaTangResponse.ToModel<QuaTangMarketingViewModel>()
            );
        }

        [HttpPut]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult> Put([FromBody] QuaTangMarketingViewModel quaTangViewModel)
        {
            var quaTang = await _quaTangService.GetByIdAsync(quaTangViewModel.Id);
            if (quaTang == null)
            {
                return NotFound();
            }

            quaTangViewModel.ToEntity(quaTang);

            await _quaTangService.UpdateAsync(quaTang);

            return NoContent();
        }

        [HttpPost("KichHoatTrangThai")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult> KichHoatTrangThai(long id)
        {
            var entity = await _quaTangService.GetByIdAsync(id);
            entity.HieuLuc = !entity.HieuLuc;

            await _quaTangService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult> Delete(long id)
        {
            var quaTang = await _quaTangService.GetByIdAsync(id);

            if (quaTang == null)
            {
                return NotFound();
            }

            await _quaTangService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.QuaTangMarketing)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var quaTangs = await _quaTangService.GetByIdsAsync(model.Ids);

            if (quaTangs == null)
            {
                return NotFound();
            }

            if (quaTangs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }

            IEnumerable<QuaTang> list = await _quaTangService.GetByIdsAsync(model.Ids);
            await _quaTangService.DeleteAsync(list);

            return NoContent();
        }
        #endregion

        [HttpPost("ExportQuaTang")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuaTangMarketing)]
        public async Task<ActionResult> ExportQuaTang(QueryInfo queryInfo)
        {
            var gridData = await _quaTangService.GetDataForGridAsync(queryInfo, true);
            var quaTangData = gridData.Data.Select(p => (QuaTangMarketingGridVo)p).ToList();
            var excelData = quaTangData.Map<List<QuaTangMarketingExporExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(QuaTangMarketingExporExcel.Ten), "Tên"),
                (nameof(QuaTangMarketingExporExcel.DonViTinh), "Đơn Vị Tính"),
                (nameof(QuaTangMarketingExporExcel.MoTa), "Mô Tả"),
                (nameof(QuaTangMarketingExporExcel.HieuLuc), "Hiệu Lực"),
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Quà Tặng");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=QuaTang" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
