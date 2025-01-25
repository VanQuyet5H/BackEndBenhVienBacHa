using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.ChucDanh;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class ChucDanhMappingProfile :Profile
    {
        public ChucDanhMappingProfile()
        {
            CreateMap<Core.Domain.Entities.ChucDanhs.ChucDanh, ChucDanhViewModel>()
                .AfterMap((s, d) =>
            {
                d.TenNhomChucDanh = s.NhomChucDanh?.Ten;

            });
            CreateMap<ChucDanhViewModel, Core.Domain.Entities.ChucDanhs.ChucDanh>()
              .ForMember(d => d.NhomChucDanh, o => o.Ignore());

            CreateMap<ChucDanhGridVo, ChucDanhExportExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.IsDisabled, o => o.MapFrom(p => p.IsDisabled == null ? "" : (p.IsDisabled == false ? "Đang sử dụng" : "Ngừng sử dụng")));
        }
       
    }
}
