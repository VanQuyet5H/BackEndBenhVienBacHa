using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien.LoaiBenhVien;
using Camino.Core.Domain.Entities.BenhVien.LoaiBenhViens;
using Camino.Core.Domain.ValueObject.BenhVien.LoaiBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class LoaiBenhVienMappingProfile : Profile
    {
        public LoaiBenhVienMappingProfile()
        {
            CreateMap<LoaiBenhVien, LoaiBenhVienViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<LoaiBenhVienViewModel, LoaiBenhVien>()
                .IgnoreAllNonExisting();

            CreateMap<LoaiBenhVienGirdVo, LoaiBenhVienExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}
