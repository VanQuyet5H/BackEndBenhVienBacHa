using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.KhoKSNKs;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetHoanTraKSNKMappingProfile : Profile
    {
        public DuyetHoanTraKSNKMappingProfile()
        {
            CreateMap<DanhSachHoanTraKSNKVo, DuyetHoanTraKSNKExportExcel>();
            CreateMap<DanhSachHoanTraKSNKChiTietVo, DuyetHoanTraKSNKExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
