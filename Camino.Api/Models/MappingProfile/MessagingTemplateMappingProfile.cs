using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Template;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Template;

namespace Camino.Api.Models.MappingProfile
{
    public class MessagingTemplateMappingProfile : Profile
    {
        public MessagingTemplateMappingProfile()
        {
            CreateMap<MessagingTemplate, MessagingTemplateViewModel>();

            CreateMap<MessagingTemplateViewModel, MessagingTemplate>();

            CreateMap<MesagingTemplateGridVo, MessagingTemplateExportExcel>().IgnoreAllNonExisting();
        }

    }
}
