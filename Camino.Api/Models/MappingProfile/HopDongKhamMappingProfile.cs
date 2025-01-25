using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class HopDongKhamMappingProfile : Profile
    {
        public HopDongKhamMappingProfile()
        {
            CreateMap<HopDongKhamSucKhoe, KhamDoanHopDongKhamViewModel>().IgnoreAllNonExisting()
                  .ForMember(x => x.HopDongKhamSucKhoeDiaDiems, o => o.MapFrom(y => y.HopDongKhamSucKhoeDiaDiems));

            CreateMap<KhamDoanHopDongKhamViewModel, HopDongKhamSucKhoe>().IgnoreAllNonExisting()
                        .ForMember(x => x.HopDongKhamSucKhoeDiaDiems, o => o.MapFrom(y => y.HopDongKhamSucKhoeDiaDiems))
                        .ForMember(x => x.DaKetThuc, o => o.MapFrom(y => y.TrangThaiHopDongKham == Core.Domain.Enums.TrangThaiHopDongKham.DaKetThucHD));

            CreateMap<HopDongSucKhoeDiaDiemViewModel, HopDongKhamSucKhoeDiaDiem>().IgnoreAllNonExisting();
            CreateMap<HopDongKhamSucKhoeDiaDiem, HopDongSucKhoeDiaDiemViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.CongViecId, o => o.MapFrom(y => y.CongViec == Core.Domain.Enums.CongViecKhamSucKhoe.LayMau.GetDescription() ?
                            Core.Domain.Enums.CongViecKhamSucKhoe.LayMau : Core.Domain.Enums.CongViecKhamSucKhoe.Kham));


            CreateMap<HopDongKhamSucKhoeNhanVien, HopDongKhamSucKhoeNhanVienViewModel>().IgnoreAllNonExisting(); 
            CreateMap<HopDongKhamSucKhoeNhanVienViewModel, HopDongKhamSucKhoeNhanVien>().IgnoreAllNonExisting()
                 .AfterMap((d, s) =>
                 {
                     if (d.NgayThangNamSinh != null)
                     {
                         s.NamSinh = d.NgayThangNamSinh.Value.Year;
                         s.ThangSinh = d.NgayThangNamSinh.Value.Month;
                         s.NgaySinh = d.NgayThangNamSinh.Value.Day;
                     }
                 });


            CreateMap<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien, DanhSachPhongKhamTaiCongTyViewModel>().IgnoreAllNonExisting();
            CreateMap<DanhSachPhongKhamTaiCongTyViewModel, Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien>().IgnoreAllNonExisting();

            CreateMap<KhamDoanHopDongKhamGridVo,HopDongKhamExportExcel>().IgnoreAllNonExisting();
            CreateMap<KhamDoanCongTyGridVo, CongTyKhamExportExcel>().IgnoreAllNonExisting();
            CreateMap<KhamDoanYeuCauNhanSuKhamSucKhoeGridVo, KhamDoanYeuCauNhanSuKhamSucKhoeExportExcel>().IgnoreAllNonExisting();
        }
    }
} 