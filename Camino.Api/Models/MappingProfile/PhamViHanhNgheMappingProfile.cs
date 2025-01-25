using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.PhamViHanhNghe;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.PhamViHanhNghe;

namespace Camino.Api.Models.MappingProfile
{
    public class PhamViHanhNgheMappingProfile : Profile
    {
        public PhamViHanhNgheMappingProfile()
        {
            CreateMap<Core.Domain.Entities.PhamViHanhNghes.PhamViHanhNghe
                , PhamViHanhNgheViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<PhamViHanhNgheViewModel
                , Core.Domain.Entities.PhamViHanhNghes.PhamViHanhNghe>()
                .IgnoreAllNonExisting();

            CreateMap<PhamViHanhNgheGridVo, PhamViHanhNgheExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}
