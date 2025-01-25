using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhNhans;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class BenhNhanMappingProfile : Profile
    {
        public BenhNhanMappingProfile()
        {
            CreateMap<BenhNhan, BenhNhanViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<BenhNhanViewModel, BenhNhan>()
                .IgnoreAllNonExisting();


            CreateMap<BenhNhanGridVo, BenhNhanExportExcel>()
                .IgnoreAllNonExisting();

            #region Tiêm chủng
            CreateMap<BenhNhan, TiemChungBenhNhanViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<TiemChungBenhNhanViewModel, BenhNhan>()
                .IgnoreAllNonExisting();

            #endregion
        }
    }
}
