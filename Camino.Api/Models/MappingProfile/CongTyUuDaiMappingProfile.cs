using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.CongTyUuDais;
using Camino.Core.Domain.ValueObject.CongTyUuDais;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class CongTyUuDaiMappingProfile : Profile
    {
        public CongTyUuDaiMappingProfile()
        {
            CreateMap<CongTyUuDai, CongTyUuDaiViewModel>();
            CreateMap<CongTyUuDaiViewModel, CongTyUuDai>();

            CreateMap<CongTyUuDaiGridVo, CongTyUuDaiExportExcel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.SuDung = s.IsDisabled != true ? "Đang sử dụng" : "Ngừng sử dụng";
                });
        }
    }
}
