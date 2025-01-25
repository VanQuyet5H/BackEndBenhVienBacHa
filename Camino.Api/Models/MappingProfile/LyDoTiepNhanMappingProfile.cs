using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LyDoTiepNhan;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.LyDoTiepNhan;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class LyDoTiepNhanMappingProfile : Profile
    {
        public LyDoTiepNhanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan, LyDoTiepNhanViewModel>().IgnoreAllNonExisting()
                 //.ForMember(x => x.LyDoTiepNhans, o => o.MapFrom(entity => entity.LyDoTiepNhans))
                    .ForMember(x => x.LyDoTiepNhans, o => o.Ignore())
                    .AfterMap((entity, viewmodel) =>
                    {
                        if (entity.LyDoTiepNhanCha != null)
                        {
                            if (entity.CapNhom == 1)
                            {
                                viewmodel.TenCha = entity.LyDoTiepNhanCha.Ten;
                            }
                            if (entity.CapNhom > 1 && entity.LyDoTiepNhanCha.LyDoTiepNhans.Count() > 0)
                            {
                                viewmodel.TenCha = entity.LyDoTiepNhanCha.LyDoTiepNhans.Where(p => p.CapNhom == entity.CapNhom).FirstOrDefault().Ten;
                            }
                        }
                        else
                        {
                            viewmodel.TenCha = entity.Ten;
                        }

                    });

            CreateMap<LyDoTiepNhanViewModel, Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>().IgnoreAllNonExisting()
                 .ForMember(x => x.LyDoTiepNhans, o => o.Ignore())
                 .AfterMap((viewModel, entity) =>
                 {
                     //AddOrUpdateLyDoTiepNhan(viewModel, entity);
                 });
        }

        //private void AddOrUpdateLyDoTiepNhan(LyDoTiepNhanViewModel viewModel, Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan entity)
        //{
        //    foreach (var item in viewModel.LyDoTiepNhans)
        //    {
        //        if (item.Id == 0)
        //        {
        //            var newEntity = new Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan();
        //            entity.LyDoTiepNhans.Add(item.ToEntity(newEntity));
        //        }
        //        else
        //        {
        //            var result = entity.LyDoTiepNhans.Single(c => c.Id == item.Id);
        //            result = item.ToEntity(result);
        //        }
        //    }
        //    foreach (var item in entity.LyDoTiepNhans)
        //    {
        //        if (item.Id != 0)
        //        {
        //            var countModel = viewModel.LyDoTiepNhans.Where(x => x.Id == item.Id).ToList();
        //            if (countModel.Count == 0)
        //            {
        //                item.WillDelete = true;
        //            }

        //        }
        //    }
        //}
    }
}
