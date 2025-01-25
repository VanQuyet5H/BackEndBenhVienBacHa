using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.DuyetTraVatTuTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class DuyetTraVatTuTuBnMappingProfile : Profile
    {
        public DuyetTraVatTuTuBnMappingProfile()
        {
            CreateMap<DuyetTraVatTuTuBnVo, DuyetPhieuHoanTraVatTuTuBnExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.TinhTrangDisplay = s.TinhTrang == true
                        ? "Đã duyệt"
                        : "Chờ duyệt";
                });
            CreateMap<DuyetTraVatTuChiTietTuBnVo, DuyetPhieuHoanTraVatTuTuBnExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
