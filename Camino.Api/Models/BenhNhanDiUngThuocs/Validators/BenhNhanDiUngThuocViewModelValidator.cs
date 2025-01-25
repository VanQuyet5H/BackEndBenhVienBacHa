using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BenhNhanDiUngThuocs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhNhanDiUngThuocViewModel>))]
    public class BenhNhanDiUngThuocViewModelValidator : AbstractValidator<BenhNhanDiUngThuocViewModel>
    {
        public BenhNhanDiUngThuocViewModelValidator(ILocalizationService localizationService, IBenhNhanDiUngThuocService benhNhanDiUngThuocService)
        {
            RuleFor(x => x.ThuocId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("DiUngThuoc.ThuocId.Required"))
            .MustAsync(async (model, input, s) => !await benhNhanDiUngThuocService.IsThuocExists(model.LoaiDiUng, model.ThuocId, model.BenhNhanId, model.Id).ConfigureAwait(false))
            .WithMessage(localizationService.GetResource("DiUngThuoc.ThuocId.Exists"));

            RuleFor(x => x.LoaiDiUng)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("DiUngThuoc.LoaiDiUng.Required"));

            RuleFor(x => x.TenDiUng)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("DiUngThuoc.ThuocId.Required"));

            RuleFor(x => x.BieuHienDiUng)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("DiUngThuoc.BieuHienDiUng.Required"));

            RuleFor(x => x.MucDo)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.MucDo.Required"));
        }
    }
}
