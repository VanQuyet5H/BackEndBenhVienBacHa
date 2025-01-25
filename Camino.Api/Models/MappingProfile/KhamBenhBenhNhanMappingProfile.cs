using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.BenhNhans;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhBenhNhanMappingProfile: Profile
    {
        public KhamBenhBenhNhanMappingProfile()
        {
            CreateMap<BenhNhan, KhamBenhBenhNhanViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.KhamBenhBenhNhanDiUngThuocs, o => o.MapFrom(y => y.BenhNhanDiUngThuocs))
                .ForMember(x => x.KhamBenhBenhNhanTienSuBenhs, o => o.MapFrom(y => y.BenhNhanTienSuBenhs))
                .ForMember(x => x.YeuCauTiepNhans, o => o.MapFrom(y => y.YeuCauTiepNhans));
            CreateMap<KhamBenhBenhNhanViewModel, BenhNhan>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhanDiUngThuocs, o => o.Ignore())
                .ForMember(x => x.BenhNhanTienSuBenhs, o => o.Ignore())
                .ForMember(x => x.YeuCauTiepNhans, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateTienSuBenh(s,d);
                    AddOrUpdateDiUngThuoc(s, d);
                });

            CreateMap<BenhNhanDiUngThuoc, KhamBenhBenhNhanDiUngThuocViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.MapFrom(y => y.BenhNhan));
            CreateMap<KhamBenhBenhNhanDiUngThuocViewModel, BenhNhanDiUngThuoc>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.Ignore());

            CreateMap<BenhNhanTienSuBenh, KhamBenhBenhNhanTienSuBenhViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.MapFrom(y => y.BenhNhan));
            CreateMap<KhamBenhBenhNhanTienSuBenhViewModel, BenhNhanTienSuBenh>().IgnoreAllNonExisting()
                .ForMember(x => x.BenhNhan, o => o.Ignore());
        }

        private void AddOrUpdateTienSuBenh(KhamBenhBenhNhanViewModel viewModel, BenhNhan model)
        {
            foreach (var item in viewModel.KhamBenhBenhNhanTienSuBenhs)
            {
                if (item.Id == 0)
                {
                    var tienSuBenhEntity = new BenhNhanTienSuBenh();
                    model.BenhNhanTienSuBenhs.Add(item.ToEntity(tienSuBenhEntity));
                }
                else
                {
                    //var result = model.BenhNhanTienSuBenhs.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);

                    var result = model.BenhNhanTienSuBenhs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.BenhNhanTienSuBenhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.KhamBenhBenhNhanTienSuBenhs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }

        private void AddOrUpdateDiUngThuoc(KhamBenhBenhNhanViewModel viewModel, BenhNhan model)
        {
            foreach (var item in viewModel.KhamBenhBenhNhanDiUngThuocs)
            {
                if (item.Id == 0)
                {
                    var diUngEntity = new BenhNhanDiUngThuoc();
                    model.BenhNhanDiUngThuocs.Add(item.ToEntity(diUngEntity));
                }
                else
                {
                    //var result = model.BenhNhanDiUngThuocs.Single(p => p.Id == item.Id);
                    //result = item.ToEntity(result);

                    var result = model.BenhNhanDiUngThuocs.FirstOrDefault(p => p.Id == item.Id);
                    if (result != null)
                    {
                        result = item.ToEntity(result);
                    }
                }
            }
            foreach (var item in model.BenhNhanDiUngThuocs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.KhamBenhBenhNhanDiUngThuocs.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }

                }

            }
        }
    }
}
