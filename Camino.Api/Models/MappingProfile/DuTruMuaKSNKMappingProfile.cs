using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaKSNK;
using Camino.Core.Domain.ValueObject.YeuCauMuaKSNK;
using Camino.Core.Helpers;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class DuTruMuaKSNKMappingProfile : Profile
    {
        public DuTruMuaKSNKMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTu, DuTruMuaKSNKViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.DuTruMuaVatTuChiTiets, o => o.MapFrom(y => y.DuTruMuaVatTuChiTiets))
                .AfterMap((model, viewModel) =>
                {
                    viewModel.TenKho = model.Kho?.Ten;
                    viewModel.TenKyDuTru = model.TuNgay.ApplyFormatDate() + " - " + model.DenNgay.ApplyFormatDate();
                    viewModel.TenNhanVienYeuCau = model.NhanVienYeuCau?.User.HoTen;
                    viewModel.TenTruongKhoa = model.TruongKhoa?.User.HoTen;

                    viewModel.TenNhanVienKhoDuoc = model.DuTruMuaVatTuTheoKhoa != null && model.DuTruMuaVatTuTheoKhoa.NhanVienKhoDuoc != null ? model.DuTruMuaVatTuTheoKhoa.NhanVienKhoDuoc.User.HoTen : model.TruongKhoa?.User.HoTen;

                    viewModel.TenGiamDoc = model.DuTruMuaVatTuTheoKhoa != null && model.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null ? model.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.GiamDoc?.User.HoTen : model.DuTruMuaVatTuKhoDuoc?.GiamDoc?.User.HoTen;

                    viewModel.NgayKhoDuocDuyet = model.DuTruMuaVatTuTheoKhoa != null ? model.DuTruMuaVatTuTheoKhoa.NgayKhoDuocDuyet : model.NgayTruongKhoaDuyet;

                    viewModel.NgayGiamDocDuyet = (model.DuTruMuaVatTuTheoKhoa != null && model.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc != null) ? model.DuTruMuaVatTuTheoKhoa.DuTruMuaVatTuKhoDuoc.NgayGiamDocDuyet : (model.DuTruMuaVatTuKhoDuoc?.NgayGiamDocDuyet);

                });

            CreateMap<DuTruMuaKSNKViewModel, Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTu>().IgnoreAllNonExisting()
                .ForMember(d => d.DuTruMuaVatTuChiTiets, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateDuTruMuaDuocPhamChiTiet(d, s);
                });

            #region Excel
            CreateMap<YeuCauMuaKSNKGridVo, YeuCauDuTruKSNKExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.TinhTrang, o => o.MapFrom(p => p.TinhTrang == 0 ? "Chờ duyệt" : (p.TinhTrang == 1 ? "Đã duyệt" : "Từ chối")));

            #endregion


            CreateMap<Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTuChiTiet, DuTruMuaKSNKChiTietViewModel>().IgnoreAllNonExisting()
                  .AfterMap((s, d) =>
                  {
                      d.Ten = s.VatTu.Ten;
                      d.DVT = s.VatTu.DonViTinh;
                      d.NhaSX = s.VatTu.NhaSanXuat;
                      d.NuocSX = s.VatTu.NuocSanXuat;
                      d.SoLuongDuTruTruongKhoaDuyet = s.SoLuongDuTruTruongKhoaDuyet;
                  });

            CreateMap<DuTruMuaKSNKChiTietViewModel, Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTuChiTiet>().IgnoreAllNonExisting();


            CreateMap<Core.Domain.Entities.VatTus.VatTu, KSNKDuTruGridViewModel>().IgnoreAllNonExisting();
            CreateMap<KSNKDuTruGridViewModel, Core.Domain.Entities.VatTus.VatTu>().IgnoreAllNonExisting();
        }
        private void AddOrUpdateDuTruMuaDuocPhamChiTiet(DuTruMuaKSNKViewModel viewModel, Core.Domain.Entities.DuTruVatTus.DuTruMuaVatTu entity)
        {
            foreach (var item in viewModel.DuTruMuaVatTuChiTiets)
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
            foreach (var item in entity.DuTruMuaVatTuChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuTruMuaVatTuChiTiets.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }

    }
}
