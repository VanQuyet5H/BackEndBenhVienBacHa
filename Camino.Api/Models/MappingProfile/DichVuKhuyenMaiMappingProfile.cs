using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKhuyenMai;
using Camino.Core.Domain.ValueObject.DichVuKhuyenMai;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuKhuyenMaiMappingProfile : Profile
    {
        public DichVuKhuyenMaiMappingProfile()
        {
            CreateMap<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel, ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>()
                .IgnoreAllNonExisting();

            CreateMap<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo, ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel>()
                .IgnoreAllNonExisting();


            CreateMap<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanViewModel, ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo>()
                .IgnoreAllNonExisting();

            CreateMap<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo, ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanViewModel>()
                .IgnoreAllNonExisting();

            CreateMap<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiViewModel, ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo>()
                .IgnoreAllNonExisting();

            CreateMap<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo, ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiViewModel>()
                .IgnoreAllNonExisting();
        }

    }
}
