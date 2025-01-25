using AutoMapper;
using Camino.Api.Models.NhaSanXuat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhaSanXuatTheoQuocGias;
using Camino.Services.NhaSanXuat;
using Camino.Core.Domain.ValueObject.NhaSanXuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Api.Extensions;

namespace Camino.Api.Models.MappingProfile
{
    public class NhaSanXuatMappingProfile : Profile
    {
        public NhaSanXuatMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhaSanXuats.NhaSanXuat, NhaSanXuatViewModel>()
                 .ForMember(x => x.NhaSanXuatTheoQuocGias, o => o.MapFrom(s => s.NhaSanXuatTheoQuocGias))
                 .AfterMap((c, d) =>
                 {
                 }); 

            CreateMap<NhaSanXuatViewModel, Core.Domain.Entities.NhaSanXuats.NhaSanXuat>()
                .ForMember(x => x.NhaSanXuatTheoQuocGias, o => o.Ignore())
                .AfterMap((c, d) =>
                {               //(mr, m)
                    foreach (var model in d.NhaSanXuatTheoQuocGias)
                    {
                        if (c.NhaSanXuatTheoQuocGias.All(x => x.Id != model.Id))
                        {
                            model.WillDelete = true;
                        }

                    }
                    foreach (var model in c.NhaSanXuatTheoQuocGias)
                    {
                        if (model.Id == 0)
                        {
                            var entity = new NhaSanXuatTheoQuocGia
                            {
                                NhaSanXuatId = model.NhaSanXuatId,
                                QuocGiaId = model.QuocGiaId,
                                DiaChi = model.DiaChi
                            };
                            d.NhaSanXuatTheoQuocGias.Add(entity);
                        }
                        else
                        {
                            if (d.NhaSanXuatTheoQuocGias.Any())
                            {
                                //var updatedSubItems = c.NhaSanXuatTheoQuocGias
                                //    .Where(p => p.Id == model.Id).ToList();
                                var updatedSubItems = c.NhaSanXuatTheoQuocGias
                                    .Where(sir => d.NhaSanXuatTheoQuocGias.Any(si => si.Id == sir.Id)).ToList();
                                var entity = new NhaSanXuatTheoQuocGia();
                                foreach (var  sir in updatedSubItems)
                                {
                                    entity = d.NhaSanXuatTheoQuocGias.SingleOrDefault(si => si.Id == sir.Id);
                                    entity.DiaChi = sir.DiaChi;
                                    entity.QuocGiaId = sir.QuocGiaId;
                                }

                            }
                        }
                    }
                   
                }); 
            CreateMap<NhaSanXuatGridVo, ExportNhaSanXuatExcel>()
           .IgnoreAllNonExisting();

        }
    }
}
