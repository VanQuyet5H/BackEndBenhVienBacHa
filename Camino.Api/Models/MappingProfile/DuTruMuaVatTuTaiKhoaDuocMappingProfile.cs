using AutoMapper;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.DuTruVatTus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Api.Models.DutruMuaVatTuTaiKhoaDuoc.DutruMuaVatTuTaiKhoaDuoc;

namespace Camino.Api.Models.MappingProfile
{
    public class DuTruMuaVatTuTaiKhoaDuocMappingProfile : Profile
    {
        public DuTruMuaVatTuTaiKhoaDuocMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTu, DuTruMuaVatTuTaiKhoaDuocDuyet>();
            CreateMap<DuTruMuaVatTuTaiKhoaDuocDuyet, Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTu>()
                 .ForMember(x => x.DuTruMuaVatTuChiTiets, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateChiTiet(d, s);
                });

            CreateMap<DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet, Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTuChiTiet>();

            CreateMap<Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTuChiTiet, DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet>();

            CreateMap<DuTruMuaVatTuTheoKhoa, DuTruMuaVatTuTaiKhoaDuocDuyet>();
            CreateMap<DuTruMuaVatTuTaiKhoaDuocDuyet, DuTruMuaVatTuTheoKhoa>()
                 .ForMember(x => x.DuTruMuaVatTuTheoKhoaChiTiets, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateKhoaChiTiet(d, s);
                });

            CreateMap<DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet, DuTruMuaVatTuTheoKhoaChiTiet>();

            CreateMap<DuTruMuaVatTuTheoKhoaChiTiet, DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet>();
            #region goi create
            CreateMap<DuTruMuaVatTuKhoDuoc, DuTruMuaVatTuTaiKhoaDuocViewModel>();
            CreateMap<DuTruMuaVatTuTaiKhoaDuocViewModel, DuTruMuaVatTuKhoDuoc>()
                //.ForMember(x => x.DuTruMuaVatTuKhoDuocChiTiets, o => o.Ignore())
                //.ForMember(x => x.DuTruMuaVatTuTheoKhoas, o => o.Ignore())
                //.ForMember(x => x.DuTruMuaVatTus, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    //AddOrUpdateChiTietGoi(d, s);
                });
            CreateMap<DuTruMuaVatTuKhoDuocChiTietVM, DuTruMuaVatTuKhoDuocChiTiet>();

            CreateMap<DuTruMuaVatTuKhoDuocChiTiet, DuTruMuaVatTuKhoDuocChiTietVM>();
            #endregion
        }
        private void AddOrUpdateChiTiet(DuTruMuaVatTuTaiKhoaDuocDuyet viewModel, Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTu entity)
        {
            foreach (var item in viewModel.ListDuTruMuaVatTuKhoDuocChiTiet)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTuChiTiet();
                    entity.DuTruMuaVatTuChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.DuTruMuaVatTuChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
        }
        private void AddOrUpdateKhoaChiTiet(DuTruMuaVatTuTaiKhoaDuocDuyet viewModel, DuTruMuaVatTuTheoKhoa entity)
        {
            foreach (var item in viewModel.ListDuTruMuaVatTuKhoDuocChiTiet)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTuTheoKhoaChiTiet();
                    entity.DuTruMuaVatTuTheoKhoaChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.DuTruMuaVatTuTheoKhoaChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
        }
        //private void AddOrUpdateChiTietGoi(DuTruMuaVatTuTaiKhoaDuocViewModel viewModel, Core.Domain.Entities.DuTruMuaVatTuKhoDuocs.DuTruMuaVatTuKhoDuoc entity)
        //{
        //    //if(viewModel.Id == 0)
        //    //{
        //    //    var newEntity = new Core.Domain.Entities.DuTruMuaVatTuKhoDuocs.DuTruMuaVatTuKhoDuoc();
        //    //    entity.AD

        //    //}
        //    foreach (var item in viewModel.DuTruMuaVatTuKhoDuocChiTiet)
        //    {
        //        if (item.Id == 0)
        //        {
        //            var newEntity = new Core.Domain.Entities.DuTruMuaVatTuKhoDuocs.DuTruMuaVatTuKhoDuocChiTiet();
        //            entity.DuTruMuaVatTuKhoDuocChiTiets.Add(item.ToEntity(newEntity));
        //        }
        //        else
        //        {
        //            var result = entity.DuTruMuaVatTuKhoDuocChiTiets.Single(c => c.Id == item.Id);
        //            result = item.ToEntity(result);
        //        }
        //    }
        //}
    }
}
