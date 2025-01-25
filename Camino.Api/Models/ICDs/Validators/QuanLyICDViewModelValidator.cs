using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.ICDs;

namespace Camino.Api.Models.ICDs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<QuanLyICDViewModel>))]

    public class QuanLyICDViewModelValidator : AbstractValidator<QuanLyICDViewModel>
    {
        public QuanLyICDViewModelValidator(ILocalizationService localizationService, IICDService iCDService)
        {

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("ICD.Ma.Required")).MustAsync(async (request, ma, id) =>
                {

                    return !await iCDService.IsMaExist(ma, request.Id);
                })
                .WithMessage(localizationService.GetResource("ICD.Ma.Exists"));

            RuleFor(x => x.TenTiengViet)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenTiengViet.Required")).MustAsync(async (request, tenVT, id) =>
                {
                    return !await iCDService.IsTenExist(tenVT, request.Id);
                })
                .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.LoaiICDId)
                .NotEmpty().WithMessage(localizationService.GetResource("ICD.TenLoaiICD.Required"));
        }
    }
}
