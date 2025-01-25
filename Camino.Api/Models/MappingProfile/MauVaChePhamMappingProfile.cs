using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.MauVaChePham;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.MauVaChePham;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class MauVaChePhamMappingProfile : Profile
    {
        public MauVaChePhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.MauVaChePhams.MauVaChePham, MauVaChePhamViewModel>()
                .AfterMap((c, d) =>
                {
                   
                    d.TenPhanLoaiMau = c.PhanLoaiMau.GetDescription();
                 
                }); ;
            CreateMap<MauVaChePhamViewModel, Core.Domain.Entities.MauVaChePhams.MauVaChePham>();
            CreateMap<MauVaChePhamGridVo, ExportMauVaChePhamExportExcel>()
           .IgnoreAllNonExisting();
        }
    }
}
