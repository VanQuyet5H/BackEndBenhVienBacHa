using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.CauhinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class CauHinhHeSoTheoThoiGianHoaHongViewModelMappingProfile : Profile
    {
        public CauHinhHeSoTheoThoiGianHoaHongViewModelMappingProfile()
        {
            CreateMap<CauHinhHeSoTheoThoiGianHoaHong, CauHinhHeSoTheoThoiGianHoaHongViewModel>();

            CreateMap<CauHinhHeSoTheoThoiGianHoaHongViewModel, CauHinhHeSoTheoThoiGianHoaHong>();

            CreateMap<CauHinhHoaHongDichVuVo, CauHinhHoaHongDichVuExportExcel>().IgnoreAllNonExisting();
            CreateMap<CauHinhNoiGioiThieuDPVTYTVo, CauHinhNoiGioiThieuDPVTYTExportExcel>().IgnoreAllNonExisting();
            CreateMap<CauHinhNoiGioiThieuDichVu, CauHinhNoiGioiThieuDichVuExportExcel>().IgnoreAllNonExisting();
            CreateMap<CauHinhHoaHongDuocPhamVTYTVo, CauHinhHoaHongDPVTYTExportExcel>().IgnoreAllNonExisting();
        }

    }
}
