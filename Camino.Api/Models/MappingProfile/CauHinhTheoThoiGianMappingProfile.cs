using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Cauhinh;
using Camino.Core.Domain.Entities.CauHinhs;

namespace Camino.Api.Models.MappingProfile
{
    public class CauHinhTheoThoiGianMappingProfile : Profile
    {
        public CauHinhTheoThoiGianMappingProfile()
        {
            CreateMap<CauHinhTheoThoiGian, CauhinhViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.CauHinhTheoThoiGianChiTiets, o => o.MapFrom(y => y.CauHinhTheoThoiGianChiTiets));
            CreateMap<CauhinhViewModel, CauHinhTheoThoiGian>()
                .ForMember(x => x.CauHinhTheoThoiGianChiTiets, o => o.Ignore())
                .AfterMap((source, destination) =>
                {
                    AddOrUpdateCauHinhChiTiet(source, destination);
                });
        }

        private void AddOrUpdateCauHinhChiTiet(CauhinhViewModel source, CauHinhTheoThoiGian destination)
        {
            foreach (var model in source.CauHinhTheoThoiGianChiTiets)
            {
                if (model.Id == 0)
                {
                    var newEntity = new CauHinhTheoThoiGianChiTiet();
                    destination.CauHinhTheoThoiGianChiTiets.Add(model.ToEntity(newEntity));
                }
                else
                {
                    var result = destination.CauHinhTheoThoiGianChiTiets.Single(c => 
                        c.Id == model.Id);
                    result = model.ToEntity(result);
                    result.CauHinhTheoThoiGian = destination;
                }
            }

            foreach (var model in destination.CauHinhTheoThoiGianChiTiets)
            {
                if (model.Id != 0)
                {
                    var countModel = source.CauHinhTheoThoiGianChiTiets.Where(x => 
                        x.Id == model.Id).ToList();

                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;
                    }
                }
            }
        }
    }
}
