using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetTraThuocTuBnMappingProfile : Profile
    {
        public DuyetTraThuocTuBnMappingProfile()
        {
            CreateMap<DuyetTraThuocTuBnVo, DuyetPhieuHoanTraThuocTuBnExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TinhTrangDisplay = s.TinhTrang == true
                        ? "Đã duyệt"
                        : "Chờ duyệt"; });
            CreateMap<DuyetTraThuocChiTietTuBnVo, DuyetPhieuHoanTraThuocTuBnExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
