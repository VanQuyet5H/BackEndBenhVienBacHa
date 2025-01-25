using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhapKhoDuocPhamChiTiets;
using Camino.Api.Models.NhapKhoVatTuChiTiets;
using Camino.Api.Models.NhapKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Camino.Core.Helpers;
using System.Linq;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class NhapKhoVatTuMappingProfile : Profile
    {
        public NhapKhoVatTuMappingProfile()
        {
            CreateMap<NhapKhoVatTu, NhapKhoVatTuViewModel>()
                .ForMember(d => d.NhapKhoVatTuChiTiets, o => o.MapFrom(s => s.NhapKhoVatTuChiTiets))
                .AfterMap((source, viewmodel) =>
                {
                    viewmodel.TenKhoVatTu = source?.Kho?.Ten ?? "";
                    viewmodel.TenNguoiGiao = source.LoaiNguoiGiao == LoaiNguoiGiaoNhan.TrongHeThong ? source?.TenNguoiGiao ?? "" : null;
                    viewmodel.TenNguoiGiaoNgoai = source.LoaiNguoiGiao == LoaiNguoiGiaoNhan.NgoaiHeThong ? source?.TenNguoiGiao ?? "" : null;
                    viewmodel.TenNguoiNhap = source?.NguoiNhap?.User?.HoTen ?? "";
                    //TODO update entity kho on 9/9/2020
                    //viewmodel.TenLoaiNhapKho = source?.LoaiNhapKho.GetDescription();
                });
            CreateMap<NhapKhoVatTuViewModel, NhapKhoVatTu>()
                .ForMember(d => d.NhapKhoVatTuChiTiets, o => o.Ignore())
                .ForMember(d => d.KhoId, o => o.MapFrom(s => s.KhoId))
                .AfterMap((s, d) =>
                {
                    d.TenNguoiGiao = s.LoaiNguoiGiao == LoaiNguoiGiaoNhan.NgoaiHeThong ? s.TenNguoiGiaoNgoai : s.TenNguoiGiao;
                })
                .AfterMap((s, d) => AddOrUpdate(s, d));

            CreateMap<NhapKhoVatTuChiTiet, NhapKhoVatTuChiTietViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.VatTuBenhViens, o => o.MapFrom(s => s.VatTuBenhVien))
                 .ForMember(d => d.HopDongThauVatTus, o => o.MapFrom(s => s.HopDongThauVatTu))
                 //.ForMember(d => d.KhoViTriId, o => o.MapFrom(s => s.KhoDuocPhamViTri))
                    .AfterMap((source, viewmodel) =>
                    {
                        viewmodel.TenVatTu = source?.VatTuBenhVien?.VatTus?.Ten ?? "";
                        viewmodel.TenHopDongThau = source?.HopDongThauVatTu?.SoHopDong ?? "";
                        viewmodel.Solo = source?.Solo ?? "";
                        viewmodel.ViTri = source?.KhoViTri?.Ten ?? "";

                        //TODO update entity kho on 9/9/2020
                        //viewmodel.TenDatChatLuong = source?.DatChatLuong == true ? "Đạt" : "Không đạt";
                        viewmodel.TextHanSuDung = source?.HanSuDung.ApplyFormatDate();
                    });
            CreateMap<NhapKhoVatTuChiTietViewModel, NhapKhoVatTuChiTiet>()
                .ForMember(d => d.KhoViTriId, o => o.MapFrom(s => s.KhoViTriId))
                 .ForMember(d => d.KhoViTri, o => o.Ignore())
                 .ForMember(x => x.VatTuBenhVien, o => o.Ignore())
                 .ForMember(x => x.HopDongThauVatTu, o => o.Ignore());

            CreateMap<NhapKhoVatTuChiTietViewModel, NhapKhoVatTuChiTiet>()
                .IgnoreAllNonExisting();

            CreateMap<NhapKhoVatTuGridVo, NhapKhoVatTuExportExcel>().IgnoreAllNonExisting();
            CreateMap<NhapKhoVatTuChiTietGripVo, NhapKhoVatTuExportExcelChild>().IgnoreAllNonExisting()
                .ForMember(d => d.LoaiSuDung, o => o.MapFrom(y => y.LoaiSuDung));

            CreateMap<NhapKhoKSNKGridVo, NhapKhoKSNKExportExcel>().IgnoreAllNonExisting();
            CreateMap<NhapKhoKSNKChiTietGripVo, NhapKhoKSNKExportExcelChild>().IgnoreAllNonExisting()
                .ForMember(d => d.LoaiSuDung, o => o.MapFrom(y => y.LoaiSuDung));
        }

        private void AddOrUpdate(NhapKhoVatTuViewModel s, NhapKhoVatTu d)
        {
            foreach (var model in d.NhapKhoVatTuChiTiets)
            {
                if (!s.NhapKhoVatTuChiTiets.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.NhapKhoVatTuChiTiets)
            {
                if (model.Id == 0)
                {
                    var newEntity = new NhapKhoVatTuChiTiet();
                    model.NgayNhap = s.NgayNhap;
                    d.NhapKhoVatTuChiTiets.Add(model.ToEntity(newEntity));
                }
                else
                {
                    if (d.NhapKhoVatTuChiTiets.Any())
                    {
                        var result = d.NhapKhoVatTuChiTiets.Single(c => c.Id == model.Id);
                        model.NgayNhap = s.NgayNhap;
                        model.NhapKhoVatTuId = d.Id;
                        result = model.ToEntity(result);

                    }
                }
            }
        }

    }
}
