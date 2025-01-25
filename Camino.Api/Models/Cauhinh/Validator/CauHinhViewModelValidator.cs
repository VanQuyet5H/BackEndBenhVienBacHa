using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.Cauhinh.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<CauhinhViewModel>))]
    public class CauHinhViewModelValidator : AbstractValidator<CauhinhViewModel>
    {
        public CauHinhViewModelValidator(ILocalizationService iLocalizationService, IValidator<CauHinhTheoThoiGianChiTietViewModel> cauHinhTheoThoiGianChiTietValidator, ICauHinhService cauHinhService,
            IValidator<CauHinhDanhSachChiTietViewModel> cauHinhDanhSachChiTietValidator)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Name.Required")).Length(0, 255)
                .WithMessage(iLocalizationService
                    .GetResource("Common.Name.Range"));
                
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Value.Required"));
            RuleFor(x => x.DataType)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.DataType.Required"));
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.MoTa.Required"))
                .MustAsync(async (model, input, s) => !await cauHinhService.IsTenCauHinhExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id, model.LoaiCauHinh).ConfigureAwait(false))
                .WithMessage(iLocalizationService.GetResource("Common.Ten.Exists"));

            RuleForEach(x => x.CauHinhTheoThoiGianChiTiets).SetValidator(cauHinhTheoThoiGianChiTietValidator);
            RuleForEach(x => x.CauHinhDanhSachChiTiets).SetValidator(cauHinhDanhSachChiTietValidator);
        }
    }
}
