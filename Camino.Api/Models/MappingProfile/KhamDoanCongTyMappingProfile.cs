using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoanCongTies;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamDoanCongTyMappingProfile : Profile
    {
        public KhamDoanCongTyMappingProfile()
        {
            CreateMap<CongTyKhamSucKhoe, KhamDoanCongTyViewModel>().IgnoreAllNonExisting();
            CreateMap<KhamDoanCongTyViewModel, CongTyKhamSucKhoe>().IgnoreAllNonExisting();
            #region map export excel lịch sử tiếp nhận khám sức khỏe đoàn
            CreateMap<LichSuTiepNhanKhamSucKhoeDoanGridVo, LichSuTiepNhanKhamSucKhoeDoanExportExcel>();
            CreateMap<LichSuTiepNhanKhamSucKhoeDoanExportExcel, LichSuTiepNhanKhamSucKhoeDoanGridVo>();
            #endregion
        }
    }
}
