using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using FluentValidation;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThuocHoacHoatChatViewModel>))]
    public class ThuocHoacHoatChatViewModelValidator : AbstractValidator<ThuocHoacHoatChatViewModel>
    {
        public ThuocHoacHoatChatViewModelValidator(ILocalizationService localizationService,
            IThuocHoacHoatChatService thuocHoacHoatChatService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .Length(0, 20).WithMessage(localizationService.GetResource("Common.Ma.Range.20"));

            RuleFor(x => x.MaATC)
                .Length(0, 20).WithMessage(localizationService.GetResource("ThuocHoacHoatChat.MaATC.Range.20"))
                .MustAsync(async (model, input, source) => !await thuocHoacHoatChatService.IsMaAtcExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("ThuocHoacHoatChat.MaATC.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 400).WithMessage(localizationService.GetResource("Common.Ten.Range.400"));

            RuleFor(x => x.DuongDungId)
                .NotEmpty().WithMessage(localizationService.GetResource("ThuocHoacHoatChat.DuongDungId.Required"));

            RuleFor(x => x.TyLeBaoHiemThanhToan)
                .NotEmpty().WithMessage(localizationService.GetResource("ThuocHoacHoatChat.TyLeBaoHiemThanhToan.Required"));

            RuleFor(x => x.NhomThuocId)
                .NotEmpty().WithMessage(localizationService.GetResource("ThuocHoacHoatChat.NhomThuocId.Required"));

            RuleFor(x => x.MoTa)
                .Length(0, 1000).WithMessage(localizationService.GetResource("Common.MoTa.Range.1000"));
        }
    }
}
