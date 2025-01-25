using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.ChuanDoanHinhAnhs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.ChuanDoanHinhAnhViewModelCategory.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChuanDoanHinhAnhViewModel>))]
    public class ChuanDoanHinhAnhViewModelValidator : AbstractValidator<ChuanDoanHinhAnhViewModel>
    {
        public ChuanDoanHinhAnhViewModelValidator(ILocalizationService localizationService, IChuanDoanHinhAnhService chuanDoanHinhAnhService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .MustAsync(async (model, input, s) => !await chuanDoanHinhAnhService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.LoaiChuanDoanHinhAnh)
                .NotEmpty().WithMessage(localizationService.GetResource("ChuanDoanHinhAnh.LoaiChuanDoanHinhAnh.Required"));
        }
    }
}
