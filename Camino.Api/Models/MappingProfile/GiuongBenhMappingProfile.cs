using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GiuongBenhs;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GiuongBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class GiuongBenhMappingProfile : Profile
    {
        public GiuongBenhMappingProfile()
        {
            CreateMap<GiuongBenh, GiuongBenhViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.KhoaId, o => o.MapFrom(s => s.PhongBenhVien.KhoaPhongId))
                .AfterMap((s, d) =>
                {
                    d.CoHieuLuc = s.IsDisabled != true;
                });

            CreateMap<GiuongBenhViewModel, GiuongBenh>()
                .ForMember(x => x.HoatDongGiuongBenhs, o => o.Ignore())
                .ForMember(x => x.YeuCauDichVuGiuongBenhViens, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.IsDisabled = s.CoHieuLuc != true;
                });
            //.AfterMap((s, d) => AddOrUpdate(s, d));
            //.ForMember(x => x.Khoa, o => o.Ignore());

            CreateMap<GiuongBenhGridVo, GiuongBenhExportExcel>().IgnoreAllNonExisting();
        }

        //private void AddOrUpdate(GiuongBenhViewModel s, GiuongBenh d)
        //{
            
        //}
    }
}