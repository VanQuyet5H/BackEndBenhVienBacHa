using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.HocViHocHam;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.HocViHocHam;

namespace Camino.Api.Models.MappingProfile
{
    public class HocViHocHamMappingProfile : Profile
    {
        public HocViHocHamMappingProfile()
        {

            CreateMap<Core.Domain.Entities.HocViHocHams.HocViHocHam, HocViHocHamViewModel>();
            CreateMap<HocViHocHamViewModel, Core.Domain.Entities.HocViHocHams.HocViHocHam>();
            CreateMap<HocViHocHamGripVo, HocViHocHamExportExcel>().IgnoreAllNonExisting();
        }
    }
}
