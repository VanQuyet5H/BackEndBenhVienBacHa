using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Khoa;
using Camino.Core.Domain.ValueObject.BenhVien.Khoa;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class KhoaMappingProfile : Profile
    {
        public KhoaMappingProfile()
        {
            CreateMap<Core.Domain.Entities.BenhVien.Khoas.Khoa
                , KhoaViewModel>()
                .IgnoreAllNonExisting();

            CreateMap<KhoaViewModel
                , Core.Domain.Entities.BenhVien.Khoas.Khoa>()
                .IgnoreAllNonExisting();

            CreateMap<KhoaGridVo,
                    ChuyenKhoaExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.HieuLuc = s.IsDisabled != true ? "Đang sử dụng" : "Ngừng sử dụng";
                });
        }
    }
}
