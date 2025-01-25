using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien.CapQuanLyBenhVien;
using Camino.Core.Domain.Entities.BenhVien.CapQuanLyBenhViens;
using Camino.Core.Domain.ValueObject.BenhVien.CapQuanLyBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class CapQuanLyBenhVienMappingProfile : Profile
    {
        public CapQuanLyBenhVienMappingProfile()
        {
            CreateMap<CapQuanLyBenhVien, CapQuanLyBenhVienViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<CapQuanLyBenhVienViewModel, CapQuanLyBenhVien>()
                .IgnoreAllNonExisting();

            CreateMap<CapQuanLyBenhVienGridVo, CapQuanLyBenhVienExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}
