using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;

namespace Camino.Api.Models.MappingProfile
{
    public class DuocPhamBenhVienPhanNhomMappingProfile : Profile
    {
        public DuocPhamBenhVienPhanNhomMappingProfile()
        {
            CreateMap<DuocPhamBenhVienPhanNhom, DuocPhamBenhVienPhanNhomViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<DuocPhamBenhVienPhanNhomViewModel, DuocPhamBenhVienPhanNhom>()
                .IgnoreAllNonExisting();
        }
    }
}
