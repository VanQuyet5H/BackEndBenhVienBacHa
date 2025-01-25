using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.XetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhongTiepNhanMauViewModel>))]
    public class KhongTiepNhanMauViewModelValidator : AbstractValidator<KhongTiepNhanMauViewModel>
    {
        public KhongTiepNhanMauViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.LyDoKhongDat)
                .NotNull().WithMessage(localizationService.GetResource("XetNghiem.NhanMau.LyDoTuChoi.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("XetNghiem.NhanMau.LyDoTuChoi.Required"))
                .Length(0, 200).WithMessage(localizationService.GetResource("XetNghiem.NhanMau.LyDoTuChoi.Range.200"));
        }
    }
}