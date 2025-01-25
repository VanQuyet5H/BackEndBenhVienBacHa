using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BenhNhanTienSuBenhs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhNhanTienSuBenhViewModel>))]
    public class BenhNhanTienSuBenhViewModelValidator : AbstractValidator<BenhNhanTienSuBenhViewModel>
    {
        public BenhNhanTienSuBenhViewModelValidator(ILocalizationService localizationService, IBenhNhanTienSuBenhService benhNhanTienSuBenhService)
        {
            //RuleFor(x => x.TenBenh)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TienSuBenh.Ten.Required"));

            //RuleFor(x => x.NgayPhatHien).MustAsync(async (model, input, source) => !await benhNhanTienSuBenhService.CheckValidDate(model.NgayPhatHien))
            //    .WithMessage(localizationService.GetResource("TienSuBenh.NgayPhatHien.Validate"));
        }
    }
}
