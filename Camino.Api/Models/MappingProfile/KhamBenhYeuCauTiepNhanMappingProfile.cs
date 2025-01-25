using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhYeuCauTiepNhanMappingProfile : Profile
    {
        public KhamBenhYeuCauTiepNhanMappingProfile()
        {
            CreateMap<KhamBenhYeuCauTiepNhanViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.KSKKetLuanPhanLoaiSucKhoe = d.PhanLoaiSucKhoeId.GetDescription();
                });
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, KhamBenhYeuCauTiepNhanViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.PhongBenhVienHangDois, o => o.MapFrom(y => y.PhongBenhVienHangDois))
                .ForMember(x => x.YeuCauTiepNhanDichVuKyThuats, o => o.MapFrom(y => y.YeuCauDichVuKyThuats))
                //.ForMember(x => x.YeuCauTiepNhanDichVuKhamBenhs, o => o.MapFrom(y => y.YeuCauKhamBenhs))
                .ForMember(x => x.YeuCauTiepNhanLichSuTrangThais, o => o.MapFrom(y => y.YeuCauTiepNhanLichSuTrangThais))
                .ForMember(x => x.KetQuaSinhHieus, o => o.MapFrom(y => y.KetQuaSinhHieus))

                .AfterMap((s, d) =>
                    {
                        d.TenCongTy = s.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten;
                        d.HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoeId;
                        d.PhanLoaiSucKhoeId = string.IsNullOrEmpty(d.KSKKetLuanPhanLoaiSucKhoe) ? (Enums.PhanLoaiSucKhoe?)null : EnumHelper.GetValueFromDescription<Enums.PhanLoaiSucKhoe>(d.KSKKetLuanPhanLoaiSucKhoe);
                        d.KSKPhanLoaiTheLuc = s.KSKPhanLoaiTheLuc == null && s.KetQuaSinhHieus.Any() && s.KetQuaSinhHieus.LastOrDefault().KSKPhanLoaiTheLuc != null ? (Enums.PhanLoaiSucKhoe)(s.KetQuaSinhHieus.LastOrDefault().KSKPhanLoaiTheLuc) : s.KSKPhanLoaiTheLuc;

                        //BVHD-3574
                        d.TrangThaiTiepNhan = s.TrangThaiYeuCauTiepNhan;

                        //BVHD-3941
                        d.CoBaoHiemTuNhan = s.CoBHTN;

                        //BVHD-3960
                        d.TenHinhThucDen = s.HinhThucDen?.Ten;
                        d.TenNoiGioiThieu = s.NoiGioiThieu?.Ten;
                    });
        }
    }
}
