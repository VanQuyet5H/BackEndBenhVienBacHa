using AutoMapper;
using Camino.Api.Models.DoiTuongUuDais;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DoiTuongUuDais;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DoiTuongUuDaiMappingProfile : Profile
    {
        public DoiTuongUuDaiMappingProfile()
        {
            CreateMap<DoiTuongUuDai, DoiTuongUuDaiViewModel>();
            CreateMap<DoiTuongUuDaiViewModel, DoiTuongUuDai>();

            CreateMap<DoiTuongUuDaiDichVuKyThuatBenhVien, DoiTuongUuDaiDichVuKyThuatBenhVienViewModel>();
            CreateMap<DoiTuongUuDaiDichVuKyThuatBenhVienViewModel, DoiTuongUuDaiDichVuKyThuatBenhVien>();

            CreateMap<DoiTuongUuDaiGridVo, DoiTuongUuDaiDichVuKyThuatExportExcel>().IgnoreAllNonExisting();
            CreateMap<DoiTuongUuDaiChildGridVo, DoiTuongUuDaiDichVuKyThuatExportExcelChild>().IgnoreAllNonExisting();

            CreateMap<DoiTuongUuDaiGridVo, DoiTuongUuDaiDichVuKhamBenhExportExcel>().IgnoreAllNonExisting();
            CreateMap<DoiTuongUuDaiChildGridVo, DoiTuongUuDaiDichVuKhamBenhExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
