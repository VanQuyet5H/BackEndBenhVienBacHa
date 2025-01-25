using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhauThuatThuThuat;
using FluentValidation;

namespace Camino.Api.Models.KhamTheoDoiBoPhanKhac.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamTheoDoiBoPhanKhacViewModel>))]
    public class KhamTheoDoiBoPhanKhacViewModelValidator : AbstractValidator<KhamTheoDoiBoPhanKhacViewModel>
    {
        public KhamTheoDoiBoPhanKhacViewModelValidator(ILocalizationService localizationService, IKhamTheoDoiBoPhanKhacService khamTheoDoiBoPhanKhacService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(p => p.Ten)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoi.BoPhanKhac.Ten.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("PTTT.TheoDoi.BoPhanKhac.Ten.Required"));

            RuleFor(p => p.NoiDung)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoi.BoPhanKhac.NoiDung.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("PTTT.TheoDoi.BoPhanKhac.NoiDung.Required"));
        }
    }
}