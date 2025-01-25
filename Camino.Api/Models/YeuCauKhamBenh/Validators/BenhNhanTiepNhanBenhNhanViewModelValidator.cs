using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhNhanTiepNhanBenhNhanViewModel>))]
    public class BenhNhanTiepNhanBenhNhanViewModelValidator : AbstractValidator<BenhNhanTiepNhanBenhNhanViewModel>
    {
        public BenhNhanTiepNhanBenhNhanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.HoTen.Required"));
            RuleFor(x => x.BHYTMaSoThe)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.BHYTMaSoThe.Required"));
            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.NgayThangNamSinh.Required"));

            //RuleFor(x => x.QuocTichId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.QuocTichId.Required"));

            RuleFor(x => x.Email)
                .Length(0, 200).WithMessage(localizationService.GetResource("BenhNhan.Email.Range"))
                .Must((model, s) => CommonHelper.IsMailValid(model.Email))
                .WithMessage(localizationService.GetResource("BenhNhan.Emai.WrongEmail"));

            RuleFor(x => x.NguoiLienHeEmail)
                .Length(0, 200).WithMessage(localizationService.GetResource("BenhNhan.NguoiLienHeEmail.Range"))
                .Must((model, s) => CommonHelper.IsMailValid(model.NguoiLienHeEmail))
                .WithMessage(localizationService.GetResource("BenhNhan.NguoiLienHeEmail.WrongEmail"));

            RuleFor(x => x.NoiLamViec)
             .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.NoiLamViec.Required"))
             .When(x => x.CoBHYT == true); 

        }
    }
}