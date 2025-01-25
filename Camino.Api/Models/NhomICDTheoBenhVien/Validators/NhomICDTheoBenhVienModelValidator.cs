using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhomICDTheoBenhViens;
using Camino.Services.QuocGias;
using FluentValidation;
using System.Linq;

namespace Camino.Api.Models.QuocGia.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhomICDTheoBenhVienViewModel>))]
    public class NhomICDTheoBenhVienModelValidator : AbstractValidator<NhomICDTheoBenhVienViewModel>
    {
        public NhomICDTheoBenhVienModelValidator(ILocalizationService localizationService, INhomICDTheoBenhVienService _nhomICDTheoBenhVienService)
        {
            RuleFor(x => x.TenTiengViet)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
               .Length(0, 250).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
               .MustAsync(async (model, input, s) => await _nhomICDTheoBenhVienService.IsTenVietTatTiengVietExists(
                   !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                   model.Id).ConfigureAwait(false))
               .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.TenTiengAnh)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
               .Length(0, 250).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
               .MustAsync(async (model, input, s) => await _nhomICDTheoBenhVienService.IsTenVietTatTiengAnhExists(
                   !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                   model.Id).ConfigureAwait(false))
               .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));


            RuleFor(x => x.Ma)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
               .Length(0, 500).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
               .MustAsync(async (model, input, s) => await _nhomICDTheoBenhVienService.IsMaBenhExists(
                   !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                   model.Id).ConfigureAwait(false))
               .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.Stt)
           .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
           .Length(0, 50).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
           .MustAsync(async (model, input, s) => await _nhomICDTheoBenhVienService.IsSTTExists(
               !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
               model.Id).ConfigureAwait(false))
           .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.MaICDs)
               .Must((model, input, s) =>
               {
                   if (model.MaICDs == null || !model.MaICDs.Any())
                   {
                       return false;
                   }

                   return true;
               }).WithMessage(localizationService.GetResource("NhomICDTheoBenhVien.MaICDs.Required"));

            RuleFor(x => x.ChuongICDId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhomICDTheoBenhVien.ChuongICDId.Required"));
        }
    }
}