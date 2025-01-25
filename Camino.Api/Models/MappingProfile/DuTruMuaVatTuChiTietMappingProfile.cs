using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaVatTu;
using Camino.Core.Domain.Entities.DuTruVatTus;

namespace Camino.Api.Models.MappingProfile
{
    public class DuTruMuaVatTuChiTietMappingProfile : Profile
    {
        public DuTruMuaVatTuChiTietMappingProfile()
        {
            CreateMap<DuTruMuaVatTuChiTiet, DuTruMuaVatTuChiTietViewModel>().IgnoreAllNonExisting()
              .AfterMap((s, d) =>
              {
                  d.Ten = s.VatTu.Ten;
                  d.DVT = s.VatTu.DonViTinh;
                  d.NhaSX = s.VatTu.NhaSanXuat;
                  d.NuocSX = s.VatTu.NuocSanXuat;
                  d.SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet;
              });

            CreateMap<DuTruMuaVatTuChiTietViewModel, DuTruMuaVatTuChiTiet>().IgnoreAllNonExisting();


            CreateMap<Core.Domain.Entities.VatTus.VatTu, VatTuDuTruGridViewModel>().IgnoreAllNonExisting();
            CreateMap<VatTuDuTruGridViewModel, Core.Domain.Entities.VatTus.VatTu>().IgnoreAllNonExisting();

        }
    }
}
