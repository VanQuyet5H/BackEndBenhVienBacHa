using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DuTruMuaDuocPham;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Helpers;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class DuTruMuaDuocPhamMappingProfile : Profile
    {
        public DuTruMuaDuocPhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham, DuTruMuaDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.DuTruMuaDuocPhamChiTiets, o => o.MapFrom(y => y.DuTruMuaDuocPhamChiTiets))
                .AfterMap((model, viewModel) =>
                {
                    viewModel.TenNhomDuTru = model.NhomDuocPhamDuTru.GetDescription();
                    viewModel.TenKho = model.Kho?.Ten;
                    viewModel.TenKyDuTru = model.TuNgay.ApplyFormatDate() + " - " + model.DenNgay.ApplyFormatDate();
                    viewModel.TenNhanVienYeuCau = model.NhanVienYeuCau?.User.HoTen;
                    viewModel.TenTruongKhoa = model.TruongKhoa?.User.HoTen;

                    viewModel.TenNhanVienKhoDuoc = model.DuTruMuaDuocPhamTheoKhoa != null && model.DuTruMuaDuocPhamTheoKhoa.NhanVienKhoDuoc != null ? model.DuTruMuaDuocPhamTheoKhoa.NhanVienKhoDuoc.User.HoTen : model.TruongKhoa?.User.HoTen;

                    viewModel.TenGiamDoc = model.DuTruMuaDuocPhamTheoKhoa != null && model.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null ? model.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.GiamDoc?.User.HoTen : model.DuTruMuaDuocPhamKhoDuoc?.GiamDoc?.User.HoTen;

                    viewModel.NgayKhoDuocDuyet = model.DuTruMuaDuocPhamTheoKhoa != null ? model.DuTruMuaDuocPhamTheoKhoa.NgayKhoDuocDuyet : model.NgayTruongKhoaDuyet;

                    viewModel.NgayGiamDocDuyet = (model.DuTruMuaDuocPhamTheoKhoa != null && model.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc != null) ? model.DuTruMuaDuocPhamTheoKhoa.DuTruMuaDuocPhamKhoDuoc.NgayGiamDocDuyet : (model.DuTruMuaDuocPhamKhoDuoc?.NgayGiamDocDuyet);

                    //viewModel.LyDoTruongKhoaTuChoi = 
                    //            model.DuTruMuaDuocPhamTheoKhoa == null && model.DuTruMuaDuocPhamKhoDuoc == null && model.TruongKhoaDuyet == false ? model.LyDoTruongKhoaTuChoi : ((model.DuTruMuaDuocPhamTheoKhoa != null && model.DuTruMuaDuocPhamTheoKhoa.KhoDuocDuyet == false) ? model.DuTruMuaDuocPhamTheoKhoa.LyDoKhoDuocTuChoi : (model.DuTruMuaDuocPhamKhoDuoc != null && model.DuTruMuaDuocPhamKhoDuoc.GiamDocDuyet == false ? model.DuTruMuaDuocPhamKhoDuoc.LyDoGiamDocTuChoi : null));

                });

            CreateMap<DuTruMuaDuocPhamViewModel, Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham>().IgnoreAllNonExisting()
                .ForMember(d => d.DuTruMuaDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateDuTruMuaDuocPhamChiTiet(d, s);
                });

            #region Excel
            CreateMap<YeuCauDuTruDuocPhamGridVo, YeuCauDuTruDuocPhamExcel>().IgnoreAllNonExisting()
                .ForMember(m => m.TinhTrang, o => o.MapFrom(p => p.TinhTrang == 0 ? "Chờ duyệt" : (p.TinhTrang == 1 ? "Đã duyệt" : "Từ chối")));

            CreateMap<BaoCaoKhamDoanHopDongTheoNhanVienVo, BaoCaoKhamDoanHopDongTheoNhanVienExportExcel>().IgnoreAllNonExisting();
            CreateMap<BaoCaoKhamDoanHopDongDichVuNgoaiVo, BaoCaoKhamDoanHopDongDichVuNgoaiExportExcel>().IgnoreAllNonExisting();



            #endregion
        }
        private void AddOrUpdateDuTruMuaDuocPhamChiTiet(DuTruMuaDuocPhamViewModel viewModel, Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham entity)
        {
            foreach (var item in viewModel.DuTruMuaDuocPhamChiTiets)
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
            foreach (var item in entity.DuTruMuaDuocPhamChiTiets)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.DuTruMuaDuocPhamChiTiets.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
