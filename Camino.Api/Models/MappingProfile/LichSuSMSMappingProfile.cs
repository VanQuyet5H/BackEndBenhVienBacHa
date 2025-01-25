using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LichSuSMS;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Messages;

namespace Camino.Api.Models.MappingProfile
{
    public class LichSuSMSMappingProfile : Profile
    {
        public LichSuSMSMappingProfile()
        {
            CreateMap<Core.Domain.Entities.Messages.LichSuSMS, LichSuSMSViewModel>().IgnoreAllNonExisting();

            CreateMap<LichSuSMSViewModel, Core.Domain.Entities.Messages.LichSuSMS>().IgnoreAllNonExisting();

            CreateMap<LichSuSMSGrid, LichSuSMSExportExcel>().IgnoreAllNonExisting();
        }
    }
}
