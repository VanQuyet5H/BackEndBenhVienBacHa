using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GayBenhAn;
using Camino.Core.Domain.ValueObject.BenhAnDienTus;

namespace Camino.Api.Models.MappingProfile
{
    public class GayBenhAnMappingProfile : Profile
    {

        public GayBenhAnMappingProfile()
        {
            CreateMap<Core.Domain.Entities.BenhAnDienTus.GayBenhAn, GayBenhAnViewModel>().IgnoreAllNonExisting()
                ;
            CreateMap<GayBenhAnViewModel, Core.Domain.Entities.BenhAnDienTus.GayBenhAn>().IgnoreAllNonExisting()
                .ForMember(x => x.GayBenhAnPhieuHoSos, o => o.Ignore())
                ;

            // export excel
            CreateMap<GayBenhAnVo, GayBenhAnExportExcel>().IgnoreAllNonExisting();
        }
    }
}
