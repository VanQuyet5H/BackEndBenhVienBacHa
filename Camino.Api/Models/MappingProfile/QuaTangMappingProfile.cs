using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Marketing;
using Camino.Core.Domain.ValueObject.Marketing;

namespace Camino.Api.Models.MappingProfile
{
    public class QuaTangMappingProfile : Profile
    {
        public QuaTangMappingProfile()
        {
            CreateMap<Core.Domain.Entities.QuaTangs.QuaTang, QuaTangMarketingViewModel>();

            CreateMap<QuaTangMarketingViewModel, Core.Domain.Entities.QuaTangs.QuaTang>();

            CreateMap<QuaTangMarketingGridVo, QuaTangMarketingExporExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.HieuLuc, o => o.MapFrom(p => p.HieuLuc != true ? "Ngừng sử dụng" : "Đang sử dụng"));
        }
    }
}
