using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LoaiGiaDichVu;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LoaiGiaDichVus;

namespace Camino.Api.Models.MappingProfile
{
    public class NhomGiaDichVuKhamBenhBenhVienMappingProfile : Profile
    {
        public NhomGiaDichVuKhamBenhBenhVienMappingProfile()
        {
            CreateMap<LoaiGiaDichVuGridVo, LoaiGiaDichVuExportExcel>().IgnoreAllNonExisting();

            CreateMap<LoaiGiaDichVuGridVo, LoaiGiaDichVuViewModel>().IgnoreAllNonExisting();
            CreateMap<LoaiGiaDichVuViewModel, LoaiGiaDichVuGridVo>().IgnoreAllNonExisting();

            CreateMap<LoaiGiaDichVuGridVo, NhomGiaDichVuKhamBenhBenhVien>().IgnoreAllNonExisting();
            CreateMap<LoaiGiaDichVuGridVo, NhomGiaDichVuKyThuatBenhVien>().IgnoreAllNonExisting();
            CreateMap<LoaiGiaDichVuGridVo, NhomGiaDichVuGiuongBenhVien>().IgnoreAllNonExisting();
        }
    }
}
