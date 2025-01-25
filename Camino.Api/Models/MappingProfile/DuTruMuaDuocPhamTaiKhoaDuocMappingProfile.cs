using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaDuocPhamTaiKhoaDuoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class DuTruMuaDuocPhamTaiKhoaDuocMappingProfile : Profile
    {
        public DuTruMuaDuocPhamTaiKhoaDuocMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham, DuTruMuaDuocPhamTaiKhoaDuocDuyet>();
            CreateMap<DuTruMuaDuocPhamTaiKhoaDuocDuyet, Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham>()
                 .ForMember(x => x.DuTruMuaDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateChiTiet(d, s);
                }); 

            CreateMap<DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet, Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPhamChiTiet>();

            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPhamChiTiet, DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet>();

            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas.DuTruMuaDuocPhamTheoKhoa, DuTruMuaDuocPhamTaiKhoaDuocDuyet>();
            CreateMap<DuTruMuaDuocPhamTaiKhoaDuocDuyet, Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas.DuTruMuaDuocPhamTheoKhoa>()
                 .ForMember(x => x.DuTruMuaDuocPhamTheoKhoaChiTiets, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateKhoaChiTiet(d, s);
                });

            CreateMap<DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet, Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas.DuTruMuaDuocPhamTheoKhoaChiTiet>();

            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas.DuTruMuaDuocPhamTheoKhoaChiTiet, DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet>();
            #region goi create
            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuoc, DuTruMuaDuocPhamTaiKhoaDuocViewModel>();
            CreateMap<DuTruMuaDuocPhamTaiKhoaDuocViewModel, Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuoc>()
                 //.ForMember(x => x.DuTruMuaDuocPhamKhoDuocChiTiets, o => o.Ignore())
                 //.ForMember(x => x.DuTruMuaDuocPhamTheoKhoas, o => o.Ignore())
                 //.ForMember(x => x.DuTruMuaDuocPhams, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    //AddOrUpdateChiTietGoi(d, s);
                });
            CreateMap<DuTruMuaDuocPhamKhoDuocChiTietVM, Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuocChiTiet>();

            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuocChiTiet, DuTruMuaDuocPhamKhoDuocChiTietVM> (); 
            #endregion
        }
        private void AddOrUpdateChiTiet(DuTruMuaDuocPhamTaiKhoaDuocDuyet viewModel, Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham entity)
        {
            foreach (var item in viewModel.ListDuTruMuaDuocPhamKhoDuocChiTiet)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPhamChiTiet();
                    entity.DuTruMuaDuocPhamChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.DuTruMuaDuocPhamChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
        }
        private void AddOrUpdateKhoaChiTiet(DuTruMuaDuocPhamTaiKhoaDuocDuyet viewModel, Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas.DuTruMuaDuocPhamTheoKhoa entity)
        {
            foreach (var item in viewModel.ListDuTruMuaDuocPhamKhoDuocChiTiet)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas.DuTruMuaDuocPhamTheoKhoaChiTiet();
                    entity.DuTruMuaDuocPhamTheoKhoaChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.DuTruMuaDuocPhamTheoKhoaChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
        }
        //private void AddOrUpdateChiTietGoi(DuTruMuaDuocPhamTaiKhoaDuocViewModel viewModel, Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuoc entity)
        //{
        //    //if(viewModel.Id == 0)
        //    //{
        //    //    var newEntity = new Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuoc();
        //    //    entity.AD

        //    //}
        //    foreach (var item in viewModel.DuTruMuaDuocPhamKhoDuocChiTiet)
        //    {
        //        if (item.Id == 0)
        //        {
        //            var newEntity = new Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs.DuTruMuaDuocPhamKhoDuocChiTiet();
        //            entity.DuTruMuaDuocPhamKhoDuocChiTiets.Add(item.ToEntity(newEntity));
        //        }
        //        else
        //        {
        //            var result = entity.DuTruMuaDuocPhamKhoDuocChiTiets.Single(c => c.Id == item.Id);
        //            result = item.ToEntity(result);
        //        }
        //    }
        //}
    }
}
