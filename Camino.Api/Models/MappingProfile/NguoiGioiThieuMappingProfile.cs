using AutoMapper;
using Camino.Api.Extensions;


namespace Camino.Api.Models.MappingProfile
{
    public class NguoiGioiThieuMappingProfile : Profile
    {
        public NguoiGioiThieuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu, NguoiGioiThieu.NguoiGioiThieuViewModel>().IgnoreAllNonExisting()
                  .AfterMap((s, d) =>
                  {
                      d.HoTenNguoiQuanLy = s.NhanVien?.User?.HoTen + "  -  " + s.NhanVien?.User?.SoDienThoai;
                  });

            CreateMap<NguoiGioiThieu.NguoiGioiThieuViewModel, Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu>().IgnoreAllNonExisting();
        }
    }
}
