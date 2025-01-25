using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.VatTuBenhViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.VatTuBenhViens;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class VatTuBenhVienMappingProfile : Profile
    {
        public VatTuBenhVienMappingProfile()
        {
            CreateMap<VatTuBenhVien, VatTuBenhVienViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
            {
                d.MaVatTuBenhVien = s.MaVatTuBenhVien;
                d.LoaiSuDung = s.LoaiSuDung;
                d.TenLoaiSuDung = s.LoaiSuDung.GetDescription();
                d.Ten = s.VatTus.Ten;
                d.Ma = s.VatTus.Ma;
                d.NhomVatTuId = s.VatTus.NhomVatTuId;
                d.TenNhomVatTu = s.VatTus.NhomVatTu?.Ten;
                d.DonViTinh = s.VatTus.DonViTinh;
                d.TyLeBaoHiemThanhToan = s.VatTus.TyLeBaoHiemThanhToan;
                d.QuyCach = s.VatTus.QuyCach;
                d.NhaSanXuat = s.VatTus.NhaSanXuat;
                d.NuocSanXuat = s.VatTus.NuocSanXuat;
                d.MoTa = s.VatTus.MoTa;
                d.IsDisabled = s.VatTus.IsDisabled;
                d.HeSoDinhMucDonViTinh = s.VatTus.HeSoDinhMucDonViTinh;
                d.HieuLuc = s.HieuLuc;
                d.DieuKienBaoHiemThanhToan = s.DieuKienBaoHiemThanhToan;
            });

            CreateMap<VatTuBenhVienViewModel, VatTuBenhVien>().IgnoreAllNonExisting().ForMember(d => d.VatTus, o => o.Ignore());
            CreateMap<VatTuBenhVienGridVo, VatTuBenhVienExportExcel>().IgnoreAllNonExisting();
        }
    }
}
