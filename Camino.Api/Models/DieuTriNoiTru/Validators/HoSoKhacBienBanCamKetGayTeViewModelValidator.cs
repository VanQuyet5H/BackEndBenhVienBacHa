using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>))]
    public class HoSoKhacBienBanCamKetGayTeViewModelValidator : AbstractValidator<HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>
    {
        public HoSoKhacBienBanCamKetGayTeViewModelValidator(ILocalizationService localizationService, IValidator<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel> thongTinQuanHeThanNhanValidator)
        {
            RuleFor(p => p.BacSiGiaiThich)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.BSGiaiThich.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.BSGiaiThich.Required"));

            RuleFor(p => p.NgayThucHien)
                .NotNull().WithMessage(localizationService.GetResource("HoSoKhac.NgayThucHien.Required"));

            RuleFor(p => p.BacSiGMHSId)
               .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.BSGMHS.Required"))
               .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.BSGMHS.Required"));

            RuleForEach(p => p.QuanHeThanNhans).SetValidator(thongTinQuanHeThanNhanValidator);
        }
    }

    [TransientDependency(ServiceType = typeof(IValidator<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>))]

    public class ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModelValidator : AbstractValidator<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>
    {
        public ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.HoTen)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));
        }
    }
}
