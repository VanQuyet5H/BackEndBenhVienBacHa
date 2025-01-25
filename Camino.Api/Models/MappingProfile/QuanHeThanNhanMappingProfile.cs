using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.QuanHeThanNhan;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.QuanHeThanNhan;

namespace Camino.Api.Models.MappingProfile
{
    public class QuanHeThanNhanMappingProfile : Profile
    {
        public QuanHeThanNhanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan,
                QuanHeThanNhanViewModel>().IgnoreAllNonExisting();

            CreateMap<QuanHeThanNhanViewModel,
                Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan>().IgnoreAllNonExisting();

            CreateMap<QuanHeThanNhanGridVo,
                QuanHeThanNhanExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.HieuLuc = s.IsDisabled != true ? "Đang sử dụng" : "Ngừng sử dụng";
                });
        }
    }
}
