using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhaThau;
using FluentValidation;

namespace Camino.Api.Models.NhaThau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhaThauViewModel>))]
    public class NhaThauViewModelValidator : AbstractValidator<NhaThauViewModel>
    {
        public NhaThauViewModelValidator(ILocalizationService localizationService, INhaThauService nhaThauService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await nhaThauService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("NhaThau.Ten.Exists"));

            RuleFor(x => x.DiaChi)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.DiaChi.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.DiaChi.Range"));

            RuleFor(x => x.MaSoThue)
                .NotEmpty().WithMessage(localizationService.GetResource("NhaThau.MaSoThue.Required"))
                .Length(0, 20).WithMessage(localizationService.GetResource("NhaThau.MaSoThue.Range.20"));

            RuleFor(x => x.TaiKhoanNganHang)
                .NotEmpty().WithMessage(localizationService.GetResource("NhaThau.TaiKhoanNganHang.Required"))
                .Length(0, 20).WithMessage(localizationService.GetResource("NhaThau.TaiKhoanNganHang.Range.20"));

            RuleFor(x => x.NguoiDaiDien)
                .NotEmpty().WithMessage(localizationService.GetResource("NhaThau.NguoiDaiDien.Required"))
                .Length(0, 100).WithMessage(localizationService.GetResource("NhaThau.NguoiDaiDien.Range.100"));

            RuleFor(x => x.NguoiLienHe)
                .NotEmpty().WithMessage(localizationService.GetResource("NhaThau.NguoiLienHe.Required"))
                .Length(0, 100).WithMessage(localizationService.GetResource("NhaThau.NguoiLienHe.Range.100"));

            RuleFor(x => x.SoDienThoaiLienHe)
                .NotEmpty().WithMessage(localizationService.GetResource("NhaThau.SoDienThoaiLienHe.Required"))
                .Length(0, 20).WithMessage(localizationService.GetResource("NhaThau.SoDienThoaiLienHe.Range.20"));

            RuleFor(x => x.EmailLienHe)
                .NotEmpty().WithMessage(localizationService.GetResource("NhaThau.EmailLienHe.Required"))
                .EmailAddress().When(email => email.EmailLienHe != "")
                .WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}
