using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhomDichVuBenhVien;
using FluentValidation;

namespace Camino.Api.Models.NhomDichVuBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhomDichVuBenhVienViewModel>))]
    public class NhomDichVuBenhVienViewModelValidator : AbstractValidator<NhomDichVuBenhVienViewModel>
    {
        public NhomDichVuBenhVienViewModelValidator(ILocalizationService iLocalizationService, INhomDichVuBenhVienService nhomDichVuBenhVienService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).Length(0, 250).WithMessage(iLocalizationService
                    .GetResource("Common.Ten.Range"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).Length(0, 50).WithMessage(iLocalizationService
                    .GetResource("Common.Ma.Range"));

            RuleFor(x => x.NhomDichVuBenhVienChaId)
                .Must((model, input, s) =>
                {
                    return model.Id != model.NhomDichVuBenhVienChaId;
                })
                .WithMessage(iLocalizationService.GetResource("NhomDichVuBenhVien.NhomDichVuBenhVienChaId.Trung"))
                .MustAsync(async (model, input, s) => await nhomDichVuBenhVienService.CheckChiDinhVong(model.Id, model.NhomDichVuBenhVienChaId)
                    .ConfigureAwait(false))
                .WithMessage(iLocalizationService.GetResource("NhomDichVuBenhVien.NhomDichVuBenhVienChaId.Vong"));
        }
    }
}
