using AutoMapper;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.ThietBiXetNghiems;

namespace Camino.Api.Models.MappingProfile
{
    public class ThietBiXetNghiemMappingProfile : Profile
    {
        public ThietBiXetNghiemMappingProfile()
        {
            CreateMap<ThietBiXetNghiemGridVo, ThietBiXetNghiemExportExcel>()
                .AfterMap((source, dest) =>
                {
                    dest.TinhTrang = source.TinhTrang == Enums.EnumConnectionStatus.Open ? "Mở" : "Đóng";
                    dest.HieuLuc = source.HieuLuc ? "Đang sử dụng" : "Ngừng sử dụng";
                });
        }
    }
}
