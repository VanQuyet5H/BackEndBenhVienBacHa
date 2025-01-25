using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhThuongDuocPham;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class LinhThuongDuocPhamMappingProfile : Profile
    {
        public LinhThuongDuocPhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham, LinhThuongDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhDuocPhamChiTiets, o => o.MapFrom(y => y.YeuCauLinhDuocPhamChiTiets))
                .AfterMap((d, s) =>
                {   
                    s.HoTenNguoiYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.HoTenNguoiDuyet = d.NhanVienDuyet?.User.HoTen;
                    s.TenKhoNhap = d.KhoNhap?.Ten;
                    s.TenKhoXuat = d.KhoXuat?.Ten;
                });

            CreateMap<LinhThuongDuocPhamViewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhDuocPhamChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoDuocPhams, o => o.Ignore())
                .ForMember(d => d.YeuCauDuocPhamBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateLinhThuongDuocPhamChiTiet(d, s);
                });
        }

        private void AddOrUpdateLinhThuongDuocPhamChiTiet(LinhThuongDuocPhamViewModel viewModel, Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham entity)
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
