using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.Template;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Template;
using Camino.Core.Helpers;
using Camino.Services;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace Camino.Api.Controllers
{
    public class TemplateController : CaminoBaseController
    {
        // GET: /<controller>/
        readonly ITemplateService _templateService;
        private readonly IExcelService _excelService;

        public TemplateController(ITemplateService templateService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _templateService = templateService;
            _excelService = excelService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            int type = (int)(Enums.TemplateType.NoiDungMauPDF);
            queryInfo.AdditionalSearchString = type.ToString();
            var gridData = await _templateService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            int type = (int)(Enums.TemplateType.NoiDungMauPDF);
            queryInfo.AdditionalSearchString = type.ToString();
            var gridData = await _templateService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<TemplateViewModel>> Get(long id)
        {
            var template = await _templateService.GetByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            return Ok(template.ToModel<TemplateViewModel>());
        }
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf)]
        public async Task<ActionResult> Put([FromBody] TemplateViewModel templateViewModel)
        {
            var template = await _templateService.GetByIdAsync(templateViewModel.Id);
            if (template == null)
            {
                return NotFound();
            }
            templateViewModel.ToEntity(template);
            var entity = new Template
            {
                TemplateType = template.TemplateType,
                Language = template.Language,
                Body = template.Body,
                Version = template.Version+1,
                Name = template.Name,
                Title = template.Title,
                LastTime = DateTime.Now,
                Description = template.Description,
                IsDisabled = true
            };
            _templateService.Add(entity);
            return NoContent();
        }

        [HttpPost("GetTemplateType")]
        public ActionResult<ICollection<LookupItemVo>> GetTemplateType()
        {
            var values = ((Enums.TemplateType[])Enum.GetValues(typeof(Enums.TemplateType))).ToList();

            var data = values.Select(s => new LookupItemVo()
            {
                KeyId = (values.IndexOf(s) + 1),
                DisplayName = s.GetDescription()
            }).ToList();

            return Ok(data);
        }

    

        [HttpPost("KhoaTemplateAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf)]
        public async Task<ActionResult> KhoaTemplate(long id)
        {
            var template =await _templateService.GetByIdAsync(id);
            template.IsDisabled = template.IsDisabled == null ? true : !template.IsDisabled;
            await _templateService.UpdateAsync(template);
            return NoContent();
        }

        [HttpPost("ExportTemplate")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyNoiDungMauXuatRaPdf)]
        public async Task<ActionResult> ExportTemplate(QueryInfo queryInfo)
        {
            var gridData = await _templateService.GetDataForGridAsync(queryInfo, true);
            var templateData = gridData.Data.Select(p => (TemplateGridVo)p).ToList();
            var excelData = templateData.Map<List<TemplatePDFExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(TemplatePDFExportExcel.Description), "Mô tả"));
            lstValueObject.Add((nameof(TemplatePDFExportExcel.PhienBan), "Phiên bản"));
            lstValueObject.Add((nameof(TemplatePDFExportExcel.DateUpdateText), "Ngày cập nhật"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nội dung mẫu PDF");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NoiDungMauPDF" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}