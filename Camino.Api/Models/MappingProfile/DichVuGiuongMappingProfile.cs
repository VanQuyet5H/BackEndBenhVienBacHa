using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{

    public class DichVuGiuongMappingProfile : Profile
    {
        public DichVuGiuongMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong, DichVuGiuong.DichVuGiuongViewModel>()
                .IgnoreAllNonExisting();

            CreateMap<DichVuGiuong.DichVuGiuongViewModel, Core.Domain.Entities.DichVuGiuongs.DichVuGiuong>()
                .ForMember(x => x.Khoa, o => o.Ignore());

            CreateMap<DichVuGiuongGridVo, DichVuGiuongExportExcel>().IgnoreAllNonExisting();
            CreateMap<DichVuGiuongThongTinGiaGridVo, DichVuGiuongExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
