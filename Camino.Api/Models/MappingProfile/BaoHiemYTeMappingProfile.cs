using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BHYT;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;

namespace Camino.Api.Models.MappingProfile
{
    public class BaoHiemYTeMappingProfile : Profile
    {
        public BaoHiemYTeMappingProfile()
        {
            CreateMap<Core.Domain.ValueObject.BHYT.ThongTinBenhNhan, ThongTinBenhNhanViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<ThongTinBenhNhanViewModel, Core.Domain.ValueObject.BHYT.ThongTinBenhNhan>()
                .IgnoreAllNonExisting();

            CreateMap<HoSoChiTietThuoc, HoSoChiTietThuocViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<HoSoChiTietThuocViewModel, HoSoChiTietThuoc>()
                .IgnoreAllNonExisting();

            CreateMap<HoSoChiTietDVKT, HoSoChiTietDVKTViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<HoSoChiTietDVKTViewModel, HoSoChiTietDVKT>()
                .IgnoreAllNonExisting();

            CreateMap<HoSoChiTietDienBienBenh, HoSoChiTietDienBienBenhViewModel>()
               .IgnoreAllNonExisting();
            CreateMap<HoSoChiTietDienBienBenhViewModel, HoSoChiTietDienBienBenh>()
                .IgnoreAllNonExisting();

            CreateMap<HoSoCanLamSang, HoSoCanLamSangViewModel>()
               .IgnoreAllNonExisting();
            CreateMap<HoSoCanLamSangViewModel, HoSoCanLamSang>()
                .IgnoreAllNonExisting();

            CreateMap<ThongTinBenhNhanXemVO, ThongTinBenhNhanModel>()
                .IgnoreAllNonExisting();
            CreateMap<ThongTinBenhNhanModel, ThongTinBenhNhanXemVO>()
                .IgnoreAllNonExisting();

            CreateMap<DanhSachChoGridVo, XacNhanBhytExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.Status = s.SoTienChoXacNhan > 0 ? "Chờ xác nhận" : "Đã xác nhận";
                });

            CreateMap<LichSuXacNhanBHYTGridVo, LichSuXacNhanBhytExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.IdLichSuXacNhan = s.Id.ToString();
                });

            CreateMap<ListXacNhanBhytDaHoanThanhGridVo, ListXacNhanBhytDaHoanThanhExportExcel>();



            CreateMap<ThongTinBHYTVO, ThongTinBHYTViewModel>().IgnoreAllNonExisting()
                 .ForMember(d => d.dsLichSuKCB2018, o => o.MapFrom(s => s.dsLichSuKCB2018))

                .ForMember(d => d.dsLichSuKT2018, o => o.MapFrom(s => s.dsLichSuKT2018));
            CreateMap<ThongTinBHYTViewModel, ThongTinBHYTVO>().IgnoreAllNonExisting()
                  .ForMember(d => d.dsLichSuKCB2018, o => o.MapFrom(s => s.dsLichSuKCB2018))

                 .ForMember(d => d.dsLichSuKT2018, o => o.MapFrom(s => s.dsLichSuKT2018));




            CreateMap<ThongTinTokenMoiVO, ThongTinTokenMoiViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.APIKey, o => o.MapFrom(s => s.APIKey));
            CreateMap<ThongTinTokenMoiViewModel, ThongTinTokenMoiVO>().IgnoreAllNonExisting()
                  .ForMember(d => d.APIKey, o => o.MapFrom(s => s.APIKey));



            var index = 1;
            CreateMap<GoiBaoHiemYTeVo, GoiBaoHiemYTeExportExcel>()
                    .AfterMap((s, d) =>
                    {
                        d.STT = index + 1;                
                    });

            CreateMap<GoiBaoHiemYTeVo, LichSuGoiBaoHiemYTeExportExcel>()
                     .AfterMap((s, d) =>
                     {
                         d.STT = index + 1;
                     });
        }
    }
}
