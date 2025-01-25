using Camino.Api;
using AutoMapper;
using Camino.Core.Helpers;
using Camino.Api.Models.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using System.Linq;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class NhapKhoDuocPhamMappingProfile : Profile
    {
        public NhapKhoDuocPhamMappingProfile()
        {
            CreateMap<NhapKhoDuocPham, NhapKhoDuocPhamViewModel>()
                .ForMember(d => d.NhapKhoDuocPhamChiTiets, o => o.MapFrom(s => s.NhapKhoDuocPhamChiTiets))
                .AfterMap((source, viewmodel) =>
                {
                    viewmodel.TenKhoDuocPham = source?.KhoDuocPhams?.Ten ?? "";
                    viewmodel.TenNguoiGiao = source.LoaiNguoiGiao == LoaiNguoiGiaoNhan.TrongHeThong ? source?.TenNguoiGiao?? "" : null;
                    viewmodel.TenNguoiGiaoNgoai = source.LoaiNguoiGiao == LoaiNguoiGiaoNhan.NgoaiHeThong ? source?.TenNguoiGiao ?? "" : null;
                    viewmodel.TenNguoiNhap = source?.NhanVienNhap?.User?.HoTen ?? "";
                    //TODO update entity kho on 9/9/2020
                    //viewmodel.TenLoaiNhapKho = source?.LoaiNhapKho.GetDescription();
                });
            CreateMap<NhapKhoDuocPhamViewModel, NhapKhoDuocPham>()
                .ForMember(d => d.NhapKhoDuocPhamChiTiets, o => o.Ignore())
                .ForMember(d => d.KhoId, o => o.MapFrom(s => s.KhoDuocPhamId))
                .AfterMap((s, d) =>
                {
                    d.TenNguoiGiao = s.LoaiNguoiGiao == LoaiNguoiGiaoNhan.NgoaiHeThong ? s.TenNguoiGiaoNgoai : s.TenNguoiGiao;
                })
                .AfterMap((s, d) => AddOrUpdate(s, d));

            CreateMap<NhapKhoDuocPhamGripVo, NhapKhoDuocPhamExportExcel>().IgnoreAllNonExisting();
            CreateMap<NhapKhoDuocPhamChiTietGripVo, NhapKhoDuocPhamExportExcelChild>().IgnoreAllNonExisting();
        }

        private void AddOrUpdate(NhapKhoDuocPhamViewModel s, NhapKhoDuocPham d)
        {
            foreach (var model in d.NhapKhoDuocPhamChiTiets)
            {
                if (!s.NhapKhoDuocPhamChiTiets.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.NhapKhoDuocPhamChiTiets)
            {
                if (model.Id == 0)
                {
                    var newEntity = new NhapKhoDuocPhamChiTiet();
                    model.NgayNhap = s.NgayNhap;
                    d.NhapKhoDuocPhamChiTiets.Add(model.ToEntity(newEntity));
                }
                else
                {
                    if (d.NhapKhoDuocPhamChiTiets.Any())
                    {
                        var result = d.NhapKhoDuocPhamChiTiets.Single(c => c.Id == model.Id);
                        model.NgayNhap = s.NgayNhap;
                        model.NhapKhoDuocPhamId = d.Id;
                        result = model.ToEntity(result);
                       
                    }
                }
            }
        }
    }
}
