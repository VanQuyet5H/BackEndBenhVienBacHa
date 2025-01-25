using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhoaPhongNhanVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhoaPhongNhanVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhoaPhongNhanVienMultiViewModel>))]
    public class KhoaPhongNhanVienMultiViewModelValidator : AbstractValidator<KhoaPhongNhanVienMultiViewModel>
    {
        public KhoaPhongNhanVienMultiViewModelValidator(ILocalizationService localizationService, IKhoaPhongNhanVienService khoaPhongNhanVienService)
        {
            RuleFor(x => x.KhoaPhongId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.Id.Required"));

            RuleFor(x => x.NhanVienIds)
                .NotEmpty().WithMessage(localizationService.GetResource("NhanVien.Id.Required"))
                .MustAsync(async (model, input, source) => !await khoaPhongNhanVienService.CheckNhanVienExistAsync(model.KhoaPhongId, model.NhanVienIds, input))
                .WithMessage(localizationService.GetResource("IdKhoaPhong.IdNhanVien.Exists"));
        }
    }
}