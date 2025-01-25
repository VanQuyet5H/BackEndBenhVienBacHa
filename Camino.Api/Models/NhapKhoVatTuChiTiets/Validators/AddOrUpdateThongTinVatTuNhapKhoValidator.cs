using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoVatTuChiTiets.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauNhapKhoVatTuChiTietGridVo>))]
    public class AddOrUpdateThongTinVatTuNhapKhoValidator : AbstractValidator<YeuCauNhapKhoVatTuChiTietGridVo>
    {
        public AddOrUpdateThongTinVatTuNhapKhoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.HopDongThauVatTuId)
                .Must((req, hopDongThauId, id) =>
                {
                    if (req.LoaiNhap == 1)
                    {
                        return hopDongThauId != null && hopDongThauId != 0;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.HopDongThauDuocPhamId.Required"));
            RuleFor(x => x.NhaThauId)
                .Must((req, nhaThauId, id) =>
                {
                    if (req.LoaiNhap == 2)
                    {
                        return nhaThauId != null && nhaThauId != 0;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("HopDongThauDuocPham.NhaThauId.Required"));
            RuleFor(x => x.VatTuBenhVienId)
              .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.VatTuBenhVienId.Required"));
            RuleFor(x => x.LaVatTuBHYT)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.LaVatTuBHYT.Required"));
            RuleFor(x => x.Solo)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.Solo.Required"));

            RuleFor(x => x.HanSuDung)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.HanSuDung.Required"));
            RuleFor(x => x.HanSuDung)
               .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.HanSuDung.Required"))
                .Must((request, ngay, id) => {
                    if (ngay != null && ngay.Value.Date <= DateTime.Now.Date)
                        return false;
                    return true;
                }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.HanSuDung.LessThanNow"));

            RuleFor(x => x.SoLuongNhap)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.SoLuongNhap.Required"));
            RuleFor(x => x.DonGiaNhap)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.DonGiaNhap.Required"));
            RuleFor(x => x.VAT)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.VAT.Required")).When(p => p.LaVatTuBHYT == false);

            RuleFor(x => x.ThanhTienSauVat)
           .NotEmpty().WithMessage(localizationService.GetResource("BHYT.ThanhToan.Required")).When(p => p.LaVatTuBHYT == false);

            RuleFor(x => x.ThanhTienTruocVat)
            .NotEmpty().WithMessage(localizationService.GetResource("BHYT.ThanhToan.Required"));

            RuleFor(x => x.TiLeBHYTThanhToan)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.TiLeBHYTThanhToan.Required")).When(p => p.LaVatTuBHYT == true);


            RuleFor(x => x.KhoNhapSauKhiDuyetId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId.Required"));

            RuleFor(x => x.NguoiNhapSauKhiDuyetId)
                    .Must((req, nguoiNhapSauKhiDuyetId, id) =>
                    {
                        if (req.KhoNhapSauKhiDuyetId>0)
                        {
                            return nguoiNhapSauKhiDuyetId != null && nguoiNhapSauKhiDuyetId != 0;
                        }

                        return true;
                    }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.NguoiNhapSauKhiDuyetId.Required"));
                //RuleFor(x => x.KhoViTriId)
                // .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.KhoViTriId.Required"));
            }
    }
}
