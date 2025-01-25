using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TuongTrinhTuVongViewModel>))]
    public class TuongTrinhTuVongViewModelValidator : AbstractValidator<TuongTrinhTuVongViewModel>
    {
        public TuongTrinhTuVongViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.TgTuVong)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoi.TuVong.ThoiGianTuVong.Required"));

            RuleFor(p => p.TuVong)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoi.TuVong.ThoiDiemTuVong.Required"));
        }
    }
}