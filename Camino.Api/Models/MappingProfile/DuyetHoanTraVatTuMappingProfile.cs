using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.KhoVatTus;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetHoanTraVatTuMappingProfile : Profile
    {
        public DuyetHoanTraVatTuMappingProfile()
        {
            CreateMap<DanhSachHoanTraVatTuVo, DuyetHoanTraVatTuExportExcel>();
            CreateMap<DanhSachHoanTraVatTuChiTietVo, DuyetHoanTraVatTuExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
