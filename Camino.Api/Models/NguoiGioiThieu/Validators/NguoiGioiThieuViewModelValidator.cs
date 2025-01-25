using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NguoiGioiThieu;
using FluentValidation;

namespace Camino.Api.Models.NguoiGioiThieu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NguoiGioiThieuViewModel>))]

    public class NguoiGioiThieuViewModelValidator : AbstractValidator<NguoiGioiThieuViewModel>
    {
        public NguoiGioiThieuViewModelValidator(ILocalizationService localizationService, INguoiGioiThieuService nguoiGioiThieuService)
        {
            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .MustAsync(async (viewModel, Id, input) =>
                   {
                       var checkExistsTen = await nguoiGioiThieuService.IsTenExists(viewModel.HoTen, viewModel.NhanVienQuanLyId, viewModel.Id);
                       return !checkExistsTen;
                   })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.SoDienThoai)
                .MustAsync(async (viewModel, Id, input) =>
                {
                    var checkExistsTen = await nguoiGioiThieuService.IsPhoneNumberExists(viewModel.HoTen, viewModel.SoDienThoai, viewModel.Id);
                    return !checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.SoDienThoai.Exists"));

            RuleFor(x => x.NhanVienQuanLyId)
                 .NotEmpty().WithMessage(localizationService.GetResource("NguoiGioiThieu.TenQuanLy.Required"));
        }
    }
}
