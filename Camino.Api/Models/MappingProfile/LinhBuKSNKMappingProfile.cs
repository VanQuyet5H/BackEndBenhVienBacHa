using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhBuKSNK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class LinhBuKSNKMappingProfile : Profile
    {
        public LinhBuKSNKMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu, LinhBuKSNKViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauVatTuBenhViens, o => o.MapFrom(y => y.YeuCauVatTuBenhViens))
                .AfterMap((d, s) =>
                {
                    s.HoTenNguoiYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.TenKhoNhap = d.KhoNhap?.Ten;
                    s.TenKhoXuat = d.KhoXuat?.Ten;
                });

            CreateMap<LinhBuKSNKViewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhVatTuChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoVatTus, o => o.Ignore())
                .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateLinhBuVatTuChiTiet(d, s);
                });


            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauVatTuBenhVien, YeuCauKSNKBenhViensViewModel>().IgnoreAllNonExisting();

            CreateMap<YeuCauKSNKBenhViensViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauVatTuBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DuyetBaoHiemChiTiets, o => o.Ignore())
                .ForMember(d => d.TaiKhoanBenhNhanChis, o => o.Ignore())
                .ForMember(d => d.TaiKhoanBenhNhanThus, o => o.Ignore())
                .ForMember(d => d.MienGiamChiPhis, o => o.Ignore())
                .ForMember(d => d.CongTyBaoHiemTuNhanCongNos, o => o.Ignore());
        }
        private void AddOrUpdateLinhBuVatTuChiTiet(LinhBuKSNKViewModel viewModel, Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu entity)
        {
            foreach (var item in viewModel.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet();
                    entity.YeuCauLinhVatTuChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.YeuCauLinhVatTuChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauLinhVatTuChiTiets.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
