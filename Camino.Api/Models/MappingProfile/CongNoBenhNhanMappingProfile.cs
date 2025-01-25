using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.CongNoBenhNhan;
using Camino.Core.Domain.ValueObject.CongNoBenhNhans;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class CongNoBenhNhanMappingProfile : Profile
    {
        public CongNoBenhNhanMappingProfile()
        {
            //CreateMap<Core.Domain.Entities.ChucVus.ChucVu, CongNoBenhNhanViewModel>().IgnoreAllNonExisting();
            CreateMap<CongNoBenhNhanViewModel, ThuTienTraNoVo>().IgnoreAllNonExisting();

            CreateMap<CongNoBenhNhanGridVo, CongNoBenhNhanExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.GioiTinh, o => o.MapFrom(p => p.GioiTinh != null ? p.GioiTinh.GetDescription() : ""));
        }
    }
}
