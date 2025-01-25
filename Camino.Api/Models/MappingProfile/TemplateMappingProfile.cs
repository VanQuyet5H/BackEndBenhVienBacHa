using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Template;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Template;

namespace Camino.Api.Models.MappingProfile
{
    public class TemplateMappingProfile : Profile
    {
        public TemplateMappingProfile()
        {
            CreateMap<Core.Domain.Template, TemplateViewModel>();

            CreateMap<TemplateViewModel, Core.Domain.Template>();

            CreateMap<TemplateGridVo, TemplatePDFExportExcel>().IgnoreAllNonExisting();
        }


    }
}
