using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauHoanTraDuocPham;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.DuocPham;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauHoanTraDuocPhamMappingProfile : Profile
    {
        public YeuCauHoanTraDuocPhamMappingProfile()
        {
            CreateMap<YeuCauTraDuocPham, YeuCauHoanTraDuocPhamViewModel>()
                .ForMember(d => d.YeuCauTraDuocPhamChiTiets, o => o.MapFrom(s => s.YeuCauTraDuocPhamChiTiets))
                .AfterMap((s, d) =>
                {
                    d.KhoXuat = s.KhoXuat?.Ten;
                    d.KhoNhap = s.KhoNhap?.Ten;
                    d.NhanVienDuyet = s.NhanVienDuyet?.User?.HoTen;
                    d.NhanVienYeuCau = s.NhanVienYeuCau?.User?.HoTen;
                }).IgnoreAllNonExisting();

            CreateMap<YeuCauHoanTraDuocPhamViewModel, YeuCauTraDuocPham>()
                .IgnoreAllNonExisting()
              .ForMember(d => d.YeuCauTraDuocPhamChiTiets, o => o.Ignore());
            //.AfterMap((s, d) => AddOrUpdate(s, d));

            CreateMap<YeuCauTraDuocPhamChiTiet, YeuCauTraDuocPhamChiTietViewModel>()
                .AfterMap((s, d) =>
                {
                    d.HopDong = s.HopDongThauDuocPham.NhaThau.Ten;
                    d.DuocPham = s.DuocPhamBenhVien.DuocPham.Ten;
                    d.Nhom = s.DuocPhamBenhVienPhanNhom?.Ten;
                }).IgnoreAllNonExisting();

            CreateMap<YeuCauTraDuocPhamChiTietViewModel, YeuCauTraDuocPhamChiTiet>().IgnoreAllNonExisting()
                ;

            CreateMap<DuocPhamHoanTraChiTiet, YeuCauTraDuocPhamChiTiet>()
                .ForMember(d => d.Id, o => o.Ignore())
                ;

            CreateMap<YeuCauTraDuocPhamChiTiet, DuocPhamHoanTraChiTiet>()
                //.ForMember(d => d.Id, o => o.Ignore())
                ;
            CreateMap<DanhSachYeuCauHoanTraDuocPhamGridVo, DanhSachYeuCauHoanTraDuocPhamExportExcel>().IgnoreAllNonExisting();
            CreateMap<DanhSachYeuCauHoanTraDuocPhamChiTietGridVo, DanhSachYeuCauHoanTraDuocPhamChiTietExportExcelChild>();


            //Update 31/12/2021
            CreateMap<YeuCauTraDuocPham, YeuCauHoanTraDuocPhamTuTrucViewModel>().IgnoreAllNonExisting()
                  .ForMember(x => x.YeuCauHoanTraDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.KhoNhap?.Ten;
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                });
            CreateMap<YeuCauHoanTraDuocPhamTuTrucViewModel, YeuCauTraDuocPham>().IgnoreAllNonExisting()
                ;
        }

        //private void AddOrUpdate(YeuCauHoanTraDuocPhamViewModel s,  YeuCauTraDuocPham d)
        //{
        //    foreach (var entity in d.YeuCauTraDuocPhamChiTiets)
        //    {
        //        //var id = int.Parse(model.Id.Split(",")[0]);
        //        if (!s.YeuCauTraDuocPhamChiTiets.Any(c => int.Parse(c.Id.Split(",")[0]) == entity.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId))
        //        {
        //            entity.WillDelete = true;
        //        }
        //    }
        //    foreach (var model in s.YeuCauTraDuocPhamChiTiets)
        //    {
        //        var id = int.Parse(model.Id.Split(",")[0]);
        //        if (!d.YeuCauTraDuocPhamChiTiets.Any(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId == id))
        //        {
        //            //var newEntity = new YeuCauTraDuocPhamChiTiet();
        //            ////model.Nhom = s.NgayNhap;
        //            //d.YeuCauTraDuocPhamChiTiets.Add(model.ToEntity(newEntity));

        //            //create new
        //        }
        //        else
        //        {
        //            if (d.YeuCauTraDuocPhamChiTiets.Any())
        //            {
        //                //var result = d.YeuCauTraDuocPhamChiTiets.Single(c => c.Id == model.Id);
        //                ////model.NgayNhap = s.NgayNhap;
        //                //model.YeuCauTraDuocPhamId = d.Id;
        //                //model.ToEntity(result);

        //                //update
        //            }
        //        }

        //        //if (model.Id == 0)
        //        //{
        //        //    var newEntity = new YeuCauTraDuocPhamChiTiet();
        //        //    //model.Nhom = s.NgayNhap;
        //        //    d.YeuCauTraDuocPhamChiTiets.Add(model.ToEntity(newEntity));
        //        //}
        //        //else
        //        //{
        //        //    if (d.YeuCauTraDuocPhamChiTiets.Any())
        //        //    {
        //        //        var result = d.YeuCauTraDuocPhamChiTiets.Single(c => c.Id == model.Id);
        //        //        //model.NgayNhap = s.NgayNhap;
        //        //        model.YeuCauTraDuocPhamId = d.Id;
        //        //        model.ToEntity(result);
        //        //    }
        //        //}
        //    }
        //}
    }
}
