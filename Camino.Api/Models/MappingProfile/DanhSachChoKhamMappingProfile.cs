using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Api.Models.MappingProfile
{
    public class DanhSachChoKhamMappingProfile : Profile
    {
        public DanhSachChoKhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, YeuCauTiepNhan.DanhSachChoKhamViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NoiTiepNhan, o => o.MapFrom(s => s.NoiTiepNhan));
            CreateMap<YeuCauTiepNhan.DanhSachChoKhamViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NoiTiepNhan, o => o.MapFrom(s => s.NoiTiepNhan));

            //lấy yêu cầu thông tin người bệnh từ yêu càu tiếp nhận
            CreateMap<Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans.YeuCauTiepNhanCongTyBaoHiemTuNhan,
                    ThongTinBenhNhan.ThongTinCongTyBaoHiemTuNhan>().IgnoreAllNonExisting()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.CongTyBaoHiemTuNhan.Id))
                .ForMember(d => d.TenCongTy, o => o.MapFrom(s => s.CongTyBaoHiemTuNhan.Ten))
                .ForMember(d => d.MaSoThe, o => o.MapFrom(s => s.MaSoThe));

            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, ThongTinBenhNhan.ThongTinThuNganBenhNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.CongTyUuDai, o => o.MapFrom(s => s.CongTyUuDai.Ten))
                .ForMember(d => d.DoiTuongUuDai, o => o.MapFrom(s => s.DoiTuongUuDai.Ten))
                .ForMember(d => d.ThongTinCongTyBaoHiemTuNhans, o => o.MapFrom(s => s.YeuCauTiepNhanCongTyBaoHiemTuNhans))

                .ForMember(d => d.TenBHYTgiayMienCungChiTra, o => o.MapFrom(s => s.BHYTGiayMienCungChiTra.Ten))
                .ForMember(d => d.BHYTgiayMienCungChiTraUrl, o => o.MapFrom(s => s.BHYTGiayMienCungChiTra.TenGuid))


                .ForMember(d => d.TenGiayChuyenVien, o => o.MapFrom(s => s.GiayChuyenVien.Ten))
                .ForMember(d => d.GiayChuyenVienUrl, o => o.MapFrom(s => s.GiayChuyenVien.TenGuid)

                );

            CreateMap<Core.Domain.Entities.BenhNhans.BenhNhan, ThongTinBenhNhan.ThongTinThuNganBenhNhanViewModel>()
                .IgnoreAllNonExisting();

            CreateMap<DanhSachChoKhamGridVo, DanhSachTiepNhanExportExcel>().IgnoreAllNonExisting();
        }
    }
}