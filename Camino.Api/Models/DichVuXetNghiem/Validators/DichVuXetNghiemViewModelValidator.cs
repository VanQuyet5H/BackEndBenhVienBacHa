using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DichVuXetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuXetNghiemViewModel>))]

    public class DichVuXetNghiemViewModelValidator : AbstractValidator<DichVuXetNghiemViewModel>
    {
        public DichVuXetNghiemViewModelValidator(ILocalizationService localizationService, IValidator<KetNoiChiSoXetNghiemViewModel> ketNoiChiSoXetNghiemValidator)
        {
            RuleFor(x => x.TenChiSo)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.TenChiSo.Required"))
                .When(x => (x.CapDichVu == 2 || x.CapDichVu == 3) && !x.IsDelete || (x.CapDichVu == 1 && x.IsCreateChild));

            RuleFor(x => x.LoaiMauXetNghiem)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.LoaiMauXetNghiem.Required"))
                .When(x => !x.IsDelete);

            RuleForEach(x => x.KetNoiChiSoXetNghiems).SetValidator(ketNoiChiSoXetNghiemValidator).When(x => !x.IsDelete);

            //RuleFor(x => x.MaChiSo)
            //  .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.MaChiSo.Required"))
            //  .When(x => !x.IsDelete);

            //RuleFor(x => x.MauMayXetNghiemId)
            //  .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.MauMayXetNghiemId.Required"))
            //  .When(x => !x.IsDelete);
        }
    }
}
