using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhChiDinhGoiDichVuMappingProfile: Profile
    {
        public KhamBenhChiDinhGoiDichVuMappingProfile()
        {
            CreateMap<ChiDinhNhomGoiDichVuThuongDungViewModel, YeuCauThemGoiDichVuThuongDungVo>()
                .IgnoreAllNonExisting()
                .ForMember(a => a.GoiDichVuIds, o => o.MapFrom(b => b.GoiDichVuIds))
                .ForMember(x => x.DichVuKhongThems, o => o.MapFrom(a => a.DichVuKhongThems));

            CreateMap<ChiDinhGoiDichVuTheoBenhNhanViewModel, ChiDinhGoiDichVuTheoBenhNhanVo>().IgnoreAllNonExisting()
                .ForMember(a => a.DichVus, o => o.MapFrom(a => a.DichVus))
                .ForMember(a => a.DichVuKhongThems, o => o.MapFrom(x => x.DichVuKhongThems));

            CreateMap<ChiDinhNhomGoiDichVuThuongDungTaoYCTNViewModel, YeuCauThemGoiDichVuThuongDungVo>()
                .IgnoreAllNonExisting()
                .ForMember(a => a.GoiDichVuIds, o => o.MapFrom(b => b.GoiDichVuIds))
                .ForMember(x => x.DichVuKhongThems, o => o.MapFrom(a => a.DichVuKhongThems));

            CreateMap<ChiTietGoiDichVuChiDinhTheoBenhNhanViewModel, ChiTietGoiDichVuChiDinhTheoBenhNhanVo>()
                .IgnoreAllNonExisting();

            CreateMap<ChiDinhGoiDichVuThuongDungDichVuLoiViewModel, ChiDinhGoiDichVuThuongDungDichVuLoiVo>()
                .IgnoreAllNonExisting();

            CreateMap<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel, ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>()
                .IgnoreAllNonExisting();
        }
    }
}
