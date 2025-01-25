using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Cauhinh;
using Camino.Api.Models.ChucDanh;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class CauHinhNguoiDuyetTheoNhomDichVuMappingProfile : Profile
    {
        public CauHinhNguoiDuyetTheoNhomDichVuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu, CauHinhNguoiDuyetTheoNhomDichVuViewModel>()
                .AfterMap((s, d) =>
                {
                    d.TenNhanVienDuyet = s.NhanVien?.User?.HoTen;
                    d.TenNhomDichVuBenhVien = s.NhanVien?.User?.HoTen;

                });
            CreateMap<CauHinhNguoiDuyetTheoNhomDichVuViewModel, Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu>();
            CreateMap<CauHinhNguoiDuyetTheoNhomDVGridVo, CauHinhNguoiDuyetTheoNhomDichVuExportExcel>().IgnoreAllNonExisting()
                   ;

        }

    }
}
