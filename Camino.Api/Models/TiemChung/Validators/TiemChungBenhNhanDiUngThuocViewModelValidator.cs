using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungBenhNhanDiUngThuocViewModel>))]
    public class TiemChungBenhNhanDiUngThuocViewModelValidator : AbstractValidator<TiemChungBenhNhanDiUngThuocViewModel>
    {
        public TiemChungBenhNhanDiUngThuocViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.LoaiDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.LoaiDiUng.Required"));

            RuleFor(p => p.TenDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.ThuocId.Required"))
                .MaximumLength(1000).WithMessage(localizationService.GetResource("DiUngThuoc.TenDiUng.Range.1000"));

            RuleFor(p => p.MucDo)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.MucDo.Required"));

            RuleFor(p => p.BieuHienDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.BieuHienDiUng.Required"))
                .MaximumLength(2000).WithMessage(localizationService.GetResource("DiUngThuoc.BieuHienDiUng.Range.2000"));
        }
    }
}