using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhongBenhVien;
using FluentValidation;

namespace Camino.Api.Models.PhongBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhongBenhVienViewModel>))]
    public class PhongBenhVienViewModelValidator : AbstractValidator<PhongBenhVienViewModel>
    {
        public PhongBenhVienViewModelValidator(ILocalizationService localizationService, IPhongBenhVienService phongBenhVienService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await phongBenhVienService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.Ma.Range"))
                .MustAsync(async (model, input, s) => !await phongBenhVienService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.KhoaPhongId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.Id.Required"));
        }
    }
}
