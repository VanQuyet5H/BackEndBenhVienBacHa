using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.KhoVatTus;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetVatTuMappingProfile : Profile
    {
        public DuyetVatTuMappingProfile()
        {
            CreateMap<DanhSachDuyetKhoVatTuVo, DuyetVatTuExportExcel>();   
            CreateMap<DanhSachDuyetKhoVatTuChiTietVo, DuyetVatTuExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
