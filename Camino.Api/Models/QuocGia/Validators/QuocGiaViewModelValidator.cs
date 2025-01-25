using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.QuocGias;
using FluentValidation;

namespace Camino.Api.Models.QuocGia.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<QuocGiaViewModel>))]
    public class QuocGiaViewModelValidator : AbstractValidator<QuocGiaViewModel>
    {
        public QuocGiaViewModelValidator(ILocalizationService localizationService, IQuocGiaService quocGiaService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .Length(0, 20).WithMessage(localizationService.GetResource("Common.Ma.Range.20"))
                .MustAsync(async (model, input, s) => !await quocGiaService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"));

            RuleFor(x => x.TenVietTat)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
                .MustAsync(async (model, input, s) => !await quocGiaService.IsTenVietTatExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.QuocTich)
                .NotEmpty().WithMessage(localizationService.GetResource("QuocGia.QuocTich.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("QuocGia.QuocTich.Range"));

            RuleFor(x => x.MaDienThoaiQuocTe)
                .NotEmpty().WithMessage(localizationService.GetResource("QuocGia.MaDienThoaiQuocTe.Required"))
                .Length(0, 100).WithMessage(localizationService.GetResource("QuocGia.MaDienThoaiQuocTe.Range"))
                .NotEqual("0").WithMessage(localizationService.GetResource("QuocGia.MaDienThoaiQuocTe.EqualZero"))
                .Must((model, s) => quocGiaService.ContainNumber(model.MaDienThoaiQuocTe)).WithMessage(localizationService.GetResource("QuocGia.MaDienThoaiQuocTe.Valid"));

            RuleFor(x => x.ThuDo)
                .NotEmpty().WithMessage(localizationService.GetResource("QuocGia.ThuDo.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("QuocGia.ThuDo.Range"));

            RuleFor(x => x.ChauLuc)
                .NotEmpty().WithMessage(localizationService.GetResource("QuocGia.ChauLuc.Required"))
                .Must((model, s) => quocGiaService.CheckChauLuc(model.ChauLuc)).WithMessage(localizationService.GetResource("QuocGia.ChauLuc.NotExists"));
        }
    }
}