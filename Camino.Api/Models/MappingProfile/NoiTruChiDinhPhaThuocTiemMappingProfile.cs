using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiTruChiDinhPhaThuocTiemMappingProfile : Profile
    {
        public NoiTruChiDinhPhaThuocTiemMappingProfile()
        {
            //CreateMap<DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel, NoiTruChiDinhPhaThuocTiem>().IgnoreAllNonExisting();
            //CreateMap<NoiTruChiDinhPhaThuocTiem, DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel>().IgnoreAllNonExisting();

            CreateMap<DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel, PhaThuocTiemBenhVienVo>().IgnoreAllNonExisting();
            CreateMap<PhaThuocTiemBenhVienVo, DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel>().IgnoreAllNonExisting();
        }
    }
}
