using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Api.Models.MappingProfile
{
    public class CanLamSangMappingProfile : Profile
    {
        public CanLamSangMappingProfile()
        {
            CreateMap<KetQuaCDHATDCNTimKiemGridVo, CanLamSangExportExcel>().IgnoreAllNonExisting();

            CreateMap<KetQuaCDHATDCNLichSuGridVo, LichSuCanLamSangExportExcel>().IgnoreAllNonExisting();
        }
    }
}
