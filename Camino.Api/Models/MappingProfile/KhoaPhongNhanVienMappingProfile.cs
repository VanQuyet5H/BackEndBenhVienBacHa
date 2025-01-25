using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhoaPhongNhanVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoaPhongNhanVien;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoaPhongNhanVienMappingProfile : Profile
    {
        public KhoaPhongNhanVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien
                    , KhoaPhongNhanVienViewModel>()
                .AfterMap((source, dest) =>
                {
                    if (source.NhanVien != null)
                    {
                        dest.NhanVienId = source.NhanVien.User.Id;
                        dest.TenNhanVien = source.NhanVien.User.HoTen;
                    }

                    if (source.KhoaPhong != null)
                    {
                        dest.TenKhoaPhong = source.KhoaPhong.Ten;
                    }
                });

            CreateMap<KhoaPhongNhanVienViewModel, Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien>()
                .ForMember(d => d.NhanVien, o => o.Ignore())
                .ForMember(d => d.KhoaPhong, o => o.Ignore());           

            CreateMap<KhoaPhongNhanVienGridVo, KhoaPhongNhanVienExportExcel>().IgnoreAllNonExisting();
        }
    }
}
