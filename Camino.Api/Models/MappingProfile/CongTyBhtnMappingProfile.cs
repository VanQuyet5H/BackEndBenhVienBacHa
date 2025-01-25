using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.CongTyBhtns;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.ValueObject.CongTyBhtns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class CongTyBhtnMappingProfile : Profile
    {
        public CongTyBhtnMappingProfile()
        {
            CreateMap<CongTyBaoHiemTuNhan, CongTyBhtnViewModel>().IgnoreAllNonExisting();
            CreateMap<CongTyBhtnViewModel, CongTyBaoHiemTuNhan>().IgnoreAllNonExisting();

            CreateMap<CongTyBhtnGridVo, CongTyBaoHiemTuNhanExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.HinhThucBaoHiem = s.HinhThucBaoHiemDisplay;
                    d.PhamViBaoHiem = s.PhamViBaoHiemDisplay;
                    d.SoDienThoai = s.SoDienThoai;
                });
        }
    }
}