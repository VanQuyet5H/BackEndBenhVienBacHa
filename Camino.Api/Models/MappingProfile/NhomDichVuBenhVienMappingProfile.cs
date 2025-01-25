using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhomDichVuBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.NhomDichVuBenhVien;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomDichVuBenhVienMappingProfile : Profile
    {
        public NhomDichVuBenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien, NhomDichVuBenhVienViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) => { d.NhomDichVuBenhVienCha = s.NhomDichVuBenhVienCha?.Ten; });
            CreateMap<NhomDichVuBenhVienViewModel, Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien>().IgnoreAllNonExisting()
                .ForMember(x=>x.NhomDichVuBenhVienCha,o=>o.Ignore());
            //CreateMap<NhomDichVuBenhVienGridVo, NhomDichVuBenhVienExportExcel>().IgnoreAllNonExisting();
        }
    }
}
