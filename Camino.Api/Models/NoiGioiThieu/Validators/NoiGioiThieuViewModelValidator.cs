using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NoiGioiThieu;
using FluentValidation;

namespace Camino.Api.Models.NoiGioiThieu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiGioiThieuViewModel>))]

    public class NoiGioiThieuViewModelValidator : AbstractValidator<NoiGioiThieuViewModel>
    {
        public NoiGioiThieuViewModelValidator(ILocalizationService localizationService, INoiGioiThieuService nguoiGioiThieuService
            , IValidator<NoiGioiThieuChiTietMienGiamViewModel> noiGioiThieuChiTietMienGiamValidator)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .MustAsync(async (viewModel, Id, input) =>
                {
                    var checkExistsTen = await nguoiGioiThieuService.IsTenExists(viewModel.Ten, viewModel.Id);
                    return !checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.SoDienThoai)
                .MustAsync(async (viewModel, Id, input) =>
                {
                    var checkExistsTen = await nguoiGioiThieuService.IsPhoneNumberExists(viewModel.Ten, viewModel.SoDienThoai, viewModel.Id);
                    return !checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.SoDienThoai.Exists"));

            RuleForEach(x => x.NoiGioiThieuChiTietMienGiams).SetValidator(noiGioiThieuChiTietMienGiamValidator);
        }
    }
}
