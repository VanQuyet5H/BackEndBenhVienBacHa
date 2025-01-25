using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.GiuongBenhs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.GiuongBenhs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiuongBenhViewModel>))]
    public class GiuongBenhViewModelValidator : AbstractValidator<GiuongBenhViewModel>
    {
        public GiuongBenhViewModelValidator(ILocalizationService localizationService, IGiuongBenhService giuongBenhService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("GiuongBenh.Ma.Required"))
                .MustAsync(async (model, input, d) => await giuongBenhService.KiemTraMaGiuongBenhAsync(model.Id, input)).WithMessage(localizationService.GetResource("GiuongBenh.Ma.IsExists"));
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("GiuongBenh.Ten.Required"));
            RuleFor(x => x.KhoaId)
                .NotEmpty().WithMessage(localizationService.GetResource("GiuongBenh.KhoaId.Required"));
            RuleFor(x => x.PhongBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("GiuongBenh.PhongBenhVienId.Required"));
        }
    }
}