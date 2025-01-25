using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using System;

namespace Camino.Api.Models.MappingProfile
{
    public class PhauThuatThuThuatThongTinBenhNhanMappingProfile : Profile
    {
        public PhauThuatThuThuatThongTinBenhNhanMappingProfile()
        {
            //CreateMap<PhauThuatThuThuatThongTinBenhNhanViewModel, YeuCauDichVuKyThuat>().IgnoreAllNonExisting();

            //CreateMap<YeuCauDichVuKyThuat, PhauThuatThuThuatThongTinBenhNhanViewModel>().IgnoreAllNonExisting()
            //    .ForMember(x => x.YeuCauTiepNhan, o => o.MapFrom(y => y.YeuCauTiepNhan))
            //    .ForMember(x => x.TrangThaiHangDoi, o => o.MapFrom(y => y.PhongBenhVienHangDois.FirstOrDefault().TrangThai))
            //    .ForMember(x => x.MaBenhNhan, o => o.MapFrom(y => y.YeuCauTiepNhan.BenhNhan.MaBN));

            CreateMap<Camino.Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, PhauThuatThuThuatThongTinBenhNhanViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.BenhNhanId))
                .ForMember(x => x.MaTN, o => o.MapFrom(y => y.MaYeuCauTiepNhan))
                .ForMember(x => x.MaBN, o => o.MapFrom(y => y.BenhNhan.MaBN))
                .ForMember(x => x.HoTen, o => o.MapFrom(y => y.HoTen))
                .ForMember(x => x.GioiTinh, o => o.MapFrom(y => y.GioiTinh == null ? null : y.GioiTinh.GetDescription()))
                .ForMember(x => x.Tuoi, o => o.MapFrom(y => y.NamSinh == null ? 0 : DateTime.Now.Year - y.NamSinh.Value))
                .ForMember(x => x.SDT, o => o.MapFrom(y => y.SoDienThoaiDisplay))
                .ForMember(x => x.DanToc, o => o.MapFrom(y => y.DanToc.Ten))
                .ForMember(x => x.DiaChi, o => o.MapFrom(y => y.DiaChiDayDu))
                .ForMember(x => x.NgheNghiep, o => o.MapFrom(y => y.NgheNghiep.Ten))
                .ForMember(x => x.TuyenKham, o => o.MapFrom(y => y.LyDoVaoVien == null ? null : y.LyDoVaoVien.GetDescription()))
                .ForMember(x => x.MucHuong, o => o.MapFrom(y => y.BHYTMucHuong))
                .ForMember(x => x.LyDoTN, o => o.MapFrom(y => y.LyDoTiepNhan.Ten))
                .ForMember(x => x.NgayTN, o => o.MapFrom(y => y.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH()))
                .ForMember(x => x.CoBHYT, o => o.MapFrom(y => y.CoBHYT ?? false))
                .ForMember(x => x.YeuCauTiepNhanId, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.KetQuaSinhHieus, o => o.MapFrom(y => y.KetQuaSinhHieus))
                .ForMember(x => x.BHYTMaSoThe, o => o.MapFrom(y => y.BHYTMaSoThe))
                .ForMember(x => x.BHYTNgayHieuLuc, o => o.MapFrom(y => y.BHYTNgayHieuLuc))
                .ForMember(x => x.BHYTNgayHetHan, o => o.MapFrom(y => y.BHYTNgayHetHan));

            #region Xuat Excel
            CreateMap<LichSuPhauThuatThuThuatGridVo, LichSuPhauThuatThuThuatExportExcel>().IgnoreAllNonExisting();
            #endregion
        }
    }
}
