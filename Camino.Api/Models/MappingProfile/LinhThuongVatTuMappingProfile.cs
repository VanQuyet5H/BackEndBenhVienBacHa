using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhThuongVatTu;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using System.Linq;


namespace Camino.Api.Models.MappingProfile
{
    public class LinhThuongVatTuMappingProfile : Profile
    {
        public LinhThuongVatTuMappingProfile()
        {
            CreateMap<YeuCauLinhVatTu, LinhThuongVatTuViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauLinhVatTuChiTiets, o => o.MapFrom(y => y.YeuCauLinhVatTuChiTiets))
                .AfterMap((d, s) =>
                {
                    s.HoTenNguoiYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.HoTenNguoiDuyet = d.NhanVienDuyet?.User.HoTen;
                    s.TenKhoNhap = d.KhoNhap?.Ten;
                    s.TenKhoXuat = d.KhoXuat?.Ten;
                });

            CreateMap<LinhThuongVatTuViewModel, YeuCauLinhVatTu>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoNhap, o => o.Ignore())
                .ForMember(d => d.KhoXuat, o => o.Ignore())
                .ForMember(d => d.NhanVienYeuCau, o => o.Ignore())
                .ForMember(d => d.NhanVienDuyet, o => o.Ignore())
                .ForMember(d => d.YeuCauLinhVatTuChiTiets, o => o.Ignore())
                .ForMember(d => d.XuatKhoVatTus, o => o.Ignore())
                .ForMember(d => d.YeuCauVatTuBenhViens, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateLinhThuongVatTuChiTiet(d, s);
                });
        }

        private void AddOrUpdateLinhThuongVatTuChiTiet(LinhThuongVatTuViewModel viewModel, YeuCauLinhVatTu entity)
        {
            foreach (var item in viewModel.YeuCauLinhVatTuChiTiets)
            {
                if (item.Id == 0)
                {
                    var newEntity = new YeuCauLinhVatTuChiTiet();
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
