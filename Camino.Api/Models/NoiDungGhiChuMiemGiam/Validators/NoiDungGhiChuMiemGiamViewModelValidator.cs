using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NoiDungGhiChuMiemGiam;
using FluentValidation;

namespace Camino.Api.Models.KetQuaVaKetLuanMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiDungGhiChuMiemGiamViewModel>))]
    public class NoiDungGhiChuMiemGiamViewModelValidator : AbstractValidator<NoiDungGhiChuMiemGiamViewModel>
    {
        public NoiDungGhiChuMiemGiamViewModelValidator(ILocalizationService localizationService,
            INoiDungGhiChuMiemGiamService _noiDungGhiChuMiemGiamService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ma.Required"))
                .MustAsync(async (model, input, d) =>
                    !await _noiDungGhiChuMiemGiamService.KiemTraTrungMa(model.Id, input)).WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ma.IsExists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ten.Required"));

            RuleFor(x => x.NoiDungMiemGiam)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.KetQuaMau.Required"));

        }
    }
}
