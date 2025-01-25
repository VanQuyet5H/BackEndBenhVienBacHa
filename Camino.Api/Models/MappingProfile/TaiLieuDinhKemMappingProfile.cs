using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien;
using Camino.Core.Domain.Entities;

namespace Camino.Api.Models.MappingProfile
{
    public class TaiLieuDinhKemMappingProfile : Profile
    {
        public TaiLieuDinhKemMappingProfile()
        {
            CreateMap<TaiLieuDinhKemViewModel, TaiLieuDinhKemEntity>().IgnoreAllNonExisting();
            CreateMap<TaiLieuDinhKemEntity, TaiLieuDinhKemViewModel>().IgnoreAllNonExisting();

        }
    }
}