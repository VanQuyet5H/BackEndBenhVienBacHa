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
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.Messages;
using Microsoft.AspNetCore.Mvc;


namespace Camino.Api.Controllers
{
    public class TemplateThongBaoController : CaminoBaseController
    {
        // GET: /<controller>/
        readonly IMessagingTemplateService _messagingTemplateService;
        public TemplateThongBaoController(IMessagingTemplateService templateService, IJwtFactory iJwtFactory)
        {
            _messagingTemplateService = templateService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacMessagingTemplate)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            int type = (int)(Enums.MessagingType.Notification);
            queryInfo.AdditionalSearchString = type.ToString();
            var gridData = await _messagingTemplateService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacMessagingTemplate)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            int type = (int)(Enums.MessagingType.Notification);
            queryInfo.AdditionalSearchString = type.ToString();
            var gridData = await _messagingTemplateService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacMessagingTemplate)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MessagingTemplateViewModel>> Get(long id)
        {
            var template = await _messagingTemplateService.GetByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            return Ok(template.ToModel<MessagingTemplateViewModel>());
        }
       
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuanLyCacMessagingTemplate)]
        public async Task<ActionResult> Put([FromBody] MessagingTemplateViewModel templateViewModel)
        {
            var template = await _messagingTemplateService.GetByIdAsync(templateViewModel.Id);
            if (template == null)
            {
                return NotFound();
            }
            templateViewModel.ToEntity(template);
            await _messagingTemplateService.UpdateAsync(template);
            return NoContent();
        }

        [HttpPost("GetTemplateType")]
        public ActionResult<ICollection<LookupItemVo>> GetTemplateType()
        {
            var values = ((Enums.MessagingType[])Enum.GetValues(typeof(Enums.MessagingType))).ToList();
            var data = values.Select(s => new LookupItemVo()
            {
                KeyId = (values.IndexOf(s) + 1),
                DisplayName = s.GetDescription()
            }).ToList();

            return Ok(data);
        }
    
        [HttpPost("KhoaTemplateAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyCacMessagingTemplate)]
        public async Task<ActionResult> KhoaTemplate(long id)
        {
            var template = await _messagingTemplateService.GetByIdAsync(id);
            template.IsDisabled = template.IsDisabled == null ? true : !template.IsDisabled;
            await _messagingTemplateService.UpdateAsync(template);
            return NoContent();
        }



    }
}