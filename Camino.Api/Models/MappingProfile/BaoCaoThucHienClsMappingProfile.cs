using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCaoThucHienCls;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class BaoCaoThucHienClsMappingProfile : Profile
    {
        public BaoCaoThucHienClsMappingProfile()
        {
            CreateMap<BaoCaoThucHienClsVo, BaoCaoThucHienClsExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.GioiTinh, o => o.MapFrom(p => p.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "Nam" : "Nữ"));
        }
    }
}
