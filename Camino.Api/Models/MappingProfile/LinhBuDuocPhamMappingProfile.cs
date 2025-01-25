using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhBuDuocPham;
using System.Collections.Generic;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class LinhBuDuocPhamMappingProfile : Profile
    {
        public LinhBuDuocPhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham, LinhBuDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauDuocPhamBenhViens, o => o.MapFrom(y => y.YeuCauDuocPhamBenhViens))
                .AfterMap((d, s) =>
                {
                    s.HoTenNguoiYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.TenKhoNhap = d.KhoNhap?.Ten;
                    s.TenKhoXuat = d.KhoXuat?.Ten;
                });

            CreateMap<LinhBuDuocPhamViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhDuocPhamChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoDuocPhams, o => o.Ignore())
                .ForMember(d => d.YeuCauDuocPhamBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    //AddOrUpdateLinhBuDuocPhamChiTiet(d, s);
                });


            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien, YeuCauDuocPhamBenhVienViewModel>().IgnoreAllNonExisting();

            CreateMap<YeuCauDuocPhamBenhVienViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DuyetBaoHiemChiTiets, o => o.Ignore())
                .ForMember(d => d.TaiKhoanBenhNhanChis, o => o.Ignore())
                .ForMember(d => d.TaiKhoanBenhNhanThus, o => o.Ignore())
                .ForMember(d => d.MienGiamChiPhis, o => o.Ignore())
                .ForMember(d => d.CongTyBaoHiemTuNhanCongNos, o => o.Ignore());
        }
        private void AddOrUpdateLinhBuDuocPhamChiTiet(LinhBuDuocPhamViewModel viewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham entity)
        {
            foreach (var item in viewModel.YeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet();
                    entity.YeuCauLinhDuocPhamChiTiets.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.YeuCauLinhDuocPhamChiTiets.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.YeuCauLinhDuocPhamChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.YeuCauLinhDuocPhamChiTiets.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
