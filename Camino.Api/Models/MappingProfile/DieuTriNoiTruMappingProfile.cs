using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class DieuTriNoiTruMappingProfile : Profile
    {
        public DieuTriNoiTruMappingProfile()
        {
            //CreateMap<NoiTruPhieuDieuTri, DieuTriNoiTruPhieuDieuTriViewModel>().IgnoreAllNonExisting();

            //CreateMap<DieuTriNoiTruPhieuDieuTriViewModel, NoiTruPhieuDieuTri>().IgnoreAllNonExisting()
            //     //.ForMember(x => x.NoiTruPhieuDieuTriChiTietYLenhs, o => o.Ignore())
            //     .ForMember(x => x.NoiTruThamKhamChanDoanKemTheos, o => o.Ignore())
            //     .ForMember(x => x.KetQuaSinhHieus, o => o.Ignore())
            //     .ForMember(x => x.YeuCauDichVuKyThuats, o => o.Ignore())
            //     .ForMember(x => x.YeuCauDuocPhamBenhViens, o => o.Ignore())
            //     .ForMember(x => x.YeuCauVatTuBenhViens, o => o.Ignore())
            //     ;

            CreateMap<NoiTruPhieuDieuTri, PhieuKhamThamKhamViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.NoiTruThamKhamChanDoanKemTheos, o => o.MapFrom(s => s.NoiTruThamKhamChanDoanKemTheos))
                .ForMember(x => x.KetQuaSinhHieus, o => o.MapFrom(s => s.KetQuaSinhHieus))
                .ForMember(x => x.ThoiGianDieuTriSoSinhViewModels, o => o.MapFrom(s => s.NoiTruThoiGianDieuTriBenhAnSoSinhs));

            CreateMap<PhieuKhamThamKhamViewModel, NoiTruPhieuDieuTri>().IgnoreAllNonExisting()
                .ForMember(x => x.NoiTruThamKhamChanDoanKemTheos, o => o.Ignore())
                .ForMember(x => x.NoiTruThoiGianDieuTriBenhAnSoSinhs, o => o.Ignore())
                .ForMember(x => x.KetQuaSinhHieus, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .AfterMap((viewModel, entity) =>
                {
                    AddOrUpdate(viewModel, entity);
                    AddOrUpdateICDKemTheo(viewModel, entity);
                    AddOrUpdateNoiTruDieuTriBenhAnSoSinh(viewModel, entity);
                }
                );

            CreateMap<PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel, NoiTruThamKhamChanDoanKemTheo>().IgnoreAllNonExisting();
            CreateMap<NoiTruThoiGianDieuTriBenhAnSoSinh, ThoiGianDieuTriSoSinhViewModel>().IgnoreAllNonExisting();
            CreateMap<ThoiGianDieuTriSoSinhViewModel, NoiTruThoiGianDieuTriBenhAnSoSinh>().IgnoreAllNonExisting();

            CreateMap<NoiTruThamKhamChanDoanKemTheo, PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.TenICD, o => o.MapFrom(s => s.ICD != null ? s.ICD.Ma + " - " + s.ICD.TenTiengViet : ""));
            CreateMap<PhieuThamKhamKetQuaSinhHieuViewModel, Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu, PhieuThamKhamKetQuaSinhHieuViewModel>().IgnoreAllNonExisting();


            CreateMap<ThoiGianDieuTriSoSinhRaVienViewModel, NoiTruPhieuDieuTri>().IgnoreAllNonExisting()
                .ForMember(x => x.NoiTruThoiGianDieuTriBenhAnSoSinhs, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .AfterMap((viewModel, entity) =>
                {
                    AddOrUpdateNoiTruDieuTriBenhAnSoSinhRaVien(viewModel, entity);
                });
        }

        #region private class

        private void AddOrUpdateICDKemTheo(PhieuKhamThamKhamViewModel viewModel, NoiTruPhieuDieuTri model)
        {
            foreach (var item in viewModel.NoiTruThamKhamChanDoanKemTheos)
            {
                if (item.Id == 0)
                {
                    var icdPhuKhacEntity = new NoiTruThamKhamChanDoanKemTheo();
                    item.NoiTruPhieuDieuId = viewModel.Id;
                    model.NoiTruThamKhamChanDoanKemTheos.Add(item.ToEntity(icdPhuKhacEntity));
                }
                else
                {
                    var result = model.NoiTruThamKhamChanDoanKemTheos.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in model.NoiTruThamKhamChanDoanKemTheos)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.NoiTruThamKhamChanDoanKemTheos.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }

            }
        }

        private void AddOrUpdateNoiTruDieuTriBenhAnSoSinh(PhieuKhamThamKhamViewModel viewModel, NoiTruPhieuDieuTri model)
        {
            foreach (var item in viewModel.ThoiGianDieuTriSoSinhViewModels)
            {
                if (item.Id == 0)
                {
                    var noiTruThoiGianDieuTriBenhAnSoSinhEntity = new NoiTruThoiGianDieuTriBenhAnSoSinh();
                    item.NoiTruBenhAnId = model.NoiTruBenhAnId;
                    item.NoiTruPhieuDieuTriId = model.Id;
                    item.NgayDieuTri = model.NgayDieuTri;
                    model.NoiTruThoiGianDieuTriBenhAnSoSinhs.Add(item.ToEntity(noiTruThoiGianDieuTriBenhAnSoSinhEntity));
                }
                else
                {
                    var result = model.NoiTruThoiGianDieuTriBenhAnSoSinhs.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in model.NoiTruThoiGianDieuTriBenhAnSoSinhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.ThoiGianDieuTriSoSinhViewModels.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }

            }
        }

        private void AddOrUpdateNoiTruDieuTriBenhAnSoSinhRaVien(ThoiGianDieuTriSoSinhRaVienViewModel viewModel, NoiTruPhieuDieuTri model)
        {
            foreach (var item in viewModel.ThoiGianDieuTriSoSinhViewModels)
            {
                if (item.Id == 0)
                {
                    var noiTruThoiGianDieuTriBenhAnSoSinhEntity = new NoiTruThoiGianDieuTriBenhAnSoSinh();
                    item.NoiTruBenhAnId = model.NoiTruBenhAnId;
                    item.NoiTruPhieuDieuTriId = model.Id;
                    item.NgayDieuTri = model.NgayDieuTri;
                    model.NoiTruThoiGianDieuTriBenhAnSoSinhs.Add(item.ToEntity(noiTruThoiGianDieuTriBenhAnSoSinhEntity));
                }
                else
                {
                    var result = model.NoiTruThoiGianDieuTriBenhAnSoSinhs.Single(p => p.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in model.NoiTruThoiGianDieuTriBenhAnSoSinhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.ThoiGianDieuTriSoSinhViewModels.Any(x => x.Id == item.Id);
                    if (!countModel)
                    {
                        item.WillDelete = true;
                    }
                }

            }
        }

        private void AddOrUpdate(PhieuKhamThamKhamViewModel s, NoiTruPhieuDieuTri d)
        {
            //foreach (var model in d.NoiTruThamKhamChanDoanKemTheos)
            //{
            //    if (!s.NoiTruThamKhamChanDoanKemTheos.Any(c => c.Id == model.Id))
            //    {
            //        model.WillDelete = true;
            //    }
            //}
            //foreach (var model in s.NoiTruThamKhamChanDoanKemTheos)
            //{
            //    if (model.Id == 0)
            //    {
            //        var newEntity = new NoiTruThamKhamChanDoanKemTheo();
            //        d.NoiTruThamKhamChanDoanKemTheos.Add(model.ToEntity(newEntity));
            //    }
            //    else
            //    {
            //        if (d.NoiTruThamKhamChanDoanKemTheos.Any())
            //        {
            //            var result = d.NoiTruThamKhamChanDoanKemTheos.Single(c => c.Id == model.Id);
            //            //model.n = d.Id;
            //            result = model.ToEntity(result);

            //        }
            //    }
            //}


            //
            foreach (var model in d.KetQuaSinhHieus)
            {
                if (!s.KetQuaSinhHieus.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.KetQuaSinhHieus)
            {
                if (model.Id == 0)
                {
                    var newEntity = new Camino.Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu();
                    d.KetQuaSinhHieus.Add(model.ToEntity(newEntity));
                }
                else
                {
                    if (d.KetQuaSinhHieus.Any())
                    {
                        var result = d.KetQuaSinhHieus.Single(c => c.Id == model.Id);
                        //model.ICDId = d.Id;
                        result = model.ToEntity(result);

                    }
                }
            }
        }
        #endregion private class
    }


}
