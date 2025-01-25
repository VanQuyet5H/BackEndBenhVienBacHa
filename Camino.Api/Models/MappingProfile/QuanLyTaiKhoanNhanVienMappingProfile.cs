using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.QuanLyTaiKhoan;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.QuanLyTaiKhoan;

namespace Camino.Api.Models.MappingProfile
{
    public class QuanLyTaiKhoanNhanVienMappingProfile : Profile
    {
        public QuanLyTaiKhoanNhanVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhanViens.NhanVien
                    , QuanLyTaiKhoanNhanVienViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NhanVienId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.HoTen, o => o.MapFrom(s => s.User != null ? s.User.HoTen : ""))
                .ForMember(d => d.SoDienThoai, o => o.MapFrom(s => s.User != null ? s.User.SoDienThoai : ""))
                .ForMember(d => d.DiaChi, o => o.MapFrom(s => s.User != null ? s.User.DiaChi : ""))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.User != null ? s.User.Email : ""))
                ;

            CreateMap<QuanLyTaiKhoanNhanVienViewModel
                    , Core.Domain.Entities.NhanViens.NhanVien>()
                .IgnoreAllNonExisting();

            CreateMap<QuanLyTaiKhoanGridVo, QuanLyTaiKhoanExportExcel>().IgnoreAllNonExisting();
        }
    }
}