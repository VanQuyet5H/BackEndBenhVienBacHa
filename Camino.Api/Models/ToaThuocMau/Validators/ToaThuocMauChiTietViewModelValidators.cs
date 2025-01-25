using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Core.Helpers;
namespace Camino.Api.Models.ToaThuocMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ToaThuocMauChiTietViewModel>))]

    public class ToaThuocMauChiTietViewModelValidators : AbstractValidator<ToaThuocMauChiTietViewModel>
    {
        public ToaThuocMauChiTietViewModelValidators(ILocalizationService localizationService)
        {
            RuleFor(x => x.DuocPhamId)
                   .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.DuocPham.Required"));

            RuleFor(x => x.SoLuong)
                   .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                   .Must((request, soLuongNhap, id) => {
                       if (soLuongNhap <= 0)
                           return false;
                       return true;
                   }).WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.SoLuongNhap.LessThan0"));

            RuleFor(x => x.DungSangDisplay)
                .Must((model, input, s) => string.IsNullOrEmpty(model.DungSangDisplay) || model.DungSangDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.DungTruaDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.DungTruaDisplay) || model.DungTruaDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.DungChieuDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.DungChieuDisplay) || model.DungChieuDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.DungToiDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.DungToiDisplay) || model.DungToiDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));
        }
    }
}
