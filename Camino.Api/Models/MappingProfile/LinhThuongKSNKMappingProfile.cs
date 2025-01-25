using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhThuongKSNK;
using Camino.Api.Models.LinhThuongVatTu;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class LinhThuongKSNKMappingProfile : Profile
    {
        public LinhThuongKSNKMappingProfile()
        {
            CreateMap<YeuCauLinhVatTu, LinhThuongKSNKViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhVatTuChiTiets, o => o.MapFrom(y => y.YeuCauLinhVatTuChiTiets))
                .AfterMap((d, s) =>
                {
                    s.HoTenNguoiYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.HoTenNguoiDuyet = d.NhanVienDuyet?.User.HoTen;
                    s.TenKhoNhap = d.KhoNhap?.Ten;
                    s.TenKhoXuat = d.KhoXuat?.Ten;
                });

            CreateMap<LinhThuongKSNKViewModel, YeuCauLinhVatTu>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhVatTuChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoVatTus, o => o.Ignore())
                .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateLinhThuongKSNKChiTiet(d, s);
                });
            #region Lĩnh thường Vật tư
            CreateMap<YeuCauLinhVatTuChiTiet, LinhThuongKNSKChiTietViewModel>().IgnoreAllNonExisting()
                 .AfterMap((s, d) =>
                 {
                     d.Ten = s.VatTuBenhVien.VatTus.Ten;
                     d.Ma = s.VatTuBenhVien.VatTus.Ma;
                     d.DVT = s.VatTuBenhVien.VatTus.DonViTinh;
                     d.NhaSX = s.VatTuBenhVien.VatTus.NhaSanXuat;
                     d.NuocSX = s.VatTuBenhVien.VatTus.NuocSanXuat;
                     d.SLYeuCau = s.SoLuong;
                     d.KhoXuatId = s.YeuCauLinhVatTu.KhoXuatId;
                     //d.SoLuongCoTheXuat = s.SoLuongCoTheXuat;
                 });

            CreateMap<LinhThuongKNSKChiTietViewModel, YeuCauLinhVatTuChiTiet>().IgnoreAllNonExisting();
            #endregion
        }

        private void AddOrUpdateLinhThuongKSNKChiTiet(LinhThuongKSNKViewModel viewModel, YeuCauLinhVatTu entity)
        {
            foreach (var item in viewModel.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new YeuCauLinhVatTuChiTiet();
                    var entitymodelMap = item.ToEntity(newEntity);
                    entity.YeuCauLinhVatTuChiTiets.Add(entitymodelMap);
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
