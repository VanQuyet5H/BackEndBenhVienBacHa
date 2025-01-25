using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhaThau;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NhaThau;

namespace Camino.Api.Models.MappingProfile
{
    public class NhaThauMappingProfile: Profile
    {
        public NhaThauMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhaThaus.NhaThau, NhaThauViewModel>().IgnoreAllNonExisting();
            CreateMap<NhaThauViewModel, Core.Domain.Entities.NhaThaus.NhaThau>().IgnoreAllNonExisting();
            CreateMap<NhaThauGridVo, NhaThauExportExcel>().IgnoreAllNonExisting();
        }
    }
}
