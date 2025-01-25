using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain.ValueObject.TiemChungs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class TiemChungChiDinhGoiDichVuMappingProfile : Profile
    {
        public TiemChungChiDinhGoiDichVuMappingProfile()
        {
            //CreateMap<TiemChungChiDinhNhomGoiDichVuThuongDungViewModel, TiemChungYeuCauThemGoiDichVuThuongDungVo>()
            //    .IgnoreAllNonExisting()
            //    .ForMember(a => a.GoiDichVuIds, o => o.MapFrom(b => b.GoiDichVuIds))
            //    .ForMember(x => x.DichVuKhongThems, o => o.MapFrom(a => a.DichVuKhongThems));

            CreateMap<TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel, TiemChungChiDinhGoiDichVuTheoBenhNhanVo>().IgnoreAllNonExisting()
                .ForMember(a => a.DichVus, o => o.MapFrom(a => a.DichVus))
                .ForMember(a => a.DichVuKhongThems, o => o.MapFrom(x => x.DichVuKhongThems));

            //CreateMap<TiemChungChiDinhNhomGoiDichVuThuongDungTaoYCTNViewModel, TiemChungYeuCauThemGoiDichVuThuongDungVo>()
            //    .IgnoreAllNonExisting()
            //    .ForMember(a => a.GoiDichVuIds, o => o.MapFrom(b => b.GoiDichVuIds))
            //    .ForMember(x => x.DichVuKhongThems, o => o.MapFrom(a => a.DichVuKhongThems));

            CreateMap<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanViewModel, TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanVo>()
                .IgnoreAllNonExisting();

            //CreateMap<TiemChungChiDinhGoiDichVuThuongDungDichVuLoiViewModel, TiemChungChiDinhGoiDichVuThuongDungDichVuLoiVo>()
            //    .IgnoreAllNonExisting();

            CreateMap<TiemChungChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel, TiemChungChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>()
                .IgnoreAllNonExisting();
        }
    }
}
