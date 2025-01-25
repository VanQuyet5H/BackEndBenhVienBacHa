using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LoaiPhongBenh.LoaiPhongBenhNoiTru;
using Camino.Core.Domain.Entities.LoaiPhongBenh.LoaiPhongBenhNoiTrus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LoaiPhongBenh.LoaiPhongBenhNoiTru;

namespace Camino.Api.Models.MappingProfile
{
    public class LoaiPhongBenhNoiTruMappingProfile : Profile
    {
        public LoaiPhongBenhNoiTruMappingProfile()
        {
            CreateMap<LoaiPhongBenhNoiTru, LoaiPhongBenhNoiTruViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<LoaiPhongBenhNoiTruViewModel, LoaiPhongBenhNoiTru>()
                .IgnoreAllNonExisting();
            CreateMap<LoaiPhongBenhNoiTruGridVo, LoaiPhongBenhNoiTruExportExcel>()
                .IgnoreAllNonExisting();
        }
    }
}
