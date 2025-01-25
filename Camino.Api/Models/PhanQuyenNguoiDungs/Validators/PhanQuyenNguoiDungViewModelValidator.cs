using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhanQuyenNguoiDungs;
using FluentValidation;

namespace Camino.Api.Models.PhanQuyenNguoiDungs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhanQuyenNguoiDungViewModel>))]
    public class PhanQuyenNguoiDungViewModelValidator : AbstractValidator<PhanQuyenNguoiDungViewModel>
    {
        public PhanQuyenNguoiDungViewModelValidator(ILocalizationService localizationService, IPhanQuyenNguoiDungService phanQuyenNguoiDungService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 100).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await phanQuyenNguoiDungService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.UserType)
                .NotEmpty().WithMessage(localizationService.GetResource("Role.UserType.Required"));
        }
    }
}
