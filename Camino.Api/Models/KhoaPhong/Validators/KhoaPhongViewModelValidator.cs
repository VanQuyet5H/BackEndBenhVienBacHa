using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhoaPhong.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhoaPhongViewModel>))]
    public class KhoaPhongViewModelValidator : AbstractValidator<KhoaPhongViewModel>
    {
        public KhoaPhongViewModelValidator(ILocalizationService localizationService, IKhoaPhongService khoaPhongService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await khoaPhongService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.Ma.Range"))
                .MustAsync(async (model, input, s) => !await khoaPhongService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));

            RuleFor(x => x.KhoaIds)
                .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.Id.Required"));
            RuleFor(x => x.LoaiKhoaPhong)
                .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.LoaiKhoaPhong.Required"));
            RuleFor(x => x.CoKhamNgoaiTru)
                .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.CoKhamNgoaiTru.Required")).When(x => x.CoKhamNgoaiTru == null && x.CoKhamNoiTru == null);
            RuleFor(x => x.CoKhamNoiTru)
               .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.CoKhamNgoaiTru.Required")).When(x => x.CoKhamNgoaiTru == null && x.CoKhamNoiTru == null);
        }
    }
}
