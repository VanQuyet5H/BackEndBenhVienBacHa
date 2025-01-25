using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.XetNghiem;

namespace Camino.Api.Models.MappingProfile
{
    public class GoiMauXetNghiemMappingProfile : Profile
    {
        public GoiMauXetNghiemMappingProfile()
        {
            CreateMap<PhieuGoiMauXetNghiem, PhieuGoiMauXetNghiemViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.NguoiGoiMauId, o => o.MapFrom(p2 => p2.NhanVienGoiMauId))
                .ForMember(p => p.NguoiGoiMauDisplay, o => o.MapFrom(p2 => p2.NhanVienGoiMau.User.HoTen))
                .ForMember(p => p.NgayGoiMau, o => o.MapFrom(p2 => p2.ThoiDiemGoiMau))
                .ForMember(p => p.NoiTiepNhanId, o => o.MapFrom(p2 => p2.PhongNhanMauId))
                .ForMember(p => p.NoiTiepNhanDisplay, o => o.MapFrom(p2 => p2.PhongNhanMau.Ten))
                .ForMember(p => p.NguoiNhanMauId, o => o.MapFrom(p2 => p2.NhanVienNhanMauId))
                .ForMember(p => p.NguoiNhanMauDisplay, o => o.MapFrom(p2 => p2.NhanVienNhanMau.User.HoTen))
                .ForMember(p => p.NgayNhanMau, o => o.MapFrom(p2 => p2.ThoiDiemNhanMau))
                .ForMember(p => p.TinhTrang, o => o.MapFrom(p2 => p2.DaNhanMau));

            CreateMap<GoiMauDanhSachXetNghiemGridVo, GoiMauDanhSachXetNghiemExportExcel>();
            CreateMap<GoiMauDanhSachNhomXetNghiemGridVo, GoiMauDanhSachNhomXetNghiemExportExcelChild>().IgnoreAllNonExisting();
            CreateMap<GoiMauDanhSachDichVuXetNghiemGridVo, GoiMauDanhSachDichVuXetNghiemExportExcelChild>().IgnoreAllNonExisting();
            CreateMap<BenhNhanXetNghiemGridVo, BenhNhanXetNghiemChuaCapCodeExcel>().IgnoreAllNonExisting();

        }
    }
}