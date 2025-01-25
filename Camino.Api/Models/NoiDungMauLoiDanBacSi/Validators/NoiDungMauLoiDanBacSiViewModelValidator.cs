using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NoiDungMauLoiDanBacSi;
using FluentValidation;

namespace Camino.Api.Models.KetQuaVaKetLuanMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiDungMauLoiDanBacSiViewModel>))]
    public class NoiDungMauLoiDanBacSiViewModelValidator : AbstractValidator<NoiDungMauLoiDanBacSiViewModel>
    {
        public NoiDungMauLoiDanBacSiViewModelValidator(ILocalizationService localizationService, INoiDungMauLoiDanBacSiService _noiDungMauLoiDanBacSiService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ma.Required"))
                .MustAsync(async (model, input, d) =>
                    !await _noiDungMauLoiDanBacSiService.KiemTraTrungMa(model.Id, input, (long)model.LoaiBenhAn)).WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ma.IsExists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.Ten.Required"));

            RuleFor(x => x.HuongXuLyLoiDanBacSi)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.KetQuaMau.Required"));

        }
    }
}
