using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.GoiDichVu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;

namespace Camino.Api.Models.MappingProfile
{
    public class GoiDichVuMappingProfile : Profile
    {
        public GoiDichVuMappingProfile()
        {
            CreateMap<GoiDvMarketingViewModel, Core.Domain.Entities.GoiDichVus.GoiDichVu>()
                .ForMember(x => x.GoiDichVuChiTietDichVuKhamBenhs,
                    o => o.MapFrom(w => w.DvTrongGois.Where(x => x.Nhom == Enums.EnumDichVuTongHop.KhamBenh)))
                .ForMember(x => x.GoiDichVuChiTietDichVuKyThuats,
                    o => o.MapFrom(w => w.DvTrongGois.Where(x => x.Nhom == Enums.EnumDichVuTongHop.KyThuat)))
                .ForMember(x => x.GoiDichVuChiTietDichVuGiuongs,
                    o => o.MapFrom(w => w.DvTrongGois.Where(x => x.Nhom == Enums.EnumDichVuTongHop.GiuongBenh)))
                .AfterMap((s, d) =>
                {
                    d.Ten = s.TenGoiDv;
                });

            CreateMap<Core.Domain.Entities.GoiDichVus.GoiDichVu, GoiDvMarketingViewModel>()
                .ForMember(x => x.DvTrongGois, o => o.Ignore())
                .AfterMap((s, d) => { d.TenGoiDv = s.Ten; });

            CreateMap<DvTrongGoiViewModel, GoiDichVuChiTietDichVuKhamBenh>()
                .AfterMap((s, d) =>
                {
                    d.DichVuKhamBenhBenhVienId = s.DvId;
                    d.NhomGiaDichVuKhamBenhBenhVienId = s.LoaiGia;
                    d.SoLan = s.SoLuong;
                    d.Id = s.IdDatabase;
                });

            CreateMap<DvTrongGoiViewModel, GoiDichVuChiTietDichVuKyThuat>()
                .AfterMap((s, d) =>
                {
                    d.DichVuKyThuatBenhVienId = s.DvId;
                    d.NhomGiaDichVuKyThuatBenhVienId = s.LoaiGia;
                    d.SoLan = s.SoLuong;
                    d.Id = s.IdDatabase;
                });

            CreateMap<DvTrongGoiViewModel, GoiDichVuChiTietDichVuGiuong>()
                .AfterMap((s, d) =>
                {
                    d.DichVuGiuongBenhVienId = s.DvId;
                    d.NhomGiaDichVuGiuongBenhVienId = s.LoaiGia;
                    d.SoLan = s.SoLuong;
                    d.Id = s.IdDatabase;
                });

            CreateMap<GoiDvMarketingGridVo, GoiDvMarketingExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.SuDung, o => o.MapFrom(p => p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng"));

            CreateMap<Core.Domain.ValueObject.NhomDichVuBenhVien.NhomDichVuThuongDungGridVo, NhomDichVuExportExcel>().IgnoreAllNonExisting()
              .ForMember(m => m.SuDung, o => o.MapFrom(p => p.TrangThai == false ? "Đang sử dụng" : "Ngừng sử dụng"));
        }
    }
}
