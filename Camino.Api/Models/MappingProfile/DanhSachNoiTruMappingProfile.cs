using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;

namespace Camino.Api.Models.MappingProfile
{
    public class DanhSachNoiTruMappingProfile : Profile
    {
        public DanhSachNoiTruMappingProfile()
        {

            CreateMap<DanhSachNoiTruBenhAnGridVo, DanhSachNoiTruExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.CapCuu, o => o.MapFrom(p => p.CapCuu == false ? "Không" : "Có"))
                .ForMember(m => m.TenTrangThai, o => o.MapFrom(p => p.TenTrangThai + (p.TrangThai == 1 && p.KetThucBenhAn == true && p.DaQuyetToan != true ? " (Chờ quyết toán)" : "")));
        }
    }
}
