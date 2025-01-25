using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.QuocGia;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class QuocGiaMappingProfile : Profile
    {
        public QuocGiaMappingProfile()
        {
            CreateMap<Camino.Core.Domain.Entities.QuocGias.QuocGia, QuocGiaViewModel>();

            CreateMap<QuocGiaViewModel, Camino.Core.Domain.Entities.QuocGias.QuocGia>();

            CreateMap<QuocGiaGridVo, QuocGiaExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
    }
}
