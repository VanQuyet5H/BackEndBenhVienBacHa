using AutoMapper;
using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.PhongBenhVien;

namespace Camino.Api.Models.MappingProfile
{
    public class PhongBenhVienMappingProfile : Profile
    {
        public PhongBenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.PhongBenhViens.PhongBenhVien, PhongBenhVienViewModel>()
                    .AfterMap((source, dest) =>
                    {
                        dest.TenKhoaPhong = source.KhoaPhong != null ? source.KhoaPhong.Ten : string.Empty;
                        dest.KieuKham = source.KhoaPhong?.CoKhamNgoaiTru;
                    });

            CreateMap<PhongBenhVienViewModel, Core.Domain.Entities.PhongBenhViens.PhongBenhVien>()
                      .ForMember(d => d.KhoaPhong, o => o.Ignore())
                      .ForMember(d => d.DichVuKhamBenhBenhVienNoiThucHiens, o => o.Ignore());

            CreateMap<PhongBenhVienGridVo, KhoaPhongPhongKhamExportExcel>()
                .AfterMap((source, dest) =>
                {
                    dest.IsDisabled = source.IsDisabled != true ? "Đang sử dụng" : "Ngừng sử dụng";
                });
        }
    }
}
