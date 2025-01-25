using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.ChuanDoanHinhAnhViewModelCategory;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Camino.Core.Domain.ValueObject.ChuanDoanHinhAnh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class ChuanDoanHinhAnhMappingProfile : Profile
    {
        public ChuanDoanHinhAnhMappingProfile()
        {
            CreateMap<ChuanDoanHinhAnh, ChuanDoanHinhAnhViewModel>()
                    .AfterMap((source, dest) =>
                    {
                        dest.LoaiChuanDoanHinhAnhDisplay = source.LoaiChuanDoanHinhAnh.GetDescription();
                    });

            CreateMap<ChuanDoanHinhAnhViewModel, ChuanDoanHinhAnh>().IgnoreAllNonExisting();

            CreateMap<ChuanDoanHinhAnhGridVo, ChanDoanHinhAnhExportExcel>()
                .AfterMap((source, dest) =>
                {
                    dest.HieuLuc = source.HieuLuc == false ? "Không" : "Có";
                });
        }
    }
}
