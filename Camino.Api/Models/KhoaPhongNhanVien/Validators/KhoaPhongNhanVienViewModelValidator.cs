using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhoaPhongNhanVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhoaPhongNhanVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhoaPhongNhanVienViewModel>))]
    public class KhoaPhongNhanVienViewModelValidator : AbstractValidator<KhoaPhongNhanVienViewModel>
    {
        public KhoaPhongNhanVienViewModelValidator(ILocalizationService localizationService, IKhoaPhongNhanVienService khoaPhongNhanVienService)
        {
            RuleFor(x => x.KhoaPhongId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhoaPhong.Id.Required"));

            RuleFor(x => x.NhanVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhanVien.Id.Required"));

            RuleFor(x => x.KhoaPhongId).MustAsync(async (model, input, source) => !await khoaPhongNhanVienService.CheckExistAsync(model.KhoaPhongId, model.NhanVienId, model.Id))
                .When(t => t.NhanVienId != 0).WithMessage(localizationService.GetResource("IdKhoaPhong.IdNhanVien.Exists"));
        }
    }
}
